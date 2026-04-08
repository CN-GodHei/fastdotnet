using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.Oidc;

namespace Fastdotnet.Core.Service.Oidc.Stores
{
    /// <summary>
    /// 基于 SqlSugar �?OpenIddict 授权存储实现
    /// </summary>
    public class SqlSugarAuthorizationStore : IOpenIddictAuthorizationStore<OidcAuthorization>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<SqlSugarAuthorizationStore> _logger;

        public SqlSugarAuthorizationStore(ISqlSugarClient db, ILogger<SqlSugarAuthorizationStore> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => new(_db.Queryable<OidcAuthorization>().Count());

        public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OidcAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcAuthorization>());
            return new ValueTask<long>(result.Count());
        }

        public async ValueTask CreateAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
        {
            authorization.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(authorization).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 授权记录: {Id}", authorization.Id);
        }

        public async ValueTask DeleteAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
        {
            await _db.Deleteable<OidcAuthorization>().Where(a => a.Id == authorization.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 授权记录: {Id}", authorization.Id);
        }

        public IAsyncEnumerable<OidcAuthorization> FindAsync(string? subject, string? client, string? status, string? type, ImmutableArray<string>? scopes, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcAuthorization>();
            if (!string.IsNullOrEmpty(subject)) query = query.Where(a => a.Subject == subject);
            if (!string.IsNullOrEmpty(client)) query = query.Where(a => a.ApplicationId == client);
            if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(type)) query = query.Where(a => a.Type == type);
            
            var list = query.ToList();
            
            // 过滤 scopes
            if (scopes.HasValue && scopes.Value.Length > 0)
            {
                var requiredScopes = scopes.Value.ToArray();
                list = list.Where(a =>
                {
                    if (string.IsNullOrEmpty(a.Scopes)) return false;
                    try
                    {
                        var authScopes = JsonSerializer.Deserialize<List<string>>(a.Scopes);
                        return authScopes != null && requiredScopes.All(rs => authScopes.Contains(rs));
                    }
                    catch { return false; }
                }).ToList();
            }
            
            return list.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcAuthorization> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcAuthorization>()
                .Where(a => a.ApplicationId == identifier)
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public async ValueTask<OidcAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcAuthorization>()
                .Where(a => a.Id == identifier)
                .FirstAsync();
        }

        public IAsyncEnumerable<OidcAuthorization> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcAuthorization>()
                .Where(a => a.Subject == subject)
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public ValueTask<string?> GetApplicationIdAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => new(authorization.ApplicationId);

        public ValueTask<TResult?> GetAsync<TState, TResult>(Func<IQueryable<OidcAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcAuthorization>(), state).FirstOrDefault();
            return new ValueTask<TResult?>(result);
        }

        public ValueTask<DateTimeOffset?> GetCreationDateAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => authorization.CreationDate.HasValue ? new ValueTask<DateTimeOffset?>(authorization.CreationDate.Value) : new ValueTask<DateTimeOffset?>(null);

        public ValueTask<string?> GetIdAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => new(authorization.Id);

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(authorization.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(authorization.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask<ImmutableArray<string>> GetScopesAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(authorization.Scopes))
            {
                try
                {
                    var scopes = JsonSerializer.Deserialize<List<string>>(authorization.Scopes);
                    return new ValueTask<ImmutableArray<string>>(scopes?.ToImmutableArray() ?? ImmutableArray<string>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableArray<string>>(ImmutableArray<string>.Empty);
        }

        public ValueTask<string?> GetStatusAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => new(authorization.Status);

        public ValueTask<string?> GetSubjectAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => new(authorization.Subject);

        public ValueTask<string?> GetTypeAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
            => new(authorization.Type);

        public ValueTask<OidcAuthorization> InstantiateAsync(CancellationToken cancellationToken)
            => new(new OidcAuthorization());

        public IAsyncEnumerable<OidcAuthorization> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcAuthorization>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OidcAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcAuthorization>(), state).ToList();
            return result.ToAsyncEnumerable();
        }

        public ValueTask SetApplicationIdAsync(OidcAuthorization authorization, string? identifier, CancellationToken cancellationToken)
        {
            authorization.ApplicationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetCreationDateAsync(OidcAuthorization authorization, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            authorization.CreationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPropertiesAsync(OidcAuthorization authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            authorization.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask SetScopesAsync(OidcAuthorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
        {
            authorization.Scopes = JsonSerializer.Serialize(scopes.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask SetStatusAsync(OidcAuthorization authorization, string? status, CancellationToken cancellationToken)
        {
            authorization.Status = status;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetSubjectAsync(OidcAuthorization authorization, string? subject, CancellationToken cancellationToken)
        {
            authorization.Subject = subject;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetTypeAsync(OidcAuthorization authorization, string? type, CancellationToken cancellationToken)
        {
            authorization.Type = type;
            return ValueTask.CompletedTask;
        }

        public async ValueTask UpdateAsync(OidcAuthorization authorization, CancellationToken cancellationToken)
        {
            await _db.Updateable(authorization).ExecuteCommandAsync();
            _logger.LogDebug("更新 OIDC 授权记录: {Id}", authorization.Id);
        }

        public async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            var count = await _db.Deleteable<OidcAuthorization>()
                .Where(a => a.CreationDate.HasValue && a.CreationDate.Value < threshold)
                .ExecuteCommandAsync();
            _logger.LogDebug("清理 OIDC 授权记录: {Count} �?, count);
            return count;
        }

        public async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
        {
            var query = _db.Updateable<OidcAuthorization>().SetColumns(a => new OidcAuthorization { Status = "revoked" });
            
            if (!string.IsNullOrEmpty(subject)) query = query.Where(a => a.Subject == subject);
            if (!string.IsNullOrEmpty(client)) query = query.Where(a => a.ApplicationId == client);
            if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(type)) query = query.Where(a => a.Type == type);
            
            var count = await query.ExecuteCommandAsync();
            _logger.LogDebug("撤销 OIDC 授权记录: {Count} �?, count);
            return count;
        }

        public async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var count = await _db.Updateable<OidcAuthorization>()
                .SetColumns(a => new OidcAuthorization { Status = "revoked" })
                .Where(a => a.ApplicationId == identifier)
                .ExecuteCommandAsync();
            _logger.LogDebug("按应�?ID 撤销授权: {Identifier}, {Count} �?, identifier, count);
            return count;
        }

        public async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            var count = await _db.Updateable<OidcAuthorization>()
                .SetColumns(a => new OidcAuthorization { Status = "revoked" })
                .Where(a => a.Subject == subject)
                .ExecuteCommandAsync();
            _logger.LogDebug("按主题撤销授权: {Subject}, {Count} �?, subject, count);
            return count;
        }
    }
}
