using Fastdotnet.Core.Plugin;
using System.ComponentModel.Composition;

namespace Fastdotnet.Demo
{
    /// <summary>
    /// Fastdotnet框架的演示插件
    /// </summary>
    [Export(typeof(IMefPlugin))]
    [ExportMetadata("Id", "fastdotnet-demo")]
    [ExportMetadata("Name", "Fastdotnet Demo Plugin")]
    [ExportMetadata("Description", "A simple demo plugin for Fastdotnet framework")]
    [ExportMetadata("Version", "1.0.0")]
    [ExportMetadata("Author", "Fastdotnet Team")]
    [ExportMetadata("RequiredFrameworkVersion", "1.0.0")]
    [ExportMetadata("Priority", 1)]
    [ExportMetadata("AutoStart", true)]
    [ExportMetadata("Dependencies", new string[] { })]
    public class DemoPlugin : IMefPlugin
    {
        /// <summary>
        /// 计数器值
        /// </summary>
        private int _counter = 0;

        /// <summary>
        /// 插件运行状态
        /// </summary>
        private bool _isRunning = false;

        /// <summary>
        /// 获取插件ID
        /// </summary>
        public string Id => "fastdotnet-demo";

        /// <summary>
        /// 获取插件名称
        /// </summary>
        public string Name => "Fastdotnet Demo Plugin";

        /// <summary>
        /// 获取插件版本
        /// </summary>
        public string Version => "1.0.0";

        /// <summary>
        /// 获取插件描述
        /// </summary>
        public string Description => "A simple demo plugin for Fastdotnet framework";

        /// <summary>
        /// 获取插件作者
        /// </summary>
        public string Author => "Fastdotnet Team";

        /// <summary>
        /// 获取插件所需的框架版本
        /// </summary>
        public string RequiredFrameworkVersion => "1.0.0";

        /// <summary>
        /// 获取或设置插件元数据
        /// </summary>
        public IPluginMetadata Metadata { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DemoPlugin()
        {
        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <remarks>
        /// 重置计数器并将插件状态设置为运行中
        /// </remarks>
        public void Initialize()
        {
            _counter = 0;
            _isRunning = true;
        }

        /// <summary>
        /// 启动插件
        /// </summary>
        /// <remarks>
        /// 如果插件未运行，则将其状态设置为运行中
        /// </remarks>
        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
            }
        }

        /// <summary>
        /// 停止插件
        /// </summary>
        /// <remarks>
        /// 如果插件正在运行，则将其状态设置为停止
        /// </remarks>
        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// 增加计数器值
        /// </summary>
        /// <returns>增加后的计数器值</returns>
        /// <exception cref="InvalidOperationException">当插件未运行时抛出此异常</exception>
        public int IncrementCounter()
        {
            if (_isRunning)
            {
                return ++_counter;
            }
            throw new InvalidOperationException("Plugin is not running");
        }

        /// <summary>
        /// 获取当前计数器值
        /// </summary>
        /// <returns>当前计数器值</returns>
        public int GetCurrentCount()
        {
            return _counter;
        }
    }
}