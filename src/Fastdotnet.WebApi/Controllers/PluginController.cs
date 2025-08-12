using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Plugin.Core.Infrastructure;
using System.Threading.Tasks;
using Fastdotnet.Core.Exceptions;
using System;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> ScanPlugins()
        {
            var result = await _pluginLoadService.ScanPluginsAsync();
            return Ok(result);
        }

        /// <summary>
        /// 启用一个插件（如果未加载，则先加载）
        /// </summary>
        [HttpPost("enable/{pluginId}")]
        public async Task<IActionResult> EnablePlugin(string pluginId)
        {
            var result = await _pluginLoadService.EnablePluginAsync(pluginId);
            return Ok(result);
        }

        /// <summary>
        /// 停用一个插件（停止业务并卸载其代码）
        /// </summary>
        [HttpPost("disable/{pluginId}")]
        public async Task<IActionResult> DisablePlugin(string pluginId)
        {
            var result = await _pluginLoadService.DisablePluginAsync(pluginId);
            return Ok(result);
        }

        /// <summary>
        /// 从磁盘物理删除一个已停用的插件
        /// </summary>
        [HttpPost("uninstall/{pluginId}")]
        public async Task<IActionResult> UninstallPlugin(string pluginId)
        {
            var result = await _pluginLoadService.UninstallPluginAsync(pluginId);
            return Ok(result);
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

        [NonAction]
        public void Test()
        {
            Console.WriteLine("这是一个测试方法");
        }
        [HttpGet("gc")]
        public void ForceGC()
        {
            // The final breakthrough: It appears two full, blocking GC cycles are required.
            // The first cycle runs the finalizers of any lingering objects.
            // The second cycle collects the now-unreferenced AssemblyLoadContext itself.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            Console.WriteLine("First GC cycle completed.");

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            Console.WriteLine("Second GC cycle completed.");
        }
    }
}