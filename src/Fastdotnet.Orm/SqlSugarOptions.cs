using SqlSugar;

namespace Fastdotnet.Orm;

public class SqlSugarOptions
{
    public const string SectionName = "SqlSugar";

    public List<ConnectionConfig> Connections { get; set; }
}
