
namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 计算结果
    public class MetricResult
    {
        public string MetricId { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
        public Dictionary<string, object> Dimensions { get; set; } = new();
        public DateTime CalculatedAt { get; set; } = DateTime.Now;
        public TimeSpan CalculationDuration { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}