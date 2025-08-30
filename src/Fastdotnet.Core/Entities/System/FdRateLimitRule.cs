using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 限流规则表
    /// </summary>
    [SugarTable("FdRateLimitRule")]
    [SugarIndex("idx_ratelimit_type_key", nameof(Type), OrderByType.Asc, nameof(Key), OrderByType.Asc, true)]
    public class FdRateLimitRule : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 限流类型 (IP, User, ApiKey, Route)
        /// IP: 针对IP地址的限流
        /// User: 针对用户ID的限流
        /// ApiKey: 针对API密钥的限流
        /// Route: 针对路由的全局限流
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string Type { get; set; }

        /// <summary>
        /// 限流键 
        /// 当 Type=IP 时，Key=具体的IP地址
        /// 当 Type=User 时，Key=具体的用户ID
        /// 当 Type=ApiKey 时，Key=具体的API密钥
        /// 当 Type=Route 时，Key=具体的路由路径，如 "/api/auth/login"
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 255)]
        public string Key { get; set; }

        /// <summary>
        /// 允许的最大请求数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int PermitLimit { get; set; }

        /// <summary>
        /// 时间窗口 (以秒为单位)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int WindowSeconds { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string Description { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}