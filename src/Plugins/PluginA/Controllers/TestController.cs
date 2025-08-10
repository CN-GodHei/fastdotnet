using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using PluginA.Entities;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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