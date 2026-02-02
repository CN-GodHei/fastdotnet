
namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件基类，提供插件通用功能
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        private PluginInfo _cachedPluginInfo;
        
        /// <summary>
        /// 获取插件名称
        /// </summary>
        public string Name => GetPluginInfo()?.name ?? string.Empty;
        
        /// <summary>
        /// 获取插件版本
        /// </summary>
        public string Version => GetPluginInfo()?.version ?? string.Empty;
        
        /// <summary>
        /// 获取当前插件的信息
        /// </summary>
        public PluginInfo GetPluginInfo()
        {
            if (_cachedPluginInfo != null)
            {
                return _cachedPluginInfo;
            }
            
            _cachedPluginInfo = PluginContext.GetCurrentPluginInfo();
            return _cachedPluginInfo;
        }
        
        /// <summary>
        /// 获取插件ID
        /// </summary>
        public string GetPluginId()
        {
            return GetPluginInfo()?.id ?? string.Empty;
        }
        
        /// <summary>
        /// 获取插件描述
        /// </summary>
        public string GetPluginDescription()
        {
            return GetPluginInfo()?.description ?? string.Empty;
        }

        // IPlugin接口的抽象方法，需要由具体插件实现
        public abstract Task InitializeAsync(IServiceProvider serviceProvider);
        public abstract Task StartAsync();
        public abstract Task StopAsync();
        public abstract Task UnloadAsync(IServiceProvider serviceProvider);
        public abstract void ConfigureServices(Autofac.ContainerBuilder builder);
    }
}