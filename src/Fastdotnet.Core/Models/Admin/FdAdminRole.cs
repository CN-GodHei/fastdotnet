using System;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class FdAdminRole : BaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否超级管理员角色
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 角色状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 数据范围（1：全部数据，2：本部门及以下数据，3：本部门数据，4：仅本人数据，5：自定义数据）
        /// </summary>
        public int DataScope { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 角色类型（1：系统角色，2：业务角色）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 部门ID列表，使用JSON数组存储
        /// </summary>
        public string DepartmentIds { get; set; }
    }
}