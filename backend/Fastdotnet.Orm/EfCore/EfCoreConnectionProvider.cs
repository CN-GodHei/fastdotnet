
namespace Fastdotnet.Orm.EfCore;

/// <summary>
/// EF Core 连接配置
/// </summary>
public record EfCoreConnectionConfig(
    string ConfigId,
    string ConnectionString,
    SqlSugar.DbType DatabaseType
);

/// <summary>
/// EF Core 连接提供器接口
/// </summary>
public interface IEfCoreConnectionProvider
{
    /// <summary>
    /// 获取指定连接的配置
    /// </summary>
    /// <param name="configId">连接标识，默认 "default"</param>
    /// <returns>连接配置，未找到返回 null</returns>
    EfCoreConnectionConfig? GetConnection(string configId = "default");

    /// <summary>
    /// 获取所有已注册的连接配置
    /// </summary>
    IEnumerable<EfCoreConnectionConfig> GetAllConnections();
}

/// <summary>
/// EF Core 连接提供器实现
/// 复用 SqlSugarOptions.Connections 中的连接配置，
/// 避免重复配置数据库连接信息
/// </summary>
public class EfCoreConnectionProvider : IEfCoreConnectionProvider
{
    private readonly List<EfCoreConnectionConfig> _connections;

    public EfCoreConnectionProvider(SqlSugarOptions options)
    {
        _connections = options.Connections?
            .Select(c => new EfCoreConnectionConfig(
                c.ConfigId?.ToString() ?? "default",
                c.ConnectionString,
                c.DbType
            ))
            .ToList() ?? new List<EfCoreConnectionConfig>();
    }

    public EfCoreConnectionConfig? GetConnection(string configId = "default")
    {
        return _connections.FirstOrDefault(c =>
            string.Equals(c.ConfigId, configId, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<EfCoreConnectionConfig> GetAllConnections()
    {
        return _connections;
    }
}
