namespace __PLUGIN_NAME__;

/// <summary>
/// 权限码常量。统一小写点分、带插件前缀，避免与宿主/其他插件冲突。
/// </summary>
public static class __PLUGIN_NAME__Permissions
{
    public const string ThingView = "__PLUGIN_SLUG__.thing.view";
    public const string ThingCreate = "__PLUGIN_SLUG__.thing.create";
    public const string ThingDelete = "__PLUGIN_SLUG__.thing.delete";
}
