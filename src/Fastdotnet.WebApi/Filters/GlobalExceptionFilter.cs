using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using Fastdotnet.Core.Models.LogModels;

namespace Fastdotnet.WebApi.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly ILogService _logService;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException)
            {
                // 可控异常，不记录日志，返回422状态码
                context.Result = new ObjectResult(CommonResult<object>.Error(context.Exception.Message))
                {
                    StatusCode = 422
                };
                context.ExceptionHandled = true;
            }
            else
            {
                // 不可控异常，记录异常日志
                var exceptionLog = new ExceptionLog
                {
                    ExceptionType = context.Exception.GetType().FullName,
                    Message = context.Exception.Message,
                    StackTrace = context.Exception.StackTrace,
                    Path = context.HttpContext.Request.Path,
                    Method = context.HttpContext.Request.Method
                };

                _ = _logService.AddExceptionLogAsync(exceptionLog);
                
                // 返回500状态码
                _logger.LogError(context.Exception, "系统内部异常");
                
                context.Result = new ObjectResult(CommonResult<object>.Error("服务器内部错误"))
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}