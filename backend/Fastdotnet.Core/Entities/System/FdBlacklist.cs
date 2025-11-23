using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 黑名单表
    /// </summary>
    [SugarTable("fd_blacklist", "黑名单表")]
    [SugarIndex("idx_blacklist_type_value", nameof(Type), OrderByType.Asc, nameof(Value), OrderByType.Asc, true)]
    public class FdBlacklist : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 黑名单类型 (IP, User, ApiKey)
        /// </summary>
        [SugarColumn(ColumnName = "type", IsNullable = false, Length = 50, ColumnDescription = "黑名单类型 (IP, User, ApiKey)")]
        public string Type { get; set; }

        /// <summary>
        /// 黑名单值 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [SugarColumn(ColumnName = "value", IsNullable = false, Length = 255, ColumnDescription = "黑名单值 (具体的IP地址、用户ID或API密钥)")]
        public string Value { get; set; }

        /// <summary>
        /// 加入黑名单的原因
        /// </summary>
        [SugarColumn(ColumnName = "reason", IsNullable = true, Length = 255, ColumnDescription = "加入黑名单的原因")]
        public string Reason { get; set; }

        /// <summary>
        /// 过期时间 (可选)
        /// </summary>
        [SugarColumn(ColumnName = "expired_at", IsNullable = true, ColumnDescription = "过期时间 (可选)")]
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        [SugarColumn(ColumnName = "is_system", ColumnDescription = "是否为系统内置")]
        public bool IsSystem { get; set; } = false;
    }
}