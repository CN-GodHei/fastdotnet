using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace PluginA.Hubs
{
    /// <summary>
    /// PluginA SignalR Hub
    /// 注意：由于ASP.NET Core限制，此Hub无法动态注册端点。
    /// 插件的SignalR功能通过主框架的通用Hub（UniversalHub）实现。
    /// 此类作为插件SignalR功能的参考实现和内部业务逻辑容器。
    /// </summary>
    [Authorize] // 插件Hub可以要求鉴权
    public class PluginAHub : Hub
    {
        /// <summary>
        /// 插件特定的业务逻辑方法（非SignalR端点方法）
        /// 这些方法可以直接在插件内部调用，不通过SignalR
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>处理结果</returns>
        public PluginANotification ProcessNotification(string message)
        {
            // 插件特定的通知处理逻辑
            return new PluginANotification
            {
                PluginId = "PluginA",
                Message = message,
                Timestamp = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 插件特定的数据处理方法
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>处理结果</returns>
        public PluginAData ProcessData(string data)
        {
            // 插件特定的数据处理逻辑
            return new PluginAData
            {
                PluginId = "PluginA",
                Content = data,
                ProcessedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 插件特定的SignalR方法（文档用途）
        /// 注意：这些方法不会被实际调用，因为Hub端点未注册
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async Task SendPluginNotification(string message)
        {
            // 发送插件特定的通知
            // 实际实现通过主框架的通用Hub完成
            await Clients.All.SendAsync("pluginNotificationReceived", new {
                PluginId = "PluginA",
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 加入插件房间（文档用途）
        /// </summary>
        /// <param name="roomName">房间名称</param>
        /// <returns></returns>
        public async Task JoinPluginRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"plugin-A-{roomName}");
        }

        /// <summary>
        /// 离开插件房间（文档用途）
        /// </summary>
        /// <param name="roomName">房间名称</param>
        /// <returns></returns>
        public async Task LeavePluginRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"plugin-A-{roomName}");
        }

        /// <summary>
        /// 发送房间消息（文档用途）
        /// </summary>
        /// <param name="roomName">房间名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async Task SendToRoom(string roomName, string message)
        {
            await Clients.Group($"plugin-A-{roomName}").SendAsync("roomMessageReceived", new {
                RoomName = roomName,
                Message = message,
                Sender = Context.ConnectionId,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 需要特定角色才能调用的方法（文档用途）
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task DeletePluginData(int id)
        {
            // 只有管理员可以调用
            // 这里应该调用实际的业务逻辑来删除数据
            await Clients.All.SendAsync("dataDeleted", id);
        }
    }

    /// <summary>
    /// 插件通知类
    /// </summary>
    public class PluginANotification
    {
        public string PluginId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    /// <summary>
    /// 插件数据类
    /// </summary>
    public class PluginAData
    {
        public string PluginId { get; set; }
        public string Content { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}