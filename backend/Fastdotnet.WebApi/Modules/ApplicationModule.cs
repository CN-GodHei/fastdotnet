using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fastdotnet.Core.Service.App;
using Fastdotnet.Core.Service.Sys;
using Fastdotnet.Service.IService.Sys;
using Fastdotnet.Service.IService.App;
using Fastdotnet.Service.Service.App;
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

        containerBuilder.RegisterType<PluginManager>().AsSelf().SingleInstance();
        containerBuilder.RegisterType<PluginStaticFileProviderRegistry>().AsSelf().SingleInstance();

        containerBuilder.Register(c => new PluginLoadService(
            c.Resolve<PluginManager>(),
            c.Resolve<ILifetimeScope>(),
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

        containerBuilder.RegisterType<FdDictDataService>().As<IFdDictDataService>().InstancePerLifetimeScope();
        
        // 应用端服务注册
        containerBuilder.RegisterType<Fastdotnet.Service.Service.App.FdTodoTaskService>().As<Fastdotnet.Service.IService.App.IFdTodoTaskService>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<Fastdotnet.Service.Service.App.FdNoticeService>().As<Fastdotnet.Service.IService.App.IFdNoticeService>().InstancePerLifetimeScope();
        
        // 管理端服务注册
        containerBuilder.RegisterType<Fastdotnet.Service.Service.Sys.FdTodoTaskService>().As<Fastdotnet.Service.IService.Sys.IFdTodoTaskService>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<Fastdotnet.Service.Service.Sys.FdNoticeService>().As<Fastdotnet.Service.IService.Sys.IFdNoticeService>().InstancePerLifetimeScope();

        // 注册密码服务
        containerBuilder.RegisterType<Fastdotnet.Service.Service.Sys.PasswordService>()
            .As<Fastdotnet.Service.IService.Sys.IPasswordService>()
            .InstancePerLifetimeScope();

        // 注册通用服务以支持泛型依赖注入，类似 Program.cs 中的注册
        containerBuilder.RegisterGeneric(typeof(BaseService<,>)).As(typeof(IBaseService<,>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

        containerBuilder.RegisterGeneric(typeof(RawRepository<>)).As(typeof(IRawRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(RawRepository<,>)).As(typeof(IRawRepository<,>)).InstancePerLifetimeScope();

        // --- 自动注册所有服务 (以 Service 结尾的类) ---
        var serviceAssembly = typeof(FdDictDataService).Assembly;
        containerBuilder.RegisterAssemblyTypes(serviceAssembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        // -------------------------------------------

        // 在 Load 方法中立即初始化 Mapster（而不是延迟注册）
        System.Console.WriteLine("[DEBUG] ApplicationModule.Load 开始执行...");
        
        var config = TypeAdapterConfig.GlobalSettings;
        
        // 显式指定要扫描的程序集（确保 Service 层的 Register 被加载）
        var assemblies = new[]
        {
            typeof(Fastdotnet.Core.Dtos.Base.IAuditableEntity).Assembly, // Core 层
            typeof(Fastdotnet.Service.Mappings.ServiceDtoMappingRegistry).Assembly, // Service 层
            typeof(Fastdotnet.WebApi.Modules.ApplicationModule).Assembly // WebApi 层
        };

        System.Console.WriteLine($"[DEBUG] 准备扫描 {assemblies.Length} 个程序集:");
        foreach (var asm in assemblies)
        {
            System.Console.WriteLine($"  - {asm.GetName().Name} (Location: {asm.Location})");
        }

        // 扫描所有程序集，自动注册 IRegister 接口
        try
        {
            config.Scan(assemblies);
            System.Console.WriteLine("[DEBUG] Mapster Scan 成功完成");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"[ERROR] Mapster Scan 失败: {ex.Message}");
            throw;
        }
        
        // 在 Autofac 中注册 Mapster（仅用于依赖注入）
        containerBuilder.Register(c => config).As<TypeAdapterConfig>().SingleInstance();

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