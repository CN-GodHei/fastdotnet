using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware.Authorization
{
    public class ApiScopeHandler : AuthorizationHandler<ApiScopeRequirement>
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ApiScopeHandler(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiScopeRequirement requirement)
        {
            // 如果用户未通过身份验证，检查是否允许匿名访问
            if (!context.User.Identity.IsAuthenticated)
            {
                // 对于匿名用户，检查是否标记了AllowAnonymous特性
                if (context.Resource is Microsoft.AspNetCore.Routing.RouteEndpoint routeEndpoint)
                {
                    var allowAnonymous = routeEndpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
                    if (allowAnonymous != null)
                    {
                        // 允许匿名访问的接口直接放行
                        context.Succeed(requirement);
                        return;
                    }
                }
                
                // 未认证且不允许匿名访问，拒绝访问
                return;
            }

            // 获取当前HTTP上下文
            if (context.Resource is Microsoft.AspNetCore.Routing.RouteEndpoint endpoint)
            {
                // 获取当前请求的控制器和动作
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    // 检查控制器或动作上是否有 ApiUsageScopeAttribute
                    var apiScopeAttr = controllerActionDescriptor.EndpointMetadata
                        .FirstOrDefault(m => m.GetType() == typeof(ApiUsageScopeAttribute)) as ApiUsageScopeAttribute;

                    // 更改默认行为：如果没有指定作用域，默认拒绝访问
                    // 这样可以避免因遗漏ApiUsageScope标记而导致的安全问题
                    if (apiScopeAttr == null)
                    {
                        // 没有显式指定访问类型，默认拒绝访问以提高安全性
                        return;
                    }

                    // 检查用户身份中的类别信息
                    var categoryClaim = context.User.FindFirst("category");
                    if (categoryClaim == null)
                    {
                        // 没有类别声明，拒绝访问
                        return;
                    }

                    // 根据用户类别确定用户类型
                    var userCategory = categoryClaim.Value;
                    var requiredScope = apiScopeAttr.Scope;

                    // 检查权限
                    if (IsAccessAllowed(userCategory, requiredScope))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            else if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext)
            {
                // 处理MVC上下文的情况
                var endpointFromMvc = mvcContext.HttpContext.GetEndpoint();
                if (endpointFromMvc != null)
                {
                    var allowAnonymous = endpointFromMvc.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
                    if (allowAnonymous != null)
                    {
                        // 允许匿名访问的接口直接放行
                        context.Succeed(requirement);
                        return;
                    }
                    
                    var controllerActionDescriptor = mvcContext.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        // 检查控制器或动作上是否有 ApiUsageScopeAttribute
                        var apiScopeAttr = controllerActionDescriptor.EndpointMetadata
                            .FirstOrDefault(m => m.GetType() == typeof(ApiUsageScopeAttribute)) as ApiUsageScopeAttribute;

                        // 更改默认行为：如果没有指定作用域，默认拒绝访问
                        // 这样可以避免因遗漏ApiUsageScope标记而导致的安全问题
                        if (apiScopeAttr == null)
                        {
                            // 没有显式指定访问类型，默认拒绝访问以提高安全性
                            return;
                        }

                        // 检查用户身份中的类别信息
                        var categoryClaim = context.User.FindFirst("category");
                        if (categoryClaim == null)
                        {
                            // 没有类别声明，拒绝访问
                            return;
                        }

                        // 根据用户类别确定用户类型
                        var userCategory = categoryClaim.Value;
                        var requiredScope = apiScopeAttr.Scope;

                        // 检查API作用访问权限
                        if (IsAccessAllowed(userCategory, requiredScope))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
            else
            {
                // 对于非端点资源，默认拒绝访问以提高安全性
                return;
            }

            await Task.CompletedTask;
        }

        private bool IsAccessAllowed(string userCategory, ApiUsageScopeEnum requiredScope)
        {
            // 根据用户类别和所需作用域判断是否允许访问
            switch (requiredScope)
            {
                case ApiUsageScopeEnum.AdminOnly:
                    // 只允许管理员访问
                    return userCategory.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                
                case ApiUsageScopeEnum.AppOnly:
                    // 只允许App用户访问
                    return userCategory.Equals("App", StringComparison.OrdinalIgnoreCase);
                
                case ApiUsageScopeEnum.Both:
                    // 两者都可以访问
                    return userCategory.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                           userCategory.Equals("App", StringComparison.OrdinalIgnoreCase);
                
                default:
                    return false;
            }
        }
    }
}