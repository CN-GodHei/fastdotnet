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
    /// 基于 SqlSugar 的 OpenIddict 令牌存储实现
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

        public Type Type => typeof(OidcToken);

        public string? GetConcurrencyToken(OidcToken token) => token.ConcurrencyToken;
        public ValueTask SetConcurrencyTokenAsync(OidcToken token, string? tokenValue, CancellationToken cancellationToken = default)
        {
            token.ConcurrencyToken = tokenValue;
            return ValueTask.CompletedTask;
        }

        public string? GetId(OidcToken token) => token.Id;
        public ValueTask<string?> GetApplicationIdAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.ApplicationId);
        public ValueTask SetApplicationIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken = default)
        {
            token.ApplicationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetAuthorizationIdAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.AuthorizationId);
        public ValueTask SetAuthorizationIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken = default)
        {
            token.AuthorizationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask<DateTime?> GetCreationDateAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.CreationDate);
        public ValueTask SetCreationDateAsync(OidcToken token, DateTime? date, CancellationToken cancellationToken = default)
        {
            token.CreationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask<DateTime?> GetExpirationDateAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.ExpirationDate);
        public ValueTask SetExpirationDateAsync(OidcToken token, DateTime? date, CancellationToken cancellationToken = default)
        {
            token.ExpirationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetPayloadAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.Payload);
        public ValueTask SetPayloadAsync(OidcToken token, string? payload, CancellationToken cancellationToken = default)
        {
            token.Payload = payload;
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcToken token, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(token.Properties))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(token.Properties);
                    return new(dict?.ToImmutableDictionary() ?? ImmutableDictionary<string, JsonElement>.Empty);
                }
                catch { }
            }
            return new(ImmutableDictionary<string, JsonElement>.Empty);
        }

        public ValueTask SetPropertiesAsync(OidcToken token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken = default)
        {
            token.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<DateTime?> GetRedemptionDateAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.RedemptionDate);
        public ValueTask SetRedemptionDateAsync(OidcToken token, DateTime? date, CancellationToken cancellationToken = default)
        {
            token.RedemptionDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetReferenceIdAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.ReferenceId);
        public ValueTask SetReferenceIdAsync(OidcToken token, string? identifier, CancellationToken cancellationToken = default)
        {
            token.ReferenceId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetStatusAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.Status);
        public ValueTask SetStatusAsync(OidcToken token, string? status, CancellationToken cancellationToken = default)
        {
            token.Status = status;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetSubjectAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.Subject);
        public ValueTask SetSubjectAsync(OidcToken token, string? subject, CancellationToken cancellationToken = default)
        {
            token.Subject = subject;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetTypeAsync(OidcToken token, CancellationToken cancellationToken = default) => new(token.Type);
        public ValueTask SetTypeAsync(OidcToken token, string? type, CancellationToken cancellationToken = default)
        {
            token.Type = type;
            return ValueTask.CompletedTask;
        }

        public async ValueTask<OidcToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken = default)
            => await _db.Queryable<OidcToken>().Where(t => t.Id == identifier).FirstAsync();

        public async ValueTask<OidcToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken = default)
            => await _db.Queryable<OidcToken>().Where(t => t.ReferenceId == identifier).FirstAsync();

        public async ValueTask<ImmutableArray<OidcToken>> FindAsync(string subject, string client, CancellationToken cancellationToken = default)
        {
            var list = await _db.Queryable<OidcToken>()
                .Where(t => t.Subject == subject && t.ApplicationId == client)
                .ToListAsync();
            return list.ToImmutableArray();
        }

        public async ValueTask<ImmutableArray<OidcToken>> FindAsync(string subject, string client, string status, CancellationToken cancellationToken = default)
        {
            var list = await _db.Queryable<OidcToken>()
                .Where(t => t.Subject == subject && t.ApplicationId == client && t.Status == status)
                .ToListAsync();
            return list.ToImmutableArray();
        }

        public async ValueTask<ImmutableArray<OidcToken>> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken = default)
        {
            var list = await _db.Queryable<OidcToken>()
                .Where(t => t.Subject == subject && t.ApplicationId == client && t.Status == status && t.Type == type)
                .ToListAsync();
            return list.ToImmutableArray();
        }

        public async ValueTask CreateAsync(OidcToken token, CancellationToken cancellationToken = default)
        {
            token.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(token).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 令牌: {Id}", token.Id);
        }

        public async ValueTask DeleteAsync(OidcToken token, CancellationToken cancellationToken = default)
        {
            await _db.Deleteable<OidcToken>().Where(t => t.Id == token.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 令牌: {Id}", token.Id);
        }

        public async ValueTask UpdateAsync(OidcToken token, CancellationToken cancellationToken = default)
            => await _db.Updateable(token).ExecuteCommandAsync();

        public async ValueTask RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken = default)
        {
            await _db.Updateable<OidcToken>()
                .SetColumns(t => new OidcToken { Status = "revoked" })
                .Where(t => t.AuthorizationId == identifier)
                .ExecuteCommandAsync();
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
            => new(_db.Queryable<OidcToken>().Count());

        public IAsyncEnumerable<OidcToken> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcToken>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }
    }
}
