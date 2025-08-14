using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 前台应用用户表
    /// </summary>
    [SugarTable("FdAppUser")]
    [SugarIndex("idx_app_username", nameof(Username), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("idx_app_email", nameof(Email), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("idx_app_phone", nameof(PhoneNumber), OrderByType.Asc, IsUnique = true)]
    public class FdAppUser : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 登录用户名 (可选, 可能使用手机或邮箱登录)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Username { get; set; }

        /// <summary>
        /// 哈希后的密码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Password { get; set; }

        /// <summary>
        /// 邮箱 (可用于登录, 必须唯一)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Email { get; set; }

        /// <summary>
        /// 手机号 (可用于登录, 必须唯一)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Nickname { get; set; }

        /// <summary>
        /// 用户头像URL
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 账户状态 (0:正常, 1:待验证, 2:已禁用)
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? LastLoginTime { get; set; }
    }
}