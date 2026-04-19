using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Core.Service.Sys;
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
            // 动态获取当前插件的ID，而不是使用硬编码的ID
            var pluginInfo = PluginContext.GetCurrentPluginInfo();
            //var pluginId = pluginInfo?.id;
            //Console.WriteLine("获取到插件信息:" + pluginInfo.id);
            var defaultConfig = new MyPluginConfiguration()
            {
                AliOssInfo = new AliOss() { AK = "1", SK = "2" },
                AliPayInfo = new AliPay() { AppId = "商户Id", Secret = "密钥111" },
                testinfo ="测试增加",
            };
            var existingConfig = await _pluginConfigurationService.GetSettingsAsync<MyPluginConfiguration>(pluginInfo.id);

            //Console.WriteLine("defaultConfig: " + JsonConvert.SerializeObject(defaultConfig));
            //Console.WriteLine("Existing: " + JsonConvert.SerializeObject(existingConfig));

            var mergedConfig = ObjectMerger.ApplyOverrides(defaultConfig,existingConfig);
            //Console.WriteLine("mergedConfig: " + JsonConvert.SerializeObject(mergedConfig));

            await _pluginConfigurationService.SaveSettingsAsync(pluginInfo.id, mergedConfig);
        }
    }
}