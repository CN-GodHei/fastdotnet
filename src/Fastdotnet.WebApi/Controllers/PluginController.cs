using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Core.Plugin;

namespace Fastdotnet.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PluginController : ControllerBase
    {
        private readonly PluginManager _pluginManager;

        public PluginController(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// 加载Demo插件
        /// </summary>
        /// <returns>加载结果</returns>
        [HttpPost("load")]
        public IActionResult LoadPlugin()
        {
            try
            {
                string pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins", "Fastdotnet.Demo");
                if (!Directory.Exists(pluginPath))
                {
                    return BadRequest(new { Message = $"插件目录不存在: {pluginPath}" });
                }

                string dllPath = Path.Combine(pluginPath, "Fastdotnet.Demo.dll");
                if (!System.IO.File.Exists(dllPath))
                {
                    return BadRequest(new { Message = $"插件DLL文件不存在: {dllPath}" });
                }

                _pluginManager.LoadPlugin(dllPath);
                return Ok(new { Message = "插件加载成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"插件加载失败: {ex.Message}" });
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
                _pluginManager.UnloadPlugin(pluginId);
                return Ok(new { Message = $"插件 {pluginId} 卸载成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"插件卸载失败: {ex.Message}" });
            }
        }
    }
}