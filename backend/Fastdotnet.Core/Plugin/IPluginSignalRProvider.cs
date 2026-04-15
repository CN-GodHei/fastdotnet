namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 插件 SignalR 方法提供者接口
    /// 插件实现此接口来暴露可通过 UniversalHub 调用的方法
    /// </summary>
    public interface IPluginSignalRProvider
    {
        /// <summary>
        /// 插件 ID
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// 注册插件的 SignalR 方法
        /// </summary>
        /// <param name="registry">方法注册器</param>
        void RegisterMethods(ISignalRMethodRegistry registry);
    }

    /// <summary>
    /// SignalR 方法注册器接口
    /// </summary>
    public interface ISignalRMethodRegistry
    {
        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="handler">方法处理器</param>
        void Register(string methodName, Func<object[], Task<object?>> handler);
    }
}
