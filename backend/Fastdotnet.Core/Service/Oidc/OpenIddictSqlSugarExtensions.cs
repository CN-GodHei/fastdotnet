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
    /// 使用 SqlSugar Store 配置 OpenIddict（适用于 OpenIddict 7.x）
    /// </summary>
    /// <param name="builder">OpenIddict 构建器</param>
    /// <returns>OpenIddict 构建器</returns>
    public static OpenIddictCoreBuilder UseSqlSugar(this OpenIddictCoreBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        // 替换 Store 实现
        builder.ReplaceApplicationStore<OpenIddictSqlSugarApplication, OpenIddictSqlSugarApplicationStore>()
               .ReplaceAuthorizationStore<OpenIddictSqlSugarAuthorization, OpenIddictSqlSugarAuthorizationStore>()
               .ReplaceScopeStore<OpenIddictSqlSugarScope, OpenIddictSqlSugarScopeStore>()
               .ReplaceTokenStore<OpenIddictSqlSugarToken, OpenIddictSqlSugarTokenStore>();

        // 重要：替换 Manager 注册，否则会使用默认的抛出异常的工厂
        builder.Services.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Scoped<OpenIddict.Abstractions.IOpenIddictApplicationManager>(provider =>
            provider.GetRequiredService<OpenIddict.Core.OpenIddictApplicationManager<OpenIddictSqlSugarApplication>>()));
        builder.Services.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Scoped<OpenIddict.Abstractions.IOpenIddictAuthorizationManager>(provider =>
            provider.GetRequiredService<OpenIddict.Core.OpenIddictAuthorizationManager<OpenIddictSqlSugarAuthorization>>()));
        builder.Services.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Scoped<OpenIddict.Abstractions.IOpenIddictScopeManager>(provider =>
            provider.GetRequiredService<OpenIddict.Core.OpenIddictScopeManager<OpenIddictSqlSugarScope>>()));
        builder.Services.Replace(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Scoped<OpenIddict.Abstractions.IOpenIddictTokenManager>(provider =>
            provider.GetRequiredService<OpenIddict.Core.OpenIddictTokenManager<OpenIddictSqlSugarToken>>()));

        // 配置选项
        builder.Services.AddOptions<OpenIddictSqlSugarOptions>();

        return builder;
    }
}
