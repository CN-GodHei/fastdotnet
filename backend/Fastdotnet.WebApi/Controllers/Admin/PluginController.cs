using Fastdotnet.Core.Dtos;
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
        public async Task<IActionResult> LoadPlugin([FromBody] DownloadPluginDto dto)
        {
            dto.IsValid();
            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            Directory.CreateDirectory(pluginsRoot);
            var targetPath = Path.Combine(pluginsRoot, dto.PluginId);

            //判断当前插件是否已存在
            if (Directory.Exists(targetPath))
            {
                // 【策略选择】：这里选择“先删除旧版本以实现覆盖更新”
                // 如果你的业务不允许覆盖，请改为返回 BadRequest
                //try
                //{
                //    Directory.Delete(targetPath, true);
                //}
                //catch (Exception ex)
                //{
                //    return BadRequest(new { Message = $"无法删除旧版本插件，可能文件被占用: {ex.Message}" });
                //}
                throw new BusinessException("插件已存在!");
            }
            if (!Uri.TryCreate(dto.Url, UriKind.Absolute, out var uriResult) ||
!(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                throw new BusinessException("无效的插件下载地址，仅支持 HTTP/HTTPS!");
            }
            string? tempPath = null;
            string? actualPluginPath = null; // 记录实际插件路径

            try
            {
                // 4. 下载插件包
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(5);

                var pluginBytes = await httpClient.GetByteArrayAsync(dto.Url);

                // 5. 创建临时目录进行解压 (避免直接解压到目标目录导致半安装状态)
                tempPath = Path.Combine(pluginsRoot, $"temp_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempPath);

                var zipPath = Path.Combine(tempPath, "plugin.zip");
                await global::System.IO.File.WriteAllBytesAsync(zipPath, pluginBytes);

                // 6. 解压到临时目录
                global::System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempPath);

                // 7. 处理可能的嵌套目录结构（ZIP 文件通常自带一层文件夹）
                actualPluginPath = FindActualPluginRoot(tempPath);

                // 8. 校验解压内容 (必须包含 DLL)
                var dllFiles = Directory.GetFiles(actualPluginPath, "*.dll", SearchOption.AllDirectories);
                if (!dllFiles.Any())
                {
                    return BadRequest(new { Message = "插件包中未找到任何 DLL 文件，可能不是有效的插件包" });
                }

                // 9. 移动实际插件目录到目标位置
                Directory.Move(actualPluginPath, targetPath);
                
                // 如果实际插件路径与临时路径不同，说明有嵌套目录，需要清理父目录
                if (!string.IsNullOrEmpty(tempPath) && actualPluginPath != tempPath && Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { /* 忽略清理异常 */ }
                }
                
                // 标记已清理完成
                tempPath = null;
                return Ok(new
                {
                    Installed = true,
                    Message = "插件安装/更新成功",
                    PluginId = dto.PluginId,
                    Path = targetPath,
                    DllCount = dllFiles.Length
                });
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { Message = $"下载失败: {ex.Message}" });
            }
            catch (IOException ex)
            {
                return BadRequest(new { Message = $"文件操作失败: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"安装过程发生未知错误: {ex.Message}" });
            }
            finally
            {
                // 9. 清理残留的临时目录
                if (!string.IsNullOrEmpty(tempPath) && Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch
                    {
                        // 记录日志，忽略清理异常
                    }
                }
            }
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
    }

    /// <summary>
    /// 插件下载传输模型
    /// </summary>
    public class DownloadPluginDto
    {
        /// <summary>
        /// 下载链接
        /// </summary>
        [Required]
        public string Url { get; set; }

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
        [Required]
        public string PluginName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [Required]
        public string Version { get; set; }
    }
}