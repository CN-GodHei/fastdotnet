
namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 支持SQL定义的指标
    public class SqlMetricDefinition : IMetricDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PluginId { get; set; } = string.Empty;
        public MetricValueType ValueType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public string[] Dimensions { get; set; } = Array.Empty<string>();
        public string[] Tags { get; set; } = Array.Empty<string>();
        public MetricDefinitionType DefinitionType { get; set; } = MetricDefinitionType.Sql;
        
        // SQL特定属性
        public string SqlTemplate { get; set; } = string.Empty;          // SQL模板
        public string DataSource { get; set; } = string.Empty;           // 数据源：sales_db
        public Dictionary<string, SqlParameter> Parameters { get; set; } = new(); // 参数
        public string CachePolicy { get; set; } = string.Empty;          // 缓存策略
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnableRealTime { get; set; } = false;                // 是否支持实时计算
        
        // SQL验证信息
        public string SqlHash { get; set; } = string.Empty;              // 用于检测SQL变更
        public DateTime LastValidated { get; set; }
    }
    
    public class SqlParameter
    {
        public string Name { get; set; } = string.Empty;
        public object Value { get; set; } = new object();
        public string DbType { get; set; } = string.Empty;  // 如 "int", "varchar", "datetime" 等
        public bool IsRequired { get; set; } = false;
    }
}