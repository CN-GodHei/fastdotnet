
namespace Fastdotnet.Plugin.Shared.AdapterAOT
{
    /// <summary>
    /// 提供服务类型检查的默认实现
    /// </summary>
    public class DefaultServiceProviderIsService : IServiceProviderIsService
    {
        public bool IsService(Type serviceType)
        {
            // 检查类型是否为服务类型
            return serviceType.IsInterface || serviceType.IsAbstract || serviceType.IsClass;
        }
    }
}
