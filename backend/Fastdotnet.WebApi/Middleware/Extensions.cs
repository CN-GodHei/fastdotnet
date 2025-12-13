using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware;

/// <summary>
/// 优雅停机中间件：拒绝新请求、跟踪活跃请求数、放行管理端点
/// </summary>
public class GracefulShutdownMiddleware
{
    private readonly RequestDelegate _next;
    private readonly GracefulShutdownState _state;
    private readonly string[] _allowedPaths;

    public GracefulShutdownMiddleware(
        RequestDelegate next,
        GracefulShutdownState state,
        string[]? allowedPaths = null)
    {
        _next = next;
        _allowedPaths = allowedPaths ?? new[] { "/shutdown/status" };
        _state = state;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // 检查是否是允许在停机期间访问的路径（如状态查询）
        bool isAllowed = false;
        foreach (var allowed in _allowedPaths)
        {
            if (path.Equals(allowed, System.StringComparison.OrdinalIgnoreCase))
            {
                isAllowed = true;
                break;
            }
        }

        // 如果正在停机且不是白名单路径 → 拒绝
        if (_state.IsShuttingDown && !isAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Server is shutting down.");
            return;
        }

        // 白名单请求不计入活跃数（轻量运维请求）
        if (isAllowed)
        {
            await _next(context);
            return;
        }

        // 业务请求：计入活跃数
        _state.IncrementActiveRequests();
        try
        {
            await _next(context);
        }
        finally
        {
            _state.DecrementActiveRequests();
        }
    }
}

/// <summary>
/// 共享状态：线程安全的停机标志和活跃请求数
/// </summary>
public class GracefulShutdownState
{
    private int _activeRequests = 0;
    private volatile bool _isShuttingDown = false;

    public bool IsShuttingDown => _isShuttingDown;
    public int ActiveRequests => Volatile.Read(ref _activeRequests);

    public void StartShutdown() => _isShuttingDown = true;

    public void IncrementActiveRequests() => Interlocked.Increment(ref _activeRequests);

    public void DecrementActiveRequests() => Interlocked.Decrement(ref _activeRequests);
}