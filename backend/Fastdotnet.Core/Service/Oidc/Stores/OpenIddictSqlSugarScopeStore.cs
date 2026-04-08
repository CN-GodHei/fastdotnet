/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Fastdotnet.Core.Entities.Oidc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using SqlSugar;

namespace Fastdotnet.Core.Service.Oidc.Stores;

/// <summary>
/// Provides methods allowing to manage the scopes stored in a database.
/// </summary>
public class OpenIddictSqlSugarScopeStore : IOpenIddictScopeStore<OpenIddictSqlSugarScope>
{
    public OpenIddictSqlSugarScopeStore(
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
    /// Gets the database set corresponding to the <see cref="OpenIddictSqlSugarScope"/> entity.
    /// </summary>
    private ISugarQueryable<OpenIddictSqlSugarScope> Scopes => Context.Queryable<OpenIddictSqlSugarScope>();

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        => await Scopes.CountAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIddictSqlSugarScope>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var collection = await Scopes.ToListAsync(cancellationToken);
        return query(collection.AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        await Context.Insertable(scope).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        await Context.Deleteable(scope).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var key = Guid.Parse(identifier);
        return await Scopes.FirstAsync(x => x.Id == key, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async ValueTask<OpenIddictSqlSugarScope?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The name cannot be null or empty.", nameof(name));
        }

        return await Scopes.FirstAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarScope> FindByNamesAsync(
        ImmutableArray<string> names, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (names.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentException("The names cannot contain null or empty values.", nameof(names));
        }

        var nameList = names.ToList();

        var scopes = await Scopes.Where(x => nameList.Contains(x.Name!)).ToListAsync(cancellationToken);
        foreach (var scope in scopes)
        {
            yield return scope;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarScope> FindByResourceAsync(
        string resource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(resource))
        {
            throw new ArgumentException("The resource cannot be null or empty.", nameof(resource));
        }

        var scopes = await Scopes.Where(x => x.Resources!.Contains(resource)).ToListAsync(cancellationToken);
        foreach (var scope in scopes)
        {
            var resources = await GetResourcesAsync(scope, cancellationToken);
            if (resources.Contains(resource, StringComparer.Ordinal))
            {
                yield return scope;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarScope>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var scopes = await Scopes.ToListAsync(cancellationToken);
        return query(scopes.AsQueryable(), state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetDescriptionAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        return new(scope.Description);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (string.IsNullOrEmpty(scope.Descriptions))
        {
            return new(ImmutableDictionary.Create<CultureInfo, string>());
        }

        var key = string.Concat("42891062-8f69-43ba-9111-db7e8ded2553", "\x1e", scope.Descriptions);
        var descriptions = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(scope.Descriptions);
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

        return new(descriptions);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetDisplayNameAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        return new(scope.DisplayName);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (string.IsNullOrEmpty(scope.DisplayNames))
        {
            return new(ImmutableDictionary.Create<CultureInfo, string>());
        }

        var key = string.Concat("e17d437b-bdd2-43f3-974e-46d524f4bae1", "\x1e", scope.DisplayNames);
        var names = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(scope.DisplayNames);
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
    public virtual ValueTask<string?> GetIdAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        return new(scope.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetNameAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        return new(scope.Name);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (string.IsNullOrEmpty(scope.Properties))
        {
            return new(ImmutableDictionary.Create<string, JsonElement>());
        }

        var key = string.Concat("78d8dfdd-3870-442e-b62e-dc9bf6eaeff7", "\x1e", scope.Properties);
        var properties = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(scope.Properties);
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
    public virtual ValueTask<ImmutableArray<string>> GetResourcesAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (string.IsNullOrEmpty(scope.Resources))
        {
            return new(ImmutableArray<string>.Empty);
        }

        var key = string.Concat("b6148250-aede-4fb9-a621-07c9bcf238c3", "\x1e", scope.Resources);
        var resources = Cache.GetOrCreate(key, entry =>
        {
            entry.SetPriority(CacheItemPriority.High)
                 .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            using var document = JsonDocument.Parse(scope.Resources);
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

        return new(resources);
    }

    /// <inheritdoc/>
    public virtual ValueTask<OpenIddictSqlSugarScope> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(Activator.CreateInstance<OpenIddictSqlSugarScope>());
        }
        catch (MemberAccessException exception)
        {
            return new(Task.FromException<OpenIddictSqlSugarScope>(
                new InvalidOperationException("An error occurred while trying to create a new scope instance.", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<OpenIddictSqlSugarScope> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = Scopes.OrderBy(scope => scope.Id!);

        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        var scopes = await query.ToListAsync(cancellationToken);
        foreach (var scope in scopes)
        {
            yield return scope;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIddictSqlSugarScope>, TState, IQueryable<TResult>> query,
        TState state, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var scopes = await Scopes.ToListAsync(cancellationToken);
        var resultQuery = query(scopes.AsQueryable(), state);

        foreach (var item in resultQuery)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDescriptionAsync(OpenIddictSqlSugarScope scope, string? description, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        scope.Description = description;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDescriptionsAsync(OpenIddictSqlSugarScope scope,
        ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (descriptions is not { Count: > 0 })
        {
            scope.Descriptions = null;
            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var description in descriptions)
        {
            writer.WritePropertyName(description.Key.Name);
            writer.WriteStringValue(description.Value);
        }

        writer.WriteEndObject();
        writer.Flush();

        scope.Descriptions = Encoding.UTF8.GetString(stream.ToArray());
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNameAsync(OpenIddictSqlSugarScope scope, string? name, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        scope.DisplayName = name;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNamesAsync(OpenIddictSqlSugarScope scope,
        ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (names is not { Count: > 0 })
        {
            scope.DisplayNames = null;
            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartObject();

        foreach (var name in names)
        {
            writer.WritePropertyName(name.Key.Name);
            writer.WriteStringValue(name.Value);
        }

        writer.WriteEndObject();
        writer.Flush();

        scope.DisplayNames = Encoding.UTF8.GetString(stream.ToArray());
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetNameAsync(OpenIddictSqlSugarScope scope, string? name, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        scope.Name = name;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(OpenIddictSqlSugarScope scope,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (properties is not { Count: > 0 })
        {
            scope.Properties = null;
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

        scope.Properties = Encoding.UTF8.GetString(stream.ToArray());
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetResourcesAsync(OpenIddictSqlSugarScope scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        if (resources.IsDefaultOrEmpty)
        {
            scope.Resources = null;
            return default;
        }

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var resource in resources)
        {
            writer.WriteStringValue(resource);
        }

        writer.WriteEndArray();
        writer.Flush();

        scope.Resources = Encoding.UTF8.GetString(stream.ToArray());
        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(OpenIddictSqlSugarScope scope, CancellationToken cancellationToken)
    {
        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        await Context.Updateable(scope).ExecuteCommandAsync();
    }
}
