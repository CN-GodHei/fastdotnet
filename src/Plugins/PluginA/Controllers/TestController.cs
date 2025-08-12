using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using PluginA.Entities;
using SqlSugar;
using System.Threading.Tasks;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public TestController(ISqlSugarClient db)
        {
            _db = db;
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
            var entities = await _db.Queryable<PluginEntity>().ToListAsync();
            return Ok(entities);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PluginEntity entity)
        {
            // 由于使用了雪花ID，在插入前就已经通过AOP设置了ID值
            var rows = await _db.Insertable(entity).ExecuteReturnIdentityAsync();
            return Ok(entity);
        }
    }
}