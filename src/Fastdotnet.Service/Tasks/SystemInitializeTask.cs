using Fastdotnet.Core.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Tasks
{
    public class SystemInitializeTask : IStartupTask
    {
        private readonly ILogger<SystemInitializeTask> _logger;

        public SystemInitializeTask(ILogger<SystemInitializeTask> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("=============== System initialization task executed successfully after application startup. ===============");
            return Task.CompletedTask;
        }
    }
}
