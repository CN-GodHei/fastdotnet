using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.App
{
    [ApiController]
    [Route("api/app/menus")]
    [Authorize]
    public class AppMenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ICurrentUser _currentUser;

        public AppMenusController(IMenuService menuService, ICurrentUser currentUser)
        {
            _menuService = menuService;
            _currentUser = currentUser;
        }

        [HttpGet("tree")]
        // 提醒：您可能需要为 App 用户定义新的权限策略，例如 "app.menus.view"
        // [Authorize(Policy = "app.menus.view")] 
        public async Task<IActionResult> GetUserMenuTree()
        {
            var userId = _currentUser.Id;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 注意这里的 "App"
            var menus = await _menuService.GetUserMenusAsync(userId, "App");
            return Ok(menus);
        }
    }
}