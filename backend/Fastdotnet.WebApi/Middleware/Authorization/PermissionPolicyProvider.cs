using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware.Authorization
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // 检查是否是API作用域策略
            if (policyName.StartsWith("ApiScope"))
            {
                var apiScopePolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ApiScopeRequirement())
                    .Build();
                return await Task.FromResult(apiScopePolicy);
            }

            var policy = await base.GetPolicyAsync(policyName);
            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }
            return policy;
        }
    }
}