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
                    // 获取控制器和动作上的 ApiUsageScopeAttribute，方法级别的优先级高于控制器级别
                    // 通过反射获取动作方法和控制器类型的特性
                    var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
                    var methodInfo = controllerActionDescriptor.MethodInfo;
                                    
                    // 首先检查动作方法上的特性（包括继承链）
                    var actionApiScopeAttr = GetInheritedAttribute<ApiUsageScopeAttribute>(methodInfo);
                                    
                    // 如果动作方法上没有，则检查控制器类型的特性（包括继承链）
                    var controllerApiScopeAttr = actionApiScopeAttr ?? GetInheritedAttribute<ApiUsageScopeAttribute>(controllerType);
                                    
                    var apiScopeAttr = actionApiScopeAttr ?? controllerApiScopeAttr;
                                
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
                        // 获取控制器和动作上的 ApiUsageScopeAttribute，方法级别的优先级高于控制器级别
                        // 通过反射获取动作方法和控制器类型的特性
                        var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
                        var methodInfo = controllerActionDescriptor.MethodInfo;
                                            
                        // 首先检查动作方法上的特性（包括继承链）
                        var actionApiScopeAttr = GetInheritedAttribute<ApiUsageScopeAttribute>(methodInfo);
                                            
                        // 如果动作方法上没有，则检查控制器类型的特性（包括继承链）
                        var controllerApiScopeAttr = actionApiScopeAttr ?? GetInheritedAttribute<ApiUsageScopeAttribute>(controllerType);
                                            
                        var apiScopeAttr = actionApiScopeAttr ?? controllerApiScopeAttr;
                                        
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

        /// <summary>
        /// 从方法继承链中获取指定类型的特性，包括重写的方法
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="methodInfo">方法信息</param>
        /// <returns>找到的特性或null</returns>
        private T GetInheritedAttribute<T>(System.Reflection.MethodInfo methodInfo) where T : System.Attribute
        {
            // 首先检查当前方法是否有特性
            var attribute = methodInfo.GetCustomAttribute<T>(false);
            if (attribute != null)
            {
                return attribute;
            }

            // 检查继承链，查找重写的方法
            var baseMethod = methodInfo.GetBaseDefinition();
            if (baseMethod != null && baseMethod != methodInfo)
            {
                attribute = baseMethod.GetCustomAttribute<T>(false);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 从类型继承链中获取指定类型的特性，包括接口
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="type">类型信息</param>
        /// <returns>找到的特性或null</returns>
        private T GetInheritedAttribute<T>(System.Type type) where T : System.Attribute
        {
            // 首先检查当前类型是否有特性
            var attribute = type.GetCustomAttribute<T>(false);
            if (attribute != null)
            {
                return attribute;
            }

            // 检查基类
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                attribute = baseType.GetCustomAttribute<T>(false);
                if (attribute != null)
                {
                    return attribute;
                }
                baseType = baseType.BaseType;
            }

            // 检查接口
            foreach (var interfaceType in type.GetInterfaces())
            {
                attribute = interfaceType.GetCustomAttribute<T>(false);
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
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