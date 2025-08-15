using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 菜单按钮表
    /// </summary>
    [SugarTable("FdMenuButton")]
    public class FdMenuButton : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 按钮名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 按钮代码（用于前端标识）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Code { get; set; }

        /// <summary>
        /// 按钮描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 关联的菜单ID
        /// </summary>
        public long MenuId { get; set; }

        /// <summary>
        /// 所属模块 (System/PluginId)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Module { get; set; }

        /// <summary>
        /// 按钮分类: Admin/App
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Category { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 关联的权限代码 (关联FdPermission)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PermissionCode { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}