using System.Collections.Concurrent;
using Fastdotnet.Core.Plugin;

namespace Fastdotnet.Core.Hubs
{
    /// <summary>
    /// SignalR 方法注册器实现
    /// </summary>
    public class SignalRMethodRegistry : ISignalRMethodRegistry
    {
        private readonly string _pluginId;
        private readonly ConcurrentDictionary<string, Func<object[], Task<object?>>> _handlers;

        public SignalRMethodRegistry(string pluginId, ConcurrentDictionary<string, Func<object[], Task<object?>>> handlers)
        {
            _pluginId = pluginId;
            _handlers = handlers;
        }

        public void Register(string methodName, Func<object[], Task<object?>> handler)
        {
            var key = $"{_pluginId}.{methodName}";
            _handlers[key] = handler;
        }
    }

    /// <summary>
    /// 插件方法注册管理器 - 管理插件通过 UniversalHub 暴露的方法
    /// </summary>
    public static class PluginMethodRegistry
    {
        // 存储插件注册的方法 (key: "pluginId.methodName", value: 方法委托)
        private static readonly ConcurrentDictionary<string, Func<object[], Task<object?>>> _methodHandlers = new();

        /// <summary>
        /// 注册插件的所有方法
        /// </summary>
        /// <param name="provider">插件方法提供者</param>
        public static void RegisterPlugin(IPluginSignalRProvider provider)
        {
            Console.WriteLine($"[PluginMethodRegistry] 开始注册插件: {provider.PluginId}");
            var registry = new SignalRMethodRegistry(provider.PluginId, _methodHandlers);
            provider.RegisterMethods(registry);
            
            // 输出已注册的方法
            var registeredMethods = GetPluginMethods(provider.PluginId);
            Console.WriteLine($"[PluginMethodRegistry] 插件 {provider.PluginId} 已注册 {registeredMethods.Count()} 个方法: {string.Join(", ", registeredMethods)}");
        }

        /// <summary>
        /// 注销插件的所有方法
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        public static void UnregisterPlugin(string pluginId)
        {
            var prefix = $"{pluginId}.";
            var keysToRemove = _methodHandlers.Keys.Where(k => k.StartsWith(prefix)).ToList();
            
            foreach (var key in keysToRemove)
            {
                _methodHandlers.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// 调用插件方法
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns>方法返回值</returns>
        public static async Task<object?> InvokeMethod(string pluginId, string methodName, object[] args)
        {
            var key = $"{pluginId}.{methodName}";
            
            Console.WriteLine($"[PluginMethodRegistry] 查找方法: {key}");
            Console.WriteLine($"[PluginMethodRegistry] 已注册的方法: {string.Join(", ", _methodHandlers.Keys)}");
            
            if (_methodHandlers.TryGetValue(key, out var handler))
            {
                Console.WriteLine($"[PluginMethodRegistry] 找到方法处理器: {key}");
                return await handler(args);
            }

            throw new InvalidOperationException($"插件 {pluginId} 未注册方法: {methodName}");
        }

        /// <summary>
        /// 检查方法是否已注册
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="methodName">方法名</param>
        /// <returns>是否已注册</returns>
        public static bool IsMethodRegistered(string pluginId, string methodName)
        {
            var key = $"{pluginId}.{methodName}";
            return _methodHandlers.ContainsKey(key);
        }

        /// <summary>
        /// 获取插件注册的所有方法名
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>方法名列表</returns>
        private static IEnumerable<string> GetPluginMethods(string pluginId)
        {
            var prefix = $"{pluginId}.";
            return _methodHandlers.Keys
                .Where(k => k.StartsWith(prefix))
                .Select(k => k.Substring(prefix.Length));
        }
    }
}
