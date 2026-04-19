# PluginA 中的 IPluginPipeline 实现示例

## 📁 文件结构

```
PluginA/
├── Contexts/
│   └── BusinessOperationContext.cs          # 业务操作上下文定义
├── Middleware/
│   ├── PluginAMiddleware.cs                  # HTTP 中间件（IDynamicMiddleware）
│   ├── BusinessOperationLoggingMiddleware.cs    # 日志中间件（IPluginPipeline）
│   ├── BusinessOperationValidationMiddleware.cs # 验证中间件（IPluginPipeline）
│   └── BusinessOperationPerformanceMiddleware.cs # 性能监控中间件（IPluginPipeline）
├── Services/
│   └── BusinessOperationExecutor.cs          # 业务操作执行器（使用管道）
└── PluginA.cs                                 # 插件入口（注册服务）
```

---

## 🎯 架构说明

### 1. **两种中间件对比**

| 特性 | IDynamicMiddleware | IPluginPipeline<TContext> |
|------|-------------------|--------------------------|
| **用途** | HTTP 请求管道 | 业务操作管道（泛型） |
| **上下文** | HttpContext | BusinessOperationContext |
| **注册位置** | DynamicMiddlewareRegistry | PluginPipelineRegistry<TContext> |
| **调度器** | DynamicMiddlewareDispatcher | PluginPipelineDispatcher<TContext> |
| **典型场景** | 请求过滤、认证、限流 | 业务验证、日志、性能监控 |

---

## 📝 核心组件详解

### 2. **BusinessOperationContext - 业务操作上下文**

```csharp
// 文件：Contexts/BusinessOperationContext.cs
public class BusinessOperationContext
{
    public string OperationId { get; set; }      // 操作 ID（自动生成）
    public string OperationType { get; set; }    // 操作类型（Create/Update/Delete/Query）
    public string DataType { get; set; }         // 数据类型
    public string DataId { get; set; }           // 数据 ID
    public string UserId { get; set; }           // 操作用户 ID
    public DateTime OperationTime { get; set; }  // 操作时间
    public bool IsAuthenticated { get; set; }    // 是否已验证
    public bool HasPermission { get; set; }      // 是否有权限
    public long? ElapsedMilliseconds { get; set; } // 执行耗时
}
```

**作用**：在管道的各个中间件之间传递数据和状态。

---

### 3. **三个管道中间件**

#### 3.1 **BusinessOperationLoggingMiddleware** - 日志记录

```csharp
// 职责：记录操作的开始、结束、异常和耗时
public class BusinessOperationLoggingMiddleware 
    : IPluginPipeline<BusinessOperationContext>
{
    public async Task InvokeAsync(BusinessOperationContext context, 
                                  Func<BusinessOperationContext, Task> next)
    {
        _logger.LogInformation("开始执行业务操作...");
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context); // 执行下一个中间件
            _logger.LogInformation("操作成功，耗时：{ElapsedMilliseconds}ms", 
                                   stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "操作失败");
            throw;
        }
    }
}
```

#### 3.2 **BusinessOperationValidationMiddleware** - 权限验证

```csharp
// 职责：验证用户身份和权限
public class BusinessOperationValidationMiddleware 
    : IPluginPipeline<BusinessOperationContext>
{
    public async Task InvokeAsync(BusinessOperationContext context, 
                                  Func<BusinessOperationContext, Task> next)
    {
        // 1. 基础验证
        if (string.IsNullOrWhiteSpace(context.UserId))
            throw new ArgumentException("操作用户 ID 不能为空");
        
        context.IsAuthenticated = true;
        
        // 2. 权限验证
        bool hasPermission = await CheckPermissionAsync(context);
        if (!hasPermission)
            throw new UnauthorizedAccessException("无权限执行此操作");
        
        context.HasPermission = true;
        
        // 3. 继续执行
        await next(context);
    }
}
```

#### 3.3 **BusinessOperationPerformanceMiddleware** - 性能监控

```csharp
// 职责：监控操作性能，超过阈值时发出警告
public class BusinessOperationPerformanceMiddleware 
    : IPluginPipeline<BusinessOperationContext>
{
    private const long PerformanceWarningThresholdMs = 1000; // 1 秒
    
    public async Task InvokeAsync(BusinessOperationContext context, 
                                  Func<BusinessOperationContext, Task> next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
            
            // 检查是否超过性能阈值
            if (stopwatch.ElapsedMilliseconds > PerformanceWarningThresholdMs)
            {
                _logger.LogWarning("操作执行时间过长：{ElapsedMilliseconds}ms", 
                                   stopwatch.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "操作异常");
            throw;
        }
    }
}
```

---

### 4. **BusinessOperationExecutor - 业务操作执行器**

```csharp
// 文件：Services/BusinessOperationExecutor.cs
public class BusinessOperationExecutor
{
    private readonly PluginPipelineDispatcher<BusinessOperationContext> _dispatcher;
    
    public async Task<BusinessOperationContext> ExecuteAsync(
        string operationType,
        string dataType,
        string dataId,
        string userId)
    {
        // 创建上下文
        var context = new BusinessOperationContext
        {
            OperationType = operationType,
            DataType = dataType,
            DataId = dataId,
            UserId = userId
        };
        
        // 通过管道执行（会依次经过所有中间件）
        await _dispatcher.ExecuteAsync(context, async (ctx) =>
        {
            // 这里是真正的业务逻辑
            // 例如：数据库操作、API 调用等
            await Task.Delay(100); // 模拟业务处理
        });
        
        return context;
    }
}
```

---

## 🔧 注册流程

### 在 PluginA.cs 中注册

```csharp
public void ConfigureServices(ContainerBuilder builder)
{
    // ========== 1. 注册管道注册表（单例） ==========
    builder.RegisterType<PluginPipelineRegistry<BusinessOperationContext>>()
        .AsSelf()
        .SingleInstance();
    
    // ========== 2. 注册管道调度器（单例） ==========
    builder.RegisterType<PluginPipelineDispatcher<BusinessOperationContext>>()
        .AsSelf()
        .SingleInstance();
    
    // ========== 3. 注册中间件（瞬态） ==========
    builder.RegisterType<BusinessOperationLoggingMiddleware>()
        .AsSelf()
        .InstancePerLifetimeScope();
    builder.RegisterType<BusinessOperationValidationMiddleware>()
        .AsSelf()
        .InstancePerLifetimeScope();
    builder.RegisterType<BusinessOperationPerformanceMiddleware>()
        .AsSelf()
        .InstancePerLifetimeScope();
    
    // ========== 4. 注册执行器（用于使用管道） ==========
    builder.RegisterType<BusinessOperationExecutor>()
        .AsSelf()
        .InstancePerLifetimeScope();
}

public Task InitializeAsync(IServiceProvider serviceProvider)
{
    // 注册到管道注册表
    var businessOpRegistry = serviceProvider.GetService<
        PluginPipelineRegistry<BusinessOperationContext>>();
    
    if (businessOpRegistry != null)
    {
        businessOpRegistry.Register(typeof(BusinessOperationLoggingMiddleware));
        businessOpRegistry.Register(typeof(BusinessOperationValidationMiddleware));
        businessOpRegistry.Register(typeof(BusinessOperationPerformanceMiddleware));
    }
    
    return Task.CompletedTask;
}
```

---

## 🚀 使用方式

### 在 Controller 中使用

```csharp
// 文件：Controllers/BusinessOperationController.cs
[ApiController]
[Route("api/[controller]")]
public class BusinessOperationController : ControllerBase
{
    private readonly BusinessOperationExecutor _executor;
    
    public BusinessOperationController(BusinessOperationExecutor executor)
    {
        _executor = executor;
    }
    
    /// <summary>
    /// 创建数据（会自动经过所有中间件）
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDto dto)
    {
        try
        {
            var context = await _executor.CreateAsync(
                dataType: "Product",
                dataId: dto.ProductId,
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
                elapsedMs = context.ElapsedMilliseconds 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    private string GetCurrentUserId()
    {
        // 从当前 HTTP 上下文获取用户 ID
        return User.FindFirst("userId")?.Value ?? "anonymous";
    }
}
```

---

## 📊 执行流程

```
用户调用 CreateAsync()
    ↓
创建 BusinessOperationContext
    ↓
PluginPipelineDispatcher.ExecuteAsync()
    ↓
构建管道链:
    PerformanceMiddleware
        → ValidationMiddleware
            → LoggingMiddleware
                → 实际业务逻辑
    ↓
执行管道:

1. LoggingMiddleware.InvokeAsync()
   - 记录："开始执行业务操作..."
   - 启动计时器
   - 调用 next() → ValidationMiddleware
   
2. ValidationMiddleware.InvokeAsync()
   - 验证 UserId 是否为空
   - 检查用户权限
   - 设置 context.IsAuthenticated = true
   - 设置 context.HasPermission = true
   - 调用 next() → LoggingMiddleware
   
3. LoggingMiddleware.InvokeAsync()
   - 停止计时器
   - 设置 context.ElapsedMilliseconds
   - 记录："操作成功，耗时：XX ms"
   
4. 实际业务逻辑
   - 执行数据库操作
   - 调用外部 API
   - 等等...
    ↓
返回结果
```

---

## ✅ 关键要点

### 1. **依赖注入支持**
所有中间件都可以通过构造函数注入服务：
```csharp
public BusinessOperationLoggingMiddleware(
    ILogger<BusinessOperationLoggingMiddleware> logger)
{
    _logger = logger;
}
```

### 2. **类型安全**
```csharp
// ✅ 正确：类型匹配
builder.RegisterType<BusinessOperationLoggingMiddleware>()
    .As<IPluginPipeline<BusinessOperationContext>>();

// ❌ 错误：类型不匹配，编译时会报错
builder.RegisterType<SomeOtherMiddleware>()
    .As<IPluginPipeline<BusinessOperationContext>>();
```

### 3. **灵活控制**
中间件可以决定是否继续执行：
```csharp
public async Task InvokeAsync(context, next)
{
    if (!HasPermission(context))
    {
        // 直接抛出异常，不调用 next()
        throw new UnauthorizedAccessException("无权限");
    }
    
    await next(context); // 只有验证通过才继续
}
```

### 4. **状态传递**
```csharp
// 中间件 A
context.IsAuthenticated = true;
await next(context);

// 中间件 B（后续）
if (context.IsAuthenticated)
{
    // 基于前一个中间件设置的状态进行处理
}
```

---

## 🎓 与 HTTP 中间件的对比

### HTTP 中间件示例（现有的 PluginAMiddleware）

```csharp
public class PluginAMiddleware : IDynamicMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine($"HTTP 请求：{context.Request.Path}");
        await next(context);
    }
}
```

**特点**：
- 只能用于 HTTP 请求管道
- 依赖 `HttpContext`
- 在主程序启动时自动注册到 HTTP 管道

### 泛型中间件示例（新增的 BusinessOperationLoggingMiddleware）

```csharp
public class BusinessOperationLoggingMiddleware 
    : IPluginPipeline<BusinessOperationContext>
{
    public async Task InvokeAsync(
        BusinessOperationContext context, 
        Func<BusinessOperationContext, Task> next)
    {
        Console.WriteLine($"业务操作：{context.OperationType}");
        await next(context);
    }
}
```

**特点**：
- 可用于任何自定义场景
- 不依赖特定框架
- 需要手动注册和使用管道调度器

---

## 📖 总结

这个示例展示了如何在插件中实现完整的 `IPluginPipeline<TContext>` 机制：

✅ **完整的架构**：上下文 + 中间件 + 注册表 + 调度器 + 执行器  
✅ **依赖注入**：所有组件都通过 DI 容器管理  
✅ **类型安全**：泛型确保编译时类型检查  
✅ **职责分离**：每个中间件只负责一个功能  
✅ **可扩展性**：可以轻松添加新的中间件  

现在你可以在插件中实现任意类型的业务管道，不再局限于 HTTP 请求！🎉

---

## 🔗 相关文档

- [IPluginPipeline使用示例](../../../Fastdotnet.Core/Middleware/README_IPluginPipeline使用示例.md)
- [插件中间件使用指南](../../../Fastdotnet.Core/Middleware/README_插件中间件使用指南.md)
