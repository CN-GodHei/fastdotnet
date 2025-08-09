using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using PluginA.Entities;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("api/plugin-a/[controller]")]
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
            var id = await _db.Insertable(entity).ExecuteReturnIdentityAsync();
            entity.Id = id;
            return Ok(entity);
        }
    }
}