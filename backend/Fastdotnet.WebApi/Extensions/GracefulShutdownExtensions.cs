using Fastdotnet.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

namespace Fastdotnet.WebApi.Extensions;

public static class GracefulShutdownExtensions
{
    public static WebApplication UseGracefulShutdown(this WebApplication app, string[]? allowedPaths = null)
    {
        var state = new GracefulShutdownState();
        app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(state.StartShutdown);

        app.UseMiddleware<GracefulShutdownMiddleware>(state, allowedPaths ?? new[] { "/shutdown/status" });

        // 状态端点：可公开或限制（这里暂不限制，你也可加 RequireAuthorization）
        app.MapGet("/shutdown/status", () => new
        {
            IsShuttingDown = state.IsShuttingDown,
            ActiveRequests = state.ActiveRequests
        }).ExcludeFromDescription();

        // 👇 关键：带 JWT 鉴权的停机端点
        app.MapPost("/admin/shutdown", (IHostApplicationLifetime lifetime) =>
        {
            lifetime.StopApplication();
            return Results.Ok(new { message = "Graceful shutdown initiated." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin")) // ← 要求角色为 Admin
        .ExcludeFromDescription();

        return app;
    }
}