# PluginA 事件总线使用示例

## 📋 概述

PluginA 作为演示插件，展示了如何在插件系统中使用**通用事件总线**实现：
- ✅ **发布事件** - 业务操作触发事件
- ✅ **订阅事件** - 响应其他插件的事件
- ✅ **事件驱动架构** - 完全解耦的插件间通信

---

## 🏗️ 文件结构

```
PluginA/
├── Events/                          # 定义 PluginA 的业务事件
│   └── MallOrderCreatedEvent.cs    # 商城订单创建事件
│
├── EventHandlers/                   # 事件处理器（订阅其他插件的事件）
│   ├── OrderPaidEventHandler.cs     # 处理支付完成事件
│   └── PluginInstalledEventHandler.cs  # 处理插件安装事件
│
├── Services/                        # 服务层（发布事件）
│   └── MallOrderService.cs         # 商城订单服务（发布订单事件）
│
└── Controllers/                     # API 控制器
    └── MallOrderController.cs       # 提供测试接口
```

---

## 💡 示例 1：定义业务事件

### MallOrderCreatedEvent.cs

```csharp
namespace Fastdotnet.PluginA.Events;

/// <summary>
/// 商城订单创建事件（PluginA 业务事件示例）
/// </summary>
[Event(Name = "MallOrderCreated", Description = "商城订单创建", Group = "Mall")]
public class MallOrderCreatedEvent : DomainEvent
{
    public override string AggregateId => OrderId;
    
    /// <summary>订单 ID</summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>用户 ID</summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>订单金额</summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>商品数量</summary>
    public int ItemCount { get; set; }
    
    /// <summary>扩展数据</summary>
    public Dictionary<string, object> OrderItems { get; set; } = new();
}
```

**关键点**：
- ✅ 继承 `DomainEvent`（领域事件基类）
- ✅ 添加 `[Event]` 特性标记
- ✅ `override AggregateId` 指定聚合根 ID
- ✅ 包含必要的业务数据

---

## 💡 示例 2：订阅并处理事件

### OrderPaidEventHandler.cs

```csharp
using Fastdotnet.Plugin.Contracts.Events;

namespace Fastdotnet.PluginA.EventHandlers;

/// <summary>
/// 支付完成事件处理器（PluginA 订阅支付插件事件）
/// </summary>
public class OrderPaidEventHandler : IEventHandler<PaymentCompletedEvent>
{
    private readonly ILogger<OrderPaidEventHandler> _logger;
    
    public OrderPaidEventHandler(ILogger<OrderPaidEventHandler> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// 处理支付完成事件
    /// </summary>
    public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("【PluginA】收到支付完成事件：订单 ID={OrderId}, 金额={Amount:C}", 
            @event.OrderId, @event.PaidAmount);
        
        // TODO: 这里可以添加业务逻辑，例如：
        // 1. 更新订单状态为已支付
        // 2. 扣减库存
        // 3. 发送通知给用户
        
        await Task.CompletedTask;
        
        _logger.LogInformation("【PluginA】支付完成事件处理完成");
    }
}
```

**关键点**：
- ✅ 实现 `IEventHandler<PaymentCompletedEvent>` 接口
- ✅ 依赖注入 `ILogger`（支持 DI 容器）
- ✅ 异步处理方法 `HandleAsync`
- ✅ 自动被事件总线发现和调用

---

## 💡 示例 3：发布事件

### MallOrderService.cs

```csharp
using Fastdotnet.Plugin.Contracts.Events;
using Fastdotnet.PluginA.Events;

namespace Fastdotnet.PluginA.Services;

/// <summary>
/// 商城订单服务（演示如何发布事件）
/// </summary>
public class MallOrderService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<MallOrderService> _logger;
    
    public MallOrderService(IEventBus eventBus, ILogger<MallOrderService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }
    
    /// <summary>
    /// 创建订单并发布事件
    /// </summary>
    public async Task<string> CreateOrderAsync(string userId, decimal totalAmount, int itemCount)
    {
        var orderId = $"ORD_{Guid.NewGuid():N}";
        
        _logger.LogInformation("【PluginA】创建订单：订单 ID={OrderId}, 金额={Amount:C}", 
            orderId, userId, totalAmount);
        
        // 保存订单到数据库...
        await Task.Delay(100); // 模拟数据库操作
        
        // 发布订单创建事件
        var orderCreatedEvent = new MallOrderCreatedEvent
        {
            OrderId = orderId,
            UserId = userId,
            TotalAmount = totalAmount,
            ItemCount = itemCount,
            Source = "PluginA",
            Data = new Dictionary<string, object>
            {
                ["CreateTime"] = DateTime.Now,
                ["Status"] = "Pending"
            }
        };
        
        await _eventBus.PublishAsync(orderCreatedEvent);
        
        _logger.LogInformation("【PluginA】订单创建成功，已发布 MallOrderCreatedEvent 事件");
        
        return orderId;
    }
}
```

**关键点**：
- ✅ 通过构造函数注入 `IEventBus`
- ✅ 创建事件对象并填充数据
- ✅ 调用 `_eventBus.PublishAsync()` 发布事件
- ✅ 发布者不知道订阅者是谁（完全解耦）

---

## 💡 示例 4：API 控制器

### MallOrderController.cs

```csharp
namespace Fastdotnet.PluginA.Controllers;

/// <summary>
/// 商城订单控制器（演示事件发布）
/// </summary>
[ApiController]
[Route("api/pluginA/[controller]")]
public class MallOrderController : ControllerBase
{
    private readonly MallOrderService _orderService;
    private readonly ILogger<MallOrderController> _logger;
    
    public MallOrderController(MallOrderService orderService, ILogger<MallOrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    /// <summary>
    /// 创建订单并发布事件
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderId = await _orderService.CreateOrderAsync(
            request.UserId, 
            request.TotalAmount, 
            request.ItemCount);
        
        return Ok(new 
        {
            success = true,
            orderId = orderId,
            message = "订单创建成功，已发布 MallOrderCreatedEvent 事件"
        });
    }
    
    /// <summary>
    /// 发起支付请求并发布事件
    /// </summary>
    [HttpPost("{orderId}/payment")]
    public async Task<IActionResult> RequestPayment(string orderId, [FromQuery] decimal amount)
    {
        var result = await _orderService.RequestPaymentAsync(orderId, amount);
        
        return Ok(new 
        {
            success = result,
            message = "支付请求已发送，等待支付插件处理 PaymentRequestedEvent 事件"
        });
    }
}
```

---

## 🔧 注册服务

### PluginA.cs

```csharp
public void ConfigureServices(ContainerBuilder builder)
{
    // ========== 事件总线演示：注册事件处理器 ==========
    builder.RegisterType<EventHandlers.OrderPaidEventHandler>()
        .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PaymentCompletedEvent>>()
        .InstancePerLifetimeScope();
    
    builder.RegisterType<EventHandlers.PluginInstalledEventHandler>()
        .As<Fastdotnet.Plugin.Contracts.Events.IEventHandler<Fastdotnet.Plugin.Contracts.Events.PluginInstalledEvent>>()
        .InstancePerLifetimeScope();
    
    // ========== 事件总线演示：注册商城订单服务（用于发布事件） ==========
    builder.RegisterType<Services.MallOrderService>().AsSelf().InstancePerLifetimeScope();
}
```

**关键点**：
- ✅ 事件处理器必须注册到 DI 容器
- ✅ 使用 `As<IEventHandler<TEvent>>()` 指定接口
- ✅ 事件总线会自动发现并订阅

---

## 🧪 测试方法

### 1. 测试发布事件

```bash
# 创建订单并发布 MallOrderCreatedEvent 事件
curl -X POST https://localhost:5001/api/pluginA/mallOrder/create \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "totalAmount": 199.99,
    "itemCount": 2
  }'
```

**预期日志**：
```
【PluginA】创建订单：订单 ID=ORD_xxx, 金额=¥199.99
【PluginA】订单创建成功，已发布 MallOrderCreatedEvent 事件
```

---

### 2. 测试订阅事件

当支付插件发布 `PaymentCompletedEvent` 时，PluginA 会自动收到并处理：

**预期日志**：
```
【PluginA】收到支付完成事件：订单 ID=ORD_xxx, 金额=¥199.99
【PluginA】支付完成事件处理完成
```

---

## 🎯 事件流转示意图

```
┌─────────────┐              ┌─────────────┐
│   User      │              │  PluginA    │
│             │              │ Controller  │
└──────┬──────┘              └──────┬──────┘
       │                            │
       │ POST /api/pluginA/mallOrder/create
       │───────────────────────────>│
       │                            │
       │                            │ 创建订单
       │                            │
       │                            │ 发布 MallOrderCreatedEvent
       │                            │──────────┐
       │                            │          │
       │                            │          ▼
       │                    ┌─────────────────────────┐
       │                    │   InMemoryEventBus      │
       │                    │  (主应用事件总线)        │
       │                    └───────────┬─────────────┘
       │                                │
       │                                │ PublishAsync
       │                                │──────────┐
       │                                │          │
       │                                │          ▼
       │                      ┌─────────────────────────┐
       │                      │  Event Handlers         │
       │                      │ - OrderPaidHandler      │
       │                      │ - PluginInstallHandler  │
       │                      │ - ... (其他插件)        │
       │                      └─────────────────────────┘
```

---

## 📊 事件类型总结

| 事件名称 | 类型 | 发布者 | 订阅者 | 用途 |
|---------|------|--------|--------|------|
| MallOrderCreated | DomainEvent | PluginA | 其他插件 | 通知订单创建 |
| PaymentCompleted | DomainEvent | 支付插件 | PluginA | 通知支付完成 |
| PluginInstalled | IntegrationEvent | 主应用 | 所有插件 | 通知插件安装 |

---

## ✅ 最佳实践

### 1. 事件命名规范
- ✅ 使用过去时态：`OrderCreated`, `PaymentCompleted`
- ✅ 清晰的动词 + 名词结构
- ❌ 避免模糊命名：`OrderEvent`, `PayEvent`

### 2. 事件设计原则
- ✅ 保持简单，只包含必要数据
- ✅ 使用 `Data` 字典传递扩展信息
- ✅ 继承正确的基类（DomainEvent / IntegrationEvent）

### 3. 处理器设计
- ✅ 单一职责：一个处理器只处理一种事件
- ✅ 无状态：不保存状态，每次都是新的实例
- ✅ 快速返回：耗时操作应该异步或后台处理

### 4. 异常处理
- ✅ 在处理器内部捕获异常
- ✅ 记录详细日志
- ❌ 不要抛出异常（会影响其他处理器）

---

## 🚀 扩展示例

### 添加库存扣减处理器

```csharp
// 新建：InventoryReductionHandler.cs
public class InventoryReductionHandler : IEventHandler<MallOrderCreatedEvent>
{
    public async Task HandleAsync(MallOrderCreatedEvent @event, CancellationToken ct)
    {
        // 扣减库存逻辑
        Console.WriteLine($"已扣减 {@event.ItemCount} 件商品库存");
    }
}

// 注册到 DI 容器
builder.RegisterType<InventoryReductionHandler>()
    .As<IEventHandler<MallOrderCreatedEvent>>()
    .InstancePerLifetimeScope();
```

---

## 📚 参考资料

- [通用事件总线系统架构](../../../fastdotnet-docs/docs/backend/通用事件总线系统架构.md)
- [支付插件事件驱动架构实现计划](../../../fastdotnet-docs/docs/backend/支付插件事件驱动架构实现计划.md)

---

*最后更新时间：2026-03-08*  
*PluginA 版本：v1.0.0*
