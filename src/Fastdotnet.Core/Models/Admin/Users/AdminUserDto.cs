using System;

namespace Fastdotnet.Core.Models.Admin.Users
{
    /// <summary>
    /// 用于显示管理员信息的DTO
    /// </summary>
    public class AdminUserDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string? LastLoginIp { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
