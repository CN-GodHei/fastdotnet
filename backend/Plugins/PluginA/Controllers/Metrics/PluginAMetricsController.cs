using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Plugin.Contracts.Metrics;

namespace PluginA.Controllers.Metrics
{
    [ApiController]
    [Route("api/plugins/plugin-a/metrics")]
    public class PluginAMetricsController : ControllerBase
    {
        private readonly IMetricRegistry _metricRegistry;

        public PluginAMetricsController(IMetricRegistry metricRegistry)
        {
            _metricRegistry = metricRegistry;
        }

        /// <summary>
        /// 获取PluginA的指标定义
        /// </summary>
        [HttpGet("definitions")]
        public async Task<IActionResult> GetMetricDefinitions()
        {
            // 从指标注册中心获取所有指标定义
            // 注意：这里需要使用接口方法，而不是具体实现
            // 由于无法直接访问具体实现，我们暂时返回一个通用响应
            return Ok(new { message = "Metrics definitions endpoint is ready" });
        }

        /// <summary>
        /// 获取特定指标的值
        /// </summary>
        [HttpGet("{metricId}")]
        public async Task<IActionResult> GetMetricValue(
            string metricId, 
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? dimensions,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var context = new MetricQueryContext
            {
                StartDate = startDate,
                EndDate = endDate,
                Dimensions = dimensions?.Split(',') ?? Array.Empty<string>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = new Dictionary<string, object>()
            };

            try
            {
                var result = await _metricRegistry.CalculateMetricAsync(metricId, context);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 批量查询指标值
        /// </summary>
        [HttpPost("query")]
        public async Task<IActionResult> QueryMetrics([FromBody] MetricQueryRequest request)
        {
            var context = new MetricQueryContext
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Dimensions = request.Dimensions?.ToArray() ?? Array.Empty<string>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Filters = request.Filters ?? new Dictionary<string, object>()
            };

            var results = new List<MetricResult>();

            foreach (var metricId in request.MetricIds)
            {
                try
                {
                    var result = await _metricRegistry.CalculateMetricAsync(metricId, context);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = $"Error calculating metric {metricId}: {ex.Message}" });
                }
            }

            return Ok(results);
        }

        /// <summary>
        /// 获取PluginA的统计摘要
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var context = new MetricQueryContext
            {
                StartDate = DateTime.Today.AddDays(-7), // 最近7天
                EndDate = DateTime.Today
            };

            var summary = new
            {
                ActiveUsers = await GetMetricValueSafe("plugin_a.active_users", context),
                RequestCount = await GetMetricValueSafe("plugin_a.requests_count", context),
                AverageResponseTime = await GetMetricValueSafe("plugin_a.average_response_time", context)
            };

            return Ok(summary);
        }

        private async Task<object?> GetMetricValueSafe(string metricId, MetricQueryContext context)
        {
            try
            {
                var result = await _metricRegistry.CalculateMetricAsync(metricId, context);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }

    public class MetricQueryRequest
    {
        public List<string> MetricIds { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? Dimensions { get; set; }
        public Dictionary<string, object>? Filters { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}