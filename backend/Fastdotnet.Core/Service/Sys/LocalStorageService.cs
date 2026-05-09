
using Fastdotnet.Core.Dtos.Storage;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Options;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 本地文件存储服务实现
    /// </summary>
    public class LocalStorageService : IStorageService
    {
        private readonly StorageOptions _options;
        IBaseService<FdDictData, string> _service;

        public LocalStorageService(IOptions<StorageOptions> options, IBaseService<FdDictData, string> service)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _service = service;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? pathPrefix = null)
        {
            // 确保目录存在
            var directory = Path.Combine(_options.LocalStoragePath, _options.DefaultBucket);
            if (!string.IsNullOrEmpty(pathPrefix))
            {
                directory = Path.Combine(directory, pathPrefix.TrimStart('/').TrimEnd('/'));
            }
            
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
            var relativePath = Path.Combine(_options.DefaultBucket, pathPrefix ?? "", uniqueFileName).Replace('\\', '/');
            // 读取配置决定使用内网还是外网域名
            var linkType = await _service.GetFirstAsync(w => w.Code == "LocalStorageLinkType");
            var domainCode = linkType?.Value == "outer" ? "CODE_09_02" : "CODE_09_01";
            var siteDomain = await _service.GetFirstAsync(w => w.Code == domainCode);

            return $"{siteDomain?.Value}{_options.BaseUrl}/{relativePath}";
        }

        //public async Task<byte[]> DownloadAsync(string fileName, string? bucketName = null)
        //{
        //    var directory = Path.Combine(_options.LocalStoragePath, bucketName ?? _options.DefaultBucket);
        //    var filePath = Path.Combine(directory, fileName);

        //    if (!File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException($"File not found: {filePath}");
        //    }

        //    return await File.ReadAllBytesAsync(filePath);
        //}

        public async Task<(Stream stream, long length)> OpenReadAsync(string filePath)
        {
            // 处理文件路径：如果filePath以"uploads/"开头，需要移除该前缀
            var relativePath = filePath;
            if (relativePath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(8); // 移除 "uploads/"
            }
            else if (relativePath.StartsWith("uploads\\", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(8); // 移除 "uploads\"
            }
            
            var fullPath = Path.Combine(_options.LocalStoragePath, relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {fullPath}");
            }

            // 打开文件流
            var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true); // 关键：useAsync: true 确保不会阻塞线程

            return (stream, stream.Length);
        }

        public async Task<bool> DeleteAsync(string filePath)
        {
            try
            {
                // 处理文件路径：如果filePath以"uploads/"开头，需要移除该前缀
                // 因为LocalStoragePath已经包含了"uploads"目录
                var relativePath = filePath;
                if (relativePath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(8); // 移除 "uploads/"
                }
                else if (relativePath.StartsWith("uploads\\", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(8); // 移除 "uploads\"
                }
                
                // 构建完整文件路径
                var fullPath = Path.Combine(_options.LocalStoragePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
                
                // 安全检查: 确保路径在允许的目录内
                var fullBasePath = Path.GetFullPath(_options.LocalStoragePath);
                var fullFilePath = Path.GetFullPath(fullPath);
                
                if (!fullFilePath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                
                if (File.Exists(fullFilePath))
                {
                    await Task.Run(() => File.Delete(fullFilePath));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetFileUrlAsync(string filePath)
        {
            // 读取配置决定使用内网还是外网域名
            var linkType = await _service.GetFirstAsync(w => w.Code == "LocalStorageLinkType");
            var domainCode = linkType?.Value == "outer" ? "CODE_09_02" : "CODE_09_01";
            var siteDomain = await _service.GetFirstAsync(w => w.Code == domainCode);
            
            return $"{siteDomain?.Value}{_options.BaseUrl}/{filePath}";
        }

        public string StorageType => "local";

        public async Task<UploadCredentialResponse> GenerateUploadCredentialAsync(UploadCredentialRequest request)
        {
            // 本地存储不支持前端直传，抛出异常表示不支持
            throw new NotImplementedException("本地存储不支持前端直传，应使用后端代理上传");
        }
    }
}