using System;
using System.Collections.Generic;

namespace Fastdotnet.Plugin.Contracts.Metrics
{
    // 指标定义接口
    public interface IMetricDefinition
    {
        string Id { get; }                     // 唯一标识：order.daily_sales
        string Name { get; }                   // 显示名称：日订单销售额
        string Category { get; }               // 分类：order/finance/user
        string PluginId { get; }               // 所属插件ID
        MetricValueType ValueType { get; }     // 值类型：Number/Currency/Percentage
        string Description { get; }
        string Formula { get; }                // 计算公式说明
        string[] Dimensions { get; }           // 支持维度：region, product_type
        string[] Tags { get; }                 // 标签：revenue,kpi
        MetricDefinitionType DefinitionType { get; }  // 定义类型：Code/Sql
    }

    public enum MetricValueType
    {
        Number,
        Currency,
        Percentage,
        String,
        Boolean,
        Date,
        DateTime
    }

    public enum MetricDefinitionType
    {
        Code,
        Sql
    }
}