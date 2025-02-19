using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Fastdotnet.Core.Version;

namespace Fastdotnet.Plugin
{
    [AutoInject(ServiceLifetime.Singleton)]
    public class TestPlugin : IPlugin
    {
        private bool _isRunning;

        public string Id => "test-plugin";
        public string Name => "测试插件";
        public string Version => "1.0.0";
        public string Description => "用于测试插件系统的基本功能和热插拔特性";
        public string Author => "System";
        public string RequiredFrameworkVersion => FrameworkVersion.FullVersion;

        public void Initialize()
        {
            Console.WriteLine($"[{Name}] 插件正在初始化...版本：{Version}，所需框架版本：{RequiredFrameworkVersion}");
            _isRunning = false;
        }

        public void Start()
        {
            if (_isRunning)
            {
                Console.WriteLine($"[{Name}] 插件已经在运行中");
                return;
            }

            Console.WriteLine($"[{Name}] 插件启动成功");
            _isRunning = true;

            // 模拟插件的一些操作
            Task.Run(async () =>
            {
                while (_isRunning)
                {
                    Console.WriteLine($"[{Name}] 插件正在运行中...");
                    await Task.Delay(5000); // 每5秒输出一次状态
                }
            });
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                Console.WriteLine($"[{Name}] 插件已经停止");
                return;
            }

            _isRunning = false;
            Console.WriteLine($"[{Name}] 插件已停止运行");
        }
    }
}