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
/// 提供解析应用程序 Store 的方法
/// </summary>
public sealed class OpenIddictSqlSugarApplicationStoreResolver : IOpenIddictApplicationStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type?> _cache;
    private readonly IServiceProvider _provider;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OpenIddictSqlSugarApplicationStoreResolver(IServiceProvider provider,
       IServiceScopeFactory serviceScopeFactory)

    {
        _cache = new ConcurrentDictionary<Type, Type?>();
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// 返回与指定应用程序类型兼容的应用程序 Store
    /// </summary>
    /// <typeparam name="TApplication">应用程序实体类型</typeparam>
    /// <returns>IOpenIddictApplicationStore 实例</returns>
    public IOpenIddictApplicationStore<TApplication> Get<TApplication>() where TApplication : class
    {
        var store = _provider.GetService<IOpenIddictApplicationStore<TApplication>>();
        if (store is not null)
        {
            return store;
        }

        var invalidMessage = "指定的类型与 SqlSugar 应用程序 Store 不兼容。";

        var type = _cache.GetOrAdd(typeof(TApplication), key =>
        {
            if(!typeof(OpenIddictSqlSugarApplication).IsAssignableFrom(key)){
                throw new InvalidOperationException(invalidMessage);
            }

            return typeof(OpenIddictSqlSugarApplicationStore);
        });

        if(type == null)
        {
            throw new InvalidOperationException(invalidMessage);   
        }

        using var scope = _provider.CreateScope();
        store = (IOpenIddictApplicationStore<TApplication>)scope.ServiceProvider.GetRequiredService(type);
        return store;
    }
}
