/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Fastdotnet.Core.Entities.Oidc;
using SqlSugar;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Fastdotnet.Core.Service.Oidc.Stores;

/// <summary>
/// Provides methods allowing to manage the authorizations stored in a database.
/// </summary>
/// <typeparam name="OpenIddictSqlSugarAuthorization">The type of the Authorization entity.</typeparam>
/// <typeparam name="TApplication">The type of the Application entity.</typeparam>
/// <typeparam name="TToken">The type of the Token entity.</typeparam>
/// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
/// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
public class OpenIddictSqlSugarAuthorizationStore : IOpenIddictAuthorizationStore<OpenIddictSqlSugarAuthorization>
{
    public OpenIddictSqlSugarAuthorizationStore(
        IMemoryCache cache,
        ISqlSugarClient context,
        IOptionsMonitor<OpenIddictSqlSugarOptions> options)
    {
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the memory cache associated with the current store.
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <summary>
    /// Gets the database context associated with the current store.
    /// </summary>
    protected ISqlSugarClient Context { get; }

    /// <summary>
    /// Gets the options associated with the current store.
    /// </summary>
    protected IOptionsMonitor<OpenIddictSqlSugarOptions> Options { get; }

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="TApplication"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarApplication> Applications => Context.Queryable<OpenIddictSqlSugarApplication>();

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="OpenIddictSqlSugarAuthorization"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarAuthorization> Authorizations => Context.Queryable<OpenIddictSqlSugarAuthorization>();

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="TToken"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarToken> Tokens => Context.Queryable<OpenIddictSqlSugarToken>();

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        => await Authorizations.CountAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictSqlSugarAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var collection = await Authorizations.ToListAsync(cancellationToken);


        return query(collection.AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        await Context.Insertable(authorization).ExecuteCommandAsync();

    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {

        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        try
        {
            await Context.Ado.BeginTranAsync();

            await Context.DeleteNav(authorization)
                .Include(x => x.Tokens)
                .ExecuteCommandAsync();

            await Context.Ado.CommitTranAsync();
        }
        catch (Exception)
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }

    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarAuthorization> FindAsync(
        string? subject, string? client,
        string? status, string? type,
        ImmutableArray<string>? scopes, [EnumeratorCancellation] CancellationToken cancellationToken)
    {

        var authorizations = await Authorizations.Includes(authorization => authorization.Application)
            .WhereIF(!string.IsNullOrEmpty(subject),x => x.Subject == subject)
            .WhereIF(!string.IsNullOrEmpty(client),x => x.Application!.ClientId == client)
            .WhereIF(!string.IsNullOrEmpty(status),x => x.Status == status)
            .WhereIF(!string.IsNullOrEmpty(type),x => x.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var authorization in authorizations)
        {
            if (scopes is null || (await GetScopesAsync(authorization, cancellationToken))
                .ToHashSet(StringComparer.Ordinal)
                .IsSupersetOf(scopes))
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarAuthorization> FindByApplicationIdAsync(
        string identifier,[EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(nameof(identifier));
        }

        var collection = await Authorizations.InnerJoin(Applications, (authorization, application) => authorization.Application!.Id == application.Id)
            .Where((x, y) => y.Id.ToString() == identifier)
            .ToListAsync(cancellationToken);

        foreach (var item in collection)
        {
            yield return item;
        }

    }

    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(nameof(identifier));
        }

        var authorization = await Authorizations.Where(x => x.Id.ToString() == identifier).FirstAsync(cancellationToken);
        return authorization;

    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarAuthorization> FindBySubjectAsync(
        string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException(nameof(subject));
        }

        var authorization = await Authorizations.Includes(x => x.Application)
            .Where(x => x.Subject == subject)
            .ToListAsync(cancellationToken);

        foreach (var item in authorization)
        {
            yield return item;
        }

    }

    /// <inheritdoc/>
    public virtual async ValueTask<string?> GetApplicationIdAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        var result = await Authorizations.Includes(x => x.Application)
        .Where(x => x.Id == authorization.Id)
        .FirstAsync();

        return result.Application!.Id.ToString();

    }

    /// <inheritdoc/>
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarAuthorization>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var collection = await Authorizations.Includes(authorization => authorization.Application).ToListAsync();

        var queryableAuthorizations = collection.AsQueryable();

        return query(queryableAuthorizations, state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (authorization.CreationDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(authorization.CreationDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (string.IsNullOrEmpty(authorization.Properties))
        {
            return new(ImmutableDictionary.Create<string, JsonElement>());
        }

        // Note: parsing the stringified properties is an expensive operation.
        // To mitigate that, the resulting object is stored in the memory cache.
        var key = string.Concat("68056e1a-dbcf-412b-9a6a-d791c7dbe726", "\x1e", authorization.Properties);
        var properties = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(authorization.Properties);
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return builder.ToImmutable();
        })!;

        return new(properties);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (string.IsNullOrEmpty(authorization.Scopes))
        {
            return new(ImmutableArray<string>.Empty);
        }

        // Note: parsing the stringified scopes is an expensive operation.
        // To mitigate that, the resulting array is stored in the memory cache.
        var key = string.Concat("2ba4ab0f-e2ec-4d48-b3bd-28e2bb660c75", "\x1e", authorization.Scopes);
        var scopes = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(authorization.Scopes);
            var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());

            foreach (var element in document.RootElement.EnumerateArray())
            {
                var value = element.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder.Add(value);
            }

            return builder.ToImmutable();
        });

        return new(scopes);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetStatusAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Status);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetSubjectAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Subject);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetTypeAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Type);
    }

    /// <inheritdoc/>
    public virtual ValueTask<OpenIddictSqlSugarAuthorization> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(new OpenIddictSqlSugarAuthorization());
        }

        catch (MemberAccessException exception)
        {
            return new(Task.FromException<OpenIddictSqlSugarAuthorization>(
                new InvalidOperationException("", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarAuthorization> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Authorizations.Includes(authorization => authorization.Application)
                          .OrderBy(authorization => authorization.Id!);

        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        var authorizations = await query.ToListAsync(cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarAuthorization>, TState, IQueryable<TResult>> query,
        TState state, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        // 使用 Includes 加载关联实体
        var authorizations = await Authorizations.Includes(authorization => authorization.Application)
                                                 .ToListAsync(cancellationToken);

        // 将结果转换为 IQueryable 并应用查询
        var resultQuery = query(authorizations.AsQueryable(), state);

        // 遍历结果并逐个返回
        foreach (var item in resultQuery)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        List<Exception>? exceptions = null;
        var result = 0L;

        // 将 DateTimeOffset 转换为 UTC DateTime，以兼容不支持 DateTimeOffset 的数据库
        var date = threshold.UtcDateTime;

        try
        {
            // 使用 SqlSugar 的批量删除操作，删除所有符合条件的记录
            var count = await Context.Deleteable<OpenIddictSqlSugarAuthorization>()
                .Where(authorization => authorization.CreationDate < date)
                .Where(authorization => authorization.Status != Statuses.Valid ||
                                       authorization.Type == AuthorizationTypes.AdHoc && !authorization.Tokens!.Any())
                .ExecuteCommandAsync(cancellationToken);

            result += count;
        }
        catch (Exception exception)
        {
            exceptions ??= new List<Exception>(capacity: 1);
            exceptions.Add(exception);
        }

        // 如果有异常发生，抛出聚合异常
        if (exceptions != null)
        {
            throw new AggregateException("在清理授权记录时发生了一个或多个错误。", exceptions);
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
    {

        var updateData = await Authorizations.Includes(x => x.Application)
                .WhereIF(!string.IsNullOrEmpty(subject), x => x.Subject == subject)
                .WhereIF(!string.IsNullOrEmpty(client), x => x.Application!.ClientId == client)
                .WhereIF(!string.IsNullOrEmpty(status), x => x.Status == status)
                .WhereIF(!string.IsNullOrEmpty(type), x => x.Type == type)
                .ToListAsync();

        foreach (var update in updateData)
        {
            update.Status = Statuses.Revoked;
        }

        return await Context.Updateable(updateData).ExecuteCommandAsync(cancellationToken);

    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("标识符不能为空或 null。", nameof(identifier));
        }

        var updateDate = await Authorizations.Includes(x => x.Application)
                .Where(x => x.Application!.Id == Guid.Parse(identifier))
                .ToListAsync(cancellationToken);

        foreach (var update in updateDate)
        {
            update.Status = Statuses.Revoked;
        }

        return await Context.Updateable(updateDate).ExecuteCommandAsync(cancellationToken);
    }


    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("Subject cannot be null or empty.", nameof(subject));
        }

        var updateData = await Authorizations.Includes(x => x.Application)
                .Where(x => x.Subject == subject)
                .ToListAsync(cancellationToken);

        foreach (var update in updateData)
        {
            update.Status = Statuses.Revoked;
        }

        return await Context.Updateable(updateData).ExecuteCommandAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async ValueTask SetApplicationIdAsync(OpenIddictSqlSugarAuthorization authorization,
        string? identifier, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            authorization.Application = await Applications.Where(x => x.Id == Guid.Parse(identifier)).FirstAsync();
        }
        else
        {
            authorization.Application = null;
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask SetCreationDateAsync(OpenIddictSqlSugarAuthorization authorization,
        DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.CreationDate = date?.UtcDateTime;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(OpenIddictSqlSugarAuthorization authorization,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (properties is not { Count: > 0 })
        {
            authorization.Properties = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var property in properties)
        {
            writer.WritePropertyName(property.Key);
            property.Value.WriteTo(writer);
        }

        writer.WriteEndObject();
        writer.Flush();

        authorization.Properties = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetScopesAsync(OpenIddictSqlSugarAuthorization authorization,
        ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (scopes.IsDefaultOrEmpty)
        {
            authorization.Scopes = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var scope in scopes)
        {
            writer.WriteStringValue(scope);
        }

        writer.WriteEndArray();
        writer.Flush();

        authorization.Scopes = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetStatusAsync(OpenIddictSqlSugarAuthorization authorization,
        string? status, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Status = status;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetSubjectAsync(OpenIddictSqlSugarAuthorization authorization,
        string? subject, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Subject = subject;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetTypeAsync(OpenIddictSqlSugarAuthorization authorization,
        string? type, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Type = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(OpenIddictSqlSugarAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        await Context.Updateable(authorization).ExecuteCommandAsync(cancellationToken);

    }
}