
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class PluginRegistrationSource : IRegistrationSource
    {
        private readonly PluginManager _pluginManager;

        public PluginRegistrationSource(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public bool IsAdapterForIndividualComponents => false;

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, System.Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (service is not IServiceWithType swt)
            {
                yield break;
            }

            // 【场景1】处理插件服务接口
            if (_pluginManager.PluginTypes.TryGetValue(swt.ServiceType, out var implementationType))
            {
                var registration = RegistrationBuilder.ForType(implementationType)
                                                      .As(swt.ServiceType)
                                                      .InstancePerLifetimeScope()
                                                      .CreateRegistration();
                yield return registration;
            }

            // 【场景2】处理插件控制器
            // 检查请求的类型是否是一个控制器，并且它是否来自于一个已加载的插件。
            if (typeof(ControllerBase).IsAssignableFrom(swt.ServiceType) && _pluginManager.IsTypeFromPluginAssembly(swt.ServiceType))
            {
                // 如果是，就为这个控制器类型创建一个即时的注册。
                var registration = RegistrationBuilder.ForType(swt.ServiceType)
                                                      .AsSelf()
                                                      .InstancePerDependency() // 控制器通常是每个请求一个实例
                                                      .CreateRegistration();
                yield return registration;
            }
        }
    }
}
