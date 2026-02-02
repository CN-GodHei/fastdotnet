
namespace Fastdotnet.Service.Initializers
{
    public class MenuButtonsInitializer : IStartupTask
    //: IApplicationInitializer
    {
        private readonly IRepository<FdMenuButton> _MenuButtonrepository;
        private readonly IRepository<FdMenu> _Menurepository;

        public MenuButtonsInitializer(IRepository<FdMenuButton> MenuButtonrepository
            , IRepository<FdMenu> Menurepository)
        {
            _MenuButtonrepository = MenuButtonrepository;
            _Menurepository = Menurepository;
        }
        public async Task ExecuteAsync()
        {
            // 查询所有菜单
            var menus = await _Menurepository.GetListAsync(x => x.Type == MenuType.Menu);

            // 为每个菜单生成默认的按钮按钮
            foreach (var menu in menus)
            {
                // 为每个菜单创建标准的CRUD按钮按钮
                var defaultButtons = new List<FdMenuButton>
                {
                    new FdMenuButton
                    {
                        Name = "查询模块",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_queryModule", // 使用菜单Code或ID作为前缀
                        Description = $"{menu.Name} - 查询模块",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 1,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_queryModule"
                    },
                    new FdMenuButton
                    {
                        Name = "详情",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_detail", // 使用菜单Code或ID作为前缀
                        Description = $"{menu.Name} - 详情",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 1,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_detail"
                    },
                    new FdMenuButton
                    {
                        Name = "查看",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_view", // 使用菜单Code或ID作为前缀
                        Description = $"{menu.Name} - 查看按钮",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 1,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_view"
                    },
                    new FdMenuButton
                    {
                        Name = "新增",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_add",
                        Description = $"{menu.Name} - 新增按钮",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 2,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_add"
                    },
                    new FdMenuButton
                    {
                        Name = "编辑",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_edit",
                        Description = $"{menu.Name} - 编辑按钮",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 3,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_edit"
                    },
                    new FdMenuButton
                    {
                        Name = "删除",
                        Code = $"{menu.Code?.ToLower() ?? menu.Id}_delete",
                        Description = $"{menu.Name} - 删除按钮",
                        MenuCode = menu.Code,
                        Module = menu.Module ?? "System",
                        Belong = menu.Belong ,
                        Sort = 4,
                        IsEnabled = true,
                        PermissionCode = $"{menu.Code?.ToLower() ?? menu.Id}_delete"
                    }
                };

                // 检查是否已存在这些默认按钮，避免重复创建
                foreach (var button in defaultButtons)
                {
                    var existingButton = await _MenuButtonrepository.GetFirstAsync(b => b.Code == button.Code && b.MenuCode == button.MenuCode);
                    if (existingButton == null)
                    {
                        await _MenuButtonrepository.InsertAsync(button);
                    }
                }
            }
        }
    }
}
