
namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 查询上下文
    public class MetricQueryContext
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new();
        public string[] Dimensions { get; set; } = Array.Empty<string>();
        public string TimeGranularity { get; set; } = "day";
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}