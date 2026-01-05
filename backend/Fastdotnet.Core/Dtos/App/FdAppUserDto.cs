using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Dtos.App
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdAppUserDto
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(255, ErrorMessage = "用户名最多255个字符")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(255, ErrorMessage = "用户名最多255个字符")]
        public string? Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "邮箱不能为空")]
        [StringLength(255, ErrorMessage = "邮箱最多255个字符")]
        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号不能为空")]
        [StringLength(255, ErrorMessage = "手机号最多255个字符")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(255, ErrorMessage = "昵称最多255个字符")]
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Required(ErrorMessage = "头像不能为空")]
        [StringLength(255, ErrorMessage = "头像最多255个字符")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        //public DateTime RegistrationDate { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdAppUserDto
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(255, ErrorMessage = "用户名最多255个字符")]
        public string? Username { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(255, ErrorMessage = "邮箱最多255个字符")]
        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(255, ErrorMessage = "手机号最多255个字符")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(255, ErrorMessage = "昵称最多255个字符")]
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(255, ErrorMessage = "头像最多255个字符")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        //public DateTime RegistrationDate { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdAppUserDto
    {

        /// <summary>
        /// 用户名
        /// </summary>

        public string? Username { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>

        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>

        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>

        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>

        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        //public DateTime RegistrationDate { get; set; }
    }
}
