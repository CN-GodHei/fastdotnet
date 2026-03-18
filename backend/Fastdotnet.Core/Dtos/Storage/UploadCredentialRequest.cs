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
        /// 存储桶名称
        /// </summary>
        public string? BucketName { get; set; }

        /// <summary>
        /// OSS类型（aliyun, tencent, aws, minio, qiniu等）
        /// </summary>
        public string? OssType { get; set; }
    }
}