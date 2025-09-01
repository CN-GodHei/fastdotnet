using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Fastdotnet.Plugin.Marketplace.Entities
{
    /// <summary>
    /// 插件市场插件信息实体
    /// 对应表: MarketplacePlugins
    /// </summary>
    [SugarTable("mk_marketplace_plugins", "插件市场插件信息")]
    public class MarketplacePlugin : BaseEntity
    {
        /// <summary>
        /// 插件标识符 (例如: Plugin.A.Admin)
        /// </summary>
        [SugarColumn(ColumnName = "plugin_id", Length = 128, IsNullable = false, UniqueGroupNameList = new string[] { "UK_PluginId" }, ColumnDescription = "插件标识符 (例如: Plugin.A.Admin, 用于标识插件包)")]
        public string PluginId { get; set; }

        /// <summary>
        /// 插件显示名称
        /// </summary>
        [SugarColumn(ColumnName = "name", Length = 255, IsNullable = false, ColumnDescription = "插件显示名称")]
        public string Name { get; set; }

        /// <summary>
        /// 插件详细描述
        /// </summary>
        [SugarColumn(ColumnName = "description", Length = -1, IsNullable = true, ColumnDescription = "插件详细描述")] // -1 表示 LONGTEXT
        public string Description { get; set; }

        /// <summary>
        /// 当前最新版本号
        /// </summary>
        [SugarColumn(ColumnName = "version", Length = 50, IsNullable = false, ColumnDescription = "当前最新版本号")]
        public string Version { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        [SugarColumn(ColumnName = "author", Length = 255, IsNullable = true, ColumnDescription = "插件作者")]
        public string Author { get; set; }

        /// <summary>
        /// 插件分类
        /// </summary>
        [SugarColumn(ColumnName = "category", Length = 100, IsNullable = true, ColumnDescription = "插件分类 (例如: CMS, ECommerce, Tools)")]
        public string Category { get; set; }

        /// <summary>
        /// 插件支持的授权模式
        /// </summary>
        [SugarColumn(ColumnName = "supported_license_mode", Length = 20, IsNullable = false, DefaultValue = "Both", ColumnDescription = "插件支持的授权模式 (SingleServerOnly, MultiServerOnly, Both)")]
        public SupportedLicenseMode SupportedLicenseMode { get; set; } = SupportedLicenseMode.Both;

        /// <summary>
        /// 单服务器授权价格
        /// </summary>
        [SugarColumn(ColumnName = "price_single_server", Length = 10, IsNullable = false, DefaultValue = "0.00", ColumnDescription = "单服务器授权价格")]
        public decimal Price_SingleServer { get; set; }

        /// <summary>
        /// 多服务器授权价格
        /// </summary>
        [SugarColumn(ColumnName = "price_multi_server", Length = 10, IsNullable = false, DefaultValue = "0.00", ColumnDescription = "多服务器授权价格")]
        public decimal Price_MultiServer { get; set; }

        /// <summary>
        /// 是否激活/上架 (true: 是, false: 否)
        /// </summary>
        [SugarColumn(ColumnName = "is_active", IsNullable = false, DefaultValue = "1", ColumnDescription = "是否激活/上架 (1: 是, 0: 否)")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 是否免费插件 (true: 是, false: 否)
        /// </summary>
        [SugarColumn(ColumnName = "is_free", IsNullable = false, DefaultValue = "0", ColumnDescription = "是否免费插件 (1: 是, 0: 否)")]
        public bool IsFree { get; set; } = false;

        /// <summary>
        /// 插件包下载地址
        /// </summary>
        [SugarColumn(ColumnName = "download_url", Length = 512, IsNullable = true, ColumnDescription = "插件包下载地址 (可以是相对路径或CDN地址)")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 插件文档地址
        /// </summary>
        [SugarColumn(ColumnName = "documentation_url", Length = 512, IsNullable = true, ColumnDescription = "插件文档地址")]
        public string DocumentationUrl { get; set; }

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
        public List<UserPluginPurchase> Purchases { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<UserPluginActivation> Activations { get; set; }
    }
}