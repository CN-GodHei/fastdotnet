/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Concurrent;
using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Core.Service.Oidc.Stores;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Fastdotnet.Core.Service.Oidc.Resolvers;

/// <summary>
/// 提供解析授权 Store 的方法
/// </summary>
public sealed class OpenIddictSqlSugarAuthorizationStoreResolver : IOpenIddictAuthorizationStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type?> _cache;
    private readonly IServiceProvider _provider;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OpenIddictSqlSugarAuthorizationStoreResolver(IServiceProvider provider,
       IServiceScopeFactory serviceScopeFactory)
    {
        _cache = new ConcurrentDictionary<Type, Type?>();
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// 返回与指定授权类型兼容的授权 Store
    /// </summary>
    /// <typeparam name="TAuthorization">授权实体类型</typeparam>
    /// <returns>IOpenIddictAuthorizationStore 实例</returns>
    public IOpenIddictAuthorizationStore<TAuthorization> Get<TAuthorization>() where TAuthorization : class
    {
        var store = _provider.GetService<IOpenIddictAuthorizationStore<TAuthorization>>();
        if (store is not null)
        {
            return store;
        }

        var invalidMessage = "指定的类型与 SqlSugar 授权 Store 不兼容。";

        var type = _cache.GetOrAdd(typeof(TAuthorization), key =>
        {
            if(!typeof(OpenIddictSqlSugarAuthorization).IsAssignableFrom(key)){
                throw new InvalidOperationException(invalidMessage);
            }

            return typeof(OpenIddictSqlSugarAuthorizationStore);
        });

        if(type == null)
        {
            throw new InvalidOperationException(invalidMessage);   
        }

        using var scope = _provider.CreateScope();
        store = (IOpenIddictAuthorizationStore<TAuthorization>)scope.ServiceProvider.GetRequiredService(type);
        return store;
    }
}
