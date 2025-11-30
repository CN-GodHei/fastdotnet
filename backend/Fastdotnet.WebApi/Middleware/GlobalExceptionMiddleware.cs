using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Middleware;

/// <summary>
/// 全局异常处理中间件
/// 注意：操作日志由 OperationLogMiddleware 统一记录，此处不再记录
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    //private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next
        //, ILogger<GlobalExceptionMiddleware> logger
        )
    {
        _next = next;
        //_logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 获取 RequestId（用于日志追踪）
        string requestId = RequestIdManager.CurrentRequestId ?? "unknown";
        string path = context.Request.Path.ToString();
        string method = context.Request.Method;

        // 默认响应
        string errorMsg = "服务器内部错误";
        int statusCode = StatusCodes.Status500InternalServerError;

        // 区分业务异常和系统异常
        if (exception is BusinessException businessEx)
        {
            // 业务异常：返回友好提示，不记录异常日志（仅操作日志）
            errorMsg = businessEx.Message;
            statusCode = StatusCodes.Status422UnprocessableEntity;
        }
        else
        {
            // 系统异常：记录异常日志 + 打印日志
            var exceptionLog = new ExceptionLog
            {
                RequestId = requestId,
                ExceptionType = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty,
                Path = path,
                Method = method,
                CreatedAt = DateTime.Now
            };

            try
            {
                var logService = context.RequestServices.GetService<ILogService>();
                if (logService != null)
                {
                    await logService.AddExceptionLogAsync(exceptionLog);
                }
            }
            catch (Exception logEx)
            {
                //_logger.LogError(logEx, "记录异常日志失败。RequestId: {RequestId}", requestId);
            }

            //_logger.LogError(exception, "系统内部异常。RequestId: {RequestId}, Path: {Path}", requestId, path);
        }

        // 返回统一错误响应（不暴露堆栈）
        var result = new ApiResult<object>();
#if DEBUG
        result = new ApiResult<object>
        {
            Code = statusCode,
            Msg = errorMsg + "Debug模式输出具体错误:" + exception.Message
        };
#else
                 result = new ApiResult<object>
                {
                    Code = statusCode,
                    Msg = errorMsg
                };
#endif


        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(result);
    }
}