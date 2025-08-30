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
        [SugarColumn(IsNullable = false, Length = 150)]
        public string Type { get; set; }

        /// <summary>
        /// 黑名单值 (具体的IP地址、用户ID或API密钥)
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 255)]
        public string Value { get; set; }

        /// <summary>
        /// 加入黑名单的原因
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string Reason { get; set; }

        /// <summary>
        /// 过期时间 (可选)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}