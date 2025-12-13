namespace Fastdotnet.WebApi.Extensions;

public static class AutofacExtensions
{
    /// <summary>
    /// 启用 Autofac 并加载自定义模块
    /// </summary>
    public static IHostBuilder UseAutofacWithCustomModules(this IHostBuilder hostBuilder)
    {
        // 设置 ServiceProviderFactory 为 Autofac
        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        // 配置容器，加载模块
        hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule<ApplicationModule>();
            // 未来可添加更多模块：builder.RegisterModule<CacheModule>();
        });

        return hostBuilder;
    }
}