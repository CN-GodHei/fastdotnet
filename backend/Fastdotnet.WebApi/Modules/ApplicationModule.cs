using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Core.Services.App;
using Fastdotnet.Core.Services.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Reflection;
using Module = Autofac.Module;

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

        // 注册应用服务
        containerBuilder.RegisterType<FdAppUserService>().As<Core.IService.App.IFdAppUserService>().InstancePerLifetimeScope();

        // 注册仓储和工作单元服务
        containerBuilder.RegisterType<SqlSugarUnitOfWork>().As<IUnitOfWork>().As<IStorageContext>().InstancePerLifetimeScope();

        //用户操作信息
        containerBuilder.RegisterType<UserRefFiller>().As<IUserRefFiller>().InstancePerLifetimeScope();
        
        // 注册用户显示名称服务
        containerBuilder.RegisterType<UserDisplayNameService>().As<IUserDisplayNameService>().InstancePerLifetimeScope();

        //插件配置
        containerBuilder.RegisterType<PluginConfigurationService>().As<IPluginConfigurationService>().InstancePerLifetimeScope();

        // 注册通用服务以支持泛型依赖注入，类似 Program.cs 中的注册
        containerBuilder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

        containerBuilder.RegisterGeneric(typeof(RawRepository<>)).As(typeof(IRawRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(RawRepository<,>)).As(typeof(IRawRepository<,>)).InstancePerLifetimeScope();
        // 在Autofac中注册AutoMapper
        containerBuilder.Register(c =>
        {
            var context = c.Resolve<IComponentContext>();
            //var loggerFactory = context.Resolve<ILoggerFactory>(); // 1. 解析 ILoggerFactory

            // 2. 创建和配置 MapperConfigurationExpression
            var expression = new AutoMapper.MapperConfigurationExpression();
            expression.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            expression.ConstructServicesUsing(context.Resolve);

            // 3. 使用你提供的特定构造函数创建 MapperConfiguration
            var config = new MapperConfiguration(expression);

            return config.CreateMapper();
        }).As<IMapper>().InstancePerLifetimeScope();

        // 注册本地存储服务
        containerBuilder.RegisterType<LocalStorageService>().AsSelf().InstancePerLifetimeScope();
                
        // 注册事件总线服务（供插件使用）
        containerBuilder.RegisterType<Fastdotnet.WebApi.Services.EventBus.InMemoryEventBus>()
            .As<Fastdotnet.Plugin.Contracts.Events.IEventBus>()
            .SingleInstance();
                
        // 如果需要在 Autofac 中进行更精细的缓存服务控制，可以在这里添加
        //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().InstancePerLifetimeScope();
        //containerBuilder.RegisterType<HybridCacheService>().As<IHybridCacheService>().OwnedByLifetimeScope();
    }
}