# 扩展点总表

> **命名空间速查**（插件里最容易找错的地方）：
> `IPlugin`/`PluginBase`/`IEventBus`/`IPluginToolProvider` → `Fastdotnet.Plugin.Contracts`；
> `IPermissionProvider`/`IStorageContext` → `Fastdotnet.Core.IService.Sys`；
> `PluginContext` → `Fastdotnet.Core.Plugin`；`IFdAppUserExtensionHandler<T>` → `Fastdotnet.Core.Extensibility.Users`；
> `IDynamicMiddleware`/`PluginPipelineRegistry<T>` → `Fastdotnet.Core.Middleware`；
> 事件总线特性 → `Fastdotnet.Plugin.Contracts.Events`；对外推送特性 → `Fastdotnet.Core.Notification`（**两个同名 EventContract，别混用**）。

插件能力都通过"接口 + `ConfigureServices` 注册"接入宿主。下表是当前真实存在的扩展点；带"自动"的由加载器扫描注册，不用你写。

| 能力 | 契约 | 注册位置 | 生效时机 |
| --- | --- | --- | --- |
| 生命周期 | `IPlugin` / `PluginBase` | 程序集内唯一实现（自动注册） | 启用 / 停用 |
| HTTP 接口 | `ControllerBase` + `[ApiUsageScope]` | **自动**注册所有控制器 | 每次请求 |
| 权限声明 | `IPermissionProvider` | 自动注册全部实现 | 启用时同步到 `fd_permission` |
| 菜单/配置初始化 | `IApplicationInitializer` | 自动注册全部实现 | 启用时按 `Order` 执行 |
| 事件订阅 | `IEventHandler<T>` | 在 `ConfigureServices` 里 `As<IEventHandler<T>>()` | 启用时自动订阅，停用自动退订 |
| 事件发布 | `IEventBus` | 构造函数注入 | 随时 |
| 可靠推送（Outbox） | `IEventPublisher` + `EventContext` + `FlushOutboxAsync` | 注入 | 业务事务内 |
| SignalR 方法 | `IPluginSignalRProvider`（或继承 `AutoSignalRProvider`） | 自动注册全部实现 | 启用时注册到 `UniversalHub` |
| 指标 | `IMetricProvider` | 在 `ConfigureServices` 注册 | 指标查询时 |
| AI 工具（MCP） | `IPluginToolProvider` | 在 `ConfigureServices` 注册 | Agent 列工具/调用时 |
| 文件存储 | `IStorageService` / `IStorageContext` | 注入（宿主实现） | 随时 |
| 工作流 | `IWorkflowManager` | 注入（需工作流插件就绪，`IsReady`） | 随时 |
| 用户扩展数据 | `IFdAppUserExtensionHandler<T>` | `ConfigureServices` 注册 + 实体表 | 用户创建/读取 |
| HTTP 中间件 | `IDynamicMiddleware` + `DynamicMiddlewareRegistry.Register` | 类型注册 DI + 初始化器里注册 | 每个请求（全局管道） |
| 泛型业务管道 | `IPluginPipeline<TContext>` + `PluginPipelineRegistry<TContext>` | 初始化器里注册 | 显式调度时 |
| 分支网关 / 反向代理 | `PluginBranchRegistry` / `PluginReverseProxyRegistry` | `OnInitializeAsync` | 特定前缀请求 |
| 跨插件调用 | `IEventBus` 弱类型事件 | 无（运行时按 key 订阅） | 随时 |

---

## 1. 事件总线（插件间首选）

> `using Fastdotnet.Plugin.Contracts.Events;`（`EventContractAttribute` 的短名参数会自动拼成 `{pluginId}.{name}`）

强类型（需要共享事件类型，通常用于宿主与插件）：

```csharp
[EventContract("order.created", Description = "订单创建", Group = "Order")]
public class OrderCreatedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    public string OrderId { get; set; } = "";
    public decimal Amount { get; set; }
}
```

弱类型（跨插件，**推荐**；key 格式 `{pluginId}.{eventName}`，支持 `*` 通配）：

```csharp
// 发布
await _eventBus.PublishAsync($"{pluginId}.order.created", new { OrderId = id, Amount = 99m });

// 订阅（在 OnStartAsync / 初始化器里）
_subscription = _eventBus.Subscribe("*.order.created", async payload =>
{
    _logger.LogInformation("收到事件 {Payload}", payload);
});
```

- 强类型处理器：实现 `IEventHandler<TEvent>`，在 `ConfigureServices` 里注册为 `IEventHandler<TEvent>`；
  宿主启用时自动订阅，停用自动退订。**自己手动 `Subscribe` 的要在 `OnStopAsync/OnUnloadAsync` 里 `Dispose`。**
- 事件类型标 `[EventContract("name", ...)]` 会进事件目录（`/api/eventcatalog/spec`、`/api/eventcatalog/ui`），
  命名不要带插件前缀之外的全局词，避免冲突。
- `PaymentEvents`/`SystemEvents` 里已有的事件（`PaymentCompletedEvent`、`PluginInstalledEvent` 等）可直接订阅；
  示例见 `backend/Plugins/PluginA/Backend/EventHandlers/`。

## 2. 可靠推送（Outbox + SignalR + Webhook）

> `using Fastdotnet.Core.Notification;`，这里的 `[EventContract("com.xxx.v1")]` 与事件总线的同名特性**不是同一个**：
> 它用于对外推送（Webhook/SignalR）并把事件写入 `SysOutbox`。

事件总线是**进程内**的；需要跨进程/可重试的推送用 `Fastdotnet.Core.Notification`：

```csharp
[EventContract("com.myplugin.order.created.v1", Description = "订单创建")]
public record OrderCreatedEvent(string OrderId, decimal Amount) : IFastdotnetEvent;

public class OrderService
{
    private readonly ISqlSugarClient _db;
    private readonly IEventPublisher _publisher;
    private readonly EventContext _events;

    public async Task CreateAsync(Order order)
    {
        await _db.Insertable(order).ExecuteCommandAsync();
        await _publisher.PublishAsync(new OrderCreatedEvent(order.Id, order.Amount));
        await _db.FlushOutboxAsync(_events);      // 与业务写入同事务，保证不丢
    }
}
```

- 事件类型名用反向域名 + 版本（`com.myplugin.order.created.v1`），避免与其他插件冲突。
- `FlushOutboxAsync` 必须在事务内调用，否则"业务成功、事件丢失"或反之。

- `OutboxDispatcher` 后台投递；支持 SignalR 与 Webhook（HMAC-SHA256），失败进死信表。
- 对外事件规范：`/_events/spec`、`/_events/ui`。

## 3. SignalR

方式 A（推荐，强类型）：继承 `AutoSignalRProvider`，用 `[SignalRMethod]` 标注：

```csharp
public class MySignalRProvider : AutoSignalRProvider
{
    public override string PluginId => "12345678901234567";

    [SignalRMethod("Echo")]
    public Task<object> EchoAsync(string message, HubCallerContext context)
        => Task.FromResult<object>(new { received = message, user = context.UserIdentifier });
}
```

方式 B：直接实现 `IPluginSignalRProvider.RegisterMethods(ISignalRMethodRegistry registry)`。

- 子类会被**自动注册**（实现 `IPluginSignalRProvider` 即可，示例见 `PluginA/Providers/PluginASignalRProvider.cs`）。
- 客户端统一走 `UniversalHub`（`/universalhub`）：`InvokePluginMethod(pluginId, methodName, args)`；
  也可用 `JoinPluginRoom/SendToPluginRoom` 做房间消息。
- 服务端主动推：注入 `IHubContext<UniversalHub>`，`Clients.All.SendAsync("pluginNotificationReceived", payload)`。
- 前端 `baseSignalRManager` 由宿主 props 注入，不要自己 new 连接。

## 4. 指标

```csharp
public class MyMetricProvider : IMetricProvider
{
    public string PluginId => "12345678901234567";
    public Task<IEnumerable<IMetricDefinition>> GetMetricDefinitionsAsync() => ...;
    public Task<MetricResult> CalculateAsync(string metricId, MetricQueryContext context) => ...;
}
```

在 `ConfigureServices` 注册：`builder.RegisterType<MyMetricProvider>().As<IMetricProvider>().InstancePerLifetimeScope();`
指标定义用 `MetricDefinition`（`Id`、`Name`、`Category`、`Dimensions`、`Tags`），未知指标抛 `MetricNotFoundException`。
示例：`PluginA/Metrics/PluginAMetricProvider.cs` 与 `Controllers/Metrics/PluginAMetricsController.cs`。

## 5. AI 工具（MCP）

```csharp
public class MyToolProvider : IPluginToolProvider
{
    public IEnumerable<PluginToolDescriptor> GetTools() => new[]
    {
        new PluginToolDescriptor
        {
            Name = "query_thing",                     // snake_case
            Title = "查询物品",
            Description = "按物品编号查询详情。当用户询问物品状态、位置或数量时使用。返回名称/状态/库存。",
            Parameters = new[]
            {
                new PluginToolParameter { Name = "thingNo", Type = ToolParameterType.String, Required = true, Description = "物品编号" }
            },
            RequiredPermission = MyPluginPermissions.ThingView,   // null = 登录即可
            IsDestructive = false,
            TimeoutSeconds = 15
        }
    };

    public Task<ToolInvocationResult> InvokeAsync(string toolName, ToolInvocationContext ctx, CancellationToken ct)
        => toolName switch
        {
            "query_thing" => Query(ctx),
            _ => Task.FromResult(ToolInvocationResult.Fail("TOOL_NOT_FOUND", $"未知工具：{toolName}"))
        };

    private Task<ToolInvocationResult> Query(ToolInvocationContext ctx)
    {
        var no = ctx.Arguments.GetValueOrDefault("thingNo")?.ToString();
        if (string.IsNullOrWhiteSpace(no))
            return Task.FromResult(ToolInvocationResult.Fail("INVALID_ARGUMENT", "thingNo 不能为空"));

        // 插件自己的服务从插件 scope 解析，不要碰宿主全局容器
        var service = ctx.PluginServices!.Resolve<MyThingService>();
        return Task.FromResult(ToolInvocationResult.Ok($"物品 {no} 状态正常"));
    }
}
```

- **注册**：`builder.RegisterType<MyToolProvider>().As<IPluginToolProvider>().InstancePerLifetimeScope();`（注册器只从插件 scope 取一个实现）。
- 对外工具名会被规整成 `fd_{pluginId}_{name}`（小写、去特殊字符、最长 64）。
- `RequiredPermission` 必须与 `IPermissionProvider` 声明的权限码一致；无权限者**看不到也调不动**。
- `IsDestructive=true` 的写操作在生产应走人工确认；`Description` 是 Agent 选对工具的关键，写清场景/参数/返回/限制。
- 端点 `/mcp`（Streamable HTTP，要求 JWT）；宿主内置工具与插件工具统一列表。
- 完整示例：`backend/Plugins/AiToolDemo/`（含"无权限看不到"验证用例）。

## 6. 存储

> `IStorageContext` 在 `Fastdotnet.Core.IService.Sys`；`IStorageService` 是宿主注册的文件存储服务。

- `IStorageContext`：最薄的 SQL 能力（`QuerySingleOrDefaultAsync`、`ExecuteNonQueryAsync`、`SaveEntityAsync`），
  适合插件自有表的小查询或用户扩展处理器内部使用。
- `IStorageService`（宿主注册）：文件上传/下载/删除，支持本地与 OSS 实现；**不要直接写 `wwwroot`**。
- 上传大小/类型/桶由宿主配置（`Storage:*`）；插件只传相对路径与业务目录。

## 7. 工作流

```csharp
public class MyWorkflowStarter
{
    private readonly IWorkflowManager _workflow;
    public MyWorkflowStarter(IWorkflowManager workflow) => _workflow = workflow;

    public async Task<string> StartAsync(string bizId)
    {
        if (!_workflow.IsReady) throw new BusinessException("工作流插件未启用");
        return await _workflow.StartWorkflowAsync("my-approval", new { BizId = bizId }, correlationId: bizId);
    }
}
```

`IWorkflowManager` 由工作流插件提供（Elsa 等）；`IsReady=false` 时不要硬调，给出业务提示。
插件之间不直接引用程序集，**通过接口 + 事件协作**。

## 8. 用户扩展数据

> `using Fastdotnet.Core.Extensibility.Users;`（`IFdAppUserExtensionHandler<T>` 不在 Contracts）。

给宿主用户表挂插件私有数据：

```csharp
[SugarTable("myplugin_user_extension", "用户扩展")]
public class MyUserExtension : BaseEntity
{
    [SugarColumn(IsNullable = false)] public string UserId { get; set; } = "";
    [SugarColumn(IsNullable = true)] public int Points { get; set; }
}

public class MyUserExtensionHandler : IFdAppUserExtensionHandler<MyUserExtension>
{
    public async Task SaveAsync(string userId, MyUserExtension data, IStorageContext ctx, CancellationToken ct = default)
    {
        data.UserId = userId;
        await ctx.SaveEntityAsync(data, ct);
    }

    public async Task<MyUserExtension?> LoadAsync(string userId, IStorageContext ctx, CancellationToken ct = default)
        => await ctx.QuerySingleOrDefaultAsync<MyUserExtension>(
            "select * from myplugin_user_extension where user_id = @userId and is_deleted = 0",
            new { userId }, ct);
}
```

注册：`builder.RegisterType<MyUserExtensionHandler>().As<IFdAppUserExtensionHandler<MyUserExtension>>().InstancePerLifetimeScope();`
宿主用户服务（`IFdAppUserService`）提供 `GetFdAppUserWithExtensionAsync<T>` / `CreateFdAppUserWithExtensionAsync`。
完整示例：`PluginA/Controllers/PluginAUserController.cs` + `Entities/PluginAUserExtension.cs`。

## 9. HTTP 中间件与业务管道

全局 HTTP 中间件（谨慎使用，影响所有请求）：

```csharp
public class MyMiddleware : IDynamicMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 只处理自己关心的路径，尽快 next
        await next(context);
    }
}

// ConfigureServices：注册类型
builder.RegisterType<MyMiddleware>().AsSelf().InstancePerLifetimeScope();

// OnInitializeAsync：登记到动态管道
sp.GetService<DynamicMiddlewareRegistry>()?.Register(typeof(MyMiddleware));

// OnUnloadAsync：反注册（Registry 用弱引用，但显式反注册更干净）
sp.GetService<DynamicMiddlewareRegistry>()?.Unregister(typeof(MyMiddleware));
```

泛型业务管道（插件内部业务切面，不经过 HTTP）：

```csharp
public class MyValidationBehavior : IPluginPipeline<MyContext>
{
    public async Task InvokeAsync(MyContext ctx, Func<MyContext, Task> next) { /* 前置 */ await next(ctx); /* 后置 */ }
}

var registry = sp.GetService<PluginPipelineRegistry<MyContext>>();
registry?.Register(typeof(MyValidationBehavior));
```

示例：`PluginA/Services/BusinessOperationExecutor.cs` 与 `Middleware/BusinessOperation*Middleware.cs`。

## 10. 分支网关与反向代理（特殊场景）

- **分支网关**：插件要把某个路径前缀（如 `/myplugin-api`）的请求完全接管，用 `PluginBranchRegistry.MapBranch(prefix, handler, serviceProvider)`。
  适合自带微型框架、需要独立管道的场景。
- **反向代理逃生舱**：插件自带 Kestrel 等 Web 服务器时，注册到 `PluginReverseProxyRegistry`，由 YARP 透明转发。
- 这两个能力会绕过宿主的认证/加密等中间件（取决于注册顺序），**必须自己保证安全**（鉴权、限流、输入校验）。

## 11. 跨插件调用的正确姿势

1. 优先：弱类型事件（`IEventBus.PublishAsync("{pluginId}.xxx", payload)` + `Subscribe("*.xxx")`）。
2. 其次：共享契约放在宿主（或独立的契约包），双方都引用契约而不是对方的实现程序集。
3. 禁止：`Assembly.Load` 对方 DLL、反射调用对方内部类型、把对方服务塞进自己的 `ConfigureServices`。
   插件有独立 ALC，类型身份不同，强转必炸。
