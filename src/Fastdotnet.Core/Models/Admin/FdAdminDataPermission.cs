using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 数据权限表
    /// </summary>
    public class FdAdminDataPermission : BaseEntity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 数据范围类型（1：全部数据，2：本部门及以下数据，3：本部门数据，4：仅本人数据，5：自定义数据）
        /// </summary>
        public int DataScopeType { get; set; }

        /// <summary>
        /// 自定义数据范围（部门ID列表，使用JSON数组存储）
        /// </summary>
        public string CustomScope { get; set; }

        /// <summary>
        /// 状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 规则配置（JSON格式，用于存储特定的数据权限规则）
        /// </summary>
        public string RuleConfig { get; set; }
    }
}