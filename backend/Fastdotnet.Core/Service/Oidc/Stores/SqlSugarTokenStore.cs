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
    /// 基于 SqlSugar �?OpenIddict 令牌存储实现
    /// </summary>
    public class SqlSugarTokenStore : IOpenIddictTokenStore<OidcToken>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<SqlSugarTokenStore> _logger;

        public SqlSugarTokenStore(ISqlSugarClient db, ILogger<SqlSugarTokenStore> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => new(_db.Queryable<OidcToken>().Count());

        public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OidcToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcToken>());
            return new ValueTask<long>(result.Count());
        }

        public async ValueTask CreateAsync(OidcToken token, CancellationToken cancellationToken)
        {
            token.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(token).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 令牌: {Id}", token.Id);
        }

        public async ValueTask DeleteAsync(OidcToken token, CancellationToken cancellationToken)
        {
            await _db.Deleteable<OidcToken>().Where(t => t.Id == token.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 令牌: {Id}", token.Id);
        }

        public IAsyncEnumerable<OidcToken> FindAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcToken>();
            if (!string.IsNullOrEmpty(subject)) query = query.Where(t => t.Subject == subject);
            if (!string.IsNullOrEmpty(client)) query = query.Where(t => t.ApplicationId == client);
            if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);
            if (!string.IsNullOrEmpty(type)) query = query.Where(t => t.Type == type);
            
            var list = query.ToList();
            return list.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcToken> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcToken>()
                .Where(t => t.ApplicationId == identifier)
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcToken> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcToken>()
                .Where(t => t.AuthorizationId == identifier)
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public async ValueTask<OidcToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcToken>()
                .Where(t => t.Id == identifier)
                .FirstAsync();
        }

        public async ValueTask<OidcToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
        {
            return await _db.Queryable<OidcToken>()
                .Where(t => t.ReferenceId == identifier)
                .FirstAsync();
        }

        public IAsyncEnumerable<OidcToken> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            var list = _db.Queryable<OidcToken>()
                .Where(t => t.Subject == subject)
                .ToList();
            return list.ToAsyncEnumerable();
        }

        public ValueTask<string?> GetApplicationIdAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.ApplicationId);

        public ValueTask<string?> GetAuthorizationIdAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.AuthorizationId);

        public ValueTask<DateTimeOffset?> GetCreationDateAsync(OidcToken token, CancellationToken cancellationToken)
            => token.CreationDate.HasValue ? new ValueTask<DateTimeOffset?>(token.CreationDate.Value) : new ValueTask<DateTimeOffset?>(null);

        public ValueTask<DateTimeOffset?> GetExpirationDateAsync(OidcToken token, CancellationToken cancellationToken)
            => token.ExpirationDate.HasValue ? new ValueTask<DateTimeOffset?>(token.ExpirationDate.Value) : new ValueTask<DateTimeOffset?>(null);

        public ValueTask<string?> GetIdAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.Id);

        public ValueTask<string?> GetPayloadAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.Payload);

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcToken token, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(token.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(token.Properties);
                    return new ValueTask<ImmutableDictionary<string, JsonElement>>(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OidcToken token, CancellationToken cancellationToken)
            => token.RedemptionDate.HasValue ? new ValueTask<DateTimeOffset?>(token.RedemptionDate.Value) : new ValueTask<DateTimeOffset?>(null);

        public ValueTask<string?> GetReferenceIdAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.ReferenceId);

        public ValueTask<string?> GetStatusAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.Status);

        public ValueTask<string?> GetSubjectAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.Subject);

        public ValueTask<string?> GetTypeAsync(OidcToken token, CancellationToken cancellationToken)
            => new(token.Type);

        public ValueTask<OidcToken> InstantiateAsync(CancellationToken cancellationToken)
            => new(new OidcToken());

        public IAsyncEnumerable<OidcToken> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<OidcToken>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OidcToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcToken>(), state).ToList();
            return result.ToAsyncEnumerable();
        }

        public ValueTask<TResult?> GetAsync<TState, TResult>(Func<IQueryable<OidcToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            var result = query(_db.Queryable<OidcToken>(), state).FirstOrDefault();
            return new ValueTask<TResult?>(result);
        }

        public async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            var count = await _db.Deleteable<OidcToken>()
                .Where(t => t.CreationDate.HasValue && t.CreationDate.Value < threshold)
                .ExecuteCommandAsync();
            _logger.LogDebug("清理 OIDC 令牌: {Count} 条记�?, count);
            return count;
        }

        public ValueTask SetApplicationIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken)
        {
            token.ApplicationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetAuthorizationIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken)
        {
            token.AuthorizationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetCreationDateAsync(OidcToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            token.CreationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetExpirationDateAsync(OidcToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            token.ExpirationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPayloadAsync(OidcToken token, string? payload, CancellationToken cancellationToken)
        {
            token.Payload = payload;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPropertiesAsync(OidcToken token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            token.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask SetRedemptionDateAsync(OidcToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            token.RedemptionDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetReferenceIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken)
        {
            token.ReferenceId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetStatusAsync(OidcToken token, string? status, CancellationToken cancellationToken)
        {
            token.Status = status;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetSubjectAsync(OidcToken token, string? subject, CancellationToken cancellationToken)
        {
            token.Subject = subject;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetTypeAsync(OidcToken token, string? type, CancellationToken cancellationToken)
        {
            token.Type = type;
            return ValueTask.CompletedTask;
        }

        public async ValueTask UpdateAsync(OidcToken token, CancellationToken cancellationToken)
        {
            await _db.Updateable(token).ExecuteCommandAsync();
            _logger.LogDebug("更新 OIDC 令牌: {Id}", token.Id);
        }

        public async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
        {
            var query = _db.Updateable<OidcToken>().SetColumns(t => new OidcToken { Status = "revoked" });
            
            if (!string.IsNullOrEmpty(subject)) query = query.Where(t => t.Subject == subject);
            if (!string.IsNullOrEmpty(client)) query = query.Where(t => t.ApplicationId == client);
            if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);
            if (!string.IsNullOrEmpty(type)) query = query.Where(t => t.Type == type);
            
            var count = await query.ExecuteCommandAsync();
            _logger.LogDebug("撤销 OIDC 令牌: {Count} �?, count);
            return count;
        }

        public async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var count = await _db.Updateable<OidcToken>()
                .SetColumns(t => new OidcToken { Status = "revoked" })
                .Where(t => t.ApplicationId == identifier)
                .ExecuteCommandAsync();
            _logger.LogDebug("按应�?ID 撤销令牌: {Identifier}, {Count} �?, identifier, count);
            return count;
        }

        public async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var count = await _db.Updateable<OidcToken>()
                .SetColumns(t => new OidcToken { Status = "revoked" })
                .Where(t => t.AuthorizationId == identifier)
                .ExecuteCommandAsync();
            _logger.LogDebug("按授�?ID 撤销令牌: {Identifier}, {Count} �?, identifier, count);
            return count;
        }

        public async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            var count = await _db.Updateable<OidcToken>()
                .SetColumns(t => new OidcToken { Status = "revoked" })
                .Where(t => t.Subject == subject)
                .ExecuteCommandAsync();
            _logger.LogDebug("按主题撤销令牌: {Subject}, {Count} �?, subject, count);
            return count;
        }
    }
}
