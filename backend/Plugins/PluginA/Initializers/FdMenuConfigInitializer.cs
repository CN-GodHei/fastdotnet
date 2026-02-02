using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Initializers
{
    public class FdMenuConfigInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdMenu> _systemConfigRepository;
        //private readonly ILogger<FdMenuConfigInitializer> _logger;

        public FdMenuConfigInitializer(IRepository<FdMenu> systemConfigRepository, ILogger<FdMenuConfigInitializer> logger)
        {
            _systemConfigRepository = systemConfigRepository;
            //_logger = logger;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Start: Initializing System Config...");

            var configs = new List<FdMenu>
            {
            new FdMenu {Title="Plugin A", Name = "", Code = "MENU_CODE_11375905679934469", Path = "/micro/11375910391972869", Icon = "ele-Monitor", Sort = 100, Type = 0, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "layout/routerView/parent.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},
            new FdMenu {Title="Plugin A Home", Name = "", Code = "MENU_CODE_11375910391972869", Path = "/micro/11375910391972869/home", Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            new FdMenu {Title="Plugin A About", Name = "", Code = "MENU_CODE_11375913497723909", Path = "/micro/11375910391972869/about", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            new FdMenu {Title="SignalRDemo", Name = "", Code = "MENU_CODE_11375913497723910", Path = "/micro/11375910391972869/signalr-demo", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            new FdMenu {Title="富文本编辑器演示", Name = "", Code = "MENU_CODE_11375913497723911", Path = "/micro/11375910391972869/rich-text-demo", Icon = "ele-Edit", Sort = 3, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            //new FdMenu {Title="富文本插件演示", Name = "", Code = "MENU_CODE_11375913497723912", Path = "/micro/11375910391972869/rich-text-plugin-demo", Icon = "ele-Document", Sort = 4, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            //new FdMenu {Title="表单集成富文本", Name = "", Code = "MENU_CODE_11375913497723913", Path = "/micro/11375910391972869/form-with-richtext", Icon = "ele-List", Sort = 5, Type = MenuType.Menu, Module = "plugin-a", Belong = SystemCategory.Admin, IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            };
            
            // 直接使用条件查询已存在的菜单项
            var existingMenus = await _systemConfigRepository.GetListAsync(m => configs.Select(c => c.Code).Contains(m.Code));
            var existingCodes = existingMenus.Select(m => m.Code).ToHashSet();
            
            // 只插入不存在的菜单项
            var menusToInsert = configs.Where(c => !existingCodes.Contains(c.Code)).ToList();
            
            if (menusToInsert.Any())
            {
                await _systemConfigRepository.InsertRangeAsync(menusToInsert);
            }

            //_logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}