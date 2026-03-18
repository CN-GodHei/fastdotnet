using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Enum;

namespace Fastdotnet.WebApi.Controllers.App
{
    [ApiController]
    [Route("api/app/menus")]
    [Authorize]
    [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
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
        public async Task<List<FdMenuDto>> GetUserMenuTree()
        {
            var userId = _currentUser.Id;
            if (userId == null)
            {
                throw new BusinessException("用户未登录");
            }

            // 注意这里的 "App"
            var menus = await _menuService.GetUserMenusAsync(userId, SystemCategory.App);
            return menus;
        }
    }
}