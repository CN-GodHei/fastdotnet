
namespace Fastdotnet.Core.Services.System
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

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
    }
}