using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 前台应用用户表
    /// </summary>
    [SugarTable("fd_app_user", "应用用户")]
    [SugarIndex("idx_app_username", nameof(Username), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("idx_app_email", nameof(Email), OrderByType.Asc, IsUnique = true)]
    [SugarIndex("idx_app_phone", nameof(PhoneNumber), OrderByType.Asc, IsUnique = true)]
    public class FdAppUser : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 登录用户名 (可选, 可能使用手机或邮箱登录)
        /// </summary>
        [SugarColumn(ColumnName = "username", IsNullable = true, ColumnDescription = "登录用户名 (可选, 可能使用手机或邮箱登录)")]
        public string? Username { get; set; }

        /// <summary>
        /// 哈希后的密码
        /// </summary>
        [SugarColumn(ColumnName = "password", IsNullable = true, ColumnDescription = "哈希后的密码")]
        public string? Password { get; set; }

        /// <summary>
        /// 邮箱 (可用于登录, 必须唯一)
        /// </summary>
        [SugarColumn(ColumnName = "email", IsNullable = true, ColumnDescription = "邮箱 (可用于登录, 必须唯一)")]
        public string? Email { get; set; }

        /// <summary>
        /// 手机号 (可用于登录, 必须唯一)
        /// </summary>
        [SugarColumn(ColumnName = "phone_number", IsNullable = true, ColumnDescription = "手机号 (可用于登录, 必须唯一)")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [SugarColumn(ColumnName = "nickname", IsNullable = false, ColumnDescription = "用户昵称")]
        public string Nickname { get; set; }

        /// <summary>
        /// 用户头像URL
        /// </summary>
        [SugarColumn(ColumnName = "avatar_url", IsNullable = true, ColumnDescription = "用户头像URL")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 账户状态 (0:正常, 1:待验证, 2:已禁用)
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDescription = "账户状态 (0:正常, 1:待验证, 2:已禁用)")]
        public int Status { get; set; } = 0;

        /// <summary>
        /// 注册时间
        /// </summary>
        [SugarColumn(ColumnName = "registration_date", ColumnDescription = "注册时间")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(ColumnName = "last_login_time", IsNullable = true, ColumnDescription = "最后登录时间")]
        public DateTime? LastLoginTime { get; set; }
    }
}