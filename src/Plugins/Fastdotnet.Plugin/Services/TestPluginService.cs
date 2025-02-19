using Fastdotnet.Core.Attributes;
using Fastdotnet.Plugin.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Plugin.Services
{
    [AutoInject(ServiceLifetime.Scoped)]
    public class TestPluginService : ITestPluginService
    {
        private readonly TestPlugin _testPlugin;

        public TestPluginService(TestPlugin testPlugin)
        {
            _testPlugin = testPlugin;
        }

        public string GetPluginStatus()
        {
            return $"插件名称: {_testPlugin.Name}\n版本: {_testPlugin.Version}\n描述: {_testPlugin.Description}";
        }

        public void Start()
        {
            _testPlugin.Start();
        }

        public void Stop()
        {
            _testPlugin.Stop();
        }
    }
}