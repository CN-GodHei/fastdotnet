using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 积分兑换详情实体
    /// 对应表: PointRedemptions
    /// </summary>
    [SugarTable("PointRedemptions")]
    public class PointRedemption : BaseEntity
    {

        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 使用的积分数
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "使用的积分数")]
        public int PointsUsed { get; set; }

        /// <summary>
        /// 用户ID (用于校验积分)
        /// </summary>
        [SugarColumn(Length = 128, IsNullable = false, ColumnDescription = "用户ID (用于校验积分)")]
        public string UserId { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "兑换时间")]
        public DateTime RedeemedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "最后更新时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public UserPluginPurchase Purchase { get; set; }
    }
}