using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.Oidc;

namespace Fastdotnet.Core.Service.Oidc.Stores
{
    /// <summary>
    /// 基于 SqlSugar 的 OpenIddict 作用域存储实现
    /// </summary>
    public class SqlSugarScopeStore : IOpenIddictScopeStore<OidcScope>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<SqlSugarScopeStore> _logger;

        public SqlSugarScopeStore(ISqlSugarClient db, ILogger<SqlSugarScopeStore> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Type Type => typeof(OidcScope);

        public string? GetConcurrencyToken(object scope)
            => scope is OidcScope s ? s.ConcurrencyToken : null;

        public ValueTask SetConcurrencyTokenAsync(object scope, string? token, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s) s.ConcurrencyToken = token;
            return ValueTask.CompletedTask;
        }

        public string? GetId(object scope)
            => scope is OidcScope s ? s.Id : null;

        public ValueTask<string?> GetNameAsync(object scope, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(scope is OidcScope s ? s.Name : null);

        public ValueTask SetNameAsync(object scope, string? name, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s) s.Name = name;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetDescriptionAsync(object scope, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(scope is OidcScope s ? s.Description : null);

        public ValueTask SetDescriptionAsync(object scope, string? description, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s) s.Description = description;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetDisplayNameAsync(object scope, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(scope is OidcScope s ? s.DisplayName : null);

        public ValueTask SetDisplayNameAsync(object scope, string? name, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s) s.DisplayName = name;
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s && !string.IsNullOrEmpty(s.Descriptions))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(s.Descriptions);
                    if (dict != null)
                        return new ValueTask<ImmutableDictionary<CultureInfo, string>>(dict.ToDictionary(k => CultureInfo.GetCultureInfo(k.Key), v => v.Value).ToImmutableDictionary());
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        public ValueTask SetDescriptionsAsync(object scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
                s.Descriptions = JsonSerializer.Serialize(descriptions.ToDictionary(k => k.Key.Name, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s && !string.IsNullOrEmpty(s.DisplayNames))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(s.DisplayNames);
                    if (dict != null)
                        return new ValueTask<ImmutableDictionary<CultureInfo, string>>(dict.ToDictionary(k => CultureInfo.GetCultureInfo(k.Key), v => v.Value).ToImmutableDictionary());
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        public ValueTask SetDisplayNamesAsync(object scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
                s.DisplayNames = JsonSerializer.Serialize(names.ToDictionary(k => k.Key.Name, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s && !string.IsNullOrEmpty(s.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(s.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask SetPropertiesAsync(object scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
                s.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetResourcesAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s && !string.IsNullOrEmpty(s.Resources))
            {
                try
                {
                    var resources = JsonSerializer.Deserialize<List<string>>(s.Resources);
                    return new ValueTask<ImmutableArray<string>>(resources?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask SetResourcesAsync(object scope, ImmutableArray<string> resources, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
                s.Resources = JsonSerializer.Serialize(resources.ToArray());
            return ValueTask.CompletedTask;
        }

        public async ValueTask<object?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var scope = await _db.Queryable<OidcScope>()
                .Where(s => s.Name == name)
                .FirstAsync();
            return scope;
        }

        public async ValueTask<object?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var scope = await _db.Queryable<OidcScope>()
                .Where(s => s.Id == id)
                .FirstAsync();
            return scope;
        }

        public async ValueTask CreateAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
            {
                s.Id = Guid.NewGuid().ToString("N");
                await _db.Insertable(s).ExecuteCommandAsync();
                _logger.LogInformation("创建 OIDC 作用域: {Name}", s.Name);
            }
        }

        public async ValueTask DeleteAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
            {
                await _db.Deleteable<OidcScope>().Where(x => x.Id == s.Id).ExecuteCommandAsync();
                _logger.LogInformation("删除 OIDC 作用域: {Name}", s.Name);
            }
        }

        public async ValueTask UpdateAsync(object scope, CancellationToken cancellationToken = default)
        {
            if (scope is OidcScope s)
                await _db.Updateable(s).ExecuteCommandAsync();
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
            => new ValueTask<long>(_db.Queryable<OidcScope>().Count());

        public IAsyncEnumerable<object> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcScope>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }
    }
}
