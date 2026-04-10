/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */
using SqlSugar;
using System.Diagnostics.CodeAnalysis;

namespace Fastdotnet.Core.Entities.Oidc;

/// <summary>
/// OpenIddict 应用程序实体
/// </summary>
[SugarTable("oidc_Applications")]
[SugarIndex("Index_OpenIddictApplications_ClientId", nameof(ClientId), OrderByType.Asc, true)]
public class OpenIddictSqlSugarApplication
{
    /// <summary>
    /// 获取或设置当前应用程序的唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public virtual Guid Id { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的应用类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public virtual string? ApplicationType { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的客户端标识符
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public virtual string? ClientId { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的客户端密钥
    /// 注意：根据用于创建此实例的应用程序管理器，此属性可能出于安全原因进行哈希或加密
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? ClientSecret { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的客户端类型
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public virtual string? ClientType { get; set; }

    /// <summary>
    /// 获取或设置并发令牌（用于乐观锁）
    /// </summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true, IsNullable = true)]
    public virtual Guid ConcurrencyToken { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的同意类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public virtual string? ConsentType { get; set; }

    /// <summary>
    /// 获取或设置当前应用程序的显示名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// 获取或设置与当前应用程序关联的本地化显示名称，序列化为 JSON 对象
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? DisplayNames { get; set; }

    /// <summary>
    /// 获取或设置与应用程序关联的 JSON Web Key Set，序列化为 JSON 对象
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? JsonWebKeySet { get; set; }

    /// <summary>
    /// 获取或设置与当前应用程序关联的权限，序列化为 JSON 数组
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Permissions { get; set; }

    /// <summary>
    /// 获取或设置与当前应用程序关联的登出后重定向 URI，序列化为 JSON 数组
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true, ColumnDataType = "text")]
    public virtual string? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// 获取或设置序列化为 JSON 对象的附加属性，如果没有则为 null
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Properties { get; set; }

    /// <summary>
    /// 获取或设置与当前应用程序关联的重定向 URI，序列化为 JSON 数组
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true, ColumnDataType = "text")]
    public virtual string? RedirectUris { get; set; }

    /// <summary>
    /// 获取或设置与当前应用程序关联的要求，序列化为 JSON 数组
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Requirements { get; set; }

    /// <summary>
    /// 获取或设置序列化为 JSON 对象的设置
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Settings { get; set; }

    /// <summary>
    /// 获取与此应用程序关联的授权列表
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(OpenIddictSqlSugarAuthorization.ApplicationId))]
    public virtual List<OpenIddictSqlSugarAuthorization>? Authorizations { get; set; }

    /// <summary>
    /// 获取与此应用程序关联的令牌列表
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(OpenIddictSqlSugarToken.ApplicationId))]
    public virtual List<OpenIddictSqlSugarToken>? Tokens { get; set; }


}

