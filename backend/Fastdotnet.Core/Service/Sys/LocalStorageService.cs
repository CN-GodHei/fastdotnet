using Fastdotnet.Core.Dto.Storage;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Plugin.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 本地文件存储服务实现
    /// </summary>
    public class LocalStorageService : IStorageService
    {
        private readonly StorageOptions _options;

        public LocalStorageService(IOptions<StorageOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? bucketName = null)
        {
            // 确保目录存在
            var directory = Path.Combine(_options.LocalStoragePath, bucketName ?? _options.DefaultBucket);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 生成唯一文件名
            var uniqueFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(directory, uniqueFileName);

            // 保存文件
            using (var file = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(file);
            }

            // 返回访问URL
            var relativePath = Path.Combine(bucketName ?? _options.DefaultBucket, uniqueFileName).Replace('\\', '/');
            return $"{_options.BaseUrl}/{relativePath}";
        }

        public async Task<byte[]> DownloadAsync(string fileName, string? bucketName = null)
        {
            var directory = Path.Combine(_options.LocalStoragePath, bucketName ?? _options.DefaultBucket);
            var filePath = Path.Combine(directory, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<bool> DeleteAsync(string fileName, string? bucketName = null)
        {
            var directory = Path.Combine(_options.LocalStoragePath, bucketName ?? _options.DefaultBucket);
            var filePath = Path.Combine(directory, fileName);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                return true;
            }

            return false;
        }

        public async Task<string> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            var relativePath = Path.Combine(bucketName ?? _options.DefaultBucket, fileName).Replace('\\', '/');
            return $"{_options.BaseUrl}/{relativePath}";
        }

        public string StorageType => "local";

        public async Task<UploadCredentialResponse> GenerateUploadCredentialAsync(UploadCredentialRequest request)
        {
            // 本地存储不支持前端直传，抛出异常表示不支持
            throw new NotImplementedException("本地存储不支持前端直传，应使用后端代理上传");
        }
    }

    /// <summary>
    /// 本地存储配置选项
    /// </summary>
    public class StorageOptions
    {
        /// <summary>
        /// 本地存储路径
        /// </summary>
        public string LocalStoragePath { get; set; } = "wwwroot/uploads";

        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = "/uploads";

        /// <summary>
        /// 默认存储桶
        /// </summary>
        public string DefaultBucket { get; set; } = "default";
    }
}