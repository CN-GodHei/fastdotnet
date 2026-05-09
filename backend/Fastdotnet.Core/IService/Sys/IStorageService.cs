
using Fastdotnet.Core.Dtos.Storage;

namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// 存储服务接口，用于文件上传、下载和删除操作
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="pathPrefix">存储路径前缀（可选），如：plugin-icons/、user-avatars/2024/01/</param>
        /// <returns>文件访问URL</returns>
        Task<string> UploadAsync(Stream fileStream, string fileName, string? pathPrefix = null);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>文件字节数组</returns>
        //Task<byte[]> DownloadAsync(string filePath);

        /// <summary>
        /// 下载文件:以流的方式读取文件
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>包含文件流和文件长度的元组</returns>
        Task<(Stream stream, long length)> OpenReadAsync(string filePath);
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string filePath);

        /// <summary>
        /// 获取文件访问URL
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>文件访问URL</returns>
        Task<string> GetFileUrlAsync(string filePath);

        /// <summary>
        /// 生成临时访问链接（预签名URL）
        /// 适用于私有bucket或需要权限控制的文件，生成带过期时间的临时访问链接
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <param name="expiresInMinutes">有效期（分钟），默认60分钟</param>
        /// <returns>临时访问URL</returns>
        Task<string> GeneratePresignedUrlAsync(string filePath, int expiresInMinutes = 60);

        /// <summary>
        /// 获取存储类型标识
        /// </summary>
        string StorageType { get; }

        /// <summary>
        /// 生成上传凭证（用于前端直传）
        /// </summary>
        /// <param name="request">上传凭证请求参数</param>
        /// <returns>上传凭证信息</returns>
        Task<UploadCredentialResponse> GenerateUploadCredentialAsync(UploadCredentialRequest request);
    }
}