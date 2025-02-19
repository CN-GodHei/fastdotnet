using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 菜单和按钮信息表
    /// </summary>
    public class FdAdminMenu : BaseEntity
    {
        /// <summary>
        /// 父级ID，顶级菜单为0
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单编码，用于前端路由
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 菜单类型（1：目录，2：菜单，3：按钮）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// 打开方式（1：页面内，2：新窗口）
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 是否可见（0：隐藏，1：显示）
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否外链（0：否，1：是）
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// 是否缓存（0：否，1：是）
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否固定在标签栏（0：否，1：是）
        /// </summary>
        public bool IsAffix { get; set; }

        /// <summary>
        /// 菜单层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 菜单树路径，格式：1,2,3
        /// </summary>
        public string TreePath { get; set; }
    }
}