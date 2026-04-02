# IPluginPipeline 用于 HTTP 请求管道

## 🎯 核心概念

**是的！`IPluginPipeline<TContext>` 完全可以用于 HTTP 请求管道！**

有两种方式可以实现：

---

## 📊 方案对比

### 方案一：HTTP 中间件调用业务管道（推荐）

```
HTTP 请求 
  ↓
HttpToBusinessOperationMiddleware (IDynamicMiddleware)
  ↓
从 HTTP 上下文提取信息创建 BusinessOperationContext
  ↓
调用 BusinessOperationExecutor.ExecuteAsync()
  ↓
执行业务操作管道:
    → LoggingMiddleware
    → ValidationMiddleware  
    → PerformanceMiddleware
    → 实际业务逻辑
  ↓
返回结果到 HTTP 响应
```

**优点**：
- ✅ 职责清晰分离
- ✅ 可以复用已有的业务管道
- ✅ 易于测试和维护
- ✅ 可以在非 HTTP 场景下也使用业务管道

### 方案二：直接使用泛型管道处理 HTTP 请求

```csharp
// 定义 HTTP 请求上下文
public class HttpRequestContext
{
    public HttpContext HttpContext { get; set; }
    public string RequestId { get; set; }
    public DateTime RequestTime { get; set; }
}

// 实现 HTTP 请求管道中间件
public class HttpAuthenticationMiddleware 
    : IPluginPipeline<HttpRequestContext>
{
    public async Task InvokeAsync(HttpRequestContext context, Func<HttpRequestContext, Task> next)
    {
        // 认证逻辑
        await next(context);
    }
}
```

**适用场景**：需要完全自定义的 HTTP 处理管道，不依赖 ASP.NET Core 的中间件机制。

---

## 🔧 方案一完整实现

### 1. HttpToBusinessOperationMiddleware

```csharp
// 文件：Middleware/HttpToBusinessOperationMiddleware.cs
public class HttpToBusinessOperationMiddleware : IDynamicMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 只对特定路径进行处理
        if (context.Request.Path.StartsWithSegments("/api/business"))
        {
            // 1. 从 HTTP 上下文创建业务操作上下文
            var businessContext = new BusinessOperationContext
            {
                OperationType = context.Request.Method switch
                {
                    "POST" => "Create",
                    "PUT" => "Update",
                    "DELETE" => "Delete",
                    "GET" => "Query",
                    _ => "Unknown"
                },
                DataType = context.Request.Path.Value?.Split('/')[3] ?? "Unknown",
                UserId = context.User.FindFirst("userId")?.Value ?? "anonymous"
            };
            
            // 2. 获取业务操作执行器
            var executor = context.RequestServices
                .GetService(typeof(BusinessOperationExecutor)) 
                as BusinessOperationExecutor;
            
            if (executor != null)
            {
                // 3. 通过业务管道执行（会经过所有中间件）
                var result = await executor.ExecuteAsync(
                    businessContext.OperationType,
                    businessContext.DataType,
                    businessContext.DataId,
                    businessContext.UserId);
                
                // 4. 将结果写入响应
                context.Response.Headers["X-Operation-Id"] = result.OperationId;
                context.Response.Headers["X-Elapsed-Milliseconds"] = result.ElapsedMilliseconds?.ToString();
            }
        }
        
        await next(context);
    }
}
```

---

## 🚀 实际使用示例

### Controller 中调用

```csharp
// 文件：Controllers/BusinessController.cs
[ApiController]
[Route("api/business/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly BusinessOperationExecutor _executor;
    
    public ProductsController(BusinessOperationExecutor executor)
    {
        _executor = executor;
    }
    
    /// <summary>
    /// 创建产品（会自动经过业务操作管道）
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            // 直接通过业务操作管道执行
            var context = await _executor.CreateAsync(
                dataType: "Product",
                dataId: dto.Id,
                userId: GetCurrentUserId(),
                extraData: new Dictionary<string, object>
                {
                    ["ProductName"] = dto.ProductName,
                    ["Price"] = dto.Price
                });
            
            return Ok(new 
            { 
                success = true,
                operationId = context.OperationId,
                elapsedMs = context.ElapsedMilliseconds,
                message = "创建成功"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    private string GetCurrentUserId()
    {
        return User.FindFirst("userId")?.Value ?? "anonymous";
    }
}
```

---

## 📊 完整的执行流程

### 场景 1: 通过 HTTP 中间件触发

```
HTTP POST /api/business/products
    ↓
HttpToBusinessOperationMiddleware (HTTP 中间件)
    ↓
创建 BusinessOperationContext:
    - OperationType: "Create"
    - DataType: "Product"
    - UserId: "user-123"
    ↓
BusinessOperationExecutor.ExecuteAsync()
    ↓
PluginPipelineDispatcher.ExecuteAsync()
    ↓
执行业务管道:
    1. LoggingMiddleware
       - 记录："开始执行业务操作..."
       - 启动计时器
       
    2. ValidationMiddleware
       - 验证 UserId 不为空 ✓
       - 检查权限 ✓
       - 设置 IsAuthenticated = true
       - 设置 HasPermission = true
       
    3. PerformanceMiddleware
       - 停止计时器
       - 检查是否超过阈值
       
    4. 实际业务逻辑
       - 执行数据库操作
       - 创建产品记录
    ↓
返回结果:
    - OperationId: "xxx"
    - ElapsedMilliseconds: 150
    - IsAuthenticated: true
    - HasPermission: true
    ↓
写入 HTTP 响应头:
    - X-Operation-Id: "xxx"
    - X-Elapsed-Milliseconds: "150"
```

### 场景 2: 直接在 Controller 中调用

```
Controller.Create()
    ↓
BusinessOperationExecutor.CreateAsync()
    ↓
PluginPipelineDispatcher.ExecuteAsync()
    ↓
执行业务管道（同上）
    ↓
返回结果
```

---

## 🎓 关键要点

### 1. **两种中间件的协作**

```csharp
// HTTP 中间件（IDynamicMiddleware）
public class HttpToBusinessOperationMiddleware : IDynamicMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 处理 HTTP 请求
        // 创建业务上下文
        // 调用业务管道
    }
}

// 业务管道中间件（IPluginPipeline<TContext>）
public class BusinessOperationLoggingMiddleware 
    : IPluginPipeline<BusinessOperationContext>
{
    public Task InvokeAsync(
        BusinessOperationContext context, 
        Func<BusinessOperationContext, Task> next)
    {
        // 处理业务操作
        // 记录日志、验证权限、监控性能等
    }
}
```

### 2. **注册顺序**

在 PluginA.cs 中：

```csharp
public void ConfigureServices(ContainerBuilder builder)
{
    // ========== 管道相关服务 ==========
    builder.RegisterType<PluginPipelineRegistry<BusinessOperationContext>>()
        .SingleInstance();
    
    builder.RegisterType<PluginPipelineDispatcher<BusinessOperationContext>>()
        .SingleInstance();
    
    // 业务管道中间件
    builder.RegisterType<BusinessOperationLoggingMiddleware>()
        .InstancePerLifetimeScope();
    builder.RegisterType<BusinessOperationValidationMiddleware>()
        .InstancePerLifetimeScope();
    builder.RegisterType<BusinessOperationPerformanceMiddleware>()
        .InstancePerLifetimeScope();
    
    // 业务执行器
    builder.RegisterType<BusinessOperationExecutor>()
        .InstancePerLifetimeScope();
    
    // ========== HTTP 中间件 ==========
    builder.RegisterType<HttpToBusinessOperationMiddleware>()
        .InstancePerLifetimeScope();
}

public Task InitializeAsync(IServiceProvider serviceProvider)
{
    // 注册 HTTP 中间件到 HTTP 管道
    var httpRegistry = serviceProvider.GetService<DynamicMiddlewareRegistry>();
    httpRegistry.Register(typeof(HttpToBusinessOperationMiddleware));
    
    // 注册业务中间件到业务管道
    var businessRegistry = serviceProvider.GetService<
        PluginPipelineRegistry<BusinessOperationContext>>();
    businessRegistry.Register(typeof(BusinessOperationLoggingMiddleware));
    businessRegistry.Register(typeof(BusinessOperationValidationMiddleware));
    businessRegistry.Register(typeof(BusinessOperationPerformanceMiddleware));
    
    return Task.CompletedTask;
}
```

---

## 💡 实际应用场景

### 场景 1: 统一的操作审计

无论请求来自哪里（HTTP API、定时任务、消息队列），都经过相同的业务管道，记录相同的审计日志。

### 场景 2: 统一的权限控制

在业务管道层面统一处理权限验证，而不是在每个 Controller 中重复编写权限代码。

### 场景 3: 统一的性能监控

自动收集所有业务操作的性能指标，不需要在每个方法中手动计时。

### 场景 4: 跨平台复用

业务管道可以在不同平台复用：
- Web API（ASP.NET Core）
- 后台任务（Hangfire/Quartz）
- 消息队列（RabbitMQ/Kafka）
- gRPC 服务

---

## ✅ 总结

**`IPluginPipeline<TContext>` 可以用于 HTTP 请求管道！**

### 推荐方式：

1. **HTTP 中间件作为入口** → `IDynamicMiddleware`
2. **业务管道处理逻辑** → `IPluginPipeline<BusinessOperationContext>`
3. **桥接两者** → 在 HTTP 中间件中创建业务上下文并调用业务管道

### 优势：

✅ **职责分离** - HTTP 处理 vs 业务逻辑  
✅ **代码复用** - 业务管道可在多个场景使用  
✅ **易于测试** - 可以单独测试业务管道  
✅ **灵活扩展** - 可以轻松添加新的中间件  
✅ **统一管理** - 所有业务操作都遵循相同的处理流程  

现在你的系统既有 HTTP 请求处理能力，又有通用的业务管道机制！🎉
