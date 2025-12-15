using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Enum;
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
    new FdMenu {Title="控制台", Name = "", Code = "MENU_CODE_11365281228129285", Path = "/dashboard", Icon = "ele-HomeFilled", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu {Title="SignalR示例", Name = "", Code = "MENU_CODE_11365281228129284", Path = "system/signalr-example.vue", Icon = "iconfont icon-shouye_dongtaihui", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu {Title="插件管理", Name = "systemPlugin", Code = "MENU_CODE_11365281228129286", Path = "/system/plugin", Icon = "iconfont icon-crew_feature", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/plugin/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu {Title="平台管理", Name = "", Code = "MENU_CODE_11365290021618693", Path = "/system", Icon = "iconfont icon-putong", Sort = 0, Type = 0, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},
    new FdMenu {Title="菜单管理", Name = "systemMenu", Code = "MENU_CODE_11365291745215493", Path = "/system/menu", Icon = "iconfont icon-bolangneng", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/menu/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu {Title="信息配置", Name = "FdSystemInfoConfig", Code = "MENU_CODE_11365291745215423", Path = "/system/systeminfoconfig", Icon = "iconfont icon-xitongshezhi", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/systeminfoconfig/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu {Title="Plugin B (Test)", Name = "", Code = "MENU_CODE_PLUGIN_B_TEST", Path = "/micro/plugin-b", Icon = "ele-Cpu", Sort = 101, Type = MenuType.Menu, Module = "plugin-b", Category = "Admin", IsEnabled = false, IsFdMicroApp = true, ParentCode=null },
    new FdMenu {Title="黑名单", Name = "FdBlacklist", Code = "MENU_CODE_11365291745215493", Path = "/system/blacklist", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/blacklist/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu {Title="限流管理", Name = "FdRatelimitRule", Code = "MENU_CODE_11365291745215493", Path = "/system/ratelimit", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/system/ratelimit/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618693"},
    new FdMenu {Title="插件商城", Name = "", Code = "MENU_CODE_11365281228129283", Path = "/micro/11365281228129286", Icon = "iconfont icon-shuju", Sort = 0, Type = MenuType.Menu, Module = "11365281228129286", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false,ParentCode=null },
    new FdMenu {Title="插件列表", Name = "", Code = "MENU_CODE_11375910391972868", Path = "/micro/11365281228129286/admin/marketplace-plugins", Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu, Module = "11365281228129286", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11365281228129283",PluginId="11365281228129286"},
    new FdMenu {Title="开发相关", Name = "", Code = "MENU_CODE_11365290021618692", Path = "/dev", Icon = "iconfont icon-zhongduancanshu", Sort = 0, Type = 0, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = null, IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},
    new FdMenu {Title="前端模板", Name = "sysCodeGen", Code = "MENU_CODE_11365291745215491", Path = "/dev/fdtemplate", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/dev/fdtemplate/index.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618692"},
    new FdMenu {Title="生成测试", Name = "FdAdminUser", Code = "MENU_CODE_11365291745215494", Path = "/dev/fdtemplatetest", Icon = "string", Sort = 0, Type = MenuType.Menu, Module = "string", Category = "Admin", IsExternal = false, ExternalUrl = "string", IsEnabled = true, PermissionCode = "string", Component = "/dev/fdtemplate/test.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode="MENU_CODE_11365290021618692"},
};
            await _systemConfigRepository.InsertRangeAsync(configs);

            _logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}