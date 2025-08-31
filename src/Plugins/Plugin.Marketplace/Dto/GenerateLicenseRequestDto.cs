using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Plugin.Marketplace.Dto
{
    public class GenerateLicenseRequestDto
    {
        [Required(ErrorMessage = "PluginId 是必需的。")]
        public string PluginId { get; set; }

        //[Required(ErrorMessage = "UserId 是必需的。")]
        //public string UserId { get; set; }

        /// <summary>
        /// 客户端机器指纹。对于 SingleServer 授权是必需的，
        /// 对于 MultiServer 授权可能不直接使用，但仍是必填项以备将来扩展或审计。
        /// </summary>
        [Required(ErrorMessage = "MachineFingerprint 是必需的。")]
        public string MachineFingerprint { get; set; }
    }
}