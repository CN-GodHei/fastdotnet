using System;
using System.Collections.Generic;

namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 计算结果
    public class MetricResult
    {
        public string MetricId { get; set; }
        public object Value { get; set; }
        public Dictionary<string, object> Dimensions { get; set; } = new();
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan CalculationDuration { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}