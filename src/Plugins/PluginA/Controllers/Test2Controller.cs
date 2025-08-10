using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using Fastdotnet.Core.Models.User;

namespace PluginA.Controllers
{
    [ApiController]
    [Route(template: "api/[controller]/[action]")]
    public class Test2Controller : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public Test2Controller(ISqlSugarClient db)
        {
            _db = db;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Queryable<FdUser>().ToListAsync();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] FdUser user)
        {
            var id = await _db.Insertable(user).ExecuteReturnIdentityAsync();
            user.Id = id;
            return Ok(user);
        }
    }
}