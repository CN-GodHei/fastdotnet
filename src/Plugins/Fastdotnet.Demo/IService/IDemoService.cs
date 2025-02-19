using Fastdotnet.Core.Plugin;

namespace Fastdotnet.Demo.IService
{
    [PluginService("Demo服务接口", "1.0.0", "演示服务接口", "作者")]
    public interface IDemoService
    {
        string GetMessage();
    }
}