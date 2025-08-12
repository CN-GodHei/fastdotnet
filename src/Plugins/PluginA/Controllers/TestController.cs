using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using PluginA.Entities;
using PluginA.IService;
using SqlSugar;
using System.Threading.Tasks;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [SkipGlobalResult] // 标记整个控制器跳过全局结果处理
    public class TestController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly IPluginEntityService _pluginEntityService;

        public TestController(ISqlSugarClient db, IPluginEntityService pluginEntityService)
        {
            _db = db;
            _pluginEntityService = pluginEntityService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 添加Debug日志测试
            //DebugLogger.Print("开始执行DisablePlugin方法", "PluginA");
            //_logger.LogInformation("正在尝试停用插件: {PluginId}", pluginId);

            //// 添加可控异常测试
            //throw new BusinessException("这是一个全局可控异常测试不会被记录到异常库-PluginA触发");

            //// 添加不可控异常测试
            //throw new Exception("这是一个全局不可控异常测试记录到异常库-PluginA触发");

            // 添加Debug日志测试 - 方法结束
            //DebugLogger.Print("DisablePlugin方法执行完成", "PluginA");
            var entities = await _pluginEntityService.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var entity = await _pluginEntityService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound("未找到指定实体");
            }
            return Ok(entity);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPage(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _pluginEntityService.GetPageAsync(null, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var entities = await _pluginEntityService.SearchByNameAsync(name);
            return Ok(entities);
        }

        [HttpGet("search/page")]
        public async Task<IActionResult> SearchByNamePaged(string name, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _pluginEntityService.SearchByNamePagedAsync(name, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PluginEntity entity)
        {
            var result = await _pluginEntityService.InsertAsync(entity);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PluginEntity entity)
        {
            var result = await _pluginEntityService.UpdateAsync(entity);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _pluginEntityService.DeleteAsync(id);
            return Ok(result);
        }
    }
}