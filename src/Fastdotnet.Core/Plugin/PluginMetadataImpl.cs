using System;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 插件元数据实现类
    /// </summary>
    public class PluginMetadataImpl : IPluginMetadata
    {
        private readonly IMefPlugin _plugin;

        public PluginMetadataImpl(IMefPlugin plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        }

        public string Id => _plugin.Id;

        public string Name => _plugin.Name;

        public string Description => _plugin.Description;

        public string Version => _plugin.Version;

        public string Author => _plugin.Author;

        public string RequiredFrameworkVersion => _plugin.RequiredFrameworkVersion;

        public int Priority => 1; // 默认优先级

        public bool AutoStart => true; // 默认自动启动

        public string[] Dependencies => Array.Empty<string>(); // 默认无依赖
    }
}