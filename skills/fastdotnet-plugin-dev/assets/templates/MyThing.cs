namespace __PLUGIN_NAME__.Entities;

/// <summary>
/// 示例业务实体。表由插件启用时的 CodeFirst 自动创建。
/// </summary>
[SugarTable("__PLUGIN_SLUG___thing", "示例业务表")]
public class MyThing : AuditableEntity
{
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true)]
    public string? Remark { get; set; }
}
