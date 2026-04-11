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

        // 立即设置 RequestId 到响应头（在响应开始前）
        try
        {
            context.Response.Headers[REQUEST_ID_HEADER] = requestId;
        }
        catch (ObjectDisposedException)
        {
            // 如果响应已经 disposed，忽略此错误
        }

        // 在响应开始时设置时间戳
        context.Response.OnStarting(() =>
        {
            try
            {
                var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                context.Response.Headers[SERVER_TIMESTAMP_HEADER] = timestamp;
            }
            catch (InvalidOperationException)
            {
                // 如果响应已经开始发送，Headers 变为只读，忽略此错误
            }
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