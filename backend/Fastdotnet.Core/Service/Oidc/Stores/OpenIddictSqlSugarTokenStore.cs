/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Fastdotnet.Core.Entities.Oidc;
using OpenIddict.Abstractions;
using SqlSugar;
using static OpenIddict.Abstractions.OpenIddictExceptions;
using System.Runtime.CompilerServices;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Fastdotnet.Core.Service.Oidc.Stores;

/// <summary>
/// Provides methods allowing to manage the tokens stored in a database.
/// </summary>
/// <typeparam name="OpenIddictSqlSugarToken">The type of the Token entity.</typeparam>
/// <typeparam name="TApplication">The type of the Application entity.</typeparam>
/// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
/// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
/// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
public class OpenIddictSqlSugarTokenStore : IOpenIddictTokenStore<OpenIddictSqlSugarToken>
{
    public OpenIddictSqlSugarTokenStore(
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
    /// Gets the database set corresponding to the <typeparamref name="TAuthorization"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarAuthorization> Authorizations => Context.Queryable<OpenIddictSqlSugarAuthorization>();

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="OpenIddictSqlSugarToken"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarToken> Tokens => Context.Queryable<OpenIddictSqlSugarToken>();
    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        => await Tokens.CountAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictSqlSugarToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var collection = await Tokens.ToListAsync(cancellationToken);
        return query(collection.AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] CreateAsync called:");
        Console.WriteLine($"[OIDC TokenStore]   Type={token.Type}, Status={token.Status}");
        Console.WriteLine($"[OIDC TokenStore]   ReferenceId={token.ReferenceId}");
        Console.WriteLine($"[OIDC TokenStore]   ApplicationId={token.ApplicationId}, AuthorizationId={token.AuthorizationId}");

        // 初始化 ConcurrencyToken（乐观锁版本号）
        token.ConcurrencyToken = Guid.NewGuid();

        // 【关键修复】如果 ReferenceId 为空，生成一个
        // OpenIddict 可能会在后续调用 SetReferenceIdAsync 来更新这个值
        if (string.IsNullOrEmpty(token.ReferenceId))
        {
            Console.WriteLine($"[OIDC TokenStore]   ⚠️ ReferenceId is empty, generating one...");
            // 生成一个 Base64URL 编码的随机字符串作为 ReferenceId
            var bytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            token.ReferenceId = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
            Console.WriteLine($"[OIDC TokenStore]   Generated ReferenceId: {token.ReferenceId}");
        }
        else
        {
            Console.WriteLine($"[OIDC TokenStore]   Using existing ReferenceId: {token.ReferenceId}");
        }

        await Context.Insertable(token).ExecuteCommandAsync();

        Console.WriteLine($"[OIDC TokenStore]   ✅ Token created successfully with ReferenceId: {token.ReferenceId}");
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        await Context.Deleteable(token).ExecuteCommandAsync();
    }
    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarToken> FindAsync(
            string? subject, string? client,
        string? status, string? type, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Tokens.Includes(token => token.Application).Includes(token => token.Authorization);

        if (!string.IsNullOrEmpty(subject))
        {
            query = query.Where(token => token.Subject == subject);
        }

        if (!string.IsNullOrEmpty(client))
        {
            query = query.Where(token => token.Application!.ClientId == client);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(token => token.Status == status);
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(token => token.Type == type);
        }

        var tokens = await query.ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            yield return token;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarToken> FindByApplicationIdAsync(
        string identifier, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        // 直接将字符串转换为 Guid
        var key = Guid.Parse(identifier);
        // 使用 SqlSugar 的 Includes 方法来加载关联实体
        var query = Tokens
            .Includes(token => token.Application)
            .Includes(token => token.Authorization)
            .Where(token => token.Application!.Id == key);

        var tokens = await query.ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            yield return token;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarToken> FindByAuthorizationIdAsync(
        string identifier, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        // 直接将字符串转换为 Guid
        var key = Guid.Parse(identifier);

        // 使用 SqlSugar 的 Includes 方法来加载关联实体
        var query = Tokens
            .Includes(token => token.Application)
            .Includes(token => token.Authorization)
            .Where(token => token.Authorization!.Id == key);

        var tokens = await query.ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            yield return token;
        }
    }
    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        // 直接将字符串转换为 Guid
        var key = Guid.Parse(identifier);

        // 使用 SqlSugar 的 Includes 方法来加载关联实体
        var token = await Tokens
            .Includes(token => token.Application)
            .Includes(token => token.Authorization)
            .Where(token => token.Id == key)
            .FirstAsync(cancellationToken);
        return token;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        Console.WriteLine($"[OIDC TokenStore] FindByReferenceIdAsync called with identifier: {identifier}");
        Console.WriteLine($"[OIDC TokenStore]   DEBUG: identifier length: {identifier?.Length}, first 20 chars: {identifier?.Substring(0, Math.Min(20, identifier.Length))}");

        try
        {
            // 调试：先检查数据库中所有 token 的 ReferenceId
            var dbTokens = await Tokens.Select(t => new { t.Id, t.ReferenceId, t.Type }).ToListAsync(cancellationToken);
            Console.WriteLine($"[OIDC TokenStore]   DEBUG: Total tokens in DB: {dbTokens.Count}");
            foreach (var t in dbTokens.Take(10))
            {
                Console.WriteLine($"[OIDC TokenStore]   DEBUG: DB Token Id={t.Id}, ReferenceId='{t.ReferenceId}', Type={t.Type}");
                // 检查是否有匹配的 ReferenceId
                if (t.ReferenceId == identifier)
                {
                    Console.WriteLine($"[OIDC TokenStore]   DEBUG: ✅ Found MATCHING ReferenceId in DB!");
                }
            }

            // 使用 SqlSugar 的 Includes 方法来加载关联实体
            // 首先使用 ReferenceId 查找 token
            var token = await Tokens
                .Where(token => token.ReferenceId == identifier)
                .FirstAsync(cancellationToken);

            if (token != null)
            {
                // 单独加载 Application（如果需要）
                if (token.ApplicationId.HasValue && token.Application == null)
                {
                    token.Application = await Applications
                        .Where(a => a.Id == token.ApplicationId.Value)
                        .FirstAsync(cancellationToken);
                }

                // 单独加载 Authorization（如果需要）
                if (token.AuthorizationId.HasValue && token.Authorization == null)
                {
                    token.Authorization = await Authorizations
                        .Where(a => a.Id == token.AuthorizationId.Value)
                        .FirstAsync(cancellationToken);
                }

                Console.WriteLine($"[OIDC TokenStore]   ✅ Found token: Id={token.Id}, Type={token.Type}, Status={token.Status}");
                Console.WriteLine($"[OIDC TokenStore]   ApplicationId={token.ApplicationId}, AuthorizationId={token.AuthorizationId}");
                Console.WriteLine($"[OIDC TokenStore]   CreationDate={token.CreationDate}, ExpirationDate={token.ExpirationDate}");
                Console.WriteLine($"[OIDC TokenStore]   RedemptionDate={token.RedemptionDate}, ReferenceId={token.ReferenceId}");

                // 调试：验证 Authorization 状态
                if (token.Authorization != null)
                {
                    Console.WriteLine($"[OIDC TokenStore]   Authorization Status={token.Authorization.Status}, Type={token.Authorization.Type}");
                    Console.WriteLine($"[OIDC TokenStore]   Authorization Subject={token.Authorization.Subject}, ApplicationId={token.Authorization.ApplicationId}");
                }
                else
                {
                    Console.WriteLine($"[OIDC TokenStore]   ⚠️ Authorization is NULL!");
                }

                // 调试：检查 Token 是否过期
                if (token.ExpirationDate.HasValue && token.ExpirationDate.Value < DateTime.UtcNow)
                {
                    Console.WriteLine($"[OIDC TokenStore]   ⚠️ Token is EXPIRED!");
                }

                // 调试：检查 Token 是否已被使用
                if (token.RedemptionDate.HasValue)
                {
                    Console.WriteLine($"[OIDC TokenStore]   ⚠️ Token has already been redeemed!");
                }
            }
            else
            {
                Console.WriteLine($"[OIDC TokenStore]   ❌ Token NOT FOUND! Searching with ReferenceId: {identifier}");
                // 调试：尝试查找所有包含此 ReferenceId 前缀的 token
                var allTokens = await Tokens
                    .Where(t => t.ReferenceId != null && t.ReferenceId.Contains(identifier.Substring(0, Math.Min(10, identifier.Length))))
                    .ToListAsync(cancellationToken);
                Console.WriteLine($"[OIDC TokenStore]   Debug: Found {allTokens.Count} tokens with similar ReferenceId prefix");
                foreach (var t in allTokens.Take(5))
                {
                    Console.WriteLine($"[OIDC TokenStore]   Debug token: ReferenceId={t.ReferenceId}, Status={t.Status}, Type={t.Type}");
                }

                // 调试：查找所有 ReferenceId 为空的 token
                var emptyRefTokens = await Tokens.Where(t => t.ReferenceId == null || t.ReferenceId == "").ToListAsync(cancellationToken);
                if (emptyRefTokens.Any())
                {
                    Console.WriteLine($"[OIDC TokenStore]   ⚠️ Found {emptyRefTokens.Count} tokens with empty ReferenceId!");
                }
            }

            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OIDC TokenStore]   ❌ Error finding token: {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarToken> FindBySubjectAsync(
        string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        // 使用 SqlSugar 的 Includes 方法来加载关联实体
        var tokens = await Tokens
            .Includes(token => token.Application)
            .Includes(token => token.Authorization)
            .Where(token => token.Subject == subject)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            yield return token;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<string?> GetApplicationIdAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] GetApplicationIdAsync called: ApplicationId={token.ApplicationId}");

        if (token.Application is null)
        {

            var application = await Applications.Where(a => a.Id == token.ApplicationId)
            .FirstAsync(cancellationToken);

            token.Application = application;


        }

        if (token.Application is null)
        {
            Console.WriteLine($"[OIDC TokenStore]   Application is NULL!");
            return null;
        }

        Console.WriteLine($"[OIDC TokenStore]   Returning ApplicationId: {token.Application.Id}");
        return token.Application.Id.ToString();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarToken>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        Console.WriteLine($"[OIDC TokenStore] GetAsync called, loading tokens...");

        try
        {
            // 先检查数据库中有多少 token
            var totalCount = await Tokens.CountAsync(cancellationToken);
            Console.WriteLine($"[OIDC TokenStore]   Total tokens in DB: {totalCount}");

            var tokens = await Tokens.Includes(token => token.Application)
                                     .Includes(token => token.Authorization)
                                     .ToListAsync(cancellationToken);

            Console.WriteLine($"[OIDC TokenStore]   Loaded {tokens.Count} tokens");

            // 打印所有 token 的 ReferenceId（调试用）
            foreach (var t in tokens.Where(t => t.ReferenceId != null).Take(5))
            {
                Console.WriteLine($"[OIDC TokenStore]   Token: ReferenceId={t.ReferenceId}, Status={t.Status}, Type={t.Type}");
            }

            var result = query(tokens.AsQueryable(), state).FirstOrDefault();
            
            if (result != null)
            {
                Console.WriteLine($"[OIDC TokenStore]   ✅ Query returned a result");
            }
            else
            {
                Console.WriteLine($"[OIDC TokenStore]   ❌ Query returned no result!");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OIDC TokenStore]   ❌ GetAsync error: {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<string?> GetAuthorizationIdAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token), "token 参数不能为 null。");
        }

        Console.WriteLine($"[OIDC TokenStore] GetAuthorizationIdAsync called: AuthorizationId={token.AuthorizationId}");

        if (token.Authorization is null)
        {
            Console.WriteLine($"[OIDC TokenStore]   Loading Authorization from database...");
            var authorization = await Authorizations.Where(a => a.Id == token.AuthorizationId)
                .FirstAsync(cancellationToken);

            token.Authorization = authorization;
        }

        if (token.Authorization is null)
        {
            Console.WriteLine($"[OIDC TokenStore]   Authorization is NULL!");
            return null;
        }

        Console.WriteLine($"[OIDC TokenStore]   Returning AuthorizationId: {token.Authorization.Id}");
        return token.Authorization.Id.ToString(); // 直接将 Guid 转换为字符串
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.CreationDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.ExpirationDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetPayloadAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Payload);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (string.IsNullOrEmpty(token.Properties))
        {
            return new(ImmutableDictionary.Create<string, JsonElement>());
        }

        var key = string.Concat("d0509397-1bbf-40e7-97e1-5e6d7bc2536c", "\x1e", token.Properties);
        var properties = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(token.Properties);
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
    public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.RedemptionDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetReferenceIdAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] GetReferenceIdAsync called: returning '{token.ReferenceId}'");
        return new(token.ReferenceId);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetStatusAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Status);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetSubjectAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Subject);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetTypeAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Type);
    }

    /// <inheritdoc/>
    public virtual ValueTask<OpenIddictSqlSugarToken> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(new OpenIddictSqlSugarToken());
        }
        catch (MemberAccessException exception)
        {
            return new(Task.FromException<OpenIddictSqlSugarToken>(
                new InvalidOperationException("", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarToken> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Tokens.Includes(token => token.Application)
                          .Includes(token => token.Authorization)
                          .OrderBy(token => token.Id!);
        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        var tokens = await query.ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            yield return token;
        }

    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarToken>, TState, IQueryable<TResult>> query,
        TState state, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var tokens = await Tokens
            .Includes(token => token.Application)
            .Includes(token => token.Authorization)
            .ToListAsync(cancellationToken);

        var resultQuery = query(tokens.AsQueryable(), state);

        foreach (var item in resultQuery)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {

        var date = threshold.UtcDateTime;

        var deleteTokens = await Tokens.Includes(token => token.Application)
        .Where(token => token.CreationDate < date
        && ((token.Status != Statuses.Inactive && token.Status != Statuses.Valid)
           || (token.Authorization != null && token.Authorization.Status != Statuses.Valid)
           || token.ExpirationDate <= DateTime.UtcNow))
            .ToListAsync(cancellationToken);

        var exeCount = await Context.Deleteable(deleteTokens).ExecuteCommandAsync(cancellationToken);
        return exeCount;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeAsync(string? subject, string? client, string? status, string? type, CancellationToken cancellationToken)
    {

        var tokens = await Tokens.Includes(token => token.Application)
        .Includes(token => token.Authorization)
        .WhereIF(!string.IsNullOrEmpty(subject), token => token.Subject == subject)
        .WhereIF(!string.IsNullOrEmpty(client), token => token.Application!.Id == Guid.Parse(client!))
        .WhereIF(!string.IsNullOrEmpty(type), token => token.Type == type)
        .WhereIF(!string.IsNullOrEmpty(status), token => token.Status == status)
        .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Status = Statuses.Revoked;
        }

        var count = await Context.Updateable(tokens).ExecuteCommandAsync(cancellationToken);
        return count;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("标识符不能为空或 null。", nameof(identifier));
        }

        var key = Guid.Parse(identifier);

        var tokens = await Tokens.Includes(token => token.Application)
        .Where(token => token.Application!.Id == key)
        .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Status = Statuses.Revoked;
        }
        var count = await Context.Updateable(tokens).ExecuteCommandAsync(cancellationToken);
        return count;

    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("标识符不能为空或 null。", nameof(identifier));
        }

        var key = Guid.Parse(identifier);

        var tokens = await Tokens.Includes(token => token.Authorization)
        .Where(token => token.Authorization!.Id == key)
        .ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Status = Statuses.Revoked;
        }
        return await Context.Updateable(tokens).ExecuteCommandAsync(cancellationToken);
    }

    public virtual async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("标识符不能为空或 null。", nameof(subject));
        }

        var tokens = await Tokens.Where(token => token.Subject! == subject).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.Status = Statuses.Revoked;
        }
        return await Context.Updateable(tokens).ExecuteCommandAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async ValueTask SetApplicationIdAsync(OpenIddictSqlSugarToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            // Convert the identifier to the appropriate type (e.g., Guid).
            var key = Guid.Parse(identifier);

            // 同时设置 ApplicationId 字段和 Application 导航属性
            token.ApplicationId = key;
            // Find the application by its ID.
            token.Application = await Applications
                .Where(x => x.Id == key)
                .FirstAsync(cancellationToken);
        }
        else
        {
            token.ApplicationId = Guid.Empty;
            token.Application = null;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask SetAuthorizationIdAsync(OpenIddictSqlSugarToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            // Convert the identifier to the appropriate type (e.g., Guid).
            var key = Guid.Parse(identifier);

            // 同时设置 AuthorizationId 字段和 Authorization 导航属性
            token.AuthorizationId = key;
            // Find the authorization by its ID.
            token.Authorization = await Authorizations
                .Where(x => x.Id == key)
                .FirstAsync(cancellationToken);
        }
        else
        {
            // If the identifier is null, set the authorization to null.
            token.AuthorizationId = Guid.Empty;
            token.Authorization = null;
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask SetCreationDateAsync(OpenIddictSqlSugarToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.CreationDate = date?.UtcDateTime;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetExpirationDateAsync(OpenIddictSqlSugarToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.ExpirationDate = date?.UtcDateTime;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPayloadAsync(OpenIddictSqlSugarToken token, string? payload, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Payload = payload;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(OpenIddictSqlSugarToken token,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (properties is not { Count: > 0 })
        {
            token.Properties = null;

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

        token.Properties = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRedemptionDateAsync(OpenIddictSqlSugarToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] SetRedemptionDateAsync called: oldValue={token.RedemptionDate}, newValue={date?.UtcDateTime}");
        token.RedemptionDate = date?.UtcDateTime;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetReferenceIdAsync(OpenIddictSqlSugarToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        var oldValue = token.ReferenceId;
        Console.WriteLine($"[OIDC TokenStore] SetReferenceIdAsync called: oldValue='{oldValue}', newValue='{identifier}'");
        
        // 【关键修复】始终更新 ReferenceId
        // OpenIddict 在 Protected Token 模式下会使用 ReferenceId 作为内部标识符
        // 授权码是经过加密的 ReferenceId，解密后得到这个内部标识符用于查找 token
        if (!string.IsNullOrEmpty(identifier))
        {
            token.ReferenceId = identifier;
            Console.WriteLine($"[OIDC TokenStore]   Updated ReferenceId to: {identifier}");
        }
        
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetStatusAsync(OpenIddictSqlSugarToken token, string? status, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] SetStatusAsync called: oldValue={token.Status}, newValue={status}");
        token.Status = status;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetSubjectAsync(OpenIddictSqlSugarToken token, string? subject, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Subject = subject;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetTypeAsync(OpenIddictSqlSugarToken token, string? type, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Type = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(OpenIddictSqlSugarToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        Console.WriteLine($"[OIDC TokenStore] UpdateAsync called: Id={token.Id}, ReferenceId={token.ReferenceId}, Status={token.Status}");
        
        // 调试：打印所有字段的当前值
        Console.WriteLine($"[OIDC TokenStore]   ApplicationId={token.ApplicationId}, AuthorizationId={token.AuthorizationId}");
        Console.WriteLine($"[OIDC TokenStore]   Type={token.Type}, Subject={token.Subject}");
        Console.WriteLine($"[OIDC TokenStore]   CreationDate={token.CreationDate}, ExpirationDate={token.ExpirationDate}");
        Console.WriteLine($"[OIDC TokenStore]   RedemptionDate={token.RedemptionDate}, ConcurrencyToken={token.ConcurrencyToken}");

        await Context.Updateable(token).ExecuteCommandAsync(cancellationToken);
        
        Console.WriteLine($"[OIDC TokenStore]   ✅ Token updated successfully");
        
        // 验证：重新从数据库读取，确认更新成功
        var verifyToken = await Tokens.Where(t => t.Id == token.Id).FirstAsync(cancellationToken);
        if (verifyToken != null)
        {
            Console.WriteLine($"[OIDC TokenStore]   Verification: DB ReferenceId={verifyToken.ReferenceId}");
        }

    }


}

