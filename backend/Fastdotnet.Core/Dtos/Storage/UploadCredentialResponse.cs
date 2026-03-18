namespace Fastdotnet.Core.Dtos.Storage
{
    /// <summary>
    /// 上传凭证响应模型
    /// </summary>
    public class UploadCredentialResponse
    {
        /// <summary>
        /// 凭证类型（local, aliyun, tencent, aws, minio, qiniu等）
        /// </summary>
        public string CredentialType { get; set; } = string.Empty;

        /// <summary>
        /// 上传URL
        /// </summary>
        public string UploadUrl { get; set; } = string.Empty;

        /// <summary>
        /// 上传参数
        /// </summary>
        public Dictionary<string, object> UploadParams { get; set; } = new();

        /// <summary>
        /// 上传头部
        /// </summary>
        public Dictionary<string, string> UploadHeaders { get; set; } = new();

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// 文件访问URL模板
        /// </summary>
        public string FileUrlTemplate { get; set; } = string.Empty;

        /// <summary>
        /// 是否支持前端直传
        /// </summary>
        public bool SupportDirectUpload { get; set; } = true;
    }

    /// <summary>
    /// 当前存储配置响应模型
    /// </summary>
    public class StorageConfigResponse
    {
        /// <summary>
        /// 存储类型（local, aliyun, tencent, aws, minio, qiniu等）
        /// </summary>
        public string StorageType { get; set; } = string.Empty;

        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string? DefaultBucket { get; set; }

        /// <summary>
        /// 访问域名
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// 是否支持前端直传
        /// </summary>
        public bool SupportDirectUpload { get; set; }

        /// <summary>
        /// 配置参数
        /// </summary>
        public Dictionary<string, object> ConfigParams { get; set; } = new();
    }
}