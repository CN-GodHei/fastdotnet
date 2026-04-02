using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.Logging;
using PluginA.Contexts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PluginA.Middleware
{
    /// <summary>
    /// 业务操作性能监控中间件 - 演示性能指标收集
    /// </summary>
    public class BusinessOperationPerformanceMiddleware : IPluginPipeline<BusinessOperationContext>
    {
        private readonly ILogger<BusinessOperationPerformanceMiddleware> _logger;
        private const long PerformanceWarningThresholdMs = 1000; // 性能警告阈值：1 秒

        public BusinessOperationPerformanceMiddleware(ILogger<BusinessOperationPerformanceMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(BusinessOperationContext context, Func<BusinessOperationContext, Task> next)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
                
                stopwatch.Stop();
                context.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                
                // 检查是否超过性能阈值
                if (context.ElapsedMilliseconds > PerformanceWarningThresholdMs)
                {
                    _logger.LogWarning(
                        "业务操作执行时间过长：{ElapsedMilliseconds}ms\n" +
                        "操作 ID: {OperationId}\n" +
                        "操作类型：{OperationType}\n" +
                        "数据类型：{DataType}",
                        context.ElapsedMilliseconds,
                        context.OperationId,
                        context.OperationType,
                        context.DataType);
                }
                else
                {
                    _logger.LogDebug(
                        "业务操作执行完成，耗时：{ElapsedMilliseconds}ms",
                        context.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                context.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                
                _logger.LogError(
                    ex,
                    "业务操作执行异常，耗时：{ElapsedMilliseconds}ms，操作 ID: {OperationId}",
                    context.ElapsedMilliseconds,
                    context.OperationId);
                throw;
            }
        }
    }
}
