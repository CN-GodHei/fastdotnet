
namespace Fastdotnet.Core.Dtos.Admin.Users
{
    /// <summary>
    /// 用户权限信息DTO，包含用户基本信息、角色和按钮权限
    /// </summary>
    public class UserPermissionsDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string? LastLoginIp { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Avatar { get; set; }
        
        /// <summary>
        /// 用户的角色ID列表
        /// </summary>
        public List<string> RoleIds { get; set; } = new List<string>();
        
        /// <summary>
        /// 用户的按钮权限列表
        /// </summary>
        public List<string> Buttons { get; set; } = new List<string>();
    }
}