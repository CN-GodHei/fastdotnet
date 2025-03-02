using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
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
