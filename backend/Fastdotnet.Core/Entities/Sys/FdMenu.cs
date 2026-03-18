namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 菜单表
    /// </summary>
    [SugarTable("fd_menu", "菜单")]
    [SugarIndex("index_fd_menu_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    public class FdMenu 
        : AuditableEntity
        //: BaseEntity, ISoftDelete
    {
        [SugarColumn(ColumnName = "title", IsNullable = false, ColumnDescription = "菜单标题")]
        public string Title { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "菜单名称(和前端组件名称对应)")]
        public string Name { get; set; }

        /// <summary>
        /// 菜单代码
        /// </summary>
        [SugarColumn(ColumnName = "code", IsNullable = true, ColumnDescription = "菜单代码")]
        public string Code { get; set; }

        /// <summary>
        /// 菜单路径（用于前端路由）
        /// </summary>
        [SugarColumn(ColumnName = "path", IsNullable = true, ColumnDescription = "菜单路径（用于前端路由）")]
        public string Path { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(ColumnName = "icon", IsNullable = true, ColumnDescription = "菜单图标")]
        public string Icon { get; set; }

        /// <summary>
        /// 父级菜单Code
        /// </summary>
        [SugarColumn(ColumnName = "parent_code", IsNullable = true, ColumnDescription = "父级菜单Code")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "sort", ColumnDescription = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 菜单类型 (目录/菜单)
        /// </summary>
        [SugarColumn(ColumnName = "type", ColumnDescription = "菜单类型 (目录/菜单)")]
        public MenuType Type { get; set; }

        /// <summary>
        /// 所属模块 (System/PluginId)
        /// </summary>
        [SugarColumn(ColumnName = "module", IsNullable = true, ColumnDescription = "所属模块 (System/PluginId)")]
        public string Module { get; set; }

        /// <summary>
        /// 菜单分类: Admin/App
        /// </summary>
        [SugarColumn(ColumnName = "belong", IsNullable = false, ColumnDescription = "属于管理端还是应用端")]
        public SystemCategory Belong { get; set; }

        /// <summary>
        /// 是否外链
        /// </summary>
        [SugarColumn(ColumnName = "is_external", ColumnDescription = "是否外链")]
        public bool IsExternal { get; set; }

        /// <summary>
        /// 外链地址
        /// </summary>
        [SugarColumn(ColumnName = "external_url", IsNullable = true, ColumnDescription = "外链地址")]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(ColumnName = "is_enabled", ColumnDescription = "是否启用")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 关联的权限代码 (关联FdPermission)
        /// </summary>
        [SugarColumn(ColumnName = "permission_code", IsNullable = true, ColumnDescription = "关联的权限代码 (关联FdPermission)")]
        public string PermissionCode { get; set; }

        /// <summary>
        /// Vue组件路径 (例如: "home/index.vue")
        /// </summary>
        [SugarColumn(ColumnName = "component", IsNullable = true, ColumnDescription = "Vue组件路径 (例如: \"home/index.vue\")")]
        public string? Component { get; set; }

        /// <summary>
        /// 是否在导航中隐藏 (对应 vue-next-admin 的 meta.isHide)
        /// </summary>
        [SugarColumn(ColumnName = "is_hide", ColumnDescription = "是否在导航中隐藏 (对应 vue-next-admin 的 meta.isHide)")]
        public bool IsHide { get; set; } = false;

        /// <summary>
        /// 是否缓存页面 (对应 vue-next-admin 的 meta.isKeepAlive)
        /// </summary>
        [SugarColumn(ColumnName = "is_keep_alive", ColumnDescription = "是否缓存页面 (对应 vue-next-admin 的 meta.isKeepAlive)")]
        public bool IsKeepAlive { get; set; } = true;

        /// <summary>
        /// 是否固定标签页 (对应 vue-next-admin 的 meta.isAffix)
        /// </summary>
        [SugarColumn(ColumnName = "is_affix", ColumnDescription = "是否固定标签页 (对应 vue-next-admin 的 meta.isAffix)")]
        public bool IsAffix { get; set; } = false;

        /// <summary>
        /// 是否以内嵌 iframe 打开 (对应 vue-next-admin 的 meta.isIframe)
        /// </summary>
        [SugarColumn(ColumnName = "is_iframe", ColumnDescription = "是否以内嵌 iframe 打开 (对应 vue-next-admin 的 meta.isIframe)")]
        public bool IsIframe { get; set; } = false;
        
        [SugarColumn(ColumnName = "is_link", ColumnDescription = "是否为链接")]
        public bool isLink { get; set; } = false;
        public bool SupportWeb { get; set; } = false;
        public bool SupportDesktop { get; set; } = false;
        public bool SupportMobile { get; set; } = false;

        /// <summary>
        /// 子菜单
        /// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public List<FdMenu> Children { get; set; }
        [SugarColumn(ColumnName = "is_fd_micro_app", ColumnDescription = "是否为微应用")]
        public bool IsFdMicroApp { get; set; }

        [SugarColumn(ColumnName = "plugin_id", IsNullable = true, ColumnDescription = "插件Id")]
        public string PluginId { get; set; }

        /// <summary>
        /// 属于
        /// </summary>
        //[SugarColumn(ColumnName = "belong", IsNullable = false, ColumnDescription = "属于管理端还是应用端")]
        //public SystemCategory Belong { get; set; }
    }
}