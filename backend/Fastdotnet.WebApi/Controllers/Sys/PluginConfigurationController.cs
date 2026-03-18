using Fastdotnet.Core.Dtos.Sys;

namespace Fastdotnet.WebApi.Controllers.Sys
{
    /// <summary>
    /// 插件配置信息
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
    public class PluginConfigurationController : ControllerBase
    {
        private readonly IPluginConfigurationService _pluginConfigurationService;
        //private readonly IBaseService<PluginConfiguration> _service;
        public PluginConfigurationController(IPluginConfigurationService pluginConfigurationService)
        {
            _pluginConfigurationService = pluginConfigurationService;
        }

        /// <summary>
        /// 使用插件Id获取插件配置信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Get-Plugin-ConfigurationBy/{PluginId}")]
        public async Task<PluginConfigurationGetRawJsonDto> GetPluginConfigurationById(string PluginId)
        {
            return await _pluginConfigurationService.GetRawJsonAsync(PluginId);
        }

        [HttpPut("{PluginId}")]
        public async Task<bool> Update(string PluginId, [FromBody] string RawJson)
        {
            await _pluginConfigurationService.SaveRawJsonAsync(PluginId, RawJson);
            return true;
        }
    }
}
