# 插件中间件使用指南

## 概述

Fastdotnet 提供了两种类型的插件中间件接口，分别用于不同的场景：

### 1. `IDynamicMiddleware` - HTTP 请求管道专用
- **用途**：仅用于 ASP.NET Core 的 HTTP 请求管道
- **上下文**：`HttpContext`
- **特点**：与 ASP.NET Core 深度集成

### 2. `IPluginPipeline<TContext>` - 通用管道（泛型）
- **用途**：适用于任何自定义场景（工作流、消息队列、后台任务等）
- **上下文**：任意泛型类型 `TContext`
- **特点**：框架无关，完全解耦

---

## HTTP 请求管道中间件

### 实现示例

```csharp
using Microsoft.AspNetCore.Http;
using Fastdotnet.Core.Middleware;

namespace MyPlugin.Middlewares
{
    /// <summary>
    /// 日志记录中间件
    /// </summary>
    public class RequestLoggingMiddleware : IDynamicMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        
        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation($"收到请求：{context.Request.Method} {context.Request.Path}");
            
            try
            {
                await next(context); // 调用管道中的下一个中间件
                _logger.LogInformation("请求处理成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "请求处理失败");
                throw;
            }
        }
    }
}
```

### 注册方式

在插件初始化时自动注册到 `DynamicMiddlewareRegistry`：

```csharp
public class MyPlugin : IPlugin
{
    public Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var registry = serviceProvider.GetRequiredService<DynamicMiddlewareRegistry>();
        registry.Register(typeof(RequestLoggingMiddleware));
        return Task.CompletedTask;
    }
}
```

---

## 通用管道中间件（泛型）

### 场景 1: Elsa 工作流管道

#### 定义工作流上下文

```csharp
public class WorkflowContext
{
    public string WorkflowDefinitionId { get; set; }
    public IDictionary<string, object> Variables { get; set; }
    public CancellationToken CancellationToken { get; set; }
}
```

#### 实现日志中间件

```csharp
using Fastdotnet.Core.Middleware;

namespace MyPlugin.WorkflowMiddlewares
{
    public class WorkflowLoggingMiddleware : IPluginPipeline<WorkflowContext>
    {
        private readonly ILogger _logger;
        
        public WorkflowLoggingMiddleware(ILogger<WorkflowLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        
        public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
        {
            _logger.LogInformation($"开始执行工作流：{context.WorkflowDefinitionId}");
            
            try
            {
                await next(context);
                _logger.LogInformation("工作流执行成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "工作流执行失败");
                throw;
            }
        }
    }
}
```

#### 实现权限校验中间件

```csharp
public class WorkflowPermissionMiddleware : IPluginPipeline<WorkflowContext>
{
    private readonly IPermissionService _permissionService;
    
    public WorkflowPermissionMiddleware(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }
    
    public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
    {
        // 检查当前用户是否有权限执行该工作流
        bool hasPermission = await _permissionService.CheckWorkflowPermissionAsync(
            context.WorkflowDefinitionId);
            
        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("无权限执行此工作流");
        }
        
        await next(context);
    }
}
```

---

### 场景 2: 消息队列处理管道

#### 定义消息上下文

```csharp
public class MessageContext<T>
{
    public T Message { get; set; }
    public string QueueName { get; set; }
    public IDictionary<string, string> Headers { get; set; }
    public DateTime ReceivedTime { get; set; }
}
```

#### 实现重试中间件

```csharp
public class MessageRetryMiddleware : IPluginPipeline<MessageContext<object>>
{
    private readonly int _maxRetries = 3;
    
    public async Task InvokeAsync(MessageContext<object> context, Func<MessageContext<object>, Task> next)
    {
        int retryCount = 0;
        
        while (retryCount < _maxRetries)
        {
            try
            {
                await next(context);
                return; // 成功则退出循环
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount == _maxRetries)
                    throw; // 达到最大重试次数，抛出异常
                    
                // 指数退避策略
                await Task.Delay(1000 * retryCount, context.CancellationToken);
            }
        }
    }
}
```

#### 实现性能监控中间件

```csharp
public class PerformanceMonitorMiddleware : IPluginPipeline<MessageContext<object>>
{
    private readonly IMetricsService _metricsService;
    
    public PerformanceMonitorMiddleware(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }
    
    public async Task InvokeAsync(MessageContext<object> context, Func<MessageContext<object>, Task> next)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await next(context);
            stopwatch.Stop();
            _metricsService.RecordSuccess(context.QueueName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsService.RecordFailure(context.QueueName, ex, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

---

### 场景 3: 后台任务管道

#### 定义任务上下文

```csharp
public class JobContext
{
    public string JobId { get; set; }
    public string JobType { get; set; }
    public JobDataMap Data { get; set; }
    public DateTime ScheduledTime { get; set; }
}
```

#### 实现事务中间件

```csharp
public class TransactionMiddleware : IPluginPipeline<JobContext>
{
    private readonly IDbTransactionScope _transactionScope;
    
    public TransactionMiddleware(IDbTransactionScope transactionScope)
    {
        _transactionScope = transactionScope;
    }
    
    public async Task InvokeAsync(JobContext context, Func<JobContext, Task> next)
    {
        using (var scope = _transactionScope.BeginTransaction())
        {
            try
            {
                await next(context);
                scope.Commit(); // 提交事务
            }
            catch
            {
                scope.Rollback(); // 回滚事务
                throw;
            }
        }
    }
}
```

---

## 对比总结

| 特性 | IDynamicMiddleware | IPluginPipeline<TContext> |
|------|-------------------|--------------------------|
| **适用场景** | HTTP 请求管道 | 任意管道（工作流、消息队列、后台任务等） |
| **上下文类型** | HttpContext | 任意泛型类型 TContext |
| **框架依赖** | ASP.NET Core | 无（框架无关） |
| **委托类型** | RequestDelegate | Func<TContext, Task> |
| **典型用途** | 认证、授权、日志、限流 | 业务验证、事务控制、重试、监控 |

---

## 最佳实践

### ✅ 推荐做法

1. **HTTP 相关逻辑** → 使用 `IDynamicMiddleware`
   - 请求/响应修改
   - HTTP 级别的认证授权
   - 基于 HttpContext 的日志记录

2. **业务逻辑** → 使用 `IPluginPipeline<TContext>`
   - 工作流执行控制
   - 消息队列处理
   - 后台任务编排
   - 事务管理

3. **职责分离**
   - 每个中间件只负责一个功能
   - 避免在单个中间件中处理过多逻辑

4. **异常处理**
   - 在合适的层级捕获和处理异常
   - 不要在中间件中吞掉所有异常

### ❌ 不推荐做法

1. **混用两种中间件**
   ```csharp
   // ❌ 错误：在 IPluginPipeline 中使用 HttpContext
   public class BadMiddleware : IPluginPipeline<MyContext>
   {
       public Task InvokeAsync(MyContext context, Func<MyContext, Task> next)
       {
           var httpContext = ...; // 试图获取 HttpContext
           // 这违反了设计原则
       }
   }
   ```

2. **过度复杂的中间件**
   ```csharp
   // ❌ 错误：一个中间件做太多事情
   public class TooMuchMiddleware : IPluginPipeline<WorkflowContext>
   {
       public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
       {
           // 做了日志、权限、事务、重试、通知... 
           // 应该拆分成多个单一职责的中间件
       }
   }
   ```

---

## 扩展阅读

- [ASP.NET Core 中间件文档](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/)
- [责任链模式](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern)
- [Elsa Workflows 文档](https://elsa-workflows.github.io/elsa-core/)
