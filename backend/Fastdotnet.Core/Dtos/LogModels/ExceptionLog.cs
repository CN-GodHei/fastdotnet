using Fastdotnet.Core.Utils;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Dtos.LogModels;

/// <summary>
/// 异常日志
/// </summary>
[SugarTable("log_exception")]
public class ExceptionLog
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = SnowflakeIdGenerator.NextStrId();

    /// <summary>
    /// 请求ID，用于追踪整个请求链路
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExceptionType { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(Length = 4000)]
    public string Message { get; set; }

    /// <summary>
    /// 堆栈跟踪
    /// </summary>
    [SugarColumn(IsJson = true, IsNullable = true)]
    public string StackTrace { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}