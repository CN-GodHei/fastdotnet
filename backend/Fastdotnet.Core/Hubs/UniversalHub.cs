using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Fastdotnet.Core.Hubs
{
    /// <summary>
    /// 通用SignalR Hub，支持公共访问和鉴权访问，以及插件功能
    /// </summary>
    [SkipAntiReplayAttribute]
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
        /// 插件特定的方法 - 调用插件方法（通用代理）
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数（JSON 数组）</param>
        /// <returns>方法返回值</returns>
        public async Task<object?> InvokePluginMethod(string pluginId, string methodName, JsonElement args)
        {
            try
            {
                Console.WriteLine($"[SignalR] 调用插件方法: {pluginId}.{methodName}");
                Console.WriteLine($"[SignalR] 原始参数: {args.GetRawText()}");
                
                // 将 JsonElement 转换为 object[]
                object[]? argsArray = null;
                if (args.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<object>();
                    foreach (var item in args.EnumerateArray())
                    {
                        list.Add(ConvertJsonElementToObject(item));
                    }
                    argsArray = list.ToArray();
                }
                else if (args.ValueKind != JsonValueKind.Null && args.ValueKind != JsonValueKind.Undefined)
                {
                    argsArray = new object[] { ConvertJsonElementToObject(args) };
                }
                
                Console.WriteLine($"[SignalR] 转换后的参数数量: {argsArray?.Length ?? 0}");
                
                // 通过 PluginMethodRegistry 调用插件注册的方法
                var result = await PluginMethodRegistry.InvokeMethod(pluginId, methodName, argsArray ?? Array.Empty<object>());
                
                Console.WriteLine($"[SignalR] 调用成功: {pluginId}.{methodName}");
                return result;
            }
            catch (InvalidOperationException ex)
            {
                // 方法未注册
                Console.WriteLine($"[SignalR] 方法未注册: {pluginId}.{methodName} - {ex.Message}");
                throw new HubException(ex.Message);
            }
            catch (Exception ex)
            {
                // 其他错误
                Console.WriteLine($"[SignalR] 调用失败: {pluginId}.{methodName} - {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                throw new HubException($"调用插件 {pluginId} 的方法 {methodName} 失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 将 JsonElement 转换为 object
        /// </summary>
        private static object? ConvertJsonElementToObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Object => element,
                JsonValueKind.Array => element,
                _ => element.ToString()
            };
        }

        /// <summary>
        /// 动态鉴权支持
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // SignalR 会自动从 JWT Token 的 Claims 中提取 UserId 并建立映射
            // 无需手动处理，直接使用 Clients.User(userId) 即可发送消息
            
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
