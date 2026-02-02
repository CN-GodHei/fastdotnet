
namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 指标提供者接口 - 业务插件实现
    public interface IMetricProvider
    {
        string PluginId { get; }
        Task<IEnumerable<IMetricDefinition>> GetMetricDefinitionsAsync();
        Task<MetricResult> CalculateAsync(string metricId, MetricQueryContext context);
    }
}