using Autofac;
using AutoMapper;
using Fastdotnet.Core.Middleware;
using Fastdotnet.Plugin.Core.Infrastructure;
using Fastdotnet.Plugin.Shared.AdapterAOT;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.WebApi.Modules;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder containerBuilder)
    {
        //// 注册单例服务
        //builder.RegisterType<PluginManager>().AsSelf().SingleInstance();
        //builder.RegisterType<PluginStaticFileProviderRegistry>().AsSelf().SingleInstance();

        //// 注册 PluginLoadService（依赖注入构造函数）
        //builder.Register(c => new PluginLoadService(
        //    c.Resolve<PluginManager>(),
        //    c.Resolve<ILifetimeScope>(),
        //    c.Resolve<ILogger<PluginLoadService>>(),
        //    c.Resolve<ILoggerFactory>(),
        //    c.Resolve<PluginStaticFileProviderRegistry>(),
        //    c.Resolve<IConfiguration>()
        //)).As<IPluginLoadService>().SingleInstance();

        //builder.RegisterType<PluginActionDescriptorProvider>()
        //       .As<IActionDescriptorProvider>()
        //       .SingleInstance();

        //// 注册 AutoMapper
        //builder.Register(c =>
        //{
        //    var context = c.Resolve<IComponentContext>();
        //    var loggerFactory = context.Resolve<ILoggerFactory>();

        //    var expression = new AutoMapper.MapperConfigurationExpression();
        //    expression.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
        //    expression.ConstructServicesUsing(context.Resolve);

        //    var config = new MapperConfiguration(expression, loggerFactory);
        //    return config.CreateMapper();
        //}).As<IMapper>().InstancePerLifetimeScope();

        //// 可选：取消注释以启用缓存服务
        //// builder.RegisterType<HybridCacheService>()
        ////        .As<IHybridCacheService>()
        ////        .InstancePerLifetimeScope();
        ///
        containerBuilder.RegisterType<PluginManager>().AsSelf().SingleInstance();
        containerBuilder.RegisterType<PluginStaticFileProviderRegistry>().AsSelf().SingleInstance();

        containerBuilder.Register(c => new PluginLoadService(
            c.Resolve<PluginManager>(),
            c.Resolve<ILifetimeScope>(),
            c.Resolve<ILogger<PluginLoadService>>(),
            c.Resolve<ILoggerFactory>(),
            c.Resolve<PluginStaticFileProviderRegistry>(),
            c.Resolve<IConfiguration>()
        )).As<IPluginLoadService>().SingleInstance();

        containerBuilder.RegisterType<PluginActionDescriptorProvider>().As<IActionDescriptorProvider>().SingleInstance();

        // 在Autofac中注册AutoMapper
        containerBuilder.Register(c =>
        {
            var context = c.Resolve<IComponentContext>();
            var loggerFactory = context.Resolve<ILoggerFactory>(); // 1. 解析 ILoggerFactory

            // 2. 创建和配置 MapperConfigurationExpression
            var expression = new AutoMapper.MapperConfigurationExpression();
            expression.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            expression.ConstructServicesUsing(context.Resolve);

            // 3. 使用你提供的特定构造函数创建 MapperConfiguration
            var config = new MapperConfiguration(expression, loggerFactory);

            return config.CreateMapper();
        }).As<IMapper>().InstancePerLifetimeScope();

        // 如果需要在Autofac中进行更精细的缓存服务控制，可以在这里添加
        //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().InstancePerLifetimeScope();
        //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().OwnedByLifetimeScope();
    }
}