using Fastdotnet.Core.Utils;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.LogModels;

/// <summary>
/// 调试日志
/// </summary>
[SugarTable("log_debug")]
public class DebugLog
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = SnowflakeIdGenerator.NextStrId();

    /// <summary>
    /// 请求ID，用于追踪整个请求链路
    /// </summary>
    public string RequestId { get; set; }


    /// <summary>
    /// 日志消息
    /// </summary>
    [SugarColumn(Length = 4000)]
    public string Message { get; set; }

    /// <summary>
    /// 业务标识
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}