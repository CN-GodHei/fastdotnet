using Fastdotnet.Core.Hubs;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Plugin.Core.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Fastdotnet.WebApi.Providers
{
    /// <summary>
    /// 主程序 SignalR 方法提供者
    /// 将主程序的功能作为"System"插件暴露给前端
    /// </summary>
    public class SystemSignalRProvider : IPluginSignalRProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemSignalRProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Console.WriteLine($"[SystemSignalRProvider] 实例已创建");
        }

        public string PluginId => "System";

        public void RegisterMethods(ISignalRMethodRegistry registry)
        {
            try
            {
                Console.WriteLine($"[SystemSignalRProvider] 开始注册方法...");
                
                // 注册检查插件安装状态的方法
                registry.Register("CheckPluginInstalling", async (args) =>
                {
                    if (args == null || args.Length == 0)
                        return false;

                    var pluginId = args[0]?.ToString();
                    if (string.IsNullOrEmpty(pluginId))
                        return false;

                    // 从当前 HTTP 上下文获取 userId
                    var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? _httpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value;

                    if (string.IsNullOrEmpty(userId))
                        return false;

                    return PluginLoadService.IsInstalling(userId, pluginId);
                });
                
                Console.WriteLine($"[SystemSignalRProvider] CheckPluginInstalling 已注册");
                
                // 注册取消插件安装的方法
                registry.Register("CancelPluginInstallation", async (args) =>
                {
                    if (args == null || args.Length == 0)
                        return new { success = false, message = "参数错误" };

                    var pluginId = args[0]?.ToString();
                    if (string.IsNullOrEmpty(pluginId))
                        return new { success = false, message = "插件ID不能为空" };

                    // 从当前 HTTP 上下文获取 userId
                    var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? _httpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value;

                    if (string.IsNullOrEmpty(userId))
                        return new { success = false, message = "未找到用户信息" };

                    var cancelled = PluginLoadService.CancelInstallation(userId, pluginId);
                    
                    return new 
                    { 
                        success = cancelled, 
                        message = cancelled ? "已请求取消安装" : "未找到正在运行的安装任务" 
                    };
                });
                
                Console.WriteLine($"[SystemSignalRProvider] CancelPluginInstallation 已注册");
                Console.WriteLine($"[SystemSignalRProvider] 所有方法注册完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SystemSignalRProvider] 注册方法时发生异常: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"[SystemSignalRProvider] 堆栈跟踪: {ex.StackTrace}");
                throw;
            }
        }
    }
}
