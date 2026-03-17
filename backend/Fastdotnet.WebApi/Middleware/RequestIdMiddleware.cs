namespace Fastdotnet.WebApi.Middleware;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string REQUEST_ID_HEADER = "X-Request-ID";
    private const string SERVER_TIMESTAMP_HEADER = "X-Server-Timestamp";

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

        // 回写 RequestID 到响应头
        context.Response.Headers[REQUEST_ID_HEADER] = requestId;

        context.Response.OnStarting(() =>
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            context.Response.Headers[SERVER_TIMESTAMP_HEADER] = timestamp;
            return Task.CompletedTask;
        });
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