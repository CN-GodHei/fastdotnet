# Fastdotnet.Core.Notification

Fastdotnet 框架底层事件推送基础设施。为所有上层模块和动态插件提供标准的异步事件发布与可靠投递能力。

**核心特性**：事务内落库 → 分布式抢占调度 → SignalR 实时推送 / Webhook 外部回调 → 指数退避重试 → 死信归仓。

## 架构全景

```
业务代码 / 动态插件
  │ IEventPublisher.PublishAsync(evt)  ← 延迟收集到 Scoped EventContext
  │ db.FlushOutboxAsync(context)       ← 事务内统一写入 SysOutbox
  ▼
┌────────────────────────────────────────────────┐
│ OutboxDispatcher (BackgroundService)           │
│  ├ Channel 内存信号 (毫秒级)                     │
│  ├ 定时轮询 (5s 兜底)                           │
│  └ SqlSugar 乐观锁 (多节点分布式抢占)            │
│       │                                        │
│       ▼                                        │
│ EventRouter                                    │
│  ├ 精确匹配 + 通配符 *                           │
│  ├ Subscribe / Unsubscribe / UnsubscribeAll    │
│  └ 快照路由 (线程安全)                          │
│       │                                        │
│       ├── SignalRSubscriber ──→ 前端实时推送    │
│       └── WebhookSubscriber  ──→ 第三方 HTTP 回调│
└────────────────────────────────────────────────┘
```

## 目录结构

```
Fastdotnet.Core.Notification/
├── Abstractions/
│   ├── EventContractAttribute.cs    ← [EventContract] 特性 + IFastdotnetEvent
│   ├── IEventPublisher.cs           ← 业务发布入口
│   └── IEventSubscriber.cs          ← 订阅者契约
├── Entities/
│   ├── OutboxStatus.cs              ← Pending/Processing/Published/Failed
│   ├── SysOutbox.cs                 ← 发件箱表
│   └── SysDeadLetter.cs             ← 死信表
├── Extensions/
│   └── NotificationServiceCollectionExtensions.cs  ← DI 注册扩展
└── Infrastructure/
    ├── EventContext.cs              ← Scoped 延迟收集器
    ├── EventPublisher.cs            ← IEventPublisher 实现
    ├── EventRouter.cs               ← 事件路由 + 订阅管理
    ├── NotificationHub.cs           ← SignalR Hub + SignalRSubscriber
    ├── OutboxDispatcher.cs          ← 后台调度器 (Channel + 轮询 + 锁)
    ├── OutboxTransactionExtensions.cs  ← FlushOutboxAsync 事务扩展
    └── WebhookSubscriber.cs         ← Webhook 投递 (HMAC-SHA256)
```

## 快速开始

### 1. 宿主注册

```csharp
// Program.cs
using Fastdotnet.Core.Notification;

builder.Services.AddFastdotnetNotification(options =>
{
    options.DispatcherOptions.PollingInterval = 5;    // 轮询间隔（秒）
    options.DispatcherOptions.BatchSize = 50;          // 批量大小
    options.DispatcherOptions.MaxRetryCount = 10;      // 最大重试次数
});
```

### 2. 定义事件

```csharp
using Fastdotnet.Core.Notification;

[EventContract("com.myapp.order.created.v1", Description = "订单创建事件")]
public record OrderCreatedEvent(
    string OrderId,
    decimal Amount,
    string CustomerId
) : IFastdotnetEvent;
```

### 3. 发布事件（事务内）

```csharp
public class OrderService
{
    private readonly ISqlSugarClient _db;
    private readonly IEventPublisher _publisher;
    private readonly EventContext _eventContext;

    public OrderService(ISqlSugarClient db, IEventPublisher publisher, EventContext eventContext)
    {
        _db = db;
        _publisher = publisher;
        _eventContext = eventContext;
    }

    public async Task CreateOrderAsync(OrderDto dto)
    {
        await _db.Ado.UseTranAsync(async () =>
        {
            // 1. 业务写入
            var entity = dto.ToEntity();
            await _db.Insertable(entity).ExecuteCommandAsync();

            // 2. 发布事件（延迟收集，不操作 DB）
            await _publisher.PublishAsync(new OrderCreatedEvent(
                entity.Id, entity.Amount, entity.UserId));

            // 3. 事务内统一落库 SysOutbox
            await _db.FlushOutboxAsync(_eventContext);
        });
    }
}
```

## API 参考

### IEventPublisher

```csharp
public interface IEventPublisher
{
    /// 发布事件到当前 Scoped 上下文（延迟持久化）
    ValueTask PublishAsync(IFastdotnetEvent @event, CancellationToken ct = default);
}
```

### EventContext

```csharp
public class EventContext
{
    /// 当前上下文中已收集的事件数量
    int Count { get; }

    /// 添加事件到收集器
    void Add(IFastdotnetEvent @event);

    /// 获取并清空所有已收集事件
    IReadOnlyList<(IFastdotnetEvent Event, DateTime Timestamp)> Drain();
}
```

### ISqlSugarClient 扩展

```csharp
/// 在当前事务内将 EventContext 中的事件批量写入 SysOutbox
public static async Task FlushOutboxAsync(
    this ISqlSugarClient db,
    EventContext context,
    string? tenantId = null,
    JsonSerializerOptions? jsonOptions = null,
    CancellationToken ct = default)
```

### EventRouter

```csharp
public class EventRouter
{
    void Subscribe(string eventType, IEventSubscriber subscriber);
    void Unsubscribe(string eventType, IEventSubscriber subscriber);
    void SubscribePattern(string pattern, IEventSubscriber subscriber);  // "com.*.order.*"
    void UnsubscribePattern(string pattern, IEventSubscriber subscriber);
    void UnsubscribeAll(IEventSubscriber subscriber);  // 插件卸载时用
    Task RouteAsync(string eventType, string payload, CancellationToken ct);
}
```

### EventContractAttribute

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class EventContractAttribute : Attribute
{
    string EventType { get; }     // 唯一标识，如 "com.fastdotnet.order.paid.v1"
    string? Description { get; }  // 可选描述
}
```

## 配置选项

| 选项 | 默认值 | 说明 |
|------|--------|------|
| `DispatcherOptions.PollingInterval` | 5 | 定时轮询间隔（秒） |
| `DispatcherOptions.BatchSize` | 50 | 每次抢占的最大消息数 |
| `DispatcherOptions.LockDurationSeconds` | 30 | 分布式锁持有时间（秒） |
| `DispatcherOptions.MaxRetryCount` | 10 | 最大重试次数，超过移入死信 |

## 数据表结构

### sys_outbox（发件箱）

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | char(36) PK | 事件唯一 ID |
| `tenant_id` | varchar(50) | 租户 ID |
| `source_module` | varchar(100) | 来源模块/插件 |
| `event_type` | varchar(150) | 事件类型标识 |
| `payload` | text | JSON 载荷 |
| `status` | int | 0-Pending, 1-Processing, 2-Published, 3-Failed |
| `retry_count` | int | 重试次数 |
| `next_attempt_time` | datetime | 下次重试时间 |
| `lock_id` | char(36) | 分布式锁持有者实例 ID |
| `lock_expired_time` | datetime | 锁过期时间 |

### sys_dead_letter（死信）

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | char(36) PK | 继承原事件 ID |
| `tenant_id` | varchar(50) | 租户 ID |
| `event_type` | varchar(150) | 事件类型 |
| `payload` | text | JSON 载荷 |
| `error_message` | text | 失败异常堆栈 |
| `failed_at` | datetime | 归入死信时间 |

## 插件集成指南

### 注册订阅者

```csharp
public class MyPlugin : PluginBase
{
    private IServiceProvider? _sp;
    private MySubscriber? _sub;

    protected override Task OnInitializeAsync(IServiceProvider sp)
    {
        _sp = sp;  // 保存引用
        return Task.CompletedTask;
    }

    protected override Task OnStartAsync()
    {
        var router = _sp!.GetRequiredService<EventRouter>();
        _sub = new MySubscriber(...);
        router.Subscribe("com.myplugin.*", _sub);  // 注册
        return Task.CompletedTask;
    }

    protected override Task OnUnloadAsync(IServiceProvider sp)
    {
        sp.GetRequiredService<EventRouter>().UnsubscribeAll(_sub!);  // 清理
        return Task.CompletedTask;
    }
}
```

### 注册 Webhook 订阅

```csharp
builder.Services.AddWebhookSubscriber(
    eventType: "com.fastdotnet.order.created.v1",
    webhookUrl: "https://partner.example.com/callback",
    secretKey:   "shared-secret-key");
```

Webhook 请求格式：
```
POST https://partner.example.com/callback
X-Fastdotnet-Signature: t=1718000000,v=abc123...
X-Event-Type: com.fastdotnet.order.created.v1
Content-Type: application/json

{"orderId":"...","amount":99.99}
```

签名算法：`HMAC-SHA256(timestamp + "." + payload, secretKey)`

## 投递保障

| 机制 | 说明 |
|------|------|
| **事务一致性** | 事件落库与业务数据在同一 SqlSugar 事务内 |
| **分布式抢占** | `UPDATE ... SET LockId=@id WHERE LockExpiredTime IS NULL` |
| **指数退避** | 失败后 `2¹, 2², 2³, ... 2ⁿ` 秒递增重试 |
| **死信机制** | 重试 10 次后移入 `sys_dead_letter`，可人工重放 |
| **双驱动调度** | Channel 内存信号（毫秒级）+ 定时轮询（5s 兜底） |

## 故障排查

| 现象 | 原因 | 解决 |
|------|------|------|
| 事件未投递 | 未调用 `FlushOutboxAsync` | 确保在事务内调用 |
| 重复投递 | 多实例未设置锁过期 | 检查 `LockDurationSeconds` 配置 |
| 死信堆积 | 订阅者处理失败 | 检查 `sys_dead_letter.error_message` |
| SignalR 无推送 | Hub 未映射 | 检查 `app.MapHub<NotificationHub>("/hubs/notification")` |
