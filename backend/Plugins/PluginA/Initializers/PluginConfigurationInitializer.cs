using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Services.System;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IPluginConfigurationService _pluginConfigurationService;

        public PluginConfigurationInitializer(IPluginConfigurationService pluginConfigurationService)
        {
            _pluginConfigurationService = pluginConfigurationService;
        }

        public async Task InitializeAsync()
        {

            var defaultConfig = new MyPluginConfiguration()
            {
                AliOssInfo = new AliOss() { AK = "1", SK = "2" },
                AliPayInfo = new AliPay() { AppId = "商户Id", Secret = "密钥111" },
                testinfo ="测试增加",
            };
            var existingConfig = await _pluginConfigurationService.GetSettingsAsync<MyPluginConfiguration>("11375910391972869");

            //Console.WriteLine("defaultConfig: " + JsonConvert.SerializeObject(defaultConfig));
            //Console.WriteLine("Existing: " + JsonConvert.SerializeObject(existingConfig));

            var mergedConfig = ObjectMerger.ApplyOverrides(defaultConfig,existingConfig);
            //Console.WriteLine("mergedConfig: " + JsonConvert.SerializeObject(mergedConfig));

            await _pluginConfigurationService.SaveSettingsAsync("11375910391972869", mergedConfig);
        }
    }
}