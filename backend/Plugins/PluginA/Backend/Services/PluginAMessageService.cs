using Fastdotnet.Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using PluginA.IService;
using System;
using System.Threading.Tasks;

namespace PluginA.Services
{


    public class PluginAMessageService : IPluginAMessageService
    {
        private readonly IHubContext<UniversalHub> _universalHubContext;

        public PluginAMessageService(IHubContext<UniversalHub> universalHubContext)
        {
            _universalHubContext = universalHubContext;
        }

        public async Task SendNotificationToClientsAsync(string message)
        {
            try
            {
                // 通过主框架的通用Hub向所有客户端发送插件通知
                await _universalHubContext.Clients.All.SendAsync("pluginNotificationReceived", new
                {
                    PluginId = "PluginA",
                    Message = message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PluginA] 发送通知失败: {ex.Message}");
            }
        }

        public async Task SendDataToClientsAsync(string data)
        {
            try
            {
                // 通过主框架的通用Hub向所有客户端发送插件数据
                await _universalHubContext.Clients.All.SendAsync("pluginDataReceived", new
                {
                    PluginId = "PluginA",
                    Data = data,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PluginA] 发送数据失败: {ex.Message}");
            }
        }
    }
}