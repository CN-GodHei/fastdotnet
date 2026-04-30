using Fastdotnet.Core.Plugin;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PluginA.Providers
{
    /// <summary>
    /// PluginA 的 SignalR 方法提供者演示
    /// 用于向主程序的 UniversalHub 注册可以在前端被直接调用的 SignalR 方法
    /// </summary>
    public class PluginASignalRProvider : IPluginSignalRProvider
    {
        public PluginASignalRProvider()
        {
            Console.WriteLine($"[{PluginId}] SignalR Provider 实例化");
        }

        // 获取当前插件信息
        public string PluginId => PluginContext.GetCurrentPluginInfo()?.id;

        public void RegisterMethods(ISignalRMethodRegistry registry)
        {
            Console.WriteLine($"[{PluginId}] 开始注册 SignalR 演示方法...");

            // 演示方法 1: Echo 消息回复 (带当前用户信息)
            registry.Register("Echo", async (args, context) =>
            {
                var message = args != null && args.Length > 0 ? args[0]?.ToString() : "空消息";
                var userId = context?.UserIdentifier ?? "匿名用户";
                var connectionId = context?.ConnectionId;
                
                Console.WriteLine($"[{PluginId}] 收到来自连接 {connectionId} (用户: {userId}) 的 Echo 请求: {message}");

                // 模拟业务处理
                await Task.Delay(100);

                return new
                {
                    received = message,
                    reply = $"PluginA 已经收到你的消息: {message}",
                    time = DateTime.Now,
                    user = userId,
                    connection = connectionId
                };
            });

            // 演示方法 2: 简单加法计算
            registry.Register("Add", async (args, context) =>
            {
                if (args == null || args.Length < 2)
                    throw new ArgumentException("参数不足，至少需要传入两个数字参数");

                if (double.TryParse(args[0]?.ToString(), out var a) && 
                    double.TryParse(args[1]?.ToString(), out var b))
                {
                    Console.WriteLine($"[{PluginId}] 收到计算请求: {a} + {b}");
                    return new
                    {
                        result = a + b,
                        operation = $"{a} + {b}",
                        time = DateTime.Now
                    };
                }

                throw new ArgumentException("参数类型错误，必须是数字");
            });

            Console.WriteLine($"[{PluginId}] SignalR 演示方法注册完成");
        }
    }
}
