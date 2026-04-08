/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Fastdotnet.Core.Entities.Oidc;
using static OpenIddict.Abstractions.OpenIddictExceptions;
using OpenIddict.Abstractions;
using SqlSugar;
using System.Linq.Expressions;
using System.Linq;


namespace Fastdotnet.Core.Service.Oidc.Stores;

/// <summary>
/// Provides methods allowing to manage the applications stored in a database.
/// </summary>
/// <typeparam name="OpenIddictSqlSugarApplication">The type of the Application entity.</typeparam>
/// <typeparam name="OpenIddictSqlSugarAuthorization">The type of the Authorization entity.</typeparam>
/// <typeparam name="OpenIddictSqlSugarToken">The type of the Token entity.</typeparam>
/// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
/// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
public class OpenIddictSqlSugarApplicationStore : IOpenIddictApplicationStore<OpenIddictSqlSugarApplication>
{
    public OpenIddictSqlSugarApplicationStore(
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
    /// Gets the database set corresponding to the <typeparamref name="OpenIddictSqlSugarApplication"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarApplication> Applications => Context.Queryable<OpenIddictSqlSugarApplication>();

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="OpenIddictSqlSugarAuthorization"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarAuthorization> Authorizations => Context.Queryable<OpenIddictSqlSugarAuthorization>();

    /// <summary>
    /// Gets the database set corresponding to the <typeparamref name="OpenIddictSqlSugarToken"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarToken> Tokens => Context.Queryable<OpenIddictSqlSugarToken>();

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        => await Applications.CountAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictSqlSugarApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var collection = await Applications.ToListAsync(cancellationToken);

        return query(collection.AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        // 初始化 ConcurrencyToken（乐观锁版本号）
        application.ConcurrencyToken = Guid.NewGuid().ToString();

        await Context.Insertable(application).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        try
        {
            await Context.Ado.BeginTranAsync();

            await Context.DeleteNav(application)
                .Include(x => x.Authorizations)
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
    public virtual async ValueTask<OpenIddictSqlSugarApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(nameof(identifier));
        }

        OpenIddictSqlSugarApplication result = await Applications.Includes(x => x.Authorizations, x => x.Tokens)
            .FirstAsync(x => x.ClientId == identifier, cancellationToken);

        return result;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException(nameof(identifier));
        }

        OpenIddictSqlSugarApplication result = await Applications.Includes(x => x.Authorizations, x => x.Tokens)
            .FirstAsync(x => x.Id == Guid.Parse(identifier), cancellationToken);

        return result;
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarApplication> FindByPostLogoutRedirectUriAsync(
        [StringSyntax(StringSyntaxAttribute.Uri)] string uri, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentException(nameof(uri));
        }

        var applications = await Applications.Where(x => x.PostLogoutRedirectUris!.Contains(uri)).ToListAsync(cancellationToken);

        foreach (var application in applications)
        {
            var uris = await GetPostLogoutRedirectUrisAsync(application, cancellationToken);
            if (uris.Contains(uri, StringComparer.Ordinal))
            {
                yield return application;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarApplication> FindByRedirectUriAsync(
        [StringSyntax(StringSyntaxAttribute.Uri)] string uri, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentException(nameof(uri));
        }

        var applications = await Applications.Where(x => x.RedirectUris!.Contains(uri)).ToListAsync(cancellationToken);

        foreach (var application in applications)
        {
            var uris = await GetRedirectUrisAsync(application, cancellationToken);
            if (uris.Contains(uri, StringComparer.Ordinal))
            {
                yield return application;
            }
        }

    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetApplicationTypeAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ApplicationType);
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarApplication>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var applications = await Applications.ToListAsync(cancellationToken);

        var queryableApplications = applications.AsQueryable();

        return query(queryableApplications, state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientIdAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ClientId);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientSecretAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ClientSecret);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientTypeAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ClientType);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetConsentTypeAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ConsentType);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetDisplayNameAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.DisplayName);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.DisplayNames))
        {
            return new(ImmutableDictionary.Create<CultureInfo, string>());
        }

        // Note: parsing the stringified display names is an expensive operation.
        // To mitigate that, the resulting object is stored in the memory cache.
        var key = string.Concat("7762c378-c113-4564-b14b-1402b3949aaa", "\x1e", application.DisplayNames);
        var names = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.DisplayNames);
            var builder = ImmutableDictionary.CreateBuilder<CultureInfo, string>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                var value = property.Value.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder[CultureInfo.GetCultureInfo(property.Name)] = value;
            }

            return builder.ToImmutable();
        })!;

        return new(names);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.JsonWebKeySet))
        {
            return new(result: null);
        }

        // Note: parsing the stringified JSON Web Key Set is an expensive operation.
        // To mitigate that, the resulting object is stored in the memory cache.
        var key = string.Concat("1e0a697d-0623-481a-927a-5e6c31458782", "\x1e", application.JsonWebKeySet);
        var set = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            return JsonWebKeySet.Create(application.JsonWebKeySet);
        })!;

        return new(set);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.Permissions))
        {
            return new(ImmutableArray<string>.Empty);
        }

        // Note: parsing the stringified permissions is an expensive operation.
        // To mitigate that, the resulting array is stored in the memory cache.
        var key = string.Concat("0347e0aa-3a26-410a-97e8-a83bdeb21a1f", "\x1e", application.Permissions);
        var permissions = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.Permissions);
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

        return new(permissions);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.PostLogoutRedirectUris))
        {
            return new(ImmutableArray<string>.Empty);
        }

        // Note: parsing the stringified URIs is an expensive operation.
        // To mitigate that, the resulting array is stored in the memory cache.
        var key = string.Concat("fb14dfb9-9216-4b77-bfa9-7e85f8201ff4", "\x1e", application.PostLogoutRedirectUris);
        var uris = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.PostLogoutRedirectUris);
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

        return new(uris);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.Properties))
        {
            return new(ImmutableDictionary.Create<string, JsonElement>());
        }

        // Note: parsing the stringified properties is an expensive operation.
        // To mitigate that, the resulting object is stored in the memory cache.
        var key = string.Concat("2e3e9680-5654-48d8-a27d-b8bb4f0f1d50", "\x1e", application.Properties);
        var properties = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.Properties);
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
    public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.RedirectUris))
        {
            return new(ImmutableArray<string>.Empty);
        }

        // Note: parsing the stringified URIs is an expensive operation.
        // To mitigate that, the resulting array is stored in the memory cache.
        var key = string.Concat("851d6f08-2ee0-4452-bbe5-ab864611ecaa", "\x1e", application.RedirectUris);
        var uris = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.RedirectUris);
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

        return new(uris);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.Requirements))
        {
            return new(ImmutableArray<string>.Empty);
        }

        // Note: parsing the stringified requirements is an expensive operation.
        // To mitigate that, the resulting array is stored in the memory cache.
        var key = string.Concat("b4808a89-8969-4512-895f-a909c62a8995", "\x1e", application.Requirements);
        var requirements = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.Requirements);
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

        return new(requirements);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (string.IsNullOrEmpty(application.Settings))
        {
            return new(ImmutableDictionary.Create<string, string>());
        }

        // Note: parsing the stringified settings is an expensive operation.
        // To mitigate that, the resulting object is stored in the memory cache.
        var key = string.Concat("492ea63f-c26f-47ea-bf9b-b0a0c3d02656", "\x1e", application.Settings);
        var settings = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(application.Settings);
            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                var value = property.Value.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                builder[property.Name] = value;
            }

            return builder.ToImmutable();
        })!;

        return new(settings);
    }

    /// <inheritdoc/>
    public virtual ValueTask<OpenIddictSqlSugarApplication> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(new OpenIddictSqlSugarApplication());
        }

        catch (MemberAccessException exception)
        {
            return new(Task.FromException<OpenIddictSqlSugarApplication>(
                new InvalidOperationException(string.Empty, exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarApplication> ListAsync(int? count, int? offset,[EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Applications.OrderBy(application => application.Id!);

        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        var applications = await query.ToListAsync(cancellationToken);

        var queryableApplications = applications.AsQueryable();
        foreach (var application in queryableApplications)
        {
            yield return application;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarApplication>, TState, IQueryable<TResult>> query,
        TState state,[EnumeratorCancellation] CancellationToken cancellationToken)
    {

        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var applications = await Applications.ToListAsync(cancellationToken);

        var queryableApplications = applications.AsQueryable();

        var resultQuery = query(queryableApplications, state);

        foreach (var item in resultQuery)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask SetApplicationTypeAsync(OpenIddictSqlSugarApplication application,
        string? type, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ApplicationType = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientIdAsync(OpenIddictSqlSugarApplication application, string? identifier, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ClientId = identifier;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientSecretAsync(OpenIddictSqlSugarApplication application, string? secret, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ClientSecret = secret;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientTypeAsync(OpenIddictSqlSugarApplication application, string? type, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ClientType = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetConsentTypeAsync(OpenIddictSqlSugarApplication application, string? type, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ConsentType = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNameAsync(OpenIddictSqlSugarApplication application, string? name, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.DisplayName = name;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNamesAsync(OpenIddictSqlSugarApplication application,
        ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (names is not { Count: > 0 })
        {
            application.DisplayNames = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var pair in names)
        {
            writer.WritePropertyName(pair.Key.Name);
            writer.WriteStringValue(pair.Value);
        }

        writer.WriteEndObject();
        writer.Flush();

        application.DisplayNames = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetJsonWebKeySetAsync(OpenIddictSqlSugarApplication application, JsonWebKeySet? set, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.JsonWebKeySet = set is not null ? System.Text.Json.JsonSerializer.Serialize(set) : null;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPermissionsAsync(OpenIddictSqlSugarApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (permissions.IsDefaultOrEmpty)
        {
            application.Permissions = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var permission in permissions)
        {
            writer.WriteStringValue(permission);
        }

        writer.WriteEndArray();
        writer.Flush();

        application.Permissions = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPostLogoutRedirectUrisAsync(OpenIddictSqlSugarApplication application,
        ImmutableArray<string> uris, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (uris.IsDefaultOrEmpty)
        {
            application.PostLogoutRedirectUris = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var uri in uris)
        {
            writer.WriteStringValue(uri);
        }

        writer.WriteEndArray();
        writer.Flush();

        application.PostLogoutRedirectUris = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(OpenIddictSqlSugarApplication application,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (properties is not { Count: > 0 })
        {
            application.Properties = null;

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

        application.Properties = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRedirectUrisAsync(OpenIddictSqlSugarApplication application,
        ImmutableArray<string> uris, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (uris.IsDefaultOrEmpty)
        {
            application.RedirectUris = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var uri in uris)
        {
            writer.WriteStringValue(uri);
        }

        writer.WriteEndArray();
        writer.Flush();

        application.RedirectUris = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRequirementsAsync(OpenIddictSqlSugarApplication application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (requirements.IsDefaultOrEmpty)
        {
            application.Requirements = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var requirement in requirements)
        {
            writer.WriteStringValue(requirement);
        }

        writer.WriteEndArray();
        writer.Flush();

        application.Requirements = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetSettingsAsync(OpenIddictSqlSugarApplication application,
        ImmutableDictionary<string, string> settings, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (settings is not { Count: > 0 })
        {
            application.Settings = null;

            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var setting in settings)
        {
            writer.WritePropertyName(setting.Key);
            writer.WriteStringValue(setting.Value);
        }

        writer.WriteEndObject();
        writer.Flush();

        application.Settings = Encoding.UTF8.GetString(stream.ToArray());

        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(OpenIddictSqlSugarApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        await Context.Updateable(application).ExecuteCommandAsync();

    }


}
