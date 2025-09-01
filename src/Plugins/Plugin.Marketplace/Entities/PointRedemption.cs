using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 积分兑换详情实体
    /// 对应表: PointRedemptions
    /// </summary>
    [SugarTable("mk_point_redemptions", "积分兑换详情")]
    public class PointRedemption : BaseEntity
    {

        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(ColumnName = "purchase_id", IsNullable = false, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 使用的积分数
        /// </summary>
        [SugarColumn(ColumnName = "points_used", IsNullable = false, ColumnDescription = "使用的积分数")]
        public int PointsUsed { get; set; }

        /// <summary>
        /// 用户ID (用于校验积分)
        /// </summary>
        [SugarColumn(ColumnName = "user_id", Length = 128, IsNullable = false, ColumnDescription = "用户ID (用于校验积分)")]
        public string UserId { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        [SugarColumn(ColumnName = "redeemed_time", IsNullable = false, ColumnDescription = "兑换时间")]
        public DateTime RedeemedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建时间
        /// </summary>
        //[SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
        //public DateTime CreateTime { get; set; } = DateTime.Now;

        ///// <summary>
        ///// 最后更新时间
        ///// </summary>
        //[SugarColumn(IsNullable = false, ColumnDescription = "最后更新时间")]
        //public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public UserPluginPurchase Purchase { get; set; }
    }
}