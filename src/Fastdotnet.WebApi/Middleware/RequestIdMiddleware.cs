using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware;

/// <summary>
/// 请求ID中间件，为每个请求生成唯一的请求ID
/// </summary>
public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string REQUEST_ID_HEADER = "X-Request-ID";

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 尝试从请求头获取已存在的RequestId（支持分布式追踪）
        var requestId = context.Request.Headers[REQUEST_ID_HEADER].ToString();
        
        // 如果请求中没有提供RequestId，则生成新的
        if (string.IsNullOrEmpty(requestId))
        {
            requestId = RequestIdManager.GenerateNewRequestId();
        }
        else
        {
            // 如果请求中提供了RequestId，则使用它
            RequestIdManager.CurrentRequestId = requestId;
        }
        
        // 将RequestId添加到响应头中
        context.Response.Headers[REQUEST_ID_HEADER] = requestId;
        
        try
        {
            // 调用下一个中间件
            await _next(context);
        }
        finally
        {
            // 请求结束时清除RequestId
            RequestIdManager.Clear();
        }
    }
}