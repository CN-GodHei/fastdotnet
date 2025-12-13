namespace Fastdotnet.WebApi.Middleware.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null) {
                return;
            }

            // 超级管理员直接拥有所有权限
            if (context.User.IsInRole("SUPER_ADMIN"))
            {
                context.Succeed(requirement);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var userCategoryClaim = context.User.FindFirst("category");

            if (userIdClaim == null || userCategoryClaim == null)
            {
                return;
            }

            var userId = userIdClaim.Value;
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId, userCategoryClaim.Value);

            if (userPermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}