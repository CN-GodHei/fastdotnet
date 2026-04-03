using Fastdotnet.Core.Hubs;
using Fastdotnet.Core.Messaging;
using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Fastdotnet.Core.Messaging
{
    /// <summary>
    /// 插件 SignalR 消息扩展方法
    /// </summary>
    public static class PluginSignalRExtensions
    {
        /// <summary>
        /// 发送插件消息（自动获取当前插件信息，如果失败则使用默认值）
        /// </summary>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="data">消息数据</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginMessageAutoAsync<T>(
            this IHubContext<UniversalHub> hubContext,
            string messageType,
            T data,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            // 尝试自动获取当前插件信息
            PluginInfo? pluginInfo = null;
            try
            {
                pluginInfo = PluginContext.GetCurrentPluginInfo();
            }
            catch (Exception ex)
            {
                // 如果在非插件环境（如主程序）中调用，会抛出异常
                // 这是正常的，我们使用默认的插件信息
                Console.WriteLine($"[PluginSignalR] 无法获取插件信息，使用默认值：{ex.Message}");
            }
            
            var message = new PluginMessage<T>
            {
                PluginId = pluginInfo?.id ?? "System",
                PluginName = pluginInfo?.name ?? "System",
                MessageType = messageType,
                Data = data,
                TargetUserId = targetUserId,
                Timestamp = DateTime.Now
            };

            await hubContext.SendPluginMessageAsync(message, cancellationToken);
        }

        /// <summary>
        /// 发送文本通知（自动获取当前插件信息，如果失败则使用默认值）
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="level">消息级别</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginNotificationAutoAsync(
            this IHubContext<UniversalHub> hubContext,
            string title,
            string message,
            string level = "info",
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            // 尝试自动获取当前插件信息
            PluginInfo? pluginInfo = null;
            try
            {
                pluginInfo = PluginContext.GetCurrentPluginInfo();
            }
            catch (Exception ex)
            {
                // 在非插件环境中，使用默认值
                Console.WriteLine($"[PluginSignalR] 无法获取插件信息，使用默认值：{ex.Message}");
            }

            await hubContext.SendPluginNotificationAsync(
                pluginInfo?.id ?? "System",
                pluginInfo?.name ?? "System",
                title,
                message,
                level,
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送成功通知（自动获取当前插件信息）
        /// </summary>
        public static async Task SendSuccessAutoAsync(
            this IHubContext<UniversalHub> hubContext,
            string title,
            string message,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            await hubContext.SendPluginNotificationAutoAsync(
                title,
                message,
                "success",
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送错误通知（自动获取当前插件信息）
        /// </summary>
        public static async Task SendErrorAutoAsync(
            this IHubContext<UniversalHub> hubContext,
            string title,
            string message,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            await hubContext.SendPluginNotificationAutoAsync(
                title,
                message,
                "error",
                targetUserId,
                cancellationToken);
        }
        /// <summary>
        /// 发送插件消息给特定用户
        /// </summary>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="message">插件消息</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginMessageAsync<T>(
            this IHubContext<UniversalHub> hubContext,
            PluginMessage<T> message,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(message.TargetUserId))
            {
                // 广播给所有客户端
                await hubContext.Clients.All.SendAsync("pluginMessage", message, cancellationToken);
            }
            else
            {
                // 只发送给特定用户
                await hubContext.Clients.User(message.TargetUserId).SendAsync("pluginMessage", message, cancellationToken);
            }
        }

        /// <summary>
        /// 发送插件消息（简化版本，自动填充插件信息）
        /// </summary>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="data">消息数据</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginMessageAsync<T>(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            string messageType,
            T data,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            var message = new PluginMessage<T>
            {
                PluginId = pluginId,
                PluginName = pluginName,
                MessageType = messageType,
                Data = data,
                TargetUserId = targetUserId,
                Timestamp = DateTime.Now
            };

            await hubContext.SendPluginMessageAsync(message, cancellationToken);
        }

        /// <summary>
        /// 发送文本通知
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="notification">通知内容</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginNotificationAsync(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            TextNotification notification,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            await hubContext.SendPluginMessageAsync(
                pluginId,
                pluginName,
                PluginMessageTypes.Notification,
                notification,
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送简单文本通知（最简版本）
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="level">消息级别</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendPluginNotificationAsync(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            string title,
            string message,
            string level = "info",
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            var notification = new TextNotification
            {
                Title = title,
                Message = message,
                Level = level,
                Dismissible = true,
                AutoCloseDelay = 3000
            };

            await hubContext.SendPluginNotificationAsync(
                pluginId,
                pluginName,
                notification,
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送进度更新
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="description">进度描述</param>
        /// <param name="current">当前进度</param>
        /// <param name="total">总进度</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendProgressUpdateAsync(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            string description,
            int current,
            int total,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            var progress = new ProgressData
            {
                Description = description,
                Current = current,
                Total = total,
                IsCompleted = current >= total
            };

            await hubContext.SendPluginMessageAsync(
                pluginId,
                pluginName,
                PluginMessageTypes.ProgressUpdate,
                progress,
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送成功通知
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendSuccessAsync(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            string title,
            string message,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            await hubContext.SendPluginNotificationAsync(
                pluginId,
                pluginName,
                title,
                message,
                "success",
                targetUserId,
                cancellationToken);
        }

        /// <summary>
        /// 发送错误通知
        /// </summary>
        /// <param name="hubContext">Hub 上下文</param>
        /// <param name="pluginId">插件 ID</param>
        /// <param name="pluginName">插件名称</param>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="targetUserId">目标用户 ID（可选）</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task SendErrorAsync(
            this IHubContext<UniversalHub> hubContext,
            string pluginId,
            string pluginName,
            string title,
            string message,
            string? targetUserId = null,
            CancellationToken cancellationToken = default)
        {
            await hubContext.SendPluginNotificationAsync(
                pluginId,
                pluginName,
                title,
                message,
                "error",
                targetUserId,
                cancellationToken);
        }
    }
}
