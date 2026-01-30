using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Contracts.Metrics
{
    public interface IMetricRegistry
    {
        void RegisterProvider(IMetricProvider provider);
        Task<MetricResult> CalculateMetricAsync(string metricId, MetricQueryContext context);
    }
}