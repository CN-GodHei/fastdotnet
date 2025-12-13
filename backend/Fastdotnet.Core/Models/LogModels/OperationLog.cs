using Fastdotnet.Core.Utils;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.LogModels;

/// <summary>
/// 操作日志
/// </summary>
[SplitTable(SplitType.Year)]
[SugarTable("log_operation_{year}_{month}_{day}")]
public class OperationLog
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "")]
    public string Id { get; set; } = SnowflakeIdGenerator.NextStrId();

    /// <summary>
    /// 请求ID，用于追踪整个请求链路
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// 操作人用户ID（如：UserId），匿名或系统任务可为 null
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? OperatorId { get; set; }

    /// <summary>
    /// 操作人用户名/昵称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true,ColumnDescription ="")]
    public string? OperatorName { get; set; }

    /// <summary>
    /// 操作描述，如“创建用户”、“删除订单”
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "")]
    public string? Operation { get; set; }

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

    [SugarColumn(Length = 15, IsNullable = true,ColumnDescription = "操作用户类型：Admin/App")]
    public string UserType { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}