using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public static class PluginDiagnostics
    {
        /// <summary>
        /// 诊断插件卸载后文件仍然被占用的问题。
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="pluginDllPath">插件DLL的完整路径</param>
        /// <param name="serviceProvider">应用程序的服务提供程序</param>
        /// <param name="logger">日志记录器</param>
        public static async Task DiagnoseAndLogLocking(string pluginId, string pluginDllPath, IServiceProvider serviceProvider, ILogger logger)
        {
            logger.LogInformation("========== 开始诊断插件 [{pluginId}] 的文件锁定问题 ==========", pluginId);
            logger.LogInformation("目标文件: {pluginDllPath}", pluginDllPath);

            // 1. 在执行任何操作之前，先强制GC一次，建立一个干净的基线
            await ForceGCAndCheckLock(pluginDllPath, "初始状态", logger);

            // 2. 这里我们假设标准的 DisablePluginAsync 已经执行完毕。
            // 在实际调用链中，此诊断方法应在 DisablePluginAsync 之后被调用。
            // 我们再次执行GC和检查，看看标准流程后文件是否解锁。
            logger.LogInformation("--- 标准卸载流程已执行完毕，现在检查文件状态 ---");
            await ForceGCAndCheckLock(pluginDllPath, "标准卸载后", logger);

            // 3. 如果文件仍然被锁定，开始逐个排查可疑的引用源
            if (IsFileLocked(new FileInfo(pluginDllPath)))
            {
                logger.LogWarning("文件在标准卸载后依然被锁定。开始深入诊断...");

                // 诊断步骤可以不断添加
                // 例如：尝试清理特定的缓存、手动释放已知的单例服务等。
                // 由于我们无法直接访问DI容器中所有已解析的实例，
                // 这里的诊断更多是基于对应用程序架构的假设。

                // 示例：尝试清理ASP.NET Core的路由缓存 (这是一个常见的嫌疑犯)
                var actionDescriptorProvider = serviceProvider.GetService<IActionDescriptorCollectionProvider>();
                if (actionDescriptorProvider is IDisposable disposableProvider)
                {
                    logger.LogInformation("--- 正在尝试清理 IActionDescriptorCollectionProvider ---");
                    // 注意：这在实际应用中可能是危险操作，仅用于诊断
                    // disposableProvider.Dispose(); 
                    // 这里我们只记录日志，不实际操作，因为可能会破坏应用
                }
                
                logger.LogInformation("--- 诊断步骤执行完毕 ---");
            }
            else
            {
                logger.LogInformation("文件在标准卸载后已成功解锁。");
            }

            logger.LogInformation("========== 插件 [{pluginId}] 的文件锁定诊断结束 ==========", pluginId);
        }

        /// <summary>
        /// 强制进行垃圾回收，并检查文件锁定状态。
        /// </summary>
        private static async Task ForceGCAndCheckLock(string filePath, string checkPoint, ILogger logger)
        {
            logger.LogInformation("--> 检查点: {checkPoint}", checkPoint);
            logger.LogInformation("    执行强制垃圾回收 (GC)...");
            
            // 使用NoInlining确保GC在此方法内完成，而不是被JIT优化掉
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void GCRun()
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            }

            GCRun();
            
            // 等待一小段时间，给操作系统反应时间
            await Task.Delay(100);

            var fileInfo = new FileInfo(filePath);
            if (IsFileLocked(fileInfo))
            {
                logger.LogError("    [文件状态]：仍然被锁定 ❌");
            }
            else
            {
                logger.LogInformation("    [文件状态]：已解锁 ✅");
            }
        }

        /// <summary>
        /// 检查文件是否被锁定。
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <returns>如果文件被锁定，则为true；否则为false。</returns>
        private static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists)
            {
                // 如果文件不存在，自然没有被锁定
                return false;
            }

            FileStream stream = null;
            try
            {
                // 尝试以读写模式打开文件。如果文件被另一个进程以写入方式锁定，
                // 或者被任何方式锁定导致我们无法获取写权限，这里会抛出IOException。
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // 捕获到异常，说明文件正在被使用
                return true;
            }
            finally
            {
                stream?.Close();
            }

            // 如果没有异常，说明文件未被锁定
            return false;
        }
    }
}