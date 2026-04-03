# 插件 SignalR 消息使用指南

## 📦 统一的插件消息类

**命名空间：** `Fastdotnet.Core.Messaging`

所有插件和主程序都应使用此命名空间中的消息类来发送 SignalR 通知。

### 核心优势

✅ **统一格式** - 所有消息使用相同的结构  
✅ **自动获取插件信息** - 使用 `Auto` 系列方法无需手动指定插件 ID  
✅ **类型安全** - 泛型支持确保数据正确  
✅ **易于扩展** - 可添加新的消息类型  
✅ **便于调试** - 清晰的来源和类型标识  

---

## 🚀 快速开始

### 最简单的用法（强烈推荐 ⭐）

```csharp
using Fastdotnet.Core.Messaging;

// 在构造函数中注入 IHubContext<UniversalHub>
private readonly IHubContext<UniversalHub> _hubContext;

// ✅ 自动获取当前插件信息，无需硬编码插件 ID！
await _hubContext.SendSuccessAutoAsync(
    title: "操作成功",
    message: "数据已保存",
    targetUserId: userId);
```

### 其他快捷方法

```csharp
// 发送错误通知
await _hubContext.SendErrorAutoAsync(
    title: "操作失败",
    message: "数据库连接超时",
    targetUserId: userId);

// 发送自定义类型的通知
await _hubContext.SendPluginNotificationAutoAsync(
    title: "订单提醒",
    message: "您有新的订单",
    level: "warning",
    targetUserId: userId);

// 发送自定义数据
await _hubContext.SendPluginMessageAutoAsync(
    messageType: PluginMessageTypes.DataChanged,
    data: new { OrderId = "123", Amount = 999.99m },
    targetUserId: userId);
```

---

## 📋 完整的 API 参考

### 1. 基础消息类

#### `PluginMessage<T>` - 泛型版本

```csharp
public class PluginMessage<T>
{
    public string PluginId { get; set; }
    public string PluginName { get; set; }
    public string MessageType { get; set; }
    public T Data { get; set; }
    public DateTime Timestamp { get; set; }
    public string? TargetUserId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
```

#### `PluginMessage` - 非泛型版本

用于动态数据类型。

### 2. 预定义的消息类型

```csharp
PluginMessageTypes.Notification      // 普通通知
PluginMessageTypes.StatusUpdate      // 状态更新
PluginMessageTypes.ProgressUpdate    // 进度更新
PluginMessageTypes.Error             // 错误消息
PluginMessageTypes.Success           // 成功消息
PluginMessageTypes.Warning           // 警告消息
PluginMessageTypes.DataChanged       // 数据变更
PluginMessageTypes.OperationCompleted // 操作完成
```

### 3. 常用的数据结构

#### `TextNotification` - 文本通知

```csharp
new TextNotification
{
    Title = "操作成功",
    Message = "数据已保存",
    Level = "success",        // info, success, warning, error
    Dismissible = true,
    AutoCloseDelay = 3000     // 毫秒，0 表示不自动关闭
}
```

#### `ProgressData` - 进度数据

```csharp
new ProgressData
{
    Description = "正在处理...",
    Current = 50,
    Total = 100,
    IsCompleted = false
}
```

#### `DataChangeInfo` - 数据变更信息

```csharp
new DataChangeInfo
{
    ChangeType = "update",    // create, update, delete
    DataType = "Order",
    DataId = "123",
    NewData = orderData,
    OldData = oldOrderData
}
```

---

## 🎯 使用场景

### 场景 1：在事件处理器中发送通知

```csharp
public class PaymentCompletedHandler : IEventHandler<PaymentCompletedEvent>
{
    private readonly IHubContext<UniversalHub> _hubContext;

    public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken ct)
    {
        // 业务处理...

        // 发送支付成功通知
        await _hubContext.SendPluginMessageAsync(
            pluginId: "Fastdotnet.Pay",
            pluginName: "Fastdotnet.Pay",
            messageType: PluginMessageTypes.Success,
            data: new
            {
                OrderId = @event.OrderId,
                TransactionNo = @event.TransactionNo,
                TotalAmount = @event.PaidAmount
            },
            targetUserId: userId,
            cancellationToken: ct);
    }
}
```

### 场景 2：在服务层发送通知

```csharp
public class OrderService : IOrderService
{
    private readonly IHubContext<UniversalHub> _hubContext;

    public async Task CreateOrderAsync(Order order, string userId)
    {
        // 创建订单逻辑...

        // 发送通知
        await _hubContext.SendPluginNotificationAsync(
            pluginId: "OrderPlugin",
            pluginName: "订单管理插件",
            title: "订单创建成功",
            message: $"订单 {order.Id} 已创建",
            level: "success",
            targetUserId: userId);
    }
}
```

### 场景 3：长时间任务的进度通知

```csharp
public async Task ProcessBatchAsync(string userId, List<Item> items)
{
    for (int i = 0; i < items.Count; i++)
    {
        // 处理数据...

        if (i % 10 == 0)
        {
            await _hubContext.SendProgressUpdateAsync(
                pluginId: "BatchProcessor",
                pluginName: "批量处理插件",
                description: $"正在处理第 {i} 项...",
                current: i,
                total: items.Count,
                targetUserId: userId);
        }
    }

    await _hubContext.SendSuccessAsync(
        pluginId: "BatchProcessor",
        pluginName: "批量处理插件",
        title: "处理完成",
        message: $"共处理 {items.Count} 项数据",
        targetUserId: userId);
}
```

### 场景 4：主程序发送系统通知

```csharp
// 主程序也可以发送消息（不需要插件 ID）
await _hubContext.SendPluginNotificationAsync(
    pluginId: "System",
    pluginName: "系统消息",
    title: "系统维护通知",
    message: "系统将于今晚 22:00 进行维护",
    level: "warning",
    targetUserId: userId);
```

---

## 🎨 前端监听

### Nuxt3 / Vue 示例

```typescript
// utils/signalr.ts
import * as signalR from '@microsoft/signalr'

class PluginSignalRManager {
  private connection: signalR.HubConnection | null = null

  async initialize(token: string) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/api/signalr?access_token=' + token)
      .build()

    // 监听插件消息
    this.connection.on('pluginMessage', (message: any) => {
      console.log('收到插件消息:', message)
      
      // message 结构:
      // {
      //   pluginId: 'Fastdotnet.Pay',
      //   pluginName: 'Fastdotnet.Pay',
      //   messageType: 'success',
      //   data: { OrderId: '...', TotalAmount: ... },
      //   timestamp: '...'
      // }

      // 根据消息类型处理
      switch (message.messageType) {
        case 'success':
          ElMessage.success(message.data)
          break
        case 'error':
          ElMessage.error(message.data)
          break
        case 'progress_update':
          updateProgress(message.data)
          break
      }
    })

    await this.connection.start()
  }
}

export const signalRManager = new PluginSignalRManager()
```

---

## ✅ 最佳实践

### 1. 始终包含插件信息

```csharp
// ✅ 好的做法
await _hubContext.SendPluginMessageAsync(
    pluginId: "MyPlugin",
    pluginName: "我的插件",
    // ...
);

// ❌ 不好的做法 - 没有插件标识
await _hubContext.Clients.User(userId).SendAsync("someEvent", data);
```

### 2. 使用预定义的消息类型

```csharp
// ✅ 清晰的语义
messageType: PluginMessageTypes.Success

// ❌ 魔法字符串
messageType: "my_custom_success_type_v2"
```

### 3. 精准推送给目标用户

```csharp
// ✅ 只发送给相关用户
targetUserId: userId

// ❌ 广播给所有人（除非确实需要）
await _hubContext.Clients.All.SendAsync(...)
```

### 4. 使用强类型数据

```csharp
// ✅ 类型安全
data: new OrderData { Id = "...", Amount = 100 }

// ❌ 匿名对象（调试困难）
data: new { id = "...", amount = 100 }
```

### 5. 添加适当的日志

```csharp
_logger.LogInformation(
    "已向用户 {UserId} 发送 {MessageType} 通知", 
    userId, 
    messageType);
```

---

## 🔧 扩展示例

### 创建插件专用的辅助方法

```csharp
namespace YourPlugin.Messaging
{
    public static class YourPluginMessages
    {
        public const string PluginId = "YourPluginId";
        public const string PluginName = "Your Plugin Name";

        // 封装常用的发送方法
        public static Task SendOrderUpdateAsync(
            this IHubContext<UniversalHub> hub,
            string userId, 
            OrderData data)
        {
            return hub.SendPluginMessageAsync(
                pluginId: PluginId,
                pluginName: PluginName,
                messageType: "order_status_changed",
                data: data,
                targetUserId: userId);
        }
    }
}

// 使用
await _hubContext.SendOrderUpdateAsync(userId, orderData);
```

---

## 📝 总结

通过使用 `Fastdotnet.Core.Messaging` 中的统一消息类：

✅ **自动包含插件信息** - 无需手动添加 `PluginId`  
✅ **标准化消息格式** - 所有插件使用相同的结构  
✅ **类型安全** - 泛型支持确保数据类型正确  
✅ **易于扩展** - 可添加新的消息类型和数据字段  
✅ **便于调试** - 清晰的消息来源和类型标识  
✅ **前端统一处理** - 根据 `messageType` 统一分发处理  
✅ **主程序和插件通用** - 统一的通信方式  

现在发送插件消息变得非常简单且规范！🎉
