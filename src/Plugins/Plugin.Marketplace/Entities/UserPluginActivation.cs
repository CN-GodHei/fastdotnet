using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 用户插件激活/使用记录实体
    /// 对应表: UserPluginActivations
    /// </summary>
    [SugarTable("UserPluginActivations")]
    public class UserPluginActivation : BaseEntity
    {

        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(IsNullable = false, IndexGroupNameList = new string[] { "IDX_PurchaseId" }, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 插件标识符
        /// </summary>
        [SugarColumn(Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_PluginId" }, ColumnDescription = "插件标识符 (关联 MarketplacePlugins 表的 PluginId)")]
        public string PluginId { get; set; }

        /// <summary>
        /// 用户唯一ID
        /// </summary>
        [SugarColumn(Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_UserId_PluginId" }, ColumnDescription = "用户唯一ID (关联用户系统)")]
        public string UserId { get; set; }

        /// <summary>
        /// 激活时使用的授权类型
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = false, ColumnDescription = "激活时使用的授权类型 (SingleServer, MultiServer)")]
        public LicenseType LicenseType { get; set; } // 使用枚举类型

        /// <summary>
        /// 激活日期
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "激活日期")]
        public DateTime ActivationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 激活时的机器指纹
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = false, IndexGroupNameList = new string[] { "IDX_MachineFingerprint" }, ColumnDescription = "激活时的机器指纹")]
        public string MachineFingerprint { get; set; }

        /// <summary>
        /// 激活时的IP地址 (IPv4 或 IPv6)
        /// </summary>
        [SugarColumn(Length = 45, IsNullable = true, ColumnDescription = "激活时的IP地址 (IPv4 或 IPv6)")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 激活时的主机名
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "激活时的主机名")]
        public string Hostname { get; set; }

        /// <summary>
        /// 生成的授权文件内容 (JSON 格式，可选存储)
        /// </summary>
        [SugarColumn(Length = -1, IsNullable = true, ColumnDescription = "生成的授权文件内容 (JSON 格式，可选存储)")] // -1 表示 LONGTEXT
        public string LicenseFileContent { get; set; }

        /// <summary>
        /// 激活状态
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = false, DefaultValue = "Active", ColumnDescription = "激活状态 (Active, Deactivated, Revoked)")]
        public ActivationStatus Status { get; set; } // 使用枚举类型

        /// <summary>
        /// 停用日期
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "停用日期")]
        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// 撤销原因
        /// </summary>
        [SugarColumn(Length = -1, IsNullable = true, ColumnDescription = "撤销原因")] // -1 表示 LONGTEXT
        public string RevocationReason { get; set; }

        /// <summary>
        /// 激活备注
        /// </summary>
        [SugarColumn(Length = -1, IsNullable = true, ColumnDescription = "激活备注")] // -1 表示 LONGTEXT
        public string Notes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = false, IndexGroupNameList = new string[] { "IDX_CreatedTime" }, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsNullable = false, IndexGroupNameList = new string[] { "IDX_UpdatedTime" }, ColumnDescription = "最后更新时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public UserPluginPurchase Purchase { get; set; }

        [SugarColumn(IsIgnore = true)]
        public MarketplacePlugin Plugin { get; set; }
    }
}