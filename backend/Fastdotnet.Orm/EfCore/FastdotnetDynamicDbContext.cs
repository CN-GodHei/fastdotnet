using Microsoft.EntityFrameworkCore;

namespace Fastdotnet.Orm.EfCore;

/// <summary>
/// Fastdotnet 动态基础设施 DbContext
/// 专门用于托管第三方插件（如 Elsa）的 EF Core 运行时实例
/// </summary>
public class FastdotnetDynamicDbContext : DbContext
{
    private readonly IEfCoreConnectionProvider _connectionProvider;
    private readonly string _configId;

    // 构造函数通过 DI 拿到连接提供器
    public FastdotnetDynamicDbContext(
        DbContextOptions<FastdotnetDynamicDbContext> options,
        IEfCoreConnectionProvider connectionProvider,
        string configId = "main") : base(options)
    {
        _connectionProvider = connectionProvider;
        _configId = configId;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 如果外部已经配置过 options（比如通过 AddDbContextPool），则跳过
        if (optionsBuilder.IsConfigured) return;

        // 动态核心：根据传入的标识，实时获取数据库连接与驱动类型
        var config = _connectionProvider.GetConnection(_configId)
            ?? throw new InvalidOperationException($"未找到 ConfigId 为 '{_configId}' 的 EF Core 数据库配置。");

        // 调用你写好的扩展方法，实现一键实例化物理驱动！
        optionsBuilder.UseDatabaseProvider(config.ConnectionString, config.DatabaseType);
    }
}