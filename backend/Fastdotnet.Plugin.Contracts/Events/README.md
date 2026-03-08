# 通用事件总线系统 - 快速开始指南

## 📦 已完成的工作

### 1. 核心接口和基类 ✅

**位置**: `Fastdotnet.Plugin.Contracts/Events/`

- ✅ `IEventBus.cs` - 事件总线接口
- ✅ `IEventHandler.cs` - 事件处理器接口（与 IEventBus 同文件）
- ✅ `EventAttribute.cs` - 事件特性标记
- ✅ `EventBase.cs` - 事件基类（包含 DomainEvent、IntegrationEvent）

### 2. 事件定义 ✅

**支付事件** (`PaymentEvents.cs`):
- ✅ `PaymentRequestedEvent` - 支付请求事件
- ✅ `PaymentCompletedEvent` - 支付完成事件
- ✅ `PaymentFailedEvent` - 支付失败事件

**系统事件** (`SystemEvents.cs`):
- ✅ `PluginInstalledEvent` - 插件安装事件
- ✅ `PluginUninstalledEvent` - 插件卸载事件
- ✅ `ConfigurationChangedEvent` - 配置变更事件

### 3. 主应用实现 ✅

**位置**: `Fastdotnet.WebApi/`

- ✅ `Services/EventBus/InMemoryEventBus.cs` - 内存事件总线实现
- ✅ `Extensions/EventBusBuilder.cs` - 服务注册和初始化扩展方法

---

## 🚀 如何使用

### 第一步：在 Program.cs 中注册事件总线

```csharp
// Fastdotnet.WebApi/Program.cs

var builder = WebApplication.CreateBuilder(args);

// 添加事件总线服务
builder.Services.AddEventBus();

// 其他服务注册...
var app = builder.Build();

// 初始化事件总线（自动订阅所有处理器）
await app.Services.InitializeEventBusAsync();

Console.WriteLine("✅ 事件总线初始化完成");

app.Run();
```

### 第二步：发布事件

```csharp
// 在任何服务或控制器中注入 IEventBus
public class OrderService
{
    private readonly IEventBus _eventBus;
    
    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task PayOrderAsync(string orderId)
    {
        // 订单支付逻辑...
        
        // 发布支付完成事件
        await _eventBus.PublishAsync(new PaymentCompletedEvent
        {
            OrderId = orderId,
            TransactionNo = $"TXN_{Guid.NewGuid():N}",
            PaidAmount = 100.00m,
            PaidAt = DateTime.UtcNow,
            PaymentChannel = "Alipay",
            Data = new Dictionary<string, object>
            {
                ["UserId"] = "user123",
                ["ItemCount"] = 2
            }
        });
    }
}
```

### 第三步：订阅事件

```csharp
// 创建事件处理器
public class SendOrderEmailHandler : IEventHandler<PaymentCompletedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendOrderEmailHandler> _logger;
    
    public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            // 从扩展数据中获取用户 ID
            var userId = @event.Data.GetValueOrDefault("UserId") as string;
            
            // 获取用户邮箱并发送邮件
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            
            await _emailService.SendAsync(
                user.Email,
                "订单支付成功",
                $"您的订单 {@event.OrderId} 已支付成功，金额：{@event.PaidAmount:C}",
                cancellationToken
            );
            
            _logger.LogInformation($"已发送支付成功邮件：{@event.OrderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送支付成功邮件失败");
        }
    }
}
```

---

## 📊 事件分类

### 已实现的事件

| 事件名称 | 分组 | 持久化 | 用途 |
|---------|------|--------|------|
| PaymentRequested | Payment | ❌ | 发起支付请求 |
| PaymentCompleted | Payment | ✅ | 支付成功通知 |
| PaymentFailed | Payment | ✅ | 支付失败通知 |
| PluginInstalled | System | ✅ | 插件安装通知 |
| PluginUninstalled | System | ✅ | 插件卸载通知 |
| ConfigurationChanged | System | ✅ | 配置变更通知 |

### 待扩展的事件

可以在以下目录继续添加事件：

- `User/` - 用户相关事件（注册、登录、权限变更等）
- `Order/` - 订单相关事件（创建、支付、发货、取消等）
- `Content/` - 内容相关事件（文章发布、评论创建等）
- `Inventory/` - 库存相关事件
- `Notification/` - 通知相关事件

---

## 💡 最佳实践

### 1. 事件命名规范

✅ **推荐**:
- `PaymentCompletedEvent` - 过去时态，表示已发生
- `OrderCreatedEvent` - 清晰的动词 + 名词
- `UserPermissionsChangedEvent` - 明确的变化类型

❌ **不推荐**:
- `PaymentEvent` - 太模糊
- `DoPaymentEvent` - 现在时态
- `OrderEvent` - 不明确是创建、更新还是删除

### 2. 事件设计原则

✅ **应该做的**:
- 保持事件简单，只包含必要的数据
- 使用 `Data` 字典传递扩展信息
- 为事件添加 `[Event]` 特性标记
- 继承 `DomainEvent` 或 `IntegrationEvent`

❌ **不应该做的**:
- 在事件中包含复杂的业务逻辑
- 事件过大（超过 10 个属性）
- 循环依赖（A 事件触发 B 事件，B 又触发 A 事件）

### 3. 处理器设计

✅ **推荐**:
```csharp
public class OrderPaidHandler : IEventHandler<PaymentCompletedEvent>
{
    // 单一职责：只处理订单支付后的逻辑
    public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken ct)
    {
        // 1. 更新订单状态
        // 2. 扣减库存
        // 3. 发送通知
    }
}
```

❌ **不推荐**:
```csharp
public class EverythingHandler : 
    IEventHandler<PaymentCompletedEvent>,
    IEventHandler<OrderCreatedEvent>,
    IEventHandler<UserRegisteredEvent>
{
    // 违反了单一职责原则
}
```

---

## 🔍 调试和监控

### 查看日志

事件总线会自动记录以下日志：

```
info: Fastdotnet.WebApi.Services.EventBus.InMemoryEventBus[0]
      订阅事件：PaymentCompletedEvent -> SendOrderEmailHandler
      
info: Fastdotnet.WebApi.Extensions.EventBusBuilder[0]
      开始初始化事件总线...
      
info: Fastdotnet.WebApi.Extensions.EventBusBuilder[0]
      发现 5 个事件处理器
      
info: Fastdotnet.WebApi.Extensions.EventBusBuilder[0]
      事件总线初始化完成，共订阅 5 个处理器
```

### 性能监控

未来可以添加：
- 事件处理时间统计
- 失败重试机制
- 死信队列
- 事件持久化（Event Store）

---

## 📝 下一步计划

### 短期（本周）

1. ✅ 在 Program.cs 中集成事件总线
2. ✅ 测试支付流程（发布 PaymentRequestedEvent）
3. ✅ 创建商城订单支付处理器

### 中期（下周）

4. ✅ 添加用户事件（UserRegisteredEvent 等）
5. ✅ 添加订单事件（OrderCreatedEvent 等）
6. ✅ 实现事件持久化（可选）

### 长期（下个月）

7. ✅ 分布式事件总线（RabbitMQ/Redis）
8. ✅ 事件溯源（Event Sourcing）
9. ✅ Saga 模式（分布式事务）

---

## 📚 参考资料

- [通用事件总线系统架构](./通用事件总线系统架构.md)
- [支付插件事件驱动架构实现计划](./支付插件事件驱动架构实现计划.md)
- [事件驱动架构模式](https://docs.microsoft.com/zh-cn/azure/architecture/patterns/)

---

*最后更新时间：2026-03-08*
