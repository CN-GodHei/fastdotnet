using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.Logging;
using PluginA.Contexts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PluginA.Middleware
{
    /// <summary>
    /// 业务操作日志记录中间件 - 演示 IPluginPipeline 的使用
    /// </summary>
    public class BusinessOperationLoggingMiddleware : IPluginPipeline<BusinessOperationContext>
    {
        private readonly ILogger<BusinessOperationLoggingMiddleware> _logger;

        public BusinessOperationLoggingMiddleware(ILogger<BusinessOperationLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(BusinessOperationContext context, Func<BusinessOperationContext, Task> next)
        {
            _logger.LogInformation(
                "========== 开始执行业务操作 ==========\n" +
                "操作 ID: {OperationId}\n" +
                "操作类型：{OperationType}\n" +
                "数据类型：{DataType}\n" +
                "数据 ID: {DataId}\n" +
                "操作用户：{UserId}\n" +
                "操作时间：{OperationTime}",
                context.OperationId,
                context.OperationType,
                context.DataType,
                context.DataId,
                context.UserId,
                context.OperationTime);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context); // 执行下一个中间件
                
                stopwatch.Stop();
                context.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                
                _logger.LogInformation(
                    "业务操作执行成功，耗时：{ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                context.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                
                _logger.LogError(
                    ex,
                    "业务操作执行失败，耗时：{ElapsedMilliseconds}ms，操作 ID: {OperationId}",
                    stopwatch.ElapsedMilliseconds,
                    context.OperationId);
                throw; // 重新抛出异常
            }
        }
    }
}
