using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Core.Service.Oidc.Stores;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Fastdotnet.Core.Service.Oidc;

/// <summary>
/// 提供用于配置 OpenIddict SqlSugar 集成的扩展方法
/// </summary>
public static class OpenIddictSqlSugarExtensions
{
    /// <summary>
    /// 使用 SqlSugar Store 配置 OpenIddict
    /// </summary>
    /// <param name="builder">OpenIddict 构建器</param>
    /// <returns>OpenIddict 构建器</returns>
    public static OpenIddictCoreBuilder UseSqlSugar(this OpenIddictCoreBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        // 注册 SqlSugar Store 实现
        builder.Services.AddScoped<IOpenIddictApplicationStore<OpenIddictSqlSugarApplication>, OpenIddictSqlSugarApplicationStore>();
        builder.Services.AddScoped<IOpenIddictAuthorizationStore<OpenIddictSqlSugarAuthorization>, OpenIddictSqlSugarAuthorizationStore>();
        builder.Services.AddScoped<IOpenIddictScopeStore<OpenIddictSqlSugarScope>, OpenIddictSqlSugarScopeStore>();
        builder.Services.AddScoped<IOpenIddictTokenStore<OpenIddictSqlSugarToken>, OpenIddictSqlSugarTokenStore>();

        // 配置选项
        builder.Services.AddOptions<OpenIddictSqlSugarOptions>();

        return builder;
    }
}
