using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 活动赠送详情实体
    /// 对应表: GiftRecords
    /// </summary>
    [SugarTable("mk_Gift_Records", "活动赠送详情")]
    public class GiftRecord : BaseEntity
    {
        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(ColumnName = "purchase_id", IsNullable = false, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 赠送原因
        /// </summary>
        [SugarColumn(ColumnName = "reason", Length = 255, IsNullable = true, ColumnDescription = "赠送原因")]
        public string Reason { get; set; }

        /// <summary>
        /// 赠送人/系统标识
        /// </summary>
        [SugarColumn(ColumnName = "given_by", Length = 128, IsNullable = true, ColumnDescription = "赠送人/系统标识")]
        public string GivenBy { get; set; }

        /// <summary>
        /// 赠送时间
        /// </summary>
        [SugarColumn(ColumnName = "given_time", IsNullable = false, ColumnDescription = "赠送时间")]
        public DateTime GivenTime { get; set; } = DateTime.Now;

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