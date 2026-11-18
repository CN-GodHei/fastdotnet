namespace __PLUGIN_NAME__.Initializers;

/// <summary>
/// 插件启用时由宿主执行（无需手动调用），用于注册菜单。
/// 幂等：按 Code 查重，只插入缺失项。
/// </summary>
public class __PLUGIN_NAME__MenuInitializer : IApplicationInitializer
{
    private readonly IRepository<FdMenu> _menus;

    public __PLUGIN_NAME__MenuInitializer(IRepository<FdMenu> menus) => _menus = menus;

    /// <summary>2001–3000 为插件级初始化。</summary>
    public int Order => 2100;

    public async Task InitializeAsync()
    {
        var pluginId = PluginContext.GetCurrentPluginInfo()?.id ?? "__PLUGIN_ID__";
        const string parentCode = "MENU_CODE___PLUGIN_SLUG___ROOT";

        var rows = new List<FdMenu>
        {
            new()
            {
                Title = "__DISPLAY_NAME__",
                Name = string.Empty,
                Code = parentCode,
                Path = $"/micro/{pluginId}",
                Icon = "ele-Monitor",
                Sort = 100,
                Type = MenuType.Directory,
                Module = pluginId,
                Belong = SystemCategory.Admin,
                Component = "layout/routerView/parent.vue",
                IsEnabled = true,
                IsFdMicroApp = false
            },
            new()
            {
                Title = "首页",
                Name = string.Empty,
                Code = "MENU_CODE___PLUGIN_SLUG___HOME",
                Path = $"/micro/{pluginId}/home",
                ParentCode = parentCode,
                Icon = "ele-HomeFilled",
                Sort = 1,
                Type = MenuType.Menu,
                Module = pluginId,
                Belong = SystemCategory.Admin,
                Component = string.Empty,
                IsEnabled = true,
                IsFdMicroApp = true,
                IsKeepAlive = true,
                PluginId = pluginId
            }
        };

        var codes = rows.Select(r => r.Code).ToList();
        var existing = (await _menus.GetListAsync(m => codes.Contains(m.Code)))
            .Select(m => m.Code)
            .ToHashSet();

        var toInsert = rows.Where(r => !existing.Contains(r.Code)).ToList();
        if (toInsert.Count > 0)
        {
            await _menus.InsertRangeAsync(toInsert);
        }
    }
}
