using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Enum;
using System.ComponentModel.DataAnnotations;

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
        /// 从URL下载并加载插件
        /// </summary>
        /// <param name="pluginUrl">插件下载地址</param>
        /// <returns>加载结果</returns>
        [HttpPost("load")]
        public async Task<ApiResult> LoadPlugin([FromBody] DownloadPluginDto dto)
        {
            dto.IsValid();
            return await _pluginLoadService.InstallPlugin(dto.PluginId, dto.Version, dto.Token);
        }

        /// <summary>
        /// 扫描插件目录以发现所有可用插件
        /// </summary>
        [HttpGet("scan")]
        [ApiUsageScope(Core.Enum.ApiUsageScopeEnum.Both)]
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
        /// 获取所有当前活动的插件
        /// </summary>
        [HttpGet("active")]
        public IActionResult GetActivePlugins()
        {
            return Ok(_pluginLoadService.GetActivePlugins());
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
        /// 查找实际的插件根目录（处理 ZIP 文件自带的嵌套文件夹）
        /// </summary>
        /// <param name="extractPath">解压后的临时目录</param>
        /// <returns>实际包含插件文件的根目录</returns>
        private static string FindActualPluginRoot(string extractPath)
        {
            var directories = Directory.GetDirectories(extractPath);

            // 如果只有一个子目录，且该子目录包含 DLL 文件，则返回子目录
            if (directories.Length == 1)
            {
                var singleSubDir = directories[0];
                var dllFiles = Directory.GetFiles(singleSubDir, "*.dll", SearchOption.AllDirectories);

                if (dllFiles.Any())
                {
                    return singleSubDir;
                }
            }

            // 否则返回原始解压目录（可能在根目录就有 DLL 文件）
            return extractPath;
        }

        /// <summary>
        /// 检查一个插件当前是否处于活动状态
        /// </summary>
        [HttpGet("active/{pluginId}")]
        public IActionResult IsPluginActive(string pluginId)
        {
            return Ok(new { IsActive = _pluginLoadService.IsPluginActive(pluginId) });
        }
        /// <summary>
        /// 设置用户授权码
        /// </summary>
        [HttpPost("SetAuthCode")]
        public async Task<bool> SetAuthCode([FromBody] SetAuthCodeDto setAuthCodeDto)
        {
            setAuthCodeDto.IsValid();
            return await _pluginLoadService.SetAuthCode(setAuthCodeDto.AuthCode);
        }

        /// <summary>
        /// 设置插件许可
        /// </summary>
        [HttpPost("SetPluginLicense")]
        public async Task<bool> SetPluginLicense([FromBody] SetPluginLicenseDto setAuthCodeDto)
        {
            setAuthCodeDto.IsValid();
            return await _pluginLoadService.SetPluginLicense(setAuthCodeDto);
        }
    }

    public class SetAuthCodeDto
    {
        /// <summary>
        /// 用户授权码
        /// </summary>
        [Required]
        public string AuthCode { get; set; }
    }
    /// <summary>
    /// 插件下载传输模型
    /// </summary>
    public class DownloadPluginDto
    {
        /// <summary>
        /// 下载链接
        /// </summary>
        //[Required]
        //public string Url { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// 插件Id
        /// </summary>
        [Required]
        public string PluginId { get; set; }

        /// <summary>
        /// 插件名称
        /// </summary>
        //[Required]
        //public string PluginName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [Required]
        public string Version { get; set; }
    }
}