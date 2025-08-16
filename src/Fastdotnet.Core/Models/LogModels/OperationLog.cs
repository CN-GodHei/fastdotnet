using Fastdotnet.Core.Utils;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.LogModels;

/// <summary>
/// 操作日志
/// </summary>
[SugarTable("log_operation")]
public class OperationLog
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = SnowflakeIdGenerator.NextId();

    /// <summary>
    /// 请求ID，用于追踪整个请求链路
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// 请求IP
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 请求头
    /// </summary>
    [SugarColumn(Length = 2000, IsNullable = true)]
    public string Headers { get; set; }
    
    /// <summary>
    /// 请求体
    /// </summary>
    [SugarColumn(Length = 2000, IsNullable = true)]
    public string Body { get; set; }

    /// <summary>
    /// 响应状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    public string ElapsedMilliseconds { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}