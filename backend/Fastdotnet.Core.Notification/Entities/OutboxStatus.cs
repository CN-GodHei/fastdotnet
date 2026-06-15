namespace Fastdotnet.Core.Notification;

/// <summary>
/// 发件箱消息状态枚举
/// </summary>
public enum OutboxStatus
{
    /// <summary>待处理</summary>
    Pending = 0,

    /// <summary>处理中（已被某节点抢占）</summary>
    Processing = 1,

    /// <summary>已投递成功</summary>
    Published = 2,

    /// <summary>投递失败（等待重试）</summary>
    Failed = 3
}
