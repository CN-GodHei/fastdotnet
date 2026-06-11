using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Fastdotnet.Orm.EfCore;

public static class EfCoreServiceCollectionExtensions
{
    /// <summary>
    /// 标准重载：直接传入 IConfiguration，内部自动解析并注册所有依赖
    /// </summary>
    public static IServiceCollection AddFastdotnetEfCore(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "SqlSugar") // 默认读取你的 SqlSugar 配置节点
    {
        // 1. 从配置中强类型绑定出 SqlSugarOptions
        var sqlSugarOptions = new SqlSugarOptions();
        configuration.GetSection(sectionName).Bind(sqlSugarOptions);

        if (sqlSugarOptions.Connections == null || !sqlSugarOptions.Connections.Any())
        {
            throw new InvalidOperationException($"[Fastdotnet.Orm.EfCore] 未能在配置节点 '{sectionName}' 下找到任何合法的数据库连接。");
        }

        // 2. 顺便把解析出来的单例选项注册进容器，防止其他插件需要用
        services.AddSingleton(sqlSugarOptions);

        // 3. 调用下方的核心注册逻辑
        return services.AddFastdotnetEfCore(sqlSugarOptions);
    }

    /// <summary>
    /// 核心重载：接受已经实例化的选项（供高级定制或进程内共享使用）
    /// </summary>
    public static IServiceCollection AddFastdotnetEfCore(
        this IServiceCollection services,
        SqlSugarOptions sqlSugarOptions)
    {
        // 1. 注册连接提供器（图纸）
        var provider = new EfCoreConnectionProvider(sqlSugarOptions);
        services.AddSingleton<IEfCoreConnectionProvider>(provider);

        // 2. 注册动态 DbContext 工厂（为什么用 Factory？因为 Elsa 等工作流存在高并发多线程调度，用工厂能彻底避免 DbContext 线程冲突）
        services.AddDbContextFactory<FastdotnetDynamicDbContext>((sp, options) =>
        {
            var connProvider = sp.GetRequiredService<IEfCoreConnectionProvider>();

            // 默认拿到主库的连接图纸
            var mainConfig = connProvider.GetConnection("main")
                ?? throw new Exception("⚠️ [Fastdotnet.Orm.EfCore] 未找到主数据库配置 [main]，请检查 appsettings.json");

            // 生产物理驱动实例
            options.UseDatabaseProvider(mainConfig.ConnectionString, mainConfig.DatabaseType);
        });

        // 3. 注册标准的范围内范围实例（Scoped），方便正常的业务代码直接在构造函数注入 DbContext
        services.AddScoped(sp =>
            sp.GetRequiredService<IDbContextFactory<FastdotnetDynamicDbContext>>().CreateDbContext());

        return services;
    }
}