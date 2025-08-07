using System.Threading.Tasks;
using Fastdotnet.Core.Initializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.WebApi.Extensions
{
    /// <summary>
    /// Provides extension methods for application initializers.
    /// </summary>
    public static class ApplicationInitializerExtensions
    {
        /// <summary>
        /// Discovers and executes all registered IApplicationInitializer services.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        public static async Task UseApplicationInitializers(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializers = scope.ServiceProvider.GetServices<IApplicationInitializer>();

            foreach (var initializer in initializers)
            {
                await initializer.InitializeAsync();
            }
        }
    }
}
