using System;

namespace Fastdotnet.Plugin.Marketplace.Dto
{
    // 对应于 Fastdotnet.Plugin.Core 中的 LicenseFile 类
    public class LicenseFileDto
    {
        public string PluginId { get; set; }
        public string UserId { get; set; }
        public string LicenseType { get; set; }
        public string MachineFingerprint { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime UpdatesUntil { get; set; }
        public string Signature { get; set; }
    }
}
