using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 用户插件购买记录实体 (主表)
    /// 对应表: UserPluginPurchases
    /// </summary>
    [SugarTable("mk_user_plugin_purchases", "用户插件购买记录")]
    public class UserPluginPurchase : BaseEntity
    {

        /// <summary>
        /// 用户唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_UserId" }, ColumnDescription = "用户唯一ID (关联用户系统)")]
        public string UserId { get; set; }

        /// <summary>
        /// 插件标识符
        /// </summary>
        [SugarColumn(ColumnName = "plugin_id", Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_PluginId" }, ColumnDescription = "插件标识符 (关联 MarketplacePlugins 表的 PluginId)")]
        public string PluginId { get; set; }

        /// <summary>
        /// 关联的订单号
        /// </summary>
        [SugarColumn(ColumnName = "order_id", Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_OrderId" }, ColumnDescription = "关联的订单号 (关联支付系统)")]
        public string OrderId { get; set; }

        /// <summary>
        /// 购买的授权类型
        /// </summary>
        [SugarColumn(ColumnName = "license_type", Length = 20, IsNullable = false, ColumnDescription = "授权类型 (SingleServer, MultiServer)")]
        public LicenseType LicenseType { get; set; } // 使用枚举类型

        /// <summary>
        /// 购买数量 (对于 MultiServer 可能有用)
        /// </summary>
        [SugarColumn(ColumnName = "quantity", IsNullable = false, DefaultValue = "1", ColumnDescription = "购买数量 (对于 MultiServer 可能有用)")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// 实际购买价格
        /// </summary>
        [SugarColumn(ColumnName = "purchase_price", Length = 10, IsNullable = false, ColumnDescription = "实际购买价格")]
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 货币类型
        /// </summary>
        [SugarColumn(ColumnName = "currency", Length = 10, IsNullable = false, DefaultValue = "CNY", ColumnDescription = "货币类型")]
        public string Currency { get; set; } = "CNY";

        /// <summary>
        /// 购买日期
        /// </summary>
        [SugarColumn(ColumnName = "purchase_date", IsNullable = false, ColumnDescription = "购买日期")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 此购买包含的免费更新截止日期
        /// </summary>
        [SugarColumn(ColumnName = "updates_until", IsNullable = false, ColumnDescription = "此购买包含的免费更新截止日期")]
        public DateTime UpdatesUntil { get; set; }

        /// <summary>
        /// 是否为永久授权 (true: 是, false: 否)
        /// </summary>
        [SugarColumn(ColumnName = "is_lifetime", IsNullable = false, DefaultValue = "0", ColumnDescription = "是否为永久授权 (1: 是, 0: 否)")]
        public bool IsLifetime { get; set; } = false;

        /// <summary>
        /// 购买状态
        /// </summary>
        [SugarColumn(ColumnName = "status", Length = 20, IsNullable = false, DefaultValue = "Completed", ColumnDescription = "购买状态 (Completed, Refunded, Cancelled)")]
        public PurchaseStatus Status { get; set; } // 使用枚举类型

        /// <summary>
        /// 购买备注
        /// </summary>
        [SugarColumn(ColumnName = "notes", Length = -1, IsNullable = true, ColumnDescription = "购买备注")] // -1 表示 LONGTEXT
        public string Notes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = false, IndexGroupNameList = new string[] { "IDX_create_time" }, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = false, IndexGroupNameList = new string[] { "IDX_update_time" }, ColumnDescription = "最后更新时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public MarketplacePlugin Plugin { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<UserPluginActivation> Activations { get; set; }

        // 新的导航属性，关联到不同的支付详情表
        [SugarColumn(IsIgnore = true)]
        public OnlinePayment OnlinePayment { get; set; }

        [SugarColumn(IsIgnore = true)]
        public PointRedemption PointRedemption { get; set; }

        [SugarColumn(IsIgnore = true)]
        public GiftRecord GiftRecord { get; set; }
    }
}