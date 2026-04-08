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
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Fastdotnet.Core.Service.Oidc.Stores
{
    /// <summary>
    /// 基于 SqlSugar 的 OpenIddict 应用存储实现
    /// </summary>
    public class SqlSugarApplicationStore : IOpenIddictApplicationStore<OidcApplication>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<SqlSugarApplicationStore> _logger;

        public SqlSugarApplicationStore(
            ISqlSugarClient db,
            ILogger<SqlSugarApplicationStore> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => new(_db.Queryable<OidcApplication>().Count());

        public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OidcApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcApplication>());
            return new ValueTask<long>(result.Count());
        }

        public async ValueTask CreateAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            application.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(application).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 应用: {Id}, ClientId: {ClientId}", application.Id, application.ClientId);
        }

        public async ValueTask DeleteAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            await _db.Deleteable<OidcApplication>().Where(a => a.Id == application.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 应用: {Id}", application.Id);
        }

        public async ValueTask<OidcApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcApplication>()
                .Where(a => a.Id == identifier)
                .FirstAsync();
        }

        public async ValueTask<OidcApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcApplication>()
                .Where(a => a.ClientId == identifier)
                .FirstAsync();
        }

        public IAsyncEnumerable<OidcApplication> FindByPostLogoutRedirectUriAsync(string uri, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcApplication>()
                .Where(a => a.PostLogoutRedirectUris != null && a.PostLogoutRedirectUris.Contains(uri))
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcApplication> FindByRedirectUriAsync(string uri, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcApplication>()
                .Where(a => a.RedirectUris != null && a.RedirectUris.Contains(uri))
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public ValueTask<string?> GetApplicationTypeAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.ApplicationType);

        public ValueTask<TResult?> GetAsync<TState, TResult>(Func<IQueryable<OidcApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcApplication>(), state).FirstOrDefault();
            return new ValueTask<TResult?>(result);
        }

        public ValueTask<string?> GetClientIdAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.ClientId);

        public ValueTask<string?> GetClientSecretAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.ClientSecret);

        public ValueTask<string?> GetClientTypeAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.ClientType);

        public ValueTask<string?> GetConsentTypeAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.ConsentType);

        public ValueTask<string?> GetDisplayNameAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.DisplayName);

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.DisplayNames))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(application.DisplayNames);
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

        public ValueTask<string?> GetIdAsync(OidcApplication application, CancellationToken cancellationToken)
            => new(application.Id);

        public ValueTask<Microsoft.IdentityModel.Tokens.JsonWebKeySet?> GetJsonWebKeySetAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.Jwks))
            {
                try
                {
                    var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(application.Jwks);
                    return new ValueTask<Microsoft.IdentityModel.Tokens.JsonWebKeySet?>(jwks);
                }
                catch { }
            }
            return new ValueTask<Microsoft.IdentityModel.Tokens.JsonWebKeySet?>(null);
        }

        public ValueTask<ImmutableArray<string>> GetPermissionsAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.Permissions))
            {
                try
                {
                    var permissions = JsonSerializer.Deserialize<List<string>>(application.Permissions);
                    return new ValueTask<ImmutableArray<string>>(permissions?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.PostLogoutRedirectUris))
            {
                try
                {
                    var uris = JsonSerializer.Deserialize<List<string>>(application.PostLogoutRedirectUris);
                    return new ValueTask<ImmutableArray<string>>(uris?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(application.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.RedirectUris))
            {
                try
                {
                    var uris = JsonSerializer.Deserialize<List<string>>(application.RedirectUris);
                    return new ValueTask<ImmutableArray<string>>(uris?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<ImmutableArray<string>> GetRequirementsAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.Requirements))
            {
                try
                {
                    var requirements = JsonSerializer.Deserialize<List<string>>(application.Requirements);
                    return new ValueTask<ImmutableArray<string>>(requirements?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(application.Settings))
            {
                try
                {
                    var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(application.Settings);
                    return new ValueTask<ImmutableDictionary<string, string>>(settings?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, string>>(ImmutableDictionary<string, string>.Empty);
        }

        public ValueTask<OidcApplication> InstantiateAsync(CancellationToken cancellationToken)
            => new(new OidcApplication());

        public IAsyncEnumerable<OidcApplication> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcApplication>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OidcApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcApplication>(), state).ToList();
            return result.ToAsyncEnumerable();
        }

        public ValueTask SetApplicationTypeAsync(OidcApplication application, string? type, CancellationToken cancellationToken)
        {
            application.ApplicationType = type;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetClientIdAsync(OidcApplication application, string? identifier, CancellationToken cancellationToken)
        {
            application.ClientId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetClientSecretAsync(OidcApplication application, string? secret, CancellationToken cancellationToken)
        {
            application.ClientSecret = secret;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetClientTypeAsync(OidcApplication application, string? type, CancellationToken cancellationToken)
        {
            application.ClientType = type;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetConsentTypeAsync(OidcApplication application, string? type, CancellationToken cancellationToken)
        {
            application.ConsentType = type;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetDisplayNameAsync(OidcApplication application, string? name, CancellationToken cancellationToken)
        {
            application.DisplayName = name;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetDisplayNamesAsync(OidcApplication application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            var dict = names.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
            application.DisplayNames = JsonSerializer.Serialize(dict);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetJsonWebKeySetAsync(OidcApplication application, Microsoft.IdentityModel.Tokens.JsonWebKeySet? set, CancellationToken cancellationToken)
        {
            application.Jwks = set?.WriteToJson();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPermissionsAsync(OidcApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
        {
            application.Permissions = JsonSerializer.Serialize(permissions.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPostLogoutRedirectUrisAsync(OidcApplication application, ImmutableArray<string> uris, CancellationToken cancellationToken)
        {
            application.PostLogoutRedirectUris = JsonSerializer.Serialize(uris.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPropertiesAsync(OidcApplication application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            application.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask SetRedirectUrisAsync(OidcApplication application, ImmutableArray<string> uris, CancellationToken cancellationToken)
        {
            application.RedirectUris = JsonSerializer.Serialize(uris.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask SetRequirementsAsync(OidcApplication application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
        {
            application.Requirements = JsonSerializer.Serialize(requirements.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask SetSettingsAsync(OidcApplication application, ImmutableDictionary<string, string> settings, CancellationToken cancellationToken)
        {
            application.Settings = JsonSerializer.Serialize(settings.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public async ValueTask UpdateAsync(OidcApplication application, CancellationToken cancellationToken)
        {
            await _db.Updateable(application).ExecuteCommandAsync();
            _logger.LogDebug("更新 OIDC 应用: {Id}", application.Id);
        }
    }
}
