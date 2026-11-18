namespace __PLUGIN_NAME__;

/// <summary>
/// 向宿主声明本插件的权限。启用时宿主会同步到 fd_permission（代码中删除的权限码会从库中删除）。
/// </summary>
public class __PLUGIN_NAME__PermissionProvider : IPermissionProvider
{
    public IEnumerable<PermissionDefinition> GetPermissions() => new List<PermissionDefinition>
    {
        new()
        {
            Module = "__PLUGIN_NAME__",
            Code = __PLUGIN_NAME__Permissions.ThingView,
            Name = "查看物品",
            Category = "Admin",
            Type = "Api"
        },
        new()
        {
            Module = "__PLUGIN_NAME__",
            Code = __PLUGIN_NAME__Permissions.ThingCreate,
            Name = "新建物品",
            Category = "Admin",
            Type = "Api"
        },
        new()
        {
            Module = "__PLUGIN_NAME__",
            Code = __PLUGIN_NAME__Permissions.ThingDelete,
            Name = "删除物品",
            Category = "Admin",
            Type = "Api"
        }
    };
}
