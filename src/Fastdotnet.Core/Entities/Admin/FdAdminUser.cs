using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.Admin
{
    /// <summary>
    /// 后台管理员表
    /// </summary>
    [SugarTable("FdAdminUser")]
    [SugarIndex("idx_admin_username", nameof(Username), OrderByType.Asc, IsUnique = true)]
    public class FdAdminUser : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 登录用户名 (必须唯一)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// 哈希后的密码
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Password { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string FullName { get; set; }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Phone { get; set; }

        /// <summary>
        /// 账户是否激活
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string? LastLoginIp { get; set; }
    }
}