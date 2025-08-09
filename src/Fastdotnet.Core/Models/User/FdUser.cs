using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.User
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [SugarTable("fd_users")]
    public class FdUser : BaseEntity, Interfaces.ISoftDelete
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        
        // ISoftDelete接口实现已经在BaseEntity中实现了
    }
}