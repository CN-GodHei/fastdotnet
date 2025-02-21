using System.ComponentModel.Composition;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// MEF插件接口
    /// </summary>
    [InheritedExport]
    public interface IMefPlugin
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
        /// 插件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 插件作者
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 插件所需的框架版本
        /// </summary>
        string RequiredFrameworkVersion { get; }

        /// <summary>
        /// 插件元数据
        /// </summary>
        [Import]
        IPluginMetadata Metadata { get; set; }

        /// <summary>
        /// 初始化插件
        /// </summary>
        void Initialize();

        /// <summary>
        /// 启动插件
        /// </summary>
        void Start();

        /// <summary>
        /// 停止插件
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 插件元数据接口
    /// </summary>
    public interface IMefPluginMetadata : IPluginMetadata
    {
    }
}