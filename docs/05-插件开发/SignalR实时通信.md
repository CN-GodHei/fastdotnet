# 插件的 SignalR 实时通信

在传统的 ASP.NET Core 开发中，SignalR Hub 的路由（如 `/myhub`）通常在程序启动时静态构建。但在 Fastdotnet 这样的热插拔架构中，为了允许插件随时加载和卸载，我们采用了一种基于**网关代理（UniversalHub）**与**特性路由**的动态 SignalR 方案。

## 架构原理

1. **唯一入口**：所有前端 WebSocket 连接都连接到主程序的统一网关 `/universalhub`。
2. **方法调度**：插件无需自己编写继承自 `Hub` 的类，而是通过实现特定的接口，将需要暴露给前端的方法注册到主程序的中央字典中。
3. **安全隔离**：当插件停用或卸载时，这些方法会自动从中央字典中注销。因为它们没有独占的 WebSocket 路由，从而杜绝了内存泄漏，实现了完美的“热插拔”。

## 开发指南

为了极大地简化开发者的负担，框架提供了基于特性的 `AutoSignalRProvider` 基类。你只需要通过简单的标签，就能完成原先繁琐的接口注册工作，甚至还能享受到强类型的参数绑定！

### 1. 编写 SignalR 提供者

在你的插件后端项目中，新建一个类并继承 `Fastdotnet.Core.Plugin.AutoSignalRProvider`。

```csharp
using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PluginA.Providers
{
    /// <summary>
    /// PluginA 的 SignalR 方法提供者
    /// 继承 AutoSignalRProvider 后，只需贴上 [SignalRMethod] 标签即可！
    /// </summary>
    public class PluginASignalRProvider : AutoSignalRProvider
    {
        // 演示 1：简单的强类型参数，并且带返回值
        // 如果不给 [SignalRMethod] 传参，它默认将函数名 "Add" 作为给前端调用的路由名称
        [SignalRMethod] 
        public object Add(double a, double b)
        {
            return new
            {
                result = a + b,
                time = DateTime.Now
            };
        }

        // 演示 2：获取连接信息与异步处理
        // 你可以通过声明 HubCallerContext 参数来获取当前连接的用户身份、ConnectionId 等信息
        [SignalRMethod("Echo")]
        public async Task<object> EchoAsync(string message, HubCallerContext context)
        {
            var userId = context?.UserIdentifier ?? "匿名用户";
            
            await Task.Delay(100); // 模拟耗时操作

            return new
            {
                reply = $"收到来自用户 {userId} 的消息: {message}"
            };
        }
    }
}
```

### 2. 底层发生了什么？

当你启动插件时，Fastdotnet 的 `PluginLoadService` 会自动通过依赖注入扫描到你的 `PluginASignalRProvider`，接着 `AutoSignalRProvider` 会利用反射自动把 `Add` 和 `EchoAsync` 封装并注册到 `UniversalHub` 的分发字典里。
同时，它内置了强大的**自动反序列化**机制：当前端传入 JSON 数组参数时，框架会自动尝试将它们与你函数签名（如 `double a, double b`）进行类型转换和映射。

### 3. 前端如何调用？

在前端 Vue / React 等项目中，当与 `/universalhub` 建立连接后，统一使用名为 `InvokePluginMethod` 的通用方法进行调用，并传入三个参数：
1. `pluginId`：你的插件 ID。
2. `methodName`：你在特性上指定的路由名。
3. `args`：参数数组，数组长度和顺序必须与后端的函数签名一致。

#### JavaScript 调用示例

```javascript
// 假设 connection 已经是启动好的 SignalR 连接对象
const pluginId = "11375910391972869"; // 替换为你的真实 PluginId

// 1. 调用 Add 方法
connection.invoke("InvokePluginMethod", pluginId, "Add", [10, 24])
    .then(response => {
        console.log("计算结果:", response.result); // 输出 34
    })
    .catch(err => {
        console.error("调用出错:", err);
    });

// 2. 调用 Echo 异步方法
connection.invoke("InvokePluginMethod", pluginId, "Echo", ["你好啊！"])
    .then(response => {
        console.log(response.reply);
    });
```
