using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fastdotnet.Plugin.Contracts.Metrics;

namespace PluginA.Metrics
{
    public class PluginAMetricProvider : IMetricProvider
    {
        public string PluginId => "plugin-a";

        public async Task<IEnumerable<IMetricDefinition>> GetMetricDefinitionsAsync()
        {
            return new List<IMetricDefinition>
            {
                new MetricDefinition
                {
                    Id = "plugin_a.active_users",
                    Name = "PluginA活跃用户数",
                    Category = "user",
                    PluginId = PluginId,
                    ValueType = MetricValueType.Number,
                    Description = "使用PluginA的活跃用户数量",
                    Dimensions = new[] { "date", "region" },
                    Tags = new[] { "user", "engagement" }
                },
                new MetricDefinition
                {
                    Id = "plugin_a.requests_count",
                    Name = "PluginA请求次数",
                    Category = "performance",
                    PluginId = PluginId,
                    ValueType = MetricValueType.Number,
                    Description = "PluginA收到的请求数量",
                    Dimensions = new[] { "endpoint", "date" },
                    Tags = new[] { "performance", "monitoring" }
                },
                new MetricDefinition
                {
                    Id = "plugin_a.average_response_time",
                    Name = "PluginA平均响应时间",
                    Category = "performance",
                    PluginId = PluginId,
                    ValueType = MetricValueType.Number,
                    Description = "PluginA的平均响应时间(毫秒)",
                    Dimensions = new[] { "endpoint", "date" },
                    Tags = new[] { "performance", "latency" }
                }
            };
        }

        public async Task<MetricResult> CalculateAsync(string metricId, MetricQueryContext context)
        {
            return metricId switch
            {
                "plugin_a.active_users" => await CalculateActiveUsersAsync(context),
                "plugin_a.requests_count" => await CalculateRequestsCountAsync(context),
                "plugin_a.average_response_time" => await CalculateAverageResponseTimeAsync(context),
                _ => throw new MetricNotFoundException(metricId)
            };
        }

        private async Task<MetricResult> CalculateActiveUsersAsync(MetricQueryContext context)
        {
            // 模拟业务逻辑，实际实现会连接数据库
            await Task.Delay(10); // 模拟延迟
            
            var value = new Random().Next(100, 1000); // 随机生成用户数
            
            return new MetricResult
            {
                MetricId = "plugin_a.active_users",
                Value = value,
                Dimensions = new Dictionary<string, object>
                {
                    ["date"] = context.StartDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    ["region"] = context.Filters.ContainsKey("region") ? context.Filters["region"].ToString() : "ALL"
                }
            };
        }

        private async Task<MetricResult> CalculateRequestsCountAsync(MetricQueryContext context)
        {
            // 模拟业务逻辑
            await Task.Delay(10); // 模拟延迟
            
            var value = new Random().Next(1000, 10000); // 随机生成请求数
            
            return new MetricResult
            {
                MetricId = "plugin_a.requests_count",
                Value = value,
                Dimensions = new Dictionary<string, object>
                {
                    ["endpoint"] = context.Filters.ContainsKey("endpoint") ? context.Filters["endpoint"].ToString() : "ALL",
                    ["date"] = context.StartDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd")
                }
            };
        }

        private async Task<MetricResult> CalculateAverageResponseTimeAsync(MetricQueryContext context)
        {
            // 模拟业务逻辑
            await Task.Delay(10); // 模拟延迟
            
            var value = new Random().Next(50, 500); // 随机生成响应时间（毫秒）
            
            return new MetricResult
            {
                MetricId = "plugin_a.average_response_time",
                Value = value,
                Dimensions = new Dictionary<string, object>
                {
                    ["endpoint"] = context.Filters.ContainsKey("endpoint") ? context.Filters["endpoint"].ToString() : "ALL",
                    ["date"] = context.StartDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd")
                }
            };
        }
    }
    
    public class MetricDefinition : IMetricDefinition
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
    }
    
    public class MetricNotFoundException : Exception
    {
        public MetricNotFoundException(string metricId) : base($"Metric with ID '{metricId}' not found.")
        {
        }
    }
}