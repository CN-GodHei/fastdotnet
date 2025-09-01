using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 在线支付详情实体
    /// 对应表: OnlinePayments
    /// </summary>
    [SugarTable("mk_online_payments", "在线支付详情")]
    public class OnlinePayment : BaseEntity
    {

        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(ColumnName = "purchase_id", IsNullable = false, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        [SugarColumn(ColumnName = "channel", Length = 20, IsNullable = false, ColumnDescription = "支付渠道 (Alipay, WeChatPay, ...)")]
        public OnlinePaymentChannel Channel { get; set; } = OnlinePaymentChannel.Unknown;

        /// <summary>
        /// 第三方支付平台交易号
        /// </summary>
        [SugarColumn(ColumnName = "transaction_id", Length = 128, IsNullable = false, UniqueGroupNameList = new string[] { "UK_TransactionId" }, ColumnDescription = "第三方支付平台交易号")]
        public string TransactionId { get; set; }

        /// <summary>
        /// 第三方支付平台返回的原始数据 (JSON格式)
        /// </summary>
        [SugarColumn(ColumnName = "third_party_data", Length = -1, IsNullable = true, ColumnDescription = "第三方支付平台返回的原始数据 (JSON格式)")]
        public string ThirdPartyData { get; set; }

        /// <summary>
        /// 支付回调时的原始数据 (JSON格式)
        /// </summary>
        [SugarColumn(ColumnName = "callback_data", Length = -1, IsNullable = true, ColumnDescription = "支付回调时的原始数据 (JSON格式)")]
        public string CallbackData { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        [SugarColumn(ColumnName = "paid_time", IsNullable = true, ColumnDescription = "支付完成时间")]
        public DateTime? PaidTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = false, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = false, ColumnDescription = "最后更新时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public UserPluginPurchase Purchase { get; set; }
    }
}