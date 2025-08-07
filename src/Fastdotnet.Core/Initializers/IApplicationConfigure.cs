using Microsoft.AspNetCore.Builder;

namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// Provides a mechanism for services to configure the application's request pipeline.
    /// </summary>
    public interface IApplicationConfigure
    {
        /// <summary>
        /// Configures the application's request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        void Configure(IApplicationBuilder app);
    }
}
