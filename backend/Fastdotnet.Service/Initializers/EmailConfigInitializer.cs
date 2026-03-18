

using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Initializers
{
    public class EmailConfigInitializer : IApplicationInitializer
    {
        private readonly IRepository<EmailConfig> _emailConfigRepository;
        private readonly ILogger<EmailConfigInitializer> _logger;

        public EmailConfigInitializer(IRepository<EmailConfig> emailConfigRepository, ILogger<EmailConfigInitializer> logger)
        {
            _emailConfigRepository = emailConfigRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Start: Initializing Email Config...");

            if (await _emailConfigRepository.ExistsAsync(a => a.Id != null))
            {
                //_logger.LogInformation("Email config already exists. Skipping initialization.");
                return;
            }

            var defaultConfig = new EmailConfig
            {
                Host = "smtp.example.com",
                Port = 465,
                Username = "user@example.com",
                Password = "your_password",
                SenderEmail = "noreply@example.com",
                SenderName = "Fastdotnet Team",
                EnableSsl = true
            };

            await _emailConfigRepository.InsertAsync(defaultConfig);

            //_logger.LogInformation("Finish: Default Email Config initialization complete.");
        }
    }
}
