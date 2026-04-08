/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */
using SqlSugar;
using System.Diagnostics.CodeAnalysis;

namespace Fastdotnet.Core.Entities.Oidc;

/// <summary>
/// OpenIddict 作用域实体
/// </summary>
[SugarTable("oidc_Scopes")]
[SugarIndex("Index_OpenIddictScopes_Name", nameof(Name), OrderByType.Asc, true)]
public class OpenIddictSqlSugarScope
{
    /// <summary>
    /// 获取或设置当前作用域的唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public virtual Guid Id { get; set; }

    /// <summary>
    /// 获取或设置并发令牌（用于乐观锁）
    /// </summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true,Length = 50,IsNullable = true)]
    public virtual Guid ConcurrencyToken { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的公共描述
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? Description { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的本地化公共描述，序列化为 JSON 对象
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Descriptions { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的显示名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的本地化显示名称，序列化为 JSON 对象
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? DisplayNames { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的唯一名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 获取或设置序列化为 JSON 对象的附加属性，如果没有则为 null
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Properties { get; set; }

    /// <summary>
    /// 获取或设置与当前作用域关联的资源，序列化为 JSON 数组
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Json)]
    [SugarColumn(IsNullable = true)]
    public virtual string? Resources { get; set; }
}