using Fastdotnet.Core;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Initializers
{
    public class FdMenuConfigInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdMenu> _systemConfigRepository;
        private readonly ILogger<FdMenuConfigInitializer> _logger;

        public FdMenuConfigInitializer(IRepository<FdMenu> systemConfigRepository, ILogger<FdMenuConfigInitializer> logger)
        {
            _systemConfigRepository = systemConfigRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Start: Initializing System Config...");

            if (await _systemConfigRepository.ExistsAsync(a => a.Id != null))
            {
                _logger.LogInformation("System config already seeded. Skipping initialization.");
                return;
            }


            var configs = new List<FdMenu>
{
    new FdMenu { Name = "控制台", Code = "MENU_CODE_11365281228129285", Path = "/dashboard", Icon = "ele-HomeFilled", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu { Name = "SignalR示例", Code = "MENU_CODE_11365281228129284", Path = "system/signalr-example.vue", Icon = "iconfont icon-shouye_dongtaihui", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu { Name = "插件管理", Code = "MENU_CODE_11365281228129286", Path = "/system/plugin", Icon = "iconfont icon-crew_feature", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/plugin/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },

    new FdMenu { Name = "系统管理", Code = "MENU_CODE_11365290021618693", Path = "/system", Icon = "iconfont icon-xitongshezhi", Sort = 0, Type = 0, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},

    new FdMenu { Name = "菜单管理", Code = "MENU_CODE_11365291745215493", Path = "/system/menu", Icon = "iconfont icon-bolangneng", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/menu/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu { Name = "信息配置", Code = "MENU_CODE_11365291745215423", Path = "/system/systeminfoconfig", Icon = "iconfont icon-xitongshezhi", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/systeminfoconfig/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},

    new FdMenu { Name = "Plugin A", Code = "MENU_CODE_11375905679934469", Path = "/micro/11375910391972869", Icon = "ele-Monitor", Sort = 100, Type = 0, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "layout/routerView/parent.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},

    new FdMenu { Name = "Plugin A Home", Code = "MENU_CODE_11375910391972869", Path = "/micro/11375910391972869/home", Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},

    new FdMenu { Name = "Plugin A About", Code = "MENU_CODE_11375913497723909", Path = "/micro/11375910391972869/about", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
    new FdMenu { Name = "SignalRDemo", Code = "MENU_CODE_11375913497723910", Path = "/micro/11375910391972869/signalr-demo", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},

    new FdMenu { Name = "Plugin B (Test)", Code = "MENU_CODE_PLUGIN_B_TEST", Path = "/micro/plugin-b", Icon = "ele-Cpu", Sort = 101, Type = MenuType.Menu, Module = "plugin-b", Category = "Admin", IsEnabled = false, IsFdMicroApp = true, ParentCode=null },
    new FdMenu { Name = "黑名单管理", Code = "MENU_CODE_11365291745215493", Path = "/system/blacklist", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/blacklist/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu { Name = "限流管理", Code = "MENU_CODE_11365291745215493", Path = "/system/ratelimit", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/ratelimit/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu { Name = "插件商城", Code = "MENU_CODE_11365281228129283", Path = "/micro/11365281228129286", Icon = "iconfont icon-shuju", Sort = 0, Type = MenuType.Menu, Module = "11365281228129286", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu { Name = "插件列表", Code = "MENU_CODE_11375910391972868", Path = "/micro/11365281228129286/admin/marketplace-plugins", Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu, Module = "11365281228129286", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11365281228129283",PluginId="11365281228129286"},
    new FdMenu { Name = "开发相关", Code = "MENU_CODE_11365290021618692", Path = "/dev", Icon = "iconfont icon-zhongduancanshu", Sort = 0, Type = 0, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},
    new FdMenu { Name = "前端模板", Code = "MENU_CODE_11365291745215491", Path = "/dev/fdtemplate", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/dev/fdtemplate/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618692"},
    new FdMenu { Name = "生成测试", Code = "MENU_CODE_11365291745215494", Path = "/dev/fdtemplatetest", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/dev/fdtemplate/test.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618692"},
};
            await _systemConfigRepository.InsertRangeAsync(configs);

            _logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}