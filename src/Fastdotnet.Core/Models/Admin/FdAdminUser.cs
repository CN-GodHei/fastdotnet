using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 管理员表
    /// </summary>
    [SugarTable("fd_admin_user")]
    public class FdAdminUser : BaseEntity
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
        /// 密码（加密存储）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 性别（0：未知，1：男，2：女）
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public long DepartmentId { get; set; }

        /// <summary>
        /// 职位/岗位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 账号状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 登录失败次数
        /// </summary>
        public int LoginFailCount { get; set; }

        /// <summary>
        /// 锁定结束时间
        /// </summary>
        public DateTime? LockEndTime { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string WechatOpenId { get; set; }

        /// <summary>
        /// 微信UnionId
        /// </summary>
        public string WechatUnionId { get; set; }

        /// <summary>
        /// 钉钉OpenId
        /// </summary>
        public string DingtalkOpenId { get; set; }

        /// <summary>
        /// 企业微信OpenId
        /// </summary>
        public string WorkWechatOpenId { get; set; }
    }
}