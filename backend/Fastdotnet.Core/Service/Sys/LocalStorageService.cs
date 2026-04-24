
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

        public async Task<(Stream stream, long length)> OpenReadAsync(string fileName, string? bucketName = null)
        {
            var directory = Path.Combine(_options.LocalStoragePath, bucketName ?? _options.DefaultBucket);
            var filePath = Path.Combine(directory, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // 打开文件流
            var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true); // 关键：useAsync: true 确保不会阻塞线程

            return (stream, stream.Length);
        }

        public async Task<bool> DeleteAsync(string fileName, string? bucketName = null)
        {
            try
            {
                // fileName 可能是完整路径(如: uploads/default/xxx.png) 或相对路径(如: default/xxx.png)
                // 需要正确解析
                
                string relativePath;
                
                // 如果 fileName 以 "uploads/" 开头,说明是完整URL路径,需要去掉
                if (fileName.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = fileName.Substring("uploads/".Length);
                }
                else
                {
                    relativePath = fileName;
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

        public async Task<string> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            var relativePath = Path.Combine(bucketName ?? _options.DefaultBucket, fileName).Replace('\\', '/');
            // 读取配置决定使用内网还是外网域名
            var linkType = await _service.GetFirstAsync(w => w.Code == "LocalStorageLinkType");
            var domainCode = linkType?.Value == "outer" ? "CODE_09_02" : "CODE_09_01";
            var siteDomain = await _service.GetFirstAsync(w => w.Code == domainCode);
            
            return $"{siteDomain?.Value}{_options.BaseUrl}/{relativePath}";
        }

        public string StorageType => "local";

        public async Task<UploadCredentialResponse> GenerateUploadCredentialAsync(UploadCredentialRequest request)
        {
            // 本地存储不支持前端直传，抛出异常表示不支持
            throw new NotImplementedException("本地存储不支持前端直传，应使用后端代理上传");
        }
    }
}