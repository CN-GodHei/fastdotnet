using Fastdotnet.Core.IService.Sys;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 存储服务代理，用于在运行时切换实际的存储服务实现
    /// </summary>
    public class StorageServiceProxy : IStorageService
    {
        private readonly IServiceProvider _serviceProvider;
        private IStorageService _currentStorageService;
        private readonly object _lock = new object();

        public StorageServiceProxy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // 初始化时使用默认的存储服务实现
            _currentStorageService = serviceProvider.GetService<IStorageService>();
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? bucketName = null)
        {
            return await _currentStorageService.UploadAsync(fileStream, fileName, bucketName);
        }

        public async Task<byte[]> DownloadAsync(string fileName, string? bucketName = null)
        {
            return await _currentStorageService.DownloadAsync(fileName, bucketName);
        }

        public async Task<bool> DeleteAsync(string fileName, string? bucketName = null)
        {
            return await _currentStorageService.DeleteAsync(fileName, bucketName);
        }

        public async Task<string> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            return await _currentStorageService.GetFileUrlAsync(fileName, bucketName);
        }

        /// <summary>
        /// 切换到指定的存储服务实现
        /// </summary>
        /// <param name="newStorageService">新的存储服务实现</param>
        public void SwitchTo(IStorageService newStorageService)
        {
            lock (_lock)
            {
                _currentStorageService = newStorageService;
            }
        }

        /// <summary>
        /// 恢复到默认的存储服务实现
        /// </summary>
        public void RestoreDefault()
        {
            lock (_lock)
            {
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                var defaultStorageService = scope.ServiceProvider.GetService<IStorageService>();
                
                if (defaultStorageService != null)
                {
                    _currentStorageService = defaultStorageService;
                }
            }
        }
    }
}