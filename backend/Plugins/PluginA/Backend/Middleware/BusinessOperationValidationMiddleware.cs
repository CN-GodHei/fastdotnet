using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.Logging;
using PluginA.Contexts;
using System;
using System.Threading.Tasks;

namespace PluginA.Middleware
{
    /// <summary>
    /// 业务操作验证中间件 - 演示权限校验逻辑
    /// </summary>
    public class BusinessOperationValidationMiddleware : IPluginPipeline<BusinessOperationContext>
    {
        private readonly ILogger<BusinessOperationValidationMiddleware> _logger;

        public BusinessOperationValidationMiddleware(ILogger<BusinessOperationValidationMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(BusinessOperationContext context, Func<BusinessOperationContext, Task> next)
        {
            // 1. 基础验证
            if (string.IsNullOrWhiteSpace(context.UserId))
            {
                throw new ArgumentException("操作用户 ID 不能为空", nameof(context.UserId));
            }

            if (string.IsNullOrWhiteSpace(context.OperationType))
            {
                throw new ArgumentException("操作类型不能为空", nameof(context.OperationType));
            }

            _logger.LogDebug("基础验证通过");
            context.IsAuthenticated = true;

            // 2. 权限验证（这里简化处理，实际应该调用权限服务）
            bool hasPermission = await CheckPermissionAsync(context);
            
            if (!hasPermission)
            {
                _logger.LogWarning(
                    "用户 {UserId} 无权限执行操作 {OperationType} 于数据类型 {DataType}",
                    context.UserId,
                    context.OperationType,
                    context.DataType);
                
                throw new UnauthorizedAccessException(
                    $"用户 {context.UserId} 无权限执行此操作");
            }

            _logger.LogDebug("权限验证通过");
            context.HasPermission = true;

            // 继续执行下一个中间件
            await next(context);
        }

        /// <summary>
        /// 检查用户权限（示例方法，实际应该注入权限服务）
        /// </summary>
        private Task<bool> CheckPermissionAsync(BusinessOperationContext context)
        {
            // TODO: 实际项目中应该在这里调用权限服务
            // 例如：_permissionService.CheckAsync(context.UserId, context.OperationType, context.DataType)
            
            // 示例中简单返回 true
            return Task.FromResult(true);
        }
    }
}
