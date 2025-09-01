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
    [SugarTable("mk_user_plugin_activations", "用户插件激活/使用记录")]
    public class UserPluginActivation : BaseEntity
    {

        /// <summary>
        /// 关联的购买记录ID (UserPluginPurchases.Id)
        /// </summary>
        [SugarColumn(ColumnName = "purchase_id", IsNullable = false, IndexGroupNameList = new string[] { "IDX_PurchaseId" }, ColumnDescription = "关联的购买记录ID (UserPluginPurchases.Id)")]
        public long PurchaseId { get; set; }

        /// <summary>
        /// 插件标识符
        /// </summary>
        [SugarColumn(ColumnName = "plugin_id", Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_PluginId" }, ColumnDescription = "插件标识符 (关联 MarketplacePlugins 表的 PluginId)")]
        public string PluginId { get; set; }

        /// <summary>
        /// 用户唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", Length = 128, IsNullable = false, IndexGroupNameList = new string[] { "IDX_UserId_PluginId" }, ColumnDescription = "用户唯一ID (关联用户系统)")]
        public string UserId { get; set; }

        /// <summary>
        /// 激活时使用的授权类型
        /// </summary>
        [SugarColumn(ColumnName = "license_type", Length = 20, IsNullable = false, ColumnDescription = "激活时使用的授权类型 (SingleServer, MultiServer)")]
        public LicenseType LicenseType { get; set; } // 使用枚举类型

        /// <summary>
        /// 激活日期
        /// </summary>
        [SugarColumn(ColumnName = "activation_date", IsNullable = false, ColumnDescription = "激活日期")]
        public DateTime ActivationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 激活时的机器指纹
        /// </summary>
        [SugarColumn(ColumnName = "machine_fingerprint", Length = 255, IsNullable = false, IndexGroupNameList = new string[] { "IDX_machine_fingerprint" }, ColumnDescription = "激活时的机器指纹")]
        public string MachineFingerprint { get; set; }

        /// <summary>
        /// 激活时的IP地址 (IPv4 或 IPv6)
        /// </summary>
        [SugarColumn(ColumnName = "ip_address", Length = 45, IsNullable = true, ColumnDescription = "激活时的IP地址 (IPv4 或 IPv6)")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 激活时的主机名
        /// </summary>
        [SugarColumn(ColumnName = "host_name", Length = 255, IsNullable = true, ColumnDescription = "激活时的主机名")]
        public string Hostname { get; set; }

        /// <summary>
        /// 生成的授权文件内容 (JSON 格式，可选存储)
        /// </summary>
        [SugarColumn(ColumnName = "license_file_content", Length = -1, IsNullable = true, ColumnDescription = "生成的授权文件内容 (JSON 格式，可选存储)")] // -1 表示 LONGTEXT
        public string LicenseFileContent { get; set; }

        /// <summary>
        /// 激活状态
        /// </summary>
        [SugarColumn(ColumnName = "status", Length = 20, IsNullable = false, DefaultValue = "Active", ColumnDescription = "激活状态 (Active, Deactivated, Revoked)")]
        public ActivationStatus Status { get; set; } // 使用枚举类型

        /// <summary>
        /// 停用日期
        /// </summary>
        [SugarColumn(ColumnName = "deactivation_date", IsNullable = true, ColumnDescription = "停用日期")]
        public DateTime? DeactivationDate { get; set; }

        /// <summary>
        /// 撤销原因
        /// </summary>
        [SugarColumn(ColumnName = "revocation_reason", Length = -1, IsNullable = true, ColumnDescription = "撤销原因")] // -1 表示 LONGTEXT
        public string RevocationReason { get; set; }

        /// <summary>
        /// 激活备注
        /// </summary>
        [SugarColumn(ColumnName = "notes", Length = -1, IsNullable = true, ColumnDescription = "激活备注")] // -1 表示 LONGTEXT
        public string Notes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = false, IndexGroupNameList = new string[] { "IDX_create_time" }, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "updatet_ime", IsNullable = false, IndexGroupNameList = new string[] { "IDX_updatet_ime" }, ColumnDescription = "最后更新时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性
        [SugarColumn(IsIgnore = true)]
        public UserPluginPurchase Purchase { get; set; }

        [SugarColumn(IsIgnore = true)]
        public MarketplacePlugin Plugin { get; set; }
    }
}