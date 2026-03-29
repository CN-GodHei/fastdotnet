using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers;

/// <summary>
/// 健康检查控制器
/// 用于 Docker 容器和负载均衡器的健康探测
/// </summary>
[ApiController]
[Route("[controller]")]
[SkipAntiReplayAttribute]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 健康检查接口
    /// </summary>
    /// <returns>健康状态信息</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [AllowAnonymous]
    public IActionResult Get()
    {
        var healthStatus = new HealthStatus
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = GetApplicationVersion(),
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        // 可以在此处添加更多健康检查逻辑
        // 例如：数据库连接检查、Redis 连接检查等
        
        return Ok(healthStatus);
    }

    /// <summary>
    /// 获取应用版本
    /// </summary>
    private static string GetApplicationVersion()
    {
        var version = typeof(HealthController).Assembly.GetName().Version;
        return version?.ToString() ?? "Unknown";
    }
}

/// <summary>
/// 健康状态响应模型
/// </summary>
public class HealthStatus
{
    /// <summary>
    /// 健康状态（Healthy/Unhealthy）
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// 检查时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// 应用版本
    /// </summary>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// 运行环境
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}
