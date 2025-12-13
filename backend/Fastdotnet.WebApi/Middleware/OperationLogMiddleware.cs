namespace Fastdotnet.WebApi.Middleware;

/// <summary>
/// 操作日志中间件：记录每个 HTTP 请求的操作日志（成功/失败均记录）
/// 注意：不记录响应体，仅记录请求信息和状态码
/// </summary>
public class OperationLogMiddleware
{
    private readonly RequestDelegate _next;
    //private readonly ILogger<OperationLogMiddleware> _logger;
    private readonly ILogService _logService; // 👈 直接注入
    private readonly ICurrentUser _currentUser; // 👈 注入 ICurrentUser
    public OperationLogMiddleware(RequestDelegate next
        //, ILogger<OperationLogMiddleware> logger
        , ICurrentUser currentUser
        , ILogService logService
        )
    {
        _next = next;
        //_logger = logger;
        _currentUser = currentUser;
        _logService = logService;

    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            // 检查是否标记了 SkipOperationLog
            var skipLog = endpoint.Metadata.GetMetadata<SkipOperationLogAttribute>() != null;
            if (skipLog)
            {
                // 跳过记录操作日志，直接继续管道
                await _next(context);
                return;
            }
        }
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        var path = request.Path.ToString();
        var method = request.Method;
        var ip = GetClientIp(context);

        string headers = null;
        string body = null;

        // 仅对可能包含 Body 的方法尝试读取（并限制长度）
        if ((method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
             method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
             method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
            && request.ContentLength > 0
            && request.ContentLength <= 2000)
        {
            try
            {
                request.EnableBuffering(); // 启用 Body 重复读取

                headers = GetHeaders(request);
                body = await GetBody(request);
            }
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "读取请求体或头信息时发生异常，跳过记录。Path: {Path}", path);
                // 即使读取失败，仍记录基础日志（不含 headers/body）
            }
        }

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var operationLog = new OperationLog
            {
                RequestId = RequestIdManager.CurrentRequestId ?? "no-request-id",
                Path = path,
                Method = method,
                Ip = ip,
                Headers = headers,
                Body = body,
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds.ToString(),
                CreatedAt = DateTime.Now,
                // ✅ 关键增强：操作人信息
                OperatorId = _currentUser.Id,        // 来自 Claims: NameIdentifier
                OperatorName = _currentUser.UserName, // 来自 Claims: Name
                UserType = _currentUser.UserType,
            };

            // 异步写入操作日志（fire-and-forget，但捕获异常）
            _ = Task.Run(async () =>
            {
                try
                {
                    await _logService.AddOperationLogAsync(operationLog);
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "异步记录操作日志失败。RequestId: {RequestId}, Path: {Path}",
                    //    operationLog.RequestId, operationLog.Path);
                }
            });
        }
    }


    private static string GetClientIp(HttpContext context)
    {
        // 1. 优先从 X-Forwarded-For 获取（支持反向代理）
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var firstIp = forwardedFor.ToString().Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp) && IsValidIpAddress(firstIp))
            {
                return NormalizeLocalhost(firstIp);
            }
        }

        // 2. 回退到 RemoteIpAddress
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp == null)
            return "unknown";

        // 3. 处理 IPv4 映射的 IPv6 地址（如 ::ffff:192.168.1.1）
        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        // 4. 转为字符串并标准化本地回环地址
        string ipString = remoteIp.ToString();
        return NormalizeLocalhost(ipString);
    }

    /// <summary>
    /// 将 IPv6 回环地址 ::1 或 IPv4 127.0.0.1 统一标准化为 "127.0.0.1"
    /// </summary>
    private static string NormalizeLocalhost(string ip)
    {
        if (string.Equals(ip, "::1", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ip, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ip, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return "127.0.0.1";
        }
        return ip;
    }

    /// <summary>
    /// 简单校验是否为有效 IP（防止 X-Forwarded-For 注入非法值）
    /// </summary>
    private static bool IsValidIpAddress(string ip)
    {
        return !string.IsNullOrWhiteSpace(ip) &&
               (System.Net.IPAddress.TryParse(ip, out _) || ip.Equals("localhost", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取请求头字符串（限制总长度 ≤1900，防止超数据库字段）
    /// </summary>
    private static string GetHeaders(HttpRequest request)
    {
        var builder = new StringBuilder();
        foreach (var header in request.Headers)
        {
            var line = $"{header.Key}: {header.Value}; ";
            if (builder.Length + line.Length > 1900)
                break; // 防止超长
            builder.Append(line);
        }
        return builder.ToString();
    }

    /// <summary>
    /// 异步读取请求体（已确保 ContentLength ≤ 2000）
    /// </summary>
    private static async Task<string> GetBody(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // 重置位置，供后续中间件或 Controller 使用
        return body;
    }
}