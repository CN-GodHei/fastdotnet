using Microsoft.AspNetCore.Authorization;

namespace Fastdotnet.WebApi.Middleware.Authorization
{
    public class ApiScopeRequirement : IAuthorizationRequirement
    {
        public ApiScopeRequirement()
        {
        }
    }
}