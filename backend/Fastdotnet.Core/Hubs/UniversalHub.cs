

namespace Fastdotnet.Core.Hubs
{
    /// <summary>
    /// 通用SignalR Hub，支持公共访问和鉴权访问，以及插件功能
    /// </summary>
    public class UniversalHub : Hub
    {
        // 存储已鉴权的连接
        private static readonly ConcurrentDictionary<string, string> _authenticatedConnections = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 公共方法 - 无需鉴权（如生成登录二维码）
        /// </summary>
        /// <returns>二维码字符串</returns>
        public async Task<string> GenerateLoginQRCode()
        {
            var qrCode = GenerateUniqueQRCode();
            // 将QR码与连接ID关联，用于后续登录确认
            _authenticatedConnections.TryAdd(qrCode, Context.ConnectionId);
            return qrCode;
        }

        /// <summary>
        /// 条件鉴权方法
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <returns>敏感数据</returns>
        public async Task<SensitiveData> GetSensitiveData(int id)
        {
            // 检查连接是否已鉴权
            if (!IsConnectionAuthenticated(Context.ConnectionId))
            {
                throw new HubException("未授权的访问");
            }

            return GetBusinessData(id);
        }

        /// <summary>
        /// 登录确认方法
        /// </summary>
        /// <param name="qrCode">二维码</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ConfirmLogin(string qrCode, string userId)
        {
            if (_authenticatedConnections.TryGetValue(qrCode, out var connectionId) &&
                connectionId == Context.ConnectionId)
            {
                // 标记连接为已鉴权
                MarkConnectionAsAuthenticated(Context.ConnectionId, userId);
                await Clients.Caller.SendAsync("loginConfirmed");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插件特定的方法 - 发送插件通知
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async Task SendPluginNotification(string pluginId, string message)
        {
            await Clients.All.SendAsync("pluginNotificationReceived", new
            {
                PluginId = pluginId,
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 插件特定的方法 - 加入插件房间
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="roomName">房间名称</param>
        /// <returns></returns>
        public async Task JoinPluginRoom(string pluginId, string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{pluginId}-{roomName}");
        }

        /// <summary>
        /// 插件特定的方法 - 离开插件房间
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="roomName">房间名称</param>
        /// <returns></returns>
        public async Task LeavePluginRoom(string pluginId, string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{pluginId}-{roomName}");
        }

        /// <summary>
        /// 插件特定的方法 - 发送房间消息
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="roomName">房间名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async Task SendToPluginRoom(string pluginId, string roomName, string message)
        {
            await Clients.Group($"{pluginId}-{roomName}").SendAsync("pluginRoomMessageReceived", new
            {
                PluginId = pluginId,
                RoomName = roomName,
                Message = message,
                Sender = Context.ConnectionId,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 插件特定的方法 - 调用插件方法
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public async Task InvokePluginMethod(string pluginId, string methodName, object[] args)
        {
            // 这里可以实现插件特定的逻辑
            // 例如，通过某种机制调用插件注册的特定方法
            await Clients.Caller.SendAsync("pluginMethodInvoked", new
            {
                PluginId = pluginId,
                MethodName = methodName,
                Args = args,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 动态鉴权支持
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // 检查连接是否已携带有效的JWT Token
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var token = httpContext.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    // 验证Token并标记连接为已鉴权
                    if (ValidateAndAuthenticateToken(token, Context.ConnectionId))
                    {
                        Console.WriteLine($"[UniversalHub] 连接 {Context.ConnectionId} 已鉴权");
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 连接断开时的处理
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // 清理已鉴权的连接
            var connectionId = Context.ConnectionId;
            var qrCodeEntry = _authenticatedConnections.FirstOrDefault(kvp => kvp.Value == connectionId);
            if (!string.IsNullOrEmpty(qrCodeEntry.Key))
            {
                _authenticatedConnections.TryRemove(qrCodeEntry.Key, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // 内部辅助方法
        private bool IsConnectionAuthenticated(string connectionId)
        {
            // 检查连接是否已通过鉴权
            return _authenticatedConnections.Values.Contains(connectionId);
        }

        private string GenerateUniqueQRCode()
        {
            // 生成唯一的二维码
            return Guid.NewGuid().ToString("N");
        }

        private void StoreQRCodeConnection(string qrCode, string connectionId)
        {
            // 将QR码与连接ID关联
            _authenticatedConnections.TryAdd(qrCode, connectionId);
        }

        private bool ValidateQRCode(string qrCode, string connectionId)
        {
            // 验证QR码与连接ID是否匹配
            return _authenticatedConnections.TryGetValue(qrCode, out var storedConnectionId) &&
                   storedConnectionId == connectionId;
        }

        private void MarkConnectionAsAuthenticated(string connectionId, string userId)
        {
            // 标记连接为已鉴权，这里简化处理，实际项目中可能需要更复杂的逻辑
            // 例如存储用户ID和连接ID的映射关系
            _authenticatedConnections.TryAdd($"user-{userId}", connectionId);
        }

        private bool ValidateAndAuthenticateToken(string token, string connectionId)
        {
            // 验证Token并标记连接为已鉴权
            // 这里简化处理，实际项目中需要实现具体的Token验证逻辑
            // 例如使用JWT验证库来验证Token的有效性
            // 如果验证成功，调用MarkConnectionAsAuthenticated方法标记连接为已鉴权
            // 返回验证结果
            return true; // 简化处理，实际项目中需要实现具体的验证逻辑
        }

        private SensitiveData GetBusinessData(int id)
        {
            // 获取业务数据
            // 这里简化处理，实际项目中需要实现具体的业务逻辑
            return new SensitiveData { Id = id, Data = "Sensitive Data" };
        }
    }

    /// <summary>
    /// 敏感数据类
    /// </summary>
    public class SensitiveData
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }
}
