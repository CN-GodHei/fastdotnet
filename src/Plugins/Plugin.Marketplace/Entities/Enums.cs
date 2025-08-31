namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 插件支持的授权模式
    /// </summary>
    public enum SupportedLicenseMode
    {
        /// <summary>
        /// 仅支持单服务器授权
        /// </summary>
        SingleServerOnly,

        /// <summary>
        /// 仅支持多服务器授权
        /// </summary>
        MultiServerOnly,

        /// <summary>
        /// 同时支持单服务器和多服务器授权
        /// </summary>
        Both
    }

    /// <summary>
    /// 授权类型枚举 (用于具体的许可证实例)
    /// </summary>
    public enum LicenseType
    {
        /// <summary>
        /// 单服务器授权
        /// </summary>
        SingleServer,

        /// <summary>
        /// 多服务器授权
        /// </summary>
        MultiServer
    }

    /// <summary>
    /// 用户插件购买记录状态枚举
    /// </summary>
    public enum PurchaseStatus
    {
        /// <summary>
        /// 已完成
        /// </summary>
        Completed,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// 用户插件激活记录状态枚举
    /// </summary>
    public enum ActivationStatus
    {
        /// <summary>
        /// 激活中
        /// </summary>
        Active,

        /// <summary>
        /// 已停用
        /// </summary>
        Deactivated,

        /// <summary>
        /// 已撤销
        /// </summary>
        Revoked
    }

    /// <summary>
    /// 在线支付渠道枚举
    /// </summary>
    public enum OnlinePaymentChannel
    {
        /// <summary>
        /// 未知或未指定
        /// </summary>
        Unknown,

        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay,

        /// <summary>
        /// 微信支付
        /// </summary>
        WeChatPay,

        /// <summary>
        /// Stripe
        /// </summary>
        Stripe,

        /// <summary>
        /// PayPal
        /// </summary>
        PayPal
        // 可以根据需要添加更多渠道
    }
}