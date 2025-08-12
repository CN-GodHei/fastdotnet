using Fastdotnet.Core.Utils;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.LogModels;

/// <summary>
/// SQL执行日志
/// </summary>
[SugarTable("log_sql_execution")]
public class SqlExecutionLog
{
    [SugarColumn(IsPrimaryKey = true)]
    public long Id { get; set; } = SnowflakeIdGenerator.NextId();

    /// <summary>
    /// 请求ID，用于追踪整个请求链路
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string RequestId { get; set; }

    /// <summary>
    /// 执行的完整SQL语句（包含参数值）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string FullSql { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long ElapsedMilliseconds { get; set; }

    /// <summary>
    /// 是否有错误
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = true)]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 堆栈跟踪
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string StackTrace { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}