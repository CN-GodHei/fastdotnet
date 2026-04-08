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
    /// 基于 SqlSugar 的 OpenIddict 授权存储实现
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

        public Type Type => typeof(OidcAuthorization);

        public string? GetConcurrencyToken(OidcAuthorization authorization)
            => authorization.ConcurrencyToken;

        public ValueTask SetConcurrencyTokenAsync(OidcAuthorization authorization, string? token, CancellationToken cancellationToken = default)
        {
            authorization.ConcurrencyToken = token;
            return ValueTask.CompletedTask;
        }

        public string? GetId(OidcAuthorization authorization)
            => authorization.Id;

        public ValueTask<string?> GetApplicationIdAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(authorization.ApplicationId);

        public ValueTask SetApplicationIdAsync(OidcAuthorization authorization, string? identifier, CancellationToken cancellationToken = default)
        {
            authorization.ApplicationId = identifier;
            return ValueTask.CompletedTask;
        }

        public ValueTask<DateTime?> GetCreationDateAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
            => new ValueTask<DateTime?>(authorization.CreationDate);

        public ValueTask SetCreationDateAsync(OidcAuthorization authorization, DateTime? date, CancellationToken cancellationToken = default)
        {
            authorization.CreationDate = date;
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
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

        public ValueTask SetPropertiesAsync(OidcAuthorization authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken = default)
        {
            authorization.Properties = JsonSerializer.Serialize(properties.ToDictionary(k => k.Key, v => v.Value));
            return ValueTask.CompletedTask;
        }

        public ValueTask<ImmutableArray<string>> GetScopesAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
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

        public ValueTask SetScopesAsync(OidcAuthorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken = default)
        {
            authorization.Scopes = JsonSerializer.Serialize(scopes.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetStatusAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(authorization.Status);

        public ValueTask SetStatusAsync(OidcAuthorization authorization, string? status, CancellationToken cancellationToken = default)
        {
            authorization.Status = status;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetSubjectAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(authorization.Subject);

        public ValueTask SetSubjectAsync(OidcAuthorization authorization, string? subject, CancellationToken cancellationToken = default)
        {
            authorization.Subject = subject;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetTypeAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
            => new ValueTask<string?>(authorization.Type);

        public ValueTask SetTypeAsync(OidcAuthorization authorization, string? type, CancellationToken cancellationToken = default)
        {
            authorization.Type = type;
            return ValueTask.CompletedTask;
        }

        public async ValueTask<OidcAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<OidcAuthorization>()
                .Where(a => a.Id == identifier)
                .FirstAsync();
        }

        public async ValueTask<OidcAuthorization?> FindBySubjectAsync(string subject, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<OidcAuthorization>()
                .Where(a => a.Subject == subject)
                .FirstAsync();
        }

        public async ValueTask<ImmutableArray<OidcAuthorization>> FindAsync(string subject, string client, CancellationToken cancellationToken = default)
        {
            var list = await _db.Queryable<OidcAuthorization>()
                .Where(a => a.Subject == subject && a.ApplicationId == client)
                .ToListAsync();
            return list.ToImmutableArray();
        }

        public async ValueTask CreateAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
        {
            authorization.Id = Guid.NewGuid().ToString("N");
            await _db.Insertable(authorization).ExecuteCommandAsync();
            _logger.LogDebug("创建 OIDC 授权记录: {Id}", authorization.Id);
        }

        public async ValueTask DeleteAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
        {
            await _db.Deleteable<OidcAuthorization>().Where(a => a.Id == authorization.Id).ExecuteCommandAsync();
            _logger.LogDebug("删除 OIDC 授权记录: {Id}", authorization.Id);
        }

        public async ValueTask UpdateAsync(OidcAuthorization authorization, CancellationToken cancellationToken = default)
        {
            await _db.Updateable(authorization).ExecuteCommandAsync();
        }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
            => new ValueTask<long>(_db.Queryable<OidcAuthorization>().Count());

        public IAsyncEnumerable<OidcAuthorization> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcAuthorization>();
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OidcAuthorization> ListBySubjectAsync(string subject, int? count, int? offset, CancellationToken cancellationToken = default)
        {
            var query = _db.Queryable<OidcAuthorization>().Where(a => a.Subject == subject);
            if (offset.HasValue) query = query.Skip(offset.Value);
            if (count.HasValue) query = query.Take(count.Value);
            return query.ToList().ToAsyncEnumerable();
        }
    }
}
