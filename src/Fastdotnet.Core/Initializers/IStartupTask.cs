using System.Threading.Tasks;

namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// Defines an interface for tasks that should be executed when the application starts.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ExecuteAsync();
    }
}
