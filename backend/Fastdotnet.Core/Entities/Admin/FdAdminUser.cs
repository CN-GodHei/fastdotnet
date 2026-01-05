using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.Dtos.Interfaces;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.Admin
{
    /// <summary>
    /// 后台管理员表
    /// </summary>
    [SugarTable("fd_admin_user", "后台管理员")]
    [SugarIndex("idx_admin_username", nameof(Username), OrderByType.Asc, IsUnique = true)]
    public class FdAdminUser : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 登录用户名 (必须唯一)
        /// </summary>
        [SugarColumn(ColumnName = "username", IsNullable = false, ColumnDescription = "登录用户名 (必须唯一)")]
        public string Username { get; set; }

        /// <summary>
        /// 哈希后的密码
        /// </summary>
        [SugarColumn(ColumnName = "password", IsNullable = false, ColumnDescription = "哈希后的密码")]
        public string Password { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = true, ColumnDescription = "真实姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        [SugarColumn(ColumnName = "email", IsNullable = true, ColumnDescription = "管理员邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "phone", IsNullable = true, ColumnDescription = "联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(ColumnName = "avatar", IsNullable = true, ColumnDescription = "头像")]
        public string Avatar { get; set; }

        /// <summary>
        /// 账户是否激活
        /// </summary>
        [SugarColumn(ColumnName = "is_active", ColumnDescription = "账户是否激活")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(ColumnName = "last_login_time", IsNullable = true, ColumnDescription = "最后登录时间")]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [SugarColumn(ColumnName = "last_login_ip", IsNullable = true, Length = 50, ColumnDescription = "最后登录IP")]
        public string? LastLoginIp { get; set; }
    }
}