/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */
using SqlSugar;
using System.Diagnostics.CodeAnalysis;

namespace Fastdotnet.Core.Entities.Oidc;

/// <summary>
/// OpenIddict 令牌实体
/// </summary>
[SugarTable("OpenIddictTokens")]
[SugarIndex("Index_OpenIddictTokens_ReferenceId", nameof(ReferenceId), OrderByType.Asc, true)]
[SugarIndex("Index_OpenIddictTokens_ApplicationId_Status_Subject_Type",
    new[] { nameof(ApplicationId), nameof(Status), nameof(Subject), nameof(Type) },
    new[] { OrderByType.Asc, OrderByType.Asc, OrderByType.Asc, OrderByType.Asc })]
public class OpenIddictSqlSugarToken
{
    /// <summary>
    /// 获取或设置当前令牌的唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public virtual Guid Id { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的应用程序
    /// </summary>
    [Navigate(NavigateType.ManyToOne,nameof(ApplicationId), nameof(OpenIddictSqlSugarApplication.Id))]
    public virtual OpenIddictSqlSugarApplication? Application { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的授权
    /// </summary>
    [Navigate(NavigateType.ManyToOne,nameof(AuthorizationId), nameof(OpenIddictSqlSugarAuthorization.Id))]
    public virtual OpenIddictSqlSugarAuthorization? Authorization { get; set; }

    /// <summary>
    /// 获取或设置并发令牌（用于乐观锁）
    /// </summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true,IsNullable = true)]
    public virtual Guid ConcurrencyToken { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的 UTC 创建时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual DateTime? CreationDate { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的 UTC 过期时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的载荷数据（如果适用）
    /// 注意：此属性仅用于引用令牌，并可能出于安全原因进行加密
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? Payload { get; set; }

    /// <summary>
    /// 获取或设置序列化为 JSON 对象的附加属性，如果没有则为 null
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Properties { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的 UTC 赎回时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual DateTime? RedemptionDate { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的引用标识符（如果适用）
    /// 注意：此属性仅用于引用令牌，并可能出于安全原因进行哈希或加密
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public virtual string? ReferenceId { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的状态
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public virtual string? Status { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的主题
    /// </summary>
    [SugarColumn(Length = 400, IsNullable = true)]
    public virtual string? Subject { get; set; }

    /// <summary>
    /// 获取或设置当前令牌的类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public virtual string? Type { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的应用程序标识符
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual Guid? ApplicationId { get; set; }

    /// <summary>
    /// 获取或设置与当前令牌关联的授权标识符
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual Guid? AuthorizationId { get; set; }
}