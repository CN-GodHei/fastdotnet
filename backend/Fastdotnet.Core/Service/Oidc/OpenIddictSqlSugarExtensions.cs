using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Core.Service.Oidc.Resolvers;
using Fastdotnet.Core.Service.Oidc.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        // 设置默认实体类型
        builder.SetDefaultApplicationEntity<OpenIddictSqlSugarApplication>()
               .SetDefaultAuthorizationEntity<OpenIddictSqlSugarAuthorization>()
               .SetDefaultTokenEntity<OpenIddictSqlSugarToken>()
               .SetDefaultScopeEntity<OpenIddictSqlSugarScope>();

        // 替换 Store Resolver
        builder.ReplaceApplicationStoreResolver<OpenIddictSqlSugarApplicationStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceAuthorizationStoreResolver<OpenIddictSqlSugarAuthorizationStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceTokenStoreResolver<OpenIddictSqlSugarTokenStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceScopeStoreResolver<OpenIddictSqlSugarScopeStoreResolver>(ServiceLifetime.Singleton);

        // 注册 Store 实现
        builder.Services.TryAddScoped(typeof(OpenIddictSqlSugarApplicationStore));
        builder.Services.TryAddScoped(typeof(OpenIddictSqlSugarAuthorizationStore));
        builder.Services.TryAddScoped(typeof(OpenIddictSqlSugarTokenStore));
        builder.Services.TryAddScoped(typeof(OpenIddictSqlSugarScopeStore));

        // 配置选项
        builder.Services.AddOptions<OpenIddictSqlSugarOptions>();

        return builder;
    }
}
