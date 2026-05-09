namespace Fastdotnet.Core.Dtos.Storage
{
    /// <summary>
    /// 上传凭证请求模型
    /// </summary>
    public class UploadCredentialRequest
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// 存储路径前缀（可选），如：plugin-icons/、user-avatars/2024/01/
        /// 注意：不包含bucket名称，仅为bucket内的相对路径
        /// </summary>
        public string? PathPrefix { get; set; }
    }
}