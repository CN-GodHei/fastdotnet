
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[Controller]")]
    [Authorize]
    public class FdMenuController : GenericDtoControllerBase<FdMenu, string, CreateFdMenuDto, UpdateFdMenuDto, FdMenuDto>
    {
        private readonly IMenuService _menuService;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<FdRole> _roleRepository;
        private readonly IRepository<FdMenuButton> _MenuBtnRepository;
        private readonly IRepository<FdRoleMenu> _roleMenuRepository;
        private readonly IRepository<FdRoleMenuButton> _roleMenuBtnRepository;
        private readonly IAdminUserService _adminUserService;
        private readonly IUserRefFiller _userRefFiller;
        public FdMenuController(
            IBaseService<FdMenu, string> service,
            IMapper mapper,
            IMenuService menuService,
            ICurrentUser currentUser,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IAdminUserService adminUserService,
            IRepository<FdRole> roleRepository,
            IRepository<FdRoleMenu> roleMenuRepository,
            IRepository<FdRoleMenuButton> roleMenuBtnRepository,
            IUserRefFiller userRefFiller,

            IRepository<FdMenuButton> menuBtnRepository) : base(service, mapper)
        {
            _menuService = menuService;
            _currentUser = currentUser;
            _adminUserService = adminUserService;
            _roleRepository = roleRepository;
            _userRefFiller = userRefFiller;
            _MenuBtnRepository = menuBtnRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleMenuBtnRepository = roleMenuBtnRepository;
        }

        [HttpGet("tree")]
        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public async Task<List<FdMenuDto>> GetUserMenuTree()
        {
            var userId = _currentUser.Id;
            if (userId == null)
            {
                //return Unauthorized();
                throw new BusinessException("用户未登录");
            }

            // 判断是否为超管角色
            bool isSuperAdmin = await _adminUserService.IsSuperAdminAsync(userId);
            List<FdMenuDto> menus;

            if (isSuperAdmin)
            {
                // 超管获取所有菜单
                var menutemp = await _service.GetListAsync(m => m.Belong == SystemCategory.Admin && m.IsEnabled);
                menus = await _menuService.BuildMenuTree(menutemp, null);
            }
            else
            {
                // 普通用户根据权限获取菜单
                menus = await _menuService.GetUserMenusAsync(userId, SystemCategory.Admin);
            }

            return menus;
        }


        [HttpGet]
        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override async Task<List<FdMenuDto>> GetAll(CancellationToken cancellationToken = default)
        {
            // 获取所有菜单
            var menus = await _service.GetListAsync(m => m.Belong == SystemCategory.Admin);

            // 构建树形结构
            var menuTree = await _menuService.BuildMenuTree(menus, null);

            // 转换为 DTO
            var menuDtos = _mapper.Map<List<FdMenuDto>>(menuTree);
            await _userRefFiller.FillNamesAsync(menuDtos, SystemCategory.Admin, x => x.Creator, x => x.Updater);
            return menuDtos;
        }

        /// <summary>
        /// 根据自定义条件获取列表后的处理逻辑
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="result">查询结果</param>
        protected override async Task<object> AfterGetListByCondition(QueryByConditionDto query, object result)
        {
            // 调用基类方法确保基础逻辑执行
            await base.AfterGetListByCondition(query, result);

            // 构建树形结构
            var menuTree = await _menuService.BuildMenuTree(_mapper.Map<List<FdMenu>>(result), null);

            // 转换为 DTO
            var menuDtos = _mapper.Map<List<FdMenuDto>>(menuTree);
            await _userRefFiller.FillNamesAsync(menuDtos, SystemCategory.Admin, x => x.Creator, x => x.Updater);
            return menuDtos;
        }

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<FdMenuDto> GetById(string id, CancellationToken cancellationToken = default) => base.GetById(id);

        [Authorize(Policy = Permissions.Admin.Menus.View)]
        public override Task<PageResult<FdMenuDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) => base.GetPage(pageIndex, pageSize);

        //[Authorize(Policy = Permissions.Admin.Menus.Create)]
        //public override Task<FdMenuDto> Create(CreateFdMenuDto dto) => base.Create(dto);

        [Authorize(Policy = Permissions.Admin.Menus.Edit)]
        public override Task<FdMenuDto> Update(string id, UpdateFdMenuDto dto) => base.Update(id, dto);

        [Authorize(Policy = Permissions.Admin.Menus.Delete)]
        public override Task<bool> Delete(string id) => base.Delete(id);

        protected override async Task BeforeCreate(FdMenu entity, CreateFdMenuDto dto)
        {
            var generatedCode = $"MENU_CODE_{SnowflakeIdGenerator.NextStrId()}";
            entity.Code = generatedCode;
            //entity.ParentCode = entity.ParentCode == 0 ? null : entity.ParentCode;
            if (string.IsNullOrEmpty(entity.ParentCode))
            {
                entity.ParentCode = null;
            }
            await base.BeforeCreate(entity, dto);
        }

        protected override async Task BeforeUpdate(FdMenu existingEntity, FdMenu updatedEntity, UpdateFdMenuDto dto)
        {
            // Code字段由系统自动生成，不允许修改
            updatedEntity.Code = existingEntity.Code;
            await base.BeforeUpdate(existingEntity, updatedEntity, dto);
        }

        [HttpGet("menu-btns")]
        //[Authorize(Policy = Permissions.Admin.Menus.View)]
        public async Task<List<MenuBtnRe>> GetMenuBtn(int Belong, string RoleId, CancellationToken cancellationToken = default)
        {
            SystemCategory systemCategory = (SystemCategory)Belong;
            
            // 获取所有符合条件的菜单
            var menus = await _service.GetListAsync(m => m.Belong == systemCategory, cancellationToken);
            
            // 如果没有找到菜单数据，直接返回空列表
            if (menus == null || !menus.Any())
            {
                return new List<MenuBtnRe>();
            }
            
            var menuCodes = menus.Select(m => m.Code).ToList();
            
            // 获取菜单下的按钮
            var menuBtns = await _MenuBtnRepository.GetListAsync(m => menuCodes.Contains(m.MenuCode), cancellationToken);
            
            // 如果角色Id不为空则说明是在已存在的角色上要进行修改，应返回角色已有的菜单和菜单 Exist 字段
            var roleExistMenuIds = new List<string>();
            var roleExistMenuBtnIds = new List<string>();
            
            if (!string.IsNullOrEmpty(RoleId))
            {
                var roleExistMenu = await _roleMenuRepository.GetListAsync(x => x.RoleId == RoleId);//对象里有 MenuId
                var roleExistMenuBtn = await _roleMenuBtnRepository.GetListAsync(x => x.RoleId == RoleId);//对象里有 MenuButtonId
                
                roleExistMenuIds = roleExistMenu.Select(m => m.MenuId).ToList();
                roleExistMenuBtnIds = roleExistMenuBtn.Select(mb => mb.MenuButtonId).ToList();
            }
            
            // 使用 _menuService.BuildMenuTree 构建菜单树形结构
            var menuTree = await _menuService.BuildMenuTree(menus, null);
            
            // 将菜单树形结构转换为 MenuBtnRe 树形结构
            var result = ConvertFdMenuTreeToMenuBtnReTree(menuTree, menuBtns, roleExistMenuIds, roleExistMenuBtnIds);
            
            return result;
        }
        
        private List<MenuBtnRe> ConvertFdMenuTreeToMenuBtnReTree(List<FdMenuDto> menuTree, List<FdMenuButton> menuBtns, List<string> roleExistMenuIds, List<string> roleExistMenuBtnIds)
        {
            var result = new List<MenuBtnRe>();
            
            foreach (var menu in menuTree)
            {
                // 获取当前菜单的按钮
                var menuButtons = menuBtns.Where(mb => mb.MenuCode == menu.Code).ToList();
                
                // 创建按钮列表的 MenuBtnReStatusDto 集合
                var btnList = menuButtons.Select(mb => new MenuBtnReStatusDto(
                    Id: mb.Id,
                    Name: mb.Name,
                    DataStatus: DataStatus.NoChange,
                    Exist: roleExistMenuBtnIds.Contains(mb.Id)  // 根据角色权限设置 Exist 字段
                )).ToList();
                
                var menuBtnRe = new MenuBtnRe
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Title = menu.Title,
                    BtnList = btnList,
                    Exist = roleExistMenuIds.Contains(menu.Id), // 根据角色权限设置 Exist 字段
                    Children = menu.Children != null ? ConvertFdMenuTreeToMenuBtnReTree(menu.Children, menuBtns, roleExistMenuIds, roleExistMenuBtnIds) : new List<MenuBtnRe>()
                };
                
                result.Add(menuBtnRe);
            }
            
            return result;
        }
    }
}