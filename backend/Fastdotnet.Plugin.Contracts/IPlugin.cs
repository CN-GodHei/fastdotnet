
namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件生命周期状态
    /// </summary>
    public enum PluginLifecycleState
    {
        /// <summary>
        /// 未初始化（刚加载）
        /// </summary>
        Uninitialized,
        
        /// <summary>
        /// 已初始化（完成 InitializeAsync）
        /// </summary>
        Initialized,
        
        /// <summary>
        /// 已启动（完成 StartAsync）
        /// </summary>
        Started,
        
        /// <summary>
        /// 已停止（完成 StopAsync）
        /// </summary>
        Stopped,
        
        /// <summary>
        /// 已卸载（完成 UnloadAsync）
        /// </summary>
        Unloaded
    }

    public interface IPlugin
    {
        /// <summary>
        /// 获取插件 Id
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// 获取插件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取插件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 获取插件当前生命周期状态
        /// </summary>
        PluginLifecycleState LifecycleState { get; }

        /// <summary>
        /// 插件初始化，在此处可以访问主程序的服务
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于解析主程序的服务</param>
        Task InitializeAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 插件启动
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// 插件停止
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// 插件卸载前的清理工作
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于解析主程序的服务以进行清理</param>
        Task UnloadAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 配置插件服务
        /// </summary>
        /// <param name="builder">Autofac 容器构建器</param>
        void ConfigureServices(ContainerBuilder builder);
    }
}
