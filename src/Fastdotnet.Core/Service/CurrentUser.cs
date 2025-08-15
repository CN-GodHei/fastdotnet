using System.Linq;
using System.Security.Claims;
using Fastdotnet.Core.IService;
using Microsoft.AspNetCore.Http;

namespace Fastdotnet.Core.Service
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

        public long? Id
        {
            get
            {
                var claim = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return claim != null ? long.Parse(claim.Value) : null;
            }
        }

        public string UserName
        {
            get
            {
                return User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }
        }

        public string UserType
        {
            get
            {
                // 这里的 "user_type" 需要与您在生成 Token 时使用的 Claim Key 一致
                return User?.Claims.FirstOrDefault(c => c.Type == "user_type")?.Value;
            }
        }
    }
}