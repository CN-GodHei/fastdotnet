using Fastdotnet.Core.Initializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Fastdotnet.WebApi.Extensions
{
    /// <summary>
    /// Provides extension methods for application configuration from IApplicationConfigure implementations.
    /// </summary>
    public static class ApplicationConfigureExtensions
    {
        /// <summary>
        /// Discovers and executes all registered IApplicationConfigure services to configure the request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseApplicationConfigures(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var configures = scope.ServiceProvider.GetServices<IApplicationConfigure>();

            foreach (var configure in configures)
            {
                configure.Configure(app);
            }

            return app;
        }
    }
}
