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

            if (await _systemConfigRepository.ExistsAsync(a => a.Id != null))
            {
                //_logger.LogInformation("System config already seeded. Skipping initialization.");
                return;
            }


            var configs = new List<FdMenu>
            {
            new FdMenu {Title="Plugin A", Name = "", Code = "MENU_CODE_11375905679934469", Path = "/micro/11375910391972869", Icon = "ele-Monitor", Sort = 100, Type = 0, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "layout/routerView/parent.vue", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = false ,ParentCode=null},
            new FdMenu {Title="Plugin A Home", Name = "", Code = "MENU_CODE_11375910391972869", Path = "/micro/11375910391972869/home", Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            new FdMenu {Title="Plugin A About", Name = "", Code = "MENU_CODE_11375913497723909", Path = "/micro/11375910391972869/about", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            new FdMenu {Title="SignalRDemo", Name = "", Code = "MENU_CODE_11375913497723910", Path = "/micro/11375910391972869/signalr-demo", Icon = "ele-InfoFilled", Sort = 2, Type = MenuType.Menu, Module = "plugin-a", Category = "Admin", IsExternal = false, ExternalUrl = "", IsEnabled = true, PermissionCode = "", Component = "", IsHide = false, IsKeepAlive = true, IsAffix = false, IsIframe = false, IsFdMicroApp = true ,ParentCode="MENU_CODE_11375905679934469",PluginId="11375910391972869"},
            };
            await _systemConfigRepository.InsertRangeAsync(configs);

            //_logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}
