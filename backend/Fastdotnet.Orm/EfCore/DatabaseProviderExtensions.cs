
using Microsoft.EntityFrameworkCore;

namespace Fastdotnet.Orm.EfCore;

/// <summary>
/// EF Core 数据库提供器扩展方法
/// 将 SqlSugar 的 DbType 映射为 EF Core 的数据库 Provider
/// </summary>
public static class DatabaseProviderExtensions
{
    /// <summary>
    /// 根据数据库类型自动配置 EF Core Provider
    /// </summary>
    /// <param name="builder">DbContextOptionsBuilder</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <param name="dbType">SqlSugar 数据库类型</param>
    /// <returns>配置后的 builder</returns>
    /// <exception cref="NotSupportedException">不支持的数据库类型</exception>
    public static DbContextOptionsBuilder UseDatabaseProvider(
        this DbContextOptionsBuilder builder,
        string connectionString,
        SqlSugar.DbType dbType)
    {
        return dbType switch
        {
            SqlSugar.DbType.PostgreSQL => builder.UseNpgsql(connectionString),
            // MySql 支持: 当 Pomelo.EntityFrameworkCore.MySql 发布 .NET 10 版本后，取消下方注释
            SqlSugar.DbType.MySql => builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
            SqlSugar.DbType.SqlServer => builder.UseSqlServer(connectionString),
            SqlSugar.DbType.Sqlite => builder.UseSqlite(connectionString),
            _ => throw new NotSupportedException($"EF Core does not support the database type '{dbType}'. Supported types: PostgreSQL, SqlServer, Sqlite.")
        };
    }
}
