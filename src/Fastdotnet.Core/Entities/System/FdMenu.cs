using System.Collections.Generic;
using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 菜单表
    /// </summary>
    [SugarTable("FdMenu")]
    public class FdMenu : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单代码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Code { get; set; }

        /// <summary>
        /// 菜单路径（用于前端路由）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Path { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单类型 (目录/菜单)
        /// </summary>
        public MenuType Type { get; set; }

        /// <summary>
        /// 所属模块 (System/PluginId)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Module { get; set; }

        /// <summary>
        /// 菜单分类: Admin/App
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Category { get; set; }

        /// <summary>
        /// 是否外链
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// 外链地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 关联的权限代码 (关联FdPermission)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PermissionCode { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<FdMenu> Children { get; set; }
    }
}