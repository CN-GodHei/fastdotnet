using Fastdotnet.Core.Models;
using Fastdotnet.Core.Utils;
using Fastdotnet.Core.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fastdotnet.Core.Models.LogModels;
using System.Net;

namespace Fastdotnet.WebApi.Middleware
{
    public class OperationLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OperationLogMiddleware> _logger;
        private readonly ILogService _logService;

        public OperationLogMiddleware(RequestDelegate next, ILogger<OperationLogMiddleware> logger, ILogService logService)
        {
            _next = next;
            _logger = logger;
            _logService = logService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // 读取请求信息
            var path = context.Request.Path;
            var method = context.Request.Method;
            var ip = GetClientIp(context); // 使用优化的IP获取方法
            
            string headers = null;
            string body = null;
            
            // 只对需要记录body的方法记录body
            if (method == "POST" || method == "PUT" || method == "PATCH")
            {
                // 允许重复读取body
                context.Request.EnableBuffering();
                
                headers = GetHeaders(context);
                body = await GetBody(context.Request);
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            int statusCode = 0;
            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            finally
            {
                stopwatch.Stop();
                
                // 记录操作日志
                var operationLog = new OperationLog
                {
                    RequestId = RequestIdManager.CurrentRequestId, // 明确设置RequestId
                    Path = path,
                    Method = method,
                    Ip = ip,
                    Headers = headers,
                    Body = body,
                    StatusCode = statusCode,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds.ToString(),
                    CreateTime = DateTime.Now // 明确设置创建时间
                };

                try 
                {
                    await _logService.AddOperationLogAsync(operationLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to record operation log");
                }

                // 将响应内容复制回原始流
                try
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to copy response body to original stream");
                }
            }
        }

        /// <summary>
        /// 获取客户端IP地址，优化IPv4映射地址格式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetClientIp(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            // 处理IPv4映射地址，如 ::ffff:127.0.0.1 -> 127.0.0.1
            if (ip.StartsWith("::ffff:"))
            {
                ip = ip.Substring(7); // 移除 "::ffff:" 前缀
            }
            
            return ip;
        }

        private string GetHeaders(HttpContext context)
        {
            var headersBuilder = new StringBuilder();
            foreach (var header in context.Request.Headers)
            {
                headersBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            return headersBuilder.ToString();
        }

        private async Task<string> GetBody(HttpRequest request)
        {
            if (request.ContentLength > 0)
            {
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return body;
            }
            return null;
        }
    }
}