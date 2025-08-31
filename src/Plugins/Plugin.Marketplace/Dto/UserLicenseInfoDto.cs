using System;

namespace Fastdotnet.Plugin.Marketplace.Dto
{
    /// <summary>
    /// 用户对特定插件的授权信息。
    /// </summary>
    public class UserLicenseInfoDto
    {
        /// <summary>
        /// 用户唯一标识符。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 插件唯一标识符。
        /// </summary>
        public string PluginId { get; set; }

        /// <summary>
        /// 授权类型 (例如: SingleServer, MultiServer)。
        /// </summary>
        public string LicenseType { get; set; }

        /// <summary>
        /// 授权生效日期。
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// 免费更新截止日期。
        /// </summary>
        public DateTime UpdatesUntil { get; set; }
        
        // 可以根据需要添加更多字段，例如是否永久授权等
    }
}