
namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// IStorageService扩展方法
    /// </summary>
    public static class IStorageServiceExtensions
    {
        /// <summary>
        /// 上传文件（重载方法，接受byte数组）
        /// </summary>
        /// <param name="storageService">存储服务</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件访问URL</returns>
        public static async Task<string> UploadAsync(this IStorageService storageService, byte[] fileData, string fileName, string? bucketName = null)
        {
            using var stream = new MemoryStream(fileData);
            return await storageService.UploadAsync(stream, fileName, bucketName);
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="storageService">存储服务</param>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件是否存在</returns>
        public static async Task<bool> ExistsAsync(this IStorageService storageService, string fileName, string? bucketName = null)
        {
            try
            {
                var url = await storageService.GetFileUrlAsync(fileName, bucketName);
                // 尝试获取文件，如果成功则说明存在
                // 这里可能需要根据具体实现来判断
                return !string.IsNullOrEmpty(url);
            }
            catch
            {
                return false;
            }
        }
    }
}