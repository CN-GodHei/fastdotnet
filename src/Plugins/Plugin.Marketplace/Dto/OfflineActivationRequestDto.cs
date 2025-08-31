using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Plugin.Marketplace.Dto
{
    /// <summary>
    /// 离线激活请求数据传输对象。
    /// 对应客户端生成的 request.req 文件内容。
    /// </summary>
    public class OfflineActivationRequestDto
    {
        /// <summary>
        /// 插件唯一标识符。
        /// </summary>
        [Required(ErrorMessage = "PluginId 是必需的。")]
        public string PluginId { get; set; }

        /// <summary>
        /// 用户唯一标识符。
        /// </summary>
        //[Required(ErrorMessage = "UserId 是必需的。")]
        //public string UserId { get; set; }

        /// <summary>
        /// 客户端机器指纹。
        /// </summary>
        [Required(ErrorMessage = "MachineFingerprint 是必需的。")]
        public string MachineFingerprint { get; set; }

        /// <summary>
        /// 客户端生成请求的时间戳 (UTC)。
        /// </summary>
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 可选的附加信息，例如客户端版本、操作系统等。
        /// </summary>
        public string ClientInfo { get; set; }

        /// <summary>
        /// 请求的唯一标识符，用于防止重放攻击。
        /// </summary>
        public string Nonce { get; set; }
    }
}