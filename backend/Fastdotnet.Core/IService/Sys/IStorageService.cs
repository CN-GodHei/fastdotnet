using System;
using System.IO;
using System.Threading.Tasks;

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
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件访问URL</returns>
        Task<string> UploadAsync(Stream fileStream, string fileName, string? bucketName = null);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件字节数组</returns>
        Task<byte[]> DownloadAsync(string fileName, string? bucketName = null);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(string fileName, string? bucketName = null);

        /// <summary>
        /// 获取文件访问URL
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件访问URL</returns>
        Task<string> GetFileUrlAsync(string fileName, string? bucketName = null);
    }
}