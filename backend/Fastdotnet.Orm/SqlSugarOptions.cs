
namespace Fastdotnet.Orm;

public class SqlSugarOptions
{
    public const string SectionName = "SqlSugar";

    public List<ConnectionConfig> Connections { get; set; }
    
    /// <summary>
    /// 是否将SQL执行日志记录到独立的日志表中
    /// </summary>
    public bool EnableSqlExecutionLogging { get; set; } = false;
}