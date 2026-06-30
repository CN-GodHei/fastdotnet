using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PluginA.Providers
{
    /// <summary>
    /// PluginA 的 SignalR 方法提供者演示
    /// PluginA 的 SignalR 方法提供者演示（特性路由进阶版）
    /// 继承 AutoSignalRProvider 后，参数自动反序列化绑定，告别冗余的装箱拆箱
    /// </summary>
    public class PluginASignalRProvider : AutoSignalRProvider
    {
        public PluginASignalRProvider()
        {
            Console.WriteLine($"[{PluginId}] SignalR Provider 实例化 (基于特性的强类型路由)");
        }

        // 演示方法 1: 带有 HubCallerContext 上下文注入的方法
        [SignalRMethod("Echo")]
        public async Task<object> EchoAsync(string message, HubCallerContext context)
        {
            var userId = context?.UserIdentifier ?? "匿名用户";
            var connectionId = context?.ConnectionId;
                
            Console.WriteLine($"[{PluginId}] 收到来自连接 {connectionId} (用户: {userId}) 的 Echo 请求: {message}");

            // 模拟业务处理
            await Task.Delay(100);

            return new
            {
                received = message ?? "空消息",
                reply = $"PluginA 已经收到你的消息: {message}",
                time = DateTime.Now,
                user = userId,
                connection = connectionId
            };
        }

        // 演示方法 2: 不传名字则默认使用函数名 "Add" 作为暴露的接口
        // 自动完成基础类型的参数转换，连判空和解析都能省去！
        [SignalRMethod] 
        public object Add(double a, double b)
        {
            Console.WriteLine($"[{PluginId}] 收到计算请求: {a} + {b}");
            return new
            {
                result = a + b,
                operation = $"{a} + {b}",
                time = DateTime.Now
            };
        }
    }
}
