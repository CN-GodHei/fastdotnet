using System.Collections.Generic;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Service.Initializers
{
    public class SystemConfigInitializer : IApplicationInitializer
    {
        private readonly IRepository<SystemConfig> _systemConfigRepository;
        private readonly ILogger<SystemConfigInitializer> _logger;

        public SystemConfigInitializer(IRepository<SystemConfig> systemConfigRepository, ILogger<SystemConfigInitializer> logger)
        {
            _systemConfigRepository = systemConfigRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Start: Initializing System Config...");

            if (await _systemConfigRepository.ExistsAsync(a=>a.Id != null))
            {
                _logger.LogInformation("System config already seeded. Skipping initialization.");
                return;
            }

            var configs = new List<SystemConfig>
            {
                new SystemConfig { Name = "App版本号", Code = "AppVersion", Value = "1.0.0", Description = "当前应用版本号", IsSystem = true },
                new SystemConfig { Name = "水印内容", Code = "Watermark", Value = "Fastdotnet", Description = "系统水印内容。在“启用水印”开启后生效。", IsSystem = true },
                new SystemConfig { Name = "启用验证码", Code = "EnableCaptcha", Value = false, Description = "控制登录、注册等功能是否开启图片或行为验证码", IsSystem = true },
                new SystemConfig { Name = "验证码类型", Code = "CaptchaType", Value = "normal", Description = "验证码类型，可选值：normal (图形验证码), behavioral (行为验证)", IsSystem = true },
                new SystemConfig { Name = "注册邮箱验证", Code = "EnableRegisterEmailVerification", Value = false, Description = "控制用户注册时是否必须通过邮箱验证", IsSystem = true },

                // 新增配置项
                new SystemConfig { Name = "系统名称", Code = "SystemName", Value = "Fastdotnet", Description = "显示在浏览器标签和登录页的系统名称", IsSystem = true },
                new SystemConfig { Name = "系统Logo", Code = "SystemLogo", Value = "", Description = "显示在登录页和侧边栏顶部的Logo图片URL", IsSystem = true },
                new SystemConfig { Name = "启用水印", Code = "EnableWatermark", Value = true, Description = "是否在系统页面上显示水印", IsSystem = true },
                new SystemConfig { Name = "版权信息", Code = "CopyrightInfo", Value = $"Copyright © 2025 Fastdotnet. All Rights Reserved.", Description = "显示在登录页和布局页脚的版权信息", IsSystem = true }
            };

            await _systemConfigRepository.InsertRangeAsync(configs);

            _logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}