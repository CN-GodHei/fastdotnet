namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 插件元数据接口
    /// </summary>
    public interface IPluginMetadata
    {
        /// <summary>
        /// 插件ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 插件作者
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 插件所需的框架版本
        /// </summary>
        string RequiredFrameworkVersion { get; }

        /// <summary>
        /// 插件优先级，数值越小优先级越高
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 是否自动启动
        /// </summary>
        bool AutoStart { get; }

        /// <summary>
        /// 插件依赖项
        /// </summary>
        string[] Dependencies { get; }
    }
}