using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Entities.Sys;
using static Fastdotnet.Core.Constants.Permissions.Admin;

namespace Fastdotnet.Core.Services.Sys
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdRole> _roleRepository;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public string? Id
        {
            get
            {
                return User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
        }

        public string UserName
        {
            get
            {
                return User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            }
        }

        public string UserType
        {
            get
            {
                // 首先尝试从token中获取category
                var userType = User?.Claims.FirstOrDefault(c => c.Type == "category")?.Value;

                // 如果token中没有category，则从请求头中获取System-Belong
                if (string.IsNullOrEmpty(userType))
                {
                    userType = _httpContextAccessor.HttpContext?.Request.Headers["System-Belong"].FirstOrDefault();
                }

                // 如果都没有获取到，返回默认值"Admin"
                return string.IsNullOrEmpty(userType) ? "Admin" : userType;
            }
        }

        /// <summary>
        /// 只有管理端才用
        /// </summary>
        public bool IsSuperAdmin
        {
            get
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
                // 获取用户的角色
                var userRoles = _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId).Result;
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                if (!roleIds.Any())
                {
                    return false;
                }

                // 获取角色信息
                var roles =  _roleRepository.GetListAsync(r => roleIds.Contains(r.Id)).Result;

                // 检查是否包含超管角色
                return roles.Any(r => r.Code == SystemConstants.SuperAdminRoleCode);
            }
        }
    }
}