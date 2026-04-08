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

        /// <summary>
        /// 获取实体类型
        /// </summary>
        public Type Type => typeof(OidcApplication);

        /// <summary>
        /// 计算并发令牌
        /// </summary>
        public string? GetConcurrencyToken(object application)
        {
            if (application is OidcApplication app)
                return app.ConcurrencyToken;
            return null;
        }

        /// <summary>
        /// 设置并发令牌
        /// </summary>
        public ValueTask SetConcurrencyTokenAsync(object application, string? token, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.ConcurrencyToken = token;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取应用 ID
        /// </summary>
        public string? GetId(object application)
        {
            return application is OidcApplication app ? app.Id : null;
        }

        /// <summary>
        /// 获取客户端 ID
        /// </summary>
        public ValueTask<string?> GetClientIdAsync(object application, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string?>(application is OidcApplication app ? app.ClientId : null);
        }

        /// <summary>
        /// 设置客户端 ID
        /// </summary>
        public ValueTask SetClientIdAsync(object application, string? clientId, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.ClientId = clientId;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取客户端密钥
        /// </summary>
        public ValueTask<string?> GetClientSecretAsync(object application, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string?>(application is OidcApplication app ? app.ClientSecret : null);
        }

        /// <summary>
        /// 设置客户端密钥
        /// </summary>
        public ValueTask SetClientSecretAsync(object application, string? secret, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.ClientSecret = secret;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取客户端类型
        /// </summary>
        public ValueTask<string?> GetClientTypeAsync(object application, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string?>(application is OidcApplication app ? app.ClientType : null);
        }

        /// <summary>
        /// 设置客户端类型
        /// </summary>
        public ValueTask SetClientTypeAsync(object application, string? type, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.ClientType = type;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取同意类型
        /// </summary>
        public ValueTask<string?> GetConsentTypeAsync(object application, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string?>(application is OidcApplication app ? app.ConsentType : null);
        }

        /// <summary>
        /// 设置同意类型
        /// </summary>
        public ValueTask SetConsentTypeAsync(object application, string? type, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.ConsentType = type;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取显示名称
        /// </summary>
        public ValueTask<string?> GetDisplayNameAsync(object application, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string?>(application is OidcApplication app ? app.DisplayName : null);
        }

        /// <summary>
        /// 设置显示名称
        /// </summary>
        public ValueTask SetDisplayNameAsync(object application, string? name, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.DisplayName = name;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 获取显示名称（多语言）
        /// </summary>
        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.DisplayNames))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(app.DisplayNames);
                    if (dict != null)
                    {
                        var result = dict.ToDictionary(
                            k => CultureInfo.GetCultureInfo(k.Key),
                            v => v.Value);
                        return new ValueTask<ImmutableDictionary<CultureInfo, string>>(result.ToImmutableDictionary());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "反序列化 DisplayNames 失败");
                }
            }
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        /// <summary>
        /// 设置显示名称（多语言）
        /// </summary>
        public ValueTask SetDisplayNamesAsync(object application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
            {
                var dict = names.ToDictionary(k => k.Key.Name, v => v.Value);
                app.DisplayNames = JsonSerializer.Serialize(dict);
            }
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 根据客户端 ID 查找应用
        /// </summary>
        public async ValueTask<object?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            var app = await _db.Queryable<OidcApplication>()
                .Where(a => a.ClientId == clientId)
                .FirstAsync();
            return app;
        }

        /// <summary>
        /// 根据 ID 查找应用
        /// </summary>
        public async ValueTask<object?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var app = await _db.Queryable<OidcApplication>()
                .Where(a => a.Id == id)
                .FirstAsync();
            return app;
        }

        /// <summary>
        /// 创建应用
        /// </summary>
        public async ValueTask CreateAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
            {
                app.Id = Guid.NewGuid().ToString("N");
                await _db.Insertable(app).ExecuteCommandAsync();
                _logger.LogInformation("创建 OIDC 应用: {ClientId}", app.ClientId);
            }
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        public async ValueTask DeleteAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
            {
                await _db.Deleteable<OidcApplication>().Where(a => a.Id == app.Id).ExecuteCommandAsync();
                _logger.LogInformation("删除 OIDC 应用: {ClientId}", app.ClientId);
            }
        }

        /// <summary>
        /// 更新应用
        /// </summary>
        public async ValueTask UpdateAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
            {
                await _db.Updateable(app).ExecuteCommandAsync();
            }
        }

        // 其他方法的默认实现（返回空或抛出 NotImplementedException）
        public ValueTask<string?> GetApplicationTypeAsync(object application, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(application is OidcApplication app ? app.ApplicationType : null);

        public ValueTask SetApplicationTypeAsync(object application, string? type, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app) app.ApplicationType = type;
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetPermissionsAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.Permissions))
            {
                try
                {
                    var permissions = JsonSerializer.Deserialize<List<string>>(app.Permissions);
                    return new ValueTask<ImmutableArray<string>>(permissions?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask SetPermissionsAsync(object application, ImmutableArray<string> permissions, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.Permissions = JsonSerializer.Serialize(permissions.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.PostLogoutRedirectUris))
            {
                try
                {
                    var uris = JsonSerializer.Deserialize<List<string>>(app.PostLogoutRedirectUris);
                    return new ValueTask<ImmutableArray<string>>(uris?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask SetPostLogoutRedirectUrisAsync(object application, ImmutableArray<string> uris, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.PostLogoutRedirectUris = JsonSerializer.Serialize(uris.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.RedirectUris))
            {
                try
                {
                    var uris = JsonSerializer.Deserialize<List<string>>(app.RedirectUris);
                    return new ValueTask<ImmutableArray<string>>(uris?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask SetRedirectUrisAsync(object application, ImmutableArray<string> uris, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.RedirectUris = JsonSerializer.Serialize(uris.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(app.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask SetPropertiesAsync(object application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetRequirementsAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.Requirements))
            {
                try
                {
                    var reqs = JsonSerializer.Deserialize<List<string>>(app.Requirements);
                    return new ValueTask<ImmutableArray<string>>(reqs?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask SetRequirementsAsync(object application, ImmutableArray<string> requirements, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.Requirements = JsonSerializer.Serialize(requirements.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask<JsonElement?> GetJsonWebKeySetAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.JsonWebKeySet))
            {
                try
                {
                    var jwks = JsonSerializer.Deserialize<JsonElement>(app.JsonWebKeySet);
                    return new ValueTask<JsonElement?>(jwks);
                }
                catch { }
            }
            return new ValueTask<JsonElement?>(null);
        }

        public ValueTask SetJsonWebKeySetAsync(object application, JsonElement? element, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.JsonWebKeySet = element.HasValue ? JsonSerializer.Serialize(element.Value) : null;
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetSettingsAsync(object application, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app && !string.IsNullOrEmpty(app.Settings))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(app.Settings);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask SetSettingsAsync(object application, ImmutableDictionary<string, JsonElement> settings, CancellationToken cancellationToken = default)
        {
            if (application is OidcApplication app)
                app.Settings = JsonSerializer.Serialize(settings.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
        {
            var count = _db.Queryable<OidcApplication>().Count();
            return new ValueTask<long>(count);
        }

        public ValueTask<long> CountByAsync(string parameter, CancellationToken cancellationToken = default)
        {
            var count = _db.Queryable<OidcApplication>()
                .Where(a => a.ClientId == parameter || a.DisplayName == parameter)
                .Count();
            return new ValueTask<long>(count);
        }

        public IAsyncEnumerable<object> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcApplication>();
            if (offset.HasValue)
                query = query.Skip(offset.Value);
            if (count.HasValue)
                query = query.Take(count.Value);
            
            var apps = query.ToList();
            return apps.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<object> ListByAsync(string parameter, int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcApplication>()
                .Where(a => a.ClientId == parameter || a.DisplayName == parameter);
            
            if (offset.HasValue)
                query = query.Skip(offset.Value);
            if (count.HasValue)
                query = query.Take(count.Value);
            
            var apps = query.ToList();
            return apps.ToAsyncEnumerable();
        }
    }
}
