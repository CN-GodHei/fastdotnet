namespace Fastdotnet.Plugin.Marketplace.Dto
{
    /// <summary>
    /// 离线激活响应数据传输对象。
    /// 包含生成的许可证文件内容。
    /// </summary>
    public class OfflineActivationResponseDto
    {
        /// <summary>
        /// 生成的许可证文件内容 (JSON 格式)。
        /// </summary>
        public LicenseFileDto LicenseFile { get; set; }

        /// <summary>
        /// 激活请求的唯一标识符。
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 服务器处理响应的时间戳 (UTC)。
        /// </summary>
        public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;
    }
}