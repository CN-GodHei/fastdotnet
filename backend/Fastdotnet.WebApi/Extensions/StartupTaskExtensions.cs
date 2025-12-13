using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Fastdotnet.WebApi.Extensions;

public static class StartupTaskExtensions
{
    /// <summary>
    /// 在应用程序启动完成后，异步执行所有注册的 IStartupTask 后台任务
    /// </summary>
    public static void RunStartupTasks(this WebApplication app)
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                Console.WriteLine("Application has started. Executing startup tasks in background...");
                try
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();
                        var _logService = scope.ServiceProvider.GetRequiredService<ILogService>(); // ✅ 提前获取日志服务

                        foreach (var task in startupTasks)
                        {
                            var taskId = $"StartupTask-{Guid.NewGuid():N}";
                            var taskType = task.GetType().Name;
                            try
                            {
                                await task.ExecuteAsync();
                            }
                            catch (Exception ex)
                            {
                                // ❗ 统一记录异常到日志系统
                                var exceptionLog = new ExceptionLog
                                {
                                    RequestId = taskId,
                                    ExceptionType = ex.GetType().FullName,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace ?? string.Empty,
                                    Path = "/startup-task",
                                    Method = taskType,
                                    CreatedAt = DateTime.Now
                                };

                                try
                                {
                                    await _logService.AddExceptionLogAsync(exceptionLog);
                                }
                                catch (Exception logEx)
                                {
                                    // 如果写日志也失败，至少输出到控制台
                                    Console.WriteLine($"Failed to log exception for {taskType}: {logEx.Message}");
                                }
                            }
                        }
                    }
                    Console.WriteLine("All startup tasks executed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during startup tasks execution: {ex.Message}");
                }

                // 初始化DebugLogger
                var logService = app.Services.GetRequiredService<ILogService>();
                DebugLogger.Initialize(logService);
            });
        });
    }
}