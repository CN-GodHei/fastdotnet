using System;

namespace Fastdotnet.Core.Models.User
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class FdUser : BaseEntity
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
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

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
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

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
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string WechatOpenId { get; set; }

        /// <summary>
        /// 微信UnionId
        /// </summary>
        public string WechatUnionId { get; set; }

        /// <summary>
        /// QQ OpenId
        /// </summary>
        public string QqOpenId { get; set; }

        /// <summary>
        /// 支付宝UserId
        /// </summary>
        public string AlipayUserId { get; set; }

        /// <summary>
        /// 注册来源（1：PC，2：Android，3：iOS，4：H5，5：小程序）
        /// </summary>
        public int RegisterSource { get; set; }

        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegisterIp { get; set; }

        /// <summary>
        /// 邀请人ID
        /// </summary>
        public long? InviterId { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 用户标签（JSON数组）
        /// </summary>
        public string Tags { get; set; }
    }
}