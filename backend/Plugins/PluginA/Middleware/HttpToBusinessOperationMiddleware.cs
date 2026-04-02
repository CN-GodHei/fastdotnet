using Fastdotnet.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PluginA.Contexts;
using PluginA.Services;
using System;
using System.Threading.Tasks;

namespace PluginA.Middleware
{
    /// <summary>
    /// HTTP 请求中间件 - 将 HTTP 请求映射到业务操作管道
    /// 演示如何结合 IDynamicMiddleware 和 IPluginPipeline<TContext>
    /// </summary>
    public class HttpToBusinessOperationMiddleware : IDynamicMiddleware
    {
        private readonly ILogger<HttpToBusinessOperationMiddleware> _logger;

        public HttpToBusinessOperationMiddleware(ILogger<HttpToBusinessOperationMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // 只对特定路径进行业务操作管道处理
            if (context.Request.Path.StartsWithSegments("/api/business"))
            {
                try
                {
                    // 从 HTTP 上下文提取信息创建业务操作上下文
                    var businessContext = new BusinessOperationContext
                    {
                        OperationType = context.Request.Method switch
                        {
                            "POST" => "Create",
                            "PUT" => "Update",
                            "DELETE" => "Delete",
                            "GET" => "Query",
                            _ => "Unknown"
                        },
                        DataType = context.Request.Path.Value?.Split('/')[3] ?? "Unknown",
                        DataId = context.Request.Query["id"].FirstOrDefault() ?? Guid.NewGuid().ToString(),
                        UserId = context.User.FindFirst("userId")?.Value ?? "anonymous",
                        OperationTime = DateTime.UtcNow,
                        ExtraData = new Dictionary<string, object>
                        {
                            ["HttpMethod"] = context.Request.Method,
                            ["RequestPath"] = context.Request.Path,
                            ["QueryString"] = context.Request.QueryString.ToString()
                        }
                    };

                    _logger.LogInformation(
                        "HTTP 请求映射到业务操作管道：{OperationType} - {DataType} - {UserId}",
                        businessContext.OperationType,
                        businessContext.DataType,
                        businessContext.UserId);

                    // 获取业务操作执行器（从 HTTP 请求的 DI 容器）
                    var executor = context.RequestServices.GetService(typeof(BusinessOperationExecutor)) 
                        as BusinessOperationExecutor;
                    
                    if (executor != null)
                    {
                        // 通过业务操作管道执行（会经过所有注册的中间件）
                        var resultContext = await executor.ExecuteAsync(
                            businessContext.OperationType,
                            businessContext.DataType,
                            businessContext.DataId,
                            businessContext.UserId,
                            businessContext.ExtraData);

                        // 将业务操作的结果写入响应
                        context.Response.Headers["X-Operation-Id"] = resultContext.OperationId;
                        context.Response.Headers["X-Elapsed-Milliseconds"] = resultContext.ElapsedMilliseconds?.ToString() ?? "0";
                        
                        _logger.LogInformation(
                            "业务操作管道执行完成，耗时：{ElapsedMilliseconds}ms",
                            resultContext.ElapsedMilliseconds);
                    }
                    else
                    {
                        _logger.LogWarning("未找到 BusinessOperationExecutor 服务");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogWarning(ex, "权限验证失败");
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync($"Forbidden: {ex.Message}");
                    return; // 不调用 next，终止管道
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex, "参数验证失败");
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"Bad Request: {ex.Message}");
                    return; // 不调用 next，终止管道
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "业务操作管道执行异常");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
                    return; // 不调用 next，终止管道
                }
            }

            // 继续执行下一个 HTTP 中间件
            await next(context);
        }
    }
}
