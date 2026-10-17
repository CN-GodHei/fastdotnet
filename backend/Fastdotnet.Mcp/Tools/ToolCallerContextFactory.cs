using System.Security.Claims;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.IService;
using Fastdotnet.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// 由宿主 JWT 身份构造 <see cref="ToolCallerContext"/>。
/// 复用宿主既有的 <see cref="ICurrentUser"/> 与 <see cref="IPermissionService"/>，
/// 不引入第二套身份体系。
/// </summary>
public sealed class ToolCallerContextFactory
{
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionService _permissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ToolCallerContextFactory> _logger;

    public ToolCallerContextFactory(
        ICurrentUser currentUser,
        IPermissionService permissionService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ToolCallerContextFactory> logger)
    {
        _currentUser = currentUser;
        _permissionService = permissionService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ToolCallerContext> CreateAsync(
        ClaimsPrincipal? principal = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.Id;
        var userName = _currentUser.UserName;
        var userType = string.IsNullOrWhiteSpace(_currentUser.UserType) ? "Admin" : _currentUser.UserType;
        var isSuperAdmin = ResolveSuperAdmin(principal);

        // 兜底：ICurrentUser 未填充时从 ClaimsPrincipal 取
        if (string.IsNullOrEmpty(userId) && principal?.Identity?.IsAuthenticated == true)
        {
            userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? principal.FindFirst("sub")?.Value
                     ?? principal.FindFirst("userId")?.Value;
            userName ??= principal.Identity.Name ?? principal.FindFirst("name")?.Value;
        }

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                var list = await _permissionService.GetUserPermissionsAsync(userId, userType);
                if (list is not null)
                {
                    foreach (var p in list)
                    {
                        if (!string.IsNullOrWhiteSpace(p)) permissions.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                // 权限查询失败时按「最小权限」处理，绝不放大权限
                _logger.LogWarning(ex, "查询用户 {UserId} 权限失败，本次按最小权限处理", userId);
            }
        }

        var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier;

        return new ToolCallerContext(
            userId,
            userName,
            userType,
            isSuperAdmin,
            TenantId: null,
            RequestId: requestId,
            Permissions: permissions);
    }

    /// <summary>
    /// 判定是否超级管理员。
    /// <para>
    /// 优先使用令牌中的角色声明（零数据库开销）；只有在令牌不含角色时才回落到
    /// <see cref="ICurrentUser.IsSuperAdmin"/>（该实现需要查库）。
    /// 任何异常都按「非超管」处理，绝不放大权限，也不让身份判定失败拖垮整个请求。
    /// </para>
    /// </summary>
    private bool ResolveSuperAdmin(ClaimsPrincipal? principal)
    {
        try
        {
            if (principal?.IsInRole(SystemConstants.SuperAdminRoleCode) == true)
            {
                return true;
            }

            return _currentUser.IsSuperAdmin;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "判定超级管理员失败，按非超管处理");
            return false;
        }
    }
}
