using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Services.System;
using Microsoft.Extensions.Logging;
using PluginA.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Initializers
{
    public class PluginConfigurationInitializer : IApplicationInitializer
    {
        //private readonly IRepository<PluginConfiguration> _Repository;
        private readonly IPluginConfigurationService _pluginConfigurationService;

        public PluginConfigurationInitializer(IPluginConfigurationService pluginConfigurationService)
        {
            _pluginConfigurationService = pluginConfigurationService;
        }

        public async Task InitializeAsync()
        {

            var configs = new MyPluginConfiguration()
            {
                AliOssInfo = new AliOss() { AK = "1", SK = "2" },
                AliPayInfo = new AliPay() { AppId = "商户Id", Secret = "密钥111" },
            };
            _pluginConfigurationService.SaveSettingsAsync<MyPluginConfiguration>("11375910391972869", configs);
        }
    }
}