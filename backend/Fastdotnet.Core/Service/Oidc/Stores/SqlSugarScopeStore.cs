using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;  // 明确使用 System.Text.Json
using System.Threading;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.Oidc;

namespace Fastdotnet.Core.Service.Oidc.Stores
{
    /// <summary>
    /// 基于 SqlSugar �?OpenIddict 作用域存储实�?
    /// </summary>
    public class SqlSugarScopeStore : IOpenIddictScopeStore<OidcScope>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<SqlSugarScopeStore> _logger;

        public SqlSugarScopeStore(
            ISqlSugarClient db,
            ILogger<SqlSugarScopeStore> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => new(_db.Queryable<OidcScope>().Count());

        public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OidcScope>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcScope>());
            return new ValueTask<long>(result.Count());
        }

        public async ValueTask CreateAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            scope.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(scope).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 作用�? {Id}, Name: {Name}", scope.Id, scope.Name);
        }

        public async ValueTask DeleteAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            await _db.Deleteable<OidcScope>().Where(s => s.Id == scope.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 作用�? {Id}", scope.Id);
        }

        public async ValueTask<OidcScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcScope>()
                .Where(s => s.Id == identifier)
                .FirstAsync();
        }

        public async ValueTask<OidcScope?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcScope>()
                .Where(s => s.Name == name)
                .FirstAsync();
        }

        public IAsyncEnumerable<OidcScope> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken)
        {
            var nameList = names.ToArray();
            var list = _db.Queryable<OidcScope>()
                .Where(s => nameList.Contains(s.Name!))
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcScope> FindByResourceAsync(string resource, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcScope>()
                .Where(s => s.Resources != null && s.Resources.Contains(resource))
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public ValueTask<TResult?> GetAsync<TState, TResult>(Func<IQueryable<OidcScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcScope>(), state).FirstOrDefault();
            return new ValueTask<TResult?>(result);
        }

        public ValueTask<string?> GetDescriptionAsync(OidcScope scope, CancellationToken cancellationToken)
            => new(scope.Description);

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scope.Descriptions))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(scope.Descriptions);
                    var result = dict?.ToDictionary(
                        kvp => CultureInfo.GetCultureInfo(kvp.Key),
                        kvp => kvp.Value
                    ) ?? new Dictionary<CultureInfo, string>();
                    return new ValueTask<ImmutableDictionary<CultureInfo, string>>(result.ToImmutableDictionary());
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        public ValueTask<string?> GetDisplayNameAsync(OidcScope scope, CancellationToken cancellationToken)
            => new(scope.DisplayName);

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scope.DisplayNames))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(scope.DisplayNames);
                    var result = dict?.ToDictionary(
                        kvp => CultureInfo.GetCultureInfo(kvp.Key),
                        kvp => kvp.Value
                    ) ?? new Dictionary<CultureInfo, string>();
                    return new ValueTask<ImmutableDictionary<CultureInfo, string>>(result.ToImmutableDictionary());
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        public ValueTask<string?> GetIdAsync(OidcScope scope, CancellationToken cancellationToken)
            => new(scope.Id);

        public ValueTask<string?> GetNameAsync(OidcScope scope, CancellationToken cancellationToken)
            => new(scope.Name);

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scope.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(scope.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask<ImmutableArray<string>> GetResourcesAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scope.Resources))
            {
                try
                {
                    var resources = JsonSerializer.Deserialize<List<string>>(scope.Resources);
                    return new ValueTask<ImmutableArray<string>>(resources?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<OidcScope> InstantiateAsync(CancellationToken cancellationToken)
            => new(new OidcScope());

        public IAsyncEnumerable<OidcScope> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcScope>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OidcScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcScope>(), state).ToList();
            return result.ToAsyncEnumerable();
        }

        public ValueTask SetDescriptionAsync(OidcScope scope, string? description, CancellationToken cancellationToken)
        {
            scope.Description = description;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetDescriptionsAsync(OidcScope scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
        {
            var dict = descriptions.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
            scope.Descriptions = JsonSerializer.Serialize(dict);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetDisplayNameAsync(OidcScope scope, string? name, CancellationToken cancellationToken)
        {
            scope.DisplayName = name;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetDisplayNamesAsync(OidcScope scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            var dict = names.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
            scope.DisplayNames = JsonSerializer.Serialize(dict);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetNameAsync(OidcScope scope, string? name, CancellationToken cancellationToken)
        {
            scope.Name = name;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPropertiesAsync(OidcScope scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            scope.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask SetResourcesAsync(OidcScope scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
        {
            scope.Resources = JsonSerializer.Serialize(resources.ToArray());
            return ValueTask.CompletedTask;
        }

        public async ValueTask UpdateAsync(OidcScope scope, CancellationToken cancellationToken)
        {
            await _db.Updateable(scope).ExecuteCommandAsync();
            _logger.LogDebug("更新 OIDC 作用�? {Id}", scope.Id);
        }
    }
}
