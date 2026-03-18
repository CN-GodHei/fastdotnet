using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 插件配置服务实现
    /// </summary>
    public class PluginConfigurationService : IPluginConfigurationService
    {
        private readonly IRawRepository<PluginConfiguration> _PluginConfigurationRepository;
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public PluginConfigurationService(IRawRepository<PluginConfiguration> PluginConfigurationRepository)
        {
            _PluginConfigurationRepository = PluginConfigurationRepository;
        }

        public async Task<T?> GetSettingsAsync<T>(string pluginId) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("PluginId cannot be null or empty.", nameof(pluginId));

            var record = await _PluginConfigurationRepository.GetByIdAsync(pluginId);
            if (record == null)
                return null;

            try
            {
                return JsonConvert.DeserializeObject<T>(record.ConfigJson, _jsonSettings);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize configuration for plugin '{pluginId}'.", ex);
            }
        }

        public async Task SaveSettingsAsync<T>(string pluginId, T settings) where T : class
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("PluginId cannot be null or empty.", nameof(pluginId));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var json = JsonConvert.SerializeObject(settings, Formatting.None, _jsonSettings);

            var config = new PluginConfiguration
            {
                PluginId = pluginId,
                ConfigJson = json
            };

            await _PluginConfigurationRepository.InsertOrUpdateAsync(config);
        }

        // ----------------------------
        // 原始 JSON 操作（高级用法）
        // ----------------------------

        public async Task<PluginConfigurationGetRawJsonDto> GetRawJsonAsync(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("PluginId cannot be null or empty.", nameof(pluginId));
            PluginConfigurationGetRawJsonDto pluginConfigurationGetRawJsonDto = new PluginConfigurationGetRawJsonDto();
            var record = await _PluginConfigurationRepository.GetByIdAsync(pluginId);
            if (record != null)
            {
                pluginConfigurationGetRawJsonDto.ExistRocord = true;
            }
            pluginConfigurationGetRawJsonDto.RawJson = record?.ConfigJson;
            return pluginConfigurationGetRawJsonDto;
        }

        public async Task SaveRawJsonAsync(string pluginId, string rawJson)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("PluginId cannot be null or empty.", nameof(pluginId));
            var pluginInfo = await _PluginConfigurationRepository.GetFirstAsync(w => w.PluginId == pluginId);
            if (pluginInfo == null)
            {
                throw new BusinessException("插件不支持配置");
            }
            if (rawJson == null)
                throw new ArgumentNullException(nameof(rawJson));

            // 可选：验证 JSON 是否有效（避免存入非法数据）
            try
            {
                JsonConvert.DeserializeObject(rawJson); // 仅验证，不使用结果
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid JSON format.", ex);
            }

            var config = new PluginConfiguration
            {
                PluginId = pluginId,
                ConfigJson = rawJson
            };
            await _PluginConfigurationRepository.InsertOrUpdateAsync(config);
        }
    }
}
