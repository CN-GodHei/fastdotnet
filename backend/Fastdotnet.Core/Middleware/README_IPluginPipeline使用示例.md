# IPluginPipeline 完整使用示例

## 1. 架构说明

`IPluginPipeline<TContext>` 不仅仅是一个接口，它包含三个核心组件：

### 组件关系图

```
┌─────────────────────────────────────────────────────────────┐
│  插件开发者                                                    │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  1. 定义上下文                                                │
│     public class WorkflowContext { ... }                     │
│                                                              │
│  2. 实现中间件                                                │
│     public class LoggingMiddleware                           │
│         : IPluginPipeline<WorkflowContext>                   │
│                                                              │
│  3. 注册到 DI 容器                                             │
│     services.AddSingleton(                                   │
│         typeof(PluginPipelineRegistry<WorkflowContext>)      │
│     );                                                       │
│                                                              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Fastdotnet.Core.Middleware                                  │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────┐    ┌──────────────────┐              │
│  │ IPluginPipeline  │    │ PluginPipeline   │              │
│  │ <TContext>       │    │ Registry<TContext>│              │
│  ├──────────────────┤    ├──────────────────┤              │
│  │ + InvokeAsync()  │    │ + Register()     │              │
│  └──────────────────┘    │ + Unregister()   │              │
│                          │ + GetTypes()     │              │
│                          └──────────────────┘              │
│                                    ↓                        │
│                          ┌──────────────────┐              │
│                          │ PluginPipeline   │              │
│                          │ Dispatcher<TContext>│            │
│                          ├──────────────────┤              │
│                          │ + ExecuteAsync() │              │
│                          │ - BuildPipeline()│              │
│                          └──────────────────┘              │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. 完整实现示例：Elsa 工作流管道

### 步骤 1: 定义工作流上下文

```csharp
// 文件位置：Fastdotnet.Elsa/Contexts/WorkflowContext.cs
namespace Fastdotnet.Elsa.Contexts
{
    /// <summary>
    /// 工作流执行上下文
    /// </summary>
    public class WorkflowContext
    {
        /// <summary>
        /// 工作流定义 ID
        /// </summary>
        public string WorkflowDefinitionId { get; set; }
        
        /// <summary>
        /// 工作流实例 ID
        /// </summary>
        public string WorkflowInstanceId { get; set; }
        
        /// <summary>
        /// 工作流变量
        /// </summary>
        public IDictionary<string, object> Variables { get; set; } 
            = new Dictionary<string, object>();
        
        /// <summary>
        /// 触发器类型（手动、定时、API 等）
        /// </summary>
        public string TriggerType { get; set; }
        
        /// <summary>
        /// 当前用户 ID（如果有）
        /// </summary>
        public string? CurrentUserId { get; set; }
        
        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
```

### 步骤 2: 实现中间件

#### 2.1 日志记录中间件

```csharp
// 文件位置：Fastdotnet.Elsa/Middlewares/WorkflowLoggingMiddleware.cs
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Elsa.Middlewares
{
    /// <summary>
    /// 工作流日志记录中间件
    /// </summary>
    public class WorkflowLoggingMiddleware : IPluginPipeline<WorkflowContext>
    {
        private readonly ILogger<WorkflowLoggingMiddleware> _logger;

        public WorkflowLoggingMiddleware(ILogger<WorkflowLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
        {
            _logger.LogInformation(
                "========== 开始执行工作流 ==========\n" +
                "工作流定义 ID: {WorkflowDefinitionId}\n" +
                "工作流实例 ID: {WorkflowInstanceId}\n" +
                "触发器类型：{TriggerType}\n" +
                "当前用户 ID: {CurrentUserId}",
                context.WorkflowDefinitionId,
                context.WorkflowInstanceId,
                context.TriggerType,
                context.CurrentUserId);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await next(context); // 执行下一个中间件
                
                stopwatch.Stop();
                _logger.LogInformation(
                    "工作流执行成功，耗时：{ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "工作流执行失败，耗时：{ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
                throw; // 重新抛出异常
            }
        }
    }
}
```

#### 2.2 权限校验中间件

```csharp
// 文件位置：Fastdotnet.Elsa/Middlewares/WorkflowPermissionMiddleware.cs
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;
using Fastdotnet.Elsa.Services;

namespace Fastdotnet.Elsa.Middlewares
{
    /// <summary>
    /// 工作流权限校验中间件
    /// </summary>
    public class WorkflowPermissionMiddleware : IPluginPipeline<WorkflowContext>
    {
        private readonly IWorkflowPermissionService _permissionService;

        public WorkflowPermissionMiddleware(IWorkflowPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
        {
            // 如果是定时触发器，跳过权限检查
            if (context.TriggerType == "Timer")
            {
                await next(context);
                return;
            }

            // 检查用户是否有执行该工作流的权限
            bool hasPermission = await _permissionService.CheckExecutePermissionAsync(
                context.CurrentUserId,
                context.WorkflowDefinitionId);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    $"用户 {context.CurrentUserId} 无权执行工作流 {context.WorkflowDefinitionId}");
            }

            _logger.LogInformation("权限校验通过");
            await next(context);
        }
    }
}
```

#### 2.3 性能监控中间件

```csharp
// 文件位置：Fastdotnet.Elsa/Middlewares/PerformanceMonitorMiddleware.cs
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;
using Fastdotnet.Elsa.Services;

namespace Fastdotnet.Elsa.Middlewares
{
    /// <summary>
    /// 性能监控中间件
    /// </summary>
    public class PerformanceMonitorMiddleware : IPluginPipeline<WorkflowContext>
    {
        private readonly IMetricsService _metricsService;

        public PerformanceMonitorMiddleware(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await next(context);
                
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                
                // 记录成功指标
                await _metricsService.RecordWorkflow执行成功Async(
                    context.WorkflowDefinitionId,
                    elapsedMs);
                    
                // 如果执行时间超过阈值，记录警告
                if (elapsedMs > 5000)
                {
                    _logger.LogWarning(
                        "工作流执行时间过长：{ElapsedMilliseconds}ms",
                        elapsedMs);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                // 记录失败指标
                await _metricsService.RecordWorkflow执行失败Async(
                    context.WorkflowDefinitionId,
                    ex,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
```

### 步骤 3: 注册服务（在插件初始化时）

```csharp
// 文件位置：Fastdotnet.Elsa/ElsaPlugin.cs
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;
using Fastdotnet.Elsa.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Elsa
{
    public class ElsaPlugin : IPlugin
    {
        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // 1. 注册工作流上下文相关的服务
            // （这些服务会被 PluginPipelineDispatcher 用于依赖注入）
            
            // 2. 注册管道注册表（单例）
            serviceProvider.GetRequiredService<IServiceCollection>()
                .AddSingleton<PluginPipelineRegistry<WorkflowContext>>();
            
            // 3. 注册管道调度器（单例）
            serviceProvider.GetRequiredService<IServiceCollection>()
                .AddSingleton<PluginPipelineDispatcher<WorkflowContext>>();
            
            // 4. 注册中间件到 DI 容器（支持依赖注入）
            serviceProvider.GetRequiredService<IServiceCollection>()
                .AddTransient<WorkflowLoggingMiddleware>()
                .AddTransient<WorkflowPermissionMiddleware>()
                .AddTransient<PerformanceMonitorMiddleware>();
            
            // 5. 将中间件类型注册到管道注册表
            var registry = serviceProvider.GetRequiredService<
                PluginPipelineRegistry<WorkflowContext>>();
            
            registry.Register(typeof(WorkflowLoggingMiddleware));
            registry.Register(typeof(WorkflowPermissionMiddleware));
            registry.Register(typeof(PerformanceMonitorMiddleware));
            
            return Task.CompletedTask;
        }
    }
}
```

### 步骤 4: 使用管道执行工作流

```csharp
// 文件位置：Fastdotnet.Elsa/Services/WorkflowExecutor.cs
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;

namespace Fastdotnet.Elsa.Services
{
    public class WorkflowExecutor
    {
        private readonly PluginPipelineDispatcher<WorkflowContext> _dispatcher;
        private readonly IWorkflowEngine _workflowEngine;

        public WorkflowExecutor(
            PluginPipelineDispatcher<WorkflowContext> dispatcher,
            IWorkflowEngine workflowEngine)
        {
            _dispatcher = dispatcher;
            _workflowEngine = workflowEngine;
        }

        /// <summary>
        /// 执行工作流（会经过所有注册的中间件）
        /// </summary>
        public async Task ExecuteWorkflowAsync(string workflowDefinitionId, string userId)
        {
            // 创建工作流上下文
            var context = new WorkflowContext
            {
                WorkflowDefinitionId = workflowDefinitionId,
                WorkflowInstanceId = Guid.NewGuid().ToString(),
                TriggerType = "Manual",
                CurrentUserId = userId,
                Variables = new Dictionary<string, object>(),
                CancellationToken = CancellationToken.None
            };

            // 通过管道执行工作流
            await _dispatcher.ExecuteAsync(context, async (ctx) =>
            {
                // 这是管道的最终执行动作（真正的业务逻辑）
                await _workflowEngine.RunAsync(
                    ctx.WorkflowDefinitionId,
                    ctx.WorkflowInstanceId,
                    ctx.Variables,
                    ctx.CancellationToken);
            });
        }
    }
}
```

---

## 3. 中间件执行流程

```
用户调用 ExecuteWorkflowAsync()
    ↓
创建 WorkflowContext
    ↓
PluginPipelineDispatcher.ExecuteAsync()
    ↓
构建管道链（从后向前）:
    PerformanceMonitorMiddleware
        → PermissionMiddleware
            → LoggingMiddleware
                → 实际工作流执行
    ↓
执行管道（从前向后）:
    1. LoggingMiddleware.InvokeAsync()
       - 记录开始日志
       - 调用 next() → PermissionMiddleware
       - 记录结束日志
       
    2. PermissionMiddleware.InvokeAsync()
       - 检查权限
       - 调用 next() → PerformanceMonitorMiddleware
       
    3. PerformanceMonitorMiddleware.InvokeAsync()
       - 开始计时
       - 调用 next() → 实际工作流执行
       - 记录性能指标
       
    4. 实际工作流执行 (_workflowEngine.RunAsync)
    ↓
返回结果
```

---

## 4. 关键点说明

### ✅ 1. 依赖注入支持

```csharp
// 中间件可以通过构造函数注入服务
public class WorkflowLoggingMiddleware : IPluginPipeline<WorkflowContext>
{
    private readonly ILogger<WorkflowLoggingMiddleware> _logger;
    
    // ✅ 正确：通过构造函数注入
    public WorkflowLoggingMiddleware(ILogger<WorkflowLoggingMiddleware> logger)
    {
        _logger = logger;
    }
}
```

### ✅ 2. 类型安全验证

```csharp
// 注册时会验证类型是否正确实现了接口
public void Register(Type pipelineType)
{
    if (!typeof(IPluginPipeline<TContext>).IsAssignableFrom(pipelineType))
    {
        throw new ArgumentException(...);
    }
}

// ✅ 正确
registry.Register(typeof(WorkflowLoggingMiddleware));

// ❌ 错误：会在编译时或运行时抛出异常
registry.Register(typeof(SomeOtherClass)); 
```

### ✅ 3. WeakReference 防止内存泄漏

```csharp
private readonly ConcurrentDictionary<string, WeakReference<Type>> _pipelineTypes;

// 使用 WeakReference 持有类型引用
// 这样即使插件卸载，也不会因为强引用导致程序集无法释放
_pipelineTypes[pipelineType.FullName] = new WeakReference<Type>(pipelineType);
```

### ✅ 4. 责任链模式

```csharp
// 从后向前构建，从前向后执行
for (int i = pipelineTypes.Count - 1; i >= 0; i--)
{
    var middlewareType = pipelineTypes[i];
    var next = pipeline; // 保存下一个委托
    
    pipeline = async context =>
    {
        var middleware = ActivatorUtilities.CreateInstance(...);
        await middleware.InvokeAsync(context, next); // 调用下一个
    };
}
```

---

## 5. 测试示例

```csharp
// 文件位置：Fastdotnet.Elsa.Tests/WorkflowPipelineTests.cs
using Xunit;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Elsa.Contexts;
using Fastdotnet.Elsa.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Elsa.Tests
{
    public class WorkflowPipelineTests
    {
        [Fact]
        public async Task ExecuteWorkflow_WithAllMiddlewares_ShouldSucceed()
        {
            // Arrange
            var services = new ServiceCollection();
            
            // 注册依赖服务
            services.AddSingleton<IMetricsService, MetricsService>();
            
            // 注册管道服务
            services.AddSingleton<PluginPipelineRegistry<WorkflowContext>>();
            services.AddSingleton<PluginPipelineDispatcher<WorkflowContext>>();
            
            // 注册中间件
            services.AddTransient<WorkflowLoggingMiddleware>();
            services.AddTransient<WorkflowPermissionMiddleware>();
            services.AddTransient<PerformanceMonitorMiddleware>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            var dispatcher = serviceProvider.GetRequiredService<
                PluginPipelineDispatcher<WorkflowContext>>();
            
            var context = new WorkflowContext
            {
                WorkflowDefinitionId = "test-workflow",
                WorkflowInstanceId = Guid.NewGuid().ToString(),
                TriggerType = "Manual",
                CurrentUserId = "user-123"
            };
            
            bool finalActionExecuted = false;
            
            // Act
            await dispatcher.ExecuteAsync(context, ctx =>
            {
                finalActionExecuted = true;
                return Task.CompletedTask;
            });
            
            // Assert
            Assert.True(finalActionExecuted);
        }
    }
}
```

---

## 6. 常见问题

### Q1: 为什么不直接使用 IDynamicMiddleware？

**A:** `IDynamicMiddleware` 只能用于 HTTP 管道（依赖 HttpContext），而 `IPluginPipeline<TContext>` 是通用的，可以用于任何场景。

### Q2: 可以为同一个 TContext 注册多个中间件吗？

**A:** 可以！这正是设计的目的。多个中间件会按注册顺序形成责任链。

### Q3: 中间件的执行顺序是怎样的？

**A:** 按照注册的顺序执行。第一个注册的中间件最先执行。

### Q4: 如何在中间件中跳过后续中间件？

**A:** 不调用 `next(context)` 即可：

```csharp
public async Task InvokeAsync(WorkflowContext context, Func<WorkflowContext, Task> next)
{
    if (!HasPermission(context))
    {
        // 直接返回，不调用 next()
        throw new UnauthorizedAccessException("无权限");
    }
    
    await next(context); // 只有权限通过才继续
}
```

### Q5: 如何测试中间件？

**A:** 单独测试每个中间件，不依赖整个管道：

```csharp
[Fact]
public async Task LoggingMiddleware_ShouldLogExecution()
{
    var logger = new Mock<ILogger<WorkflowLoggingMiddleware>>();
    var middleware = new WorkflowLoggingMiddleware(logger.Object);
    
    var context = new WorkflowContext { ... };
    var nextCalled = false;
    Func<WorkflowContext, Task> next = ctx => 
    { 
        nextCalled = true; 
        return Task.CompletedTask; 
    };
    
    await middleware.InvokeAsync(context, next);
    
    Assert.True(nextCalled);
    // 验证日志是否记录
}
```

---

## 7. 总结

`IPluginPipeline<TContext>` 提供了一个完整的、可扩展的插件化管道机制：

✅ **完整的架构**：接口 + 注册表 + 调度器  
✅ **依赖注入支持**：中间件可以通过 DI 获取服务  
✅ **类型安全**：泛型确保编译时类型检查  
✅ **灵活扩展**：可以为任何场景定义专用上下文  
✅ **热插拔**：支持动态注册和注销中间件  
✅ **内存安全**：使用 WeakReference 防止内存泄漏  

现在你可以在插件中实现任意类型的中间件管道了！🎉
