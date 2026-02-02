
namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// Defines an interface for a service that should be initialized after the application's services are configured.
    /// </summary>
    public interface IApplicationInitializer
    {
        /// <summary>
        /// Executes the initialization logic for the service.
        /// </summary>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        Task InitializeAsync();
    }
}
