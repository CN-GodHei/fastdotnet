using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Core.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;
using System;
using System.Linq;
using Fastdotnet.Plugin.Core.Infrastructure;

namespace Fastdotnet.WebApi.Controllers
{
    /// <summary>
    /// 插件模块
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PluginController : ControllerBase
    {
        //private readonly PluginManager _pluginManager;

        //public PluginController(PluginManager pluginManager)
        //{
        //    _pluginManager = pluginManager;
        //}
        private readonly IPluginLoadService _pluginLoadService;

        public PluginController(IPluginLoadService pluginLoadService)
        {
            _pluginLoadService = pluginLoadService;
        }

        /// <summary>
        /// 从URL下载并加载插件
        /// </summary>
        /// <param name="pluginUrl">插件下载地址</param>
        /// <returns>加载结果</returns>
        [HttpPost("load")]
        public async Task<IActionResult> LoadPlugin([FromBody] string pluginUrl)
        {
            try
            {
                using var httpClient = new HttpClient();
                var pluginBytes = await httpClient.GetByteArrayAsync(pluginUrl);
                
                // 创建临时目录解压插件
                var tempPath = Path.Combine(AppContext.BaseDirectory, "plugins", Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);
                
                // 保存并解压插件包
                var zipPath = Path.Combine(tempPath, "plugin.zip");
                await System.IO.File.WriteAllBytesAsync(zipPath, pluginBytes);
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempPath);
                
                // 查找插件DLL
                var configPath = Directory.GetFiles(tempPath, "plugin.json", SearchOption.AllDirectories).FirstOrDefault();
                if (configPath == null)
                {
                    Directory.Delete(tempPath, true);
                    return BadRequest(new { Message = "插件包中未找到plugin.json文件" });
                }

                var configJson = await System.IO.File.ReadAllTextAsync(configPath);
                var config = System.Text.Json.JsonSerializer.Deserialize<Fastdotnet.Core.Plugin.PluginConfig>(configJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (config == null || string.IsNullOrEmpty(config.id))
                {
                    Directory.Delete(tempPath, true);
                    return BadRequest(new { Message = "无法读取插件ID" });
                }

                var result = await _pluginLoadService.LoadPluginAsync(config.id);

                return Ok(new { Message = result.Msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"插件加载失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>启用结果</returns>
        [HttpPost("enable/{pluginId}")]
        public async Task<IActionResult> EnablePlugin(string pluginId)
        {
            try
            {
                var pluginPath = _pluginLoadService.GetPluginPath(pluginId);
                if (string.IsNullOrEmpty(pluginPath))
                {
                    return BadRequest(new { Message = "插件不存在" });
                }

                var configPath = Path.Combine(pluginPath, "plugin.json");
                if (!System.IO.File.Exists(configPath))
                {
                    return BadRequest(new { Message = "插件配置文件不存在" });
                }

                // 读取并修改配置
                //var config = JObject.Parse(System.IO.File.ReadAllText(configPath));
                //config["enabled"] = true;

                try
                {
                    // 尝试加载插件
                    //var dllPath = Directory.GetFiles(pluginPath, "*.dll").First();
                    var result = await _pluginLoadService.LoadPluginAsync(pluginId);
                    if (!result.Result)
                    {
                        throw new Exception(result.Msg);
                    }

                    // 保存配置
                    //System.IO.File.WriteAllText(configPath, config.ToString(Formatting.Indented));
                    return Ok(new { Message = "插件启用成功" });
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"{ex.Message}" });
            }
        }

        /// <summary>
        /// 停用插件
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>停用结果</returns>
        [HttpPost("disable/{pluginId}")]
        public async Task<IActionResult> DisablePlugin(string pluginId)
        {
            try
            {
                var pluginPath = _pluginLoadService.GetPluginPath(pluginId);
                if (string.IsNullOrEmpty(pluginPath))
                {
                    return BadRequest(new { Message = "插件不存在" });
                }

                // 卸载插件服务和路由
              var result = await  _pluginLoadService.UnloadPluginAsync(pluginId);

                // 更新配置文件
                //var configPath = Path.Combine(pluginPath, "plugin.json");
                //if (System.IO.File.Exists(configPath))
                //{
                //    var config = JObject.Parse(System.IO.File.ReadAllText(configPath));
                //    config["enabled"] = false;
                //    System.IO.File.WriteAllText(configPath, config.ToString(Formatting.Indented));
                //}

                return Ok(new { Message = "插件停用成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"插件停用失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 卸载指定ID的插件
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>卸载结果</returns>
        [HttpPost("unload/{pluginId}")]
        public IActionResult UnloadPlugin(string pluginId)
        {
            try
            {
                var pluginPath = _pluginLoadService.GetPluginPath(pluginId);
                if (string.IsNullOrEmpty(pluginPath))
                {
                    return BadRequest(new { Message = "插件不存在" });
                }

                // 检查插件状态
                var configPath = Path.Combine(pluginPath, "plugin.json");
                if (System.IO.File.Exists(configPath))
                {
                    var config = JObject.Parse(System.IO.File.ReadAllText(configPath));
                    if (config["enabled"]?.Value<bool>() == true)
                    {
                        return BadRequest(new { Message = "请先停用插件再卸载" });
                    }
                }

                // 删除插件目录
                Directory.Delete(pluginPath, true);
                return Ok(new { Message = "插件卸载成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"插件卸载失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 判断插件是否已加载
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        [HttpGet("{pluginName}")]
        public IActionResult IsPluginLoaded(string pluginName)
        {
            var isLoaded = _pluginLoadService.IsPluginLoaded(pluginName);
            return Ok(new { isLoaded });
        }

        /// <summary>
        /// 获取已加载的插件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLoadedPlugins()
        {
            var plugins = _pluginLoadService.GetLoadedPlugins();
            return Ok(plugins);
        }
    }
}