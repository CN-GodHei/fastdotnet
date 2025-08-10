using Fastdotnet.Core.Middleware;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public static class PluginDiagnostics
    {
        /// <summary>
        /// 执行具有破坏性的诊断，以找出文件锁的根源。
        /// </summary>
        public static async Task PerformDestructiveDiagnostics(string pluginDllPath, IServiceProvider rootProvider, ILogger logger)
        {
            logger.LogWarning("!!!!!!!!!! 开始执行破坏性诊断 !!!!!!!!!!");
            logger.LogWarning("此过程将尝试破坏应用状态以定位引用泄漏。");

            if (!await CheckLockAndLog(pluginDllPath, "进入诊断前的初始状态", logger))
            {
                logger.LogInformation("文件在进入诊断前已解锁，诊断中止。");
                return;
            }

            // --- 目标 1: DynamicMiddlewareRegistry ---
            logger.LogWarning("--> 诊断目标 1: 清理 DynamicMiddlewareRegistry 的内部列表...");
            try
            {
                var registry = rootProvider.GetService<DynamicMiddlewareRegistry>();
                if (registry != null)
                {
                    var field = typeof(DynamicMiddlewareRegistry).GetField("_middlewareTypes", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        if (field.GetValue(registry) is IList list)
                        {
                            list.Clear();
                            logger.LogInformation("    成功：DynamicMiddlewareRegistry._middlewareTypes 已被强制清空。");
                        }
                    }
                }
            }
            catch (Exception ex) { logger.LogError(ex, "    清理 DynamicMiddlewareRegistry 时发生错误。"); }
            if (!await CheckLockAndLog(pluginDllPath, "清理 DynamicMiddlewareRegistry 后", logger)) return;


            // --- 目标 2: SqlSugar 实体缓存 ---
            logger.LogWarning("--> 诊断目标 2: 清理 SqlSugar 的实体缓存...");
            try
            {
                var sqlClient = rootProvider.GetService<SqlSugar.ISqlSugarClient>();
                if (sqlClient != null)
                {
                    var field = typeof(SqlSugar.EntityMaintenance).GetField("EntityList", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        if (field.GetValue(sqlClient.EntityMaintenance) is IList list)
                        {
                            list.Clear();
                            logger.LogInformation("    成功：SqlSugar.EntityMaintenance.EntityList 已被强制清空。");
                        }
                    }
                }
            }
            catch (Exception ex) { logger.LogError(ex, "    清理 SqlSugar 缓存时发生错误。"); }
            if (!await CheckLockAndLog(pluginDllPath, "清理 SqlSugar 后", logger)) return;


            // --- 目标 3: ASP.NET Core MVC 路由缓存 ---
            logger.LogWarning("--> 诊断目标 3: 强制刷新 MVC 路由缓存...");
            try
            {
                var provider = rootProvider.GetService<IActionDescriptorChangeProvider>();
                ActionDescriptorChangeProvider.Instance.NotifyChanges();
                logger.LogInformation("    成功：已再次触发 ActionDescriptorChangeProvider.Instance.NotifyChanges()。");
            }
            catch (Exception ex) { logger.LogError(ex, "    强制刷新 MVC 路由时发生错误。"); }
            if (!await CheckLockAndLog(pluginDllPath, "强制刷新 MVC 路由后", logger)) return;


            logger.LogCritical("!!!!!!!!!! 所有诊断步骤执行完毕，文件依然被锁定。问题根源未知。 !!!!!!!!!!");
        }

        private static async Task<bool> CheckLockAndLog(string filePath, string checkPoint, ILogger logger)
        {
            logger.LogInformation("    检查点: {checkPoint}", checkPoint);
            ForceGC();
            await Task.Delay(100); // Give OS time to release handle

            var isLocked = IsFileLocked(new FileInfo(filePath));
            if (isLocked)
            {
                logger.LogError("        [文件状态]: 仍然被锁定 ❌");
            }
            else
            {
                logger.LogInformation("        [文件状态]: 已成功解锁 ✅ <--- 问题可能在此步骤解决！");
            }
            return isLocked;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ForceGC()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }

        private static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists) return false;
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
    }
}
