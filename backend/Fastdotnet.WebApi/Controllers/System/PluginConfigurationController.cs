namespace Fastdotnet.WebApi.Controllers.System
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
        [HttpGet("Get-Plugin-ConfigurationBy/{id}")]
        public async Task<string?> GetPluginConfigurationById(string PluginId)
        {
            return await _pluginConfigurationService.GetRawJsonAsync(PluginId);
        }

        [HttpPut("{id}")]
        public async Task<bool> Update(string Id, [FromBody] string RawJson)
        {
            await _pluginConfigurationService.SaveRawJsonAsync(Id, RawJson);
            return true;
        }
    }
}
