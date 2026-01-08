using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Enum;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
    public class PluginController : ControllerBase
    {
        private readonly IPluginLoadService _pluginLoadService;
        private readonly ILogger<PluginController> _logger;

        public PluginController(IPluginLoadService pluginLoadService, ILogger<PluginController> logger)
        {
            _pluginLoadService = pluginLoadService;
            _logger = logger;
        }

        /// <summary>
        /// 扫描插件目录以发现所有可用插件
        /// </summary>
        [HttpGet("scan")]
        public async Task<List<PluginInfo>> ScanPlugins()
        {
            var result = await _pluginLoadService.ScanPluginsAsync();
            return result;
        }

        /// <summary>
        /// 启用一个插件（如果未加载，则先加载）
        /// </summary>
        [HttpPost("enable/{pluginId}")]
        public async Task<ApiResult> EnablePlugin(string pluginId)
        {
            var result = await _pluginLoadService.EnablePluginAsync(pluginId);
            return result;
        }

        /// <summary>
        /// 停用一个插件（停止业务并卸载其代码）
        /// </summary>
        [HttpPost("disable/{pluginId}")]
        public async Task<ApiResult> DisablePlugin(string pluginId)
        {
            var result = await _pluginLoadService.DisablePluginAsync(pluginId);
            return result;
        }

        /// <summary>
        /// 从磁盘物理删除一个已停用的插件
        /// </summary>
        [HttpPost("uninstall/{pluginId}")]
        public async Task<ApiResult> UninstallPlugin(string pluginId)
        {
            var result = await _pluginLoadService.UninstallPluginAsync(pluginId);
            return result;
        }

        /// <summary>
        /// 获取所有已加载的插件（无论是否激活）
        /// </summary>
        [HttpGet("loaded")]
        public IActionResult GetLoadedPlugins()
        {
            return Ok(_pluginLoadService.GetLoadedPlugins());
        }

        /// <summary>
        /// 获取所有当前活动的插件
        /// </summary>
        [HttpGet("active")]
        public IActionResult GetActivePlugins()
        {
            return Ok(_pluginLoadService.GetActivePlugins());
        }

        /// <summary>
        /// 检查一个插件当前是否处于活动状态
        /// </summary>
        [HttpGet("active/{pluginId}")]
        public IActionResult IsPluginActive(string pluginId)
        {
            return Ok(new { IsActive = _pluginLoadService.IsPluginActive(pluginId) });
        }
    }
}