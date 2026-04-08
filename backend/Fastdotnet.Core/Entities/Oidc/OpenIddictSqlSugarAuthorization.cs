/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using SqlSugar;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Fastdotnet.Core.Entities.Oidc;

/// <summary>
/// OpenIddict 授权实体
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; Subject = {Subject,nq} ; Type = {Type,nq} ; Status = {Status,nq}")]
[SugarTable("oidc_Authorizations")]
[SugarIndex("Index_ApplicationId_Status_Subject_Type", [nameof(ApplicationId),nameof(Status),nameof(Subject),nameof(Type)]
, [OrderByType.Asc,OrderByType.Asc,OrderByType.Asc,OrderByType.Asc])]
public class OpenIddictSqlSugarAuthorization
{

    /// <summary>
    /// 获取或设置当前授权的唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public virtual Guid Id { get; set; }

    [SugarColumn(IsNullable = true)]
    public virtual Guid ApplicationId { get; set; }

    /// <summary>
    /// 获取或设置并发令牌（用于乐观锁）
    /// </summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true, IsNullable = true)]
    public virtual string? ConcurrencyToken { get; set; }

    /// <summary>
    /// 获取或设置当前授权的 UTC 创建时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual DateTime? CreationDate { get; set; }

    /// <summary>
    /// 获取或设置序列化为 JSON 对象的附加属性，如果没有则为 null
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Properties { get; set; }

    /// <summary>
    /// 获取或设置与当前授权关联的作用域，序列化为 JSON 数组
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? Scopes { get; set; }

    /// <summary>
    /// 获取或设置当前授权的状态
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public virtual string? Status { get; set; }

    /// <summary>
    /// 获取或设置与当前授权关联的主题
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public virtual string? Subject { get; set; }

    /// <summary>
    /// 获取或设置当前授权的类型
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public virtual string? Type { get; set; }

    /// <summary>
    /// 获取或设置与当前授权关联的应用程序
    /// </summary>
    [Navigate(NavigateType.ManyToOne,nameof(ApplicationId),nameof(OpenIddictSqlSugarApplication.Id))]
    public virtual OpenIddictSqlSugarApplication? Application { get; set; }

    /// <summary>
    /// 获取与当前授权关联的令牌列表
    /// </summary>
    [Navigate(NavigateType.OneToMany,nameof(OpenIddictSqlSugarToken.AuthorizationId))]
    public virtual List<OpenIddictSqlSugarToken>? Tokens { get; set; }

}
