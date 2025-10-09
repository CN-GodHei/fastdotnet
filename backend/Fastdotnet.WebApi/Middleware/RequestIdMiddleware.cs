// Fastdotnet.WebApi/Middleware/RequestIdMiddleware.cs
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware;

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
        var requestId = context.Request.Headers[REQUEST_ID_HEADER].ToString();

        if (string.IsNullOrWhiteSpace(requestId))
        {
            requestId = RequestIdManager.GenerateNewRequestId();
        }

        // 设置到上下文
        RequestIdManager.CurrentRequestId = requestId;

        // 回写到响应头
        context.Response.Headers[REQUEST_ID_HEADER] = requestId;

        try
        {
            await _next(context);
        }
        finally
        {
            // 清理上下文（防止 AsyncLocal 泄漏）
            RequestIdManager.Clear();
        }
    }
}