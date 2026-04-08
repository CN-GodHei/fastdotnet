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
/// 提供解析作用域 Store 的方法
/// </summary>
public sealed class OpenIddictSqlSugarScopeStoreResolver : IOpenIddictScopeStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type?> _cache;
    private readonly IServiceProvider _provider;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OpenIddictSqlSugarScopeStoreResolver(IServiceProvider provider,
       IServiceScopeFactory serviceScopeFactory)
    {
        _cache = new ConcurrentDictionary<Type, Type?>();
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// 返回与指定作用域类型兼容的作用域 Store
    /// </summary>
    /// <typeparam name="TScope">作用域实体类型</typeparam>
    /// <returns>IOpenIddictScopeStore 实例</returns>
    public IOpenIddictScopeStore<TScope> Get<TScope>() where TScope : class
    {
        var store = _provider.GetService<IOpenIddictScopeStore<TScope>>();
        if (store is not null)
        {
            return store;
        }

        var invalidMessage = "指定的类型与 SqlSugar 作用域 Store 不兼容。";

        var type = _cache.GetOrAdd(typeof(TScope), key =>
        {
            if(!typeof(OpenIddictSqlSugarScope).IsAssignableFrom(key)){
                throw new InvalidOperationException(invalidMessage);
            }

            return typeof(OpenIddictSqlSugarScopeStore);
        });

        if(type == null)
        {
            throw new InvalidOperationException(invalidMessage);   
        }

        using var scope = _provider.CreateScope();
        store = (IOpenIddictScopeStore<TScope>)scope.ServiceProvider.GetRequiredService(type);
        return store;
    }
}
