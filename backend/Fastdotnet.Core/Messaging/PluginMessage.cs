namespace Fastdotnet.Core.Messaging
{
    /// <summary>
    /// 插件消息基类 - 所有插件消息都应继承此类
    /// </summary>
    /// <typeparam name="T">消息数据类型</typeparam>
    public class PluginMessage<T>
    {
        /// <summary>
        /// 插件 ID（唯一标识）
        /// </summary>
        public string PluginId { get; set; } = string.Empty;

        /// <summary>
        /// 插件名称（友好显示）
        /// </summary>
        public string PluginName { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型（用于前端区分不同的消息）
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// 消息数据
        /// </summary>
        public T Data { get; set; } = default!;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 目标用户 ID（可选，为空则广播给所有人）
        /// </summary>
        public string? TargetUserId { get; set; }

        /// <summary>
        /// 额外元数据
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }

    /// <summary>
    /// 插件消息（非泛化版本，用于传输对象）
    /// </summary>
    public class PluginMessage
    {
        /// <summary>
        /// 插件 ID（唯一标识）
        /// </summary>
        public string PluginId { get; set; } = string.Empty;

        /// <summary>
        /// 插件名称（友好显示）
        /// </summary>
        public string PluginName { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型（用于前端区分不同的消息）
        /// </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// 消息数据（JSON 字符串）
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 目标用户 ID（可选，为空则广播给所有人）
        /// </summary>
        public string? TargetUserId { get; set; }

        /// <summary>
        /// 额外元数据
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }

    /// <summary>
    /// 常用的插件消息数据类型
    /// </summary>
    public static class PluginMessageTypes
    {
        /// <summary>
        /// 通知消息
        /// </summary>
        public const string Notification = "notification";

        /// <summary>
        /// 状态更新
        /// </summary>
        public const string StatusUpdate = "status_update";

        /// <summary>
        /// 进度更新
        /// </summary>
        public const string ProgressUpdate = "progress_update";

        /// <summary>
        /// 错误消息
        /// </summary>
        public const string Error = "error";

        /// <summary>
        /// 成功消息
        /// </summary>
        public const string Success = "success";

        /// <summary>
        /// 警告消息
        /// </summary>
        public const string Warning = "warning";

        /// <summary>
        /// 数据变更
        /// </summary>
        public const string DataChanged = "data_changed";

        /// <summary>
        /// 操作完成
        /// </summary>
        public const string OperationCompleted = "operation_completed";
    }

    /// <summary>
    /// 简单的文本通知消息
    /// </summary>
    public class TextNotification
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 消息级别（info, success, warning, error）
        /// </summary>
        public string Level { get; set; } = "info";

        /// <summary>
        /// 是否可关闭
        /// </summary>
        public bool Dismissible { get; set; } = true;

        /// <summary>
        /// 自动关闭时间（毫秒），0 表示不自动关闭
        /// </summary>
        public int AutoCloseDelay { get; set; } = 3000;
    }

    /// <summary>
    /// 进度更新消息
    /// </summary>
    public class ProgressData
    {
        /// <summary>
        /// 进度描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 当前进度值
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// 总进度值
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 进度百分比（0-100）
        /// </summary>
        public int Percentage => Total > 0 ? (int)((double)Current / Total * 100) : 0;

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get; set; } = false;
    }

    /// <summary>
    /// 数据变更消息
    /// </summary>
    public class DataChangeInfo
    {
        /// <summary>
        /// 变更类型（create, update, delete）
        /// </summary>
        public string ChangeType { get; set; } = string.Empty;

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 数据 ID
        /// </summary>
        public string DataId { get; set; } = string.Empty;

        /// <summary>
        /// 变更后的数据（可选）
        /// </summary>
        public object? NewData { get; set; }

        /// <summary>
        /// 变更前的数据（可选）
        /// </summary>
        public object? OldData { get; set; }
    }
}
