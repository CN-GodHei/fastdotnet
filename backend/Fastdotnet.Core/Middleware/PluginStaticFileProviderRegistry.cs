using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;

namespace Fastdotnet.Core.Middleware
{
    public class PluginStaticFileProviderRegistry
    {
        private readonly ConcurrentDictionary<string, IFileProvider> _providers = new ConcurrentDictionary<string, IFileProvider>(StringComparer.OrdinalIgnoreCase);

        public void Register(string requestPath, string physicalPath)
        {
            if (Directory.Exists(physicalPath))
            {
                var fileProvider = new PhysicalFileProvider(physicalPath);
                _providers[requestPath] = fileProvider;
                Console.WriteLine($"Plugin static files registered. Request path: {requestPath}, Physical path: {physicalPath}");
            }
        }

        public void Unregister(string requestPath)
        {
            if (_providers.TryRemove(requestPath, out _))
            {
                Console.WriteLine($"Plugin static files unregistered. Request path: {requestPath}");
            }
        }

        public IFileProvider? GetProvider(string requestPath)
        {
            // Find the longest matching prefix
            var bestMatch = _providers.Keys
                .Where(prefix => requestPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();

            if (bestMatch != null && _providers.TryGetValue(bestMatch, out var provider))
            {
                return provider;
            }

            return null;
        }

        public string? GetBestMatchRequestPath(string requestPath)
        {
            return _providers.Keys
                .Where(prefix => requestPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(prefix => prefix.Length)
                .FirstOrDefault();
        }
    }
}
