using Fastdotnet.Core.IService.Sys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        private bool _initialized = false;

        public StorageServiceProxy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // 延迟初始化，避免在构造函数中产生循环依赖
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                lock (_lock)
                {
                    if (!_initialized)
                    {
                        // 使用IServiceScopeFactory创建一个临时作用域来获取默认实现
                        // 这样可以避免直接从根ServiceProvider获取IStorageService导致的循环依赖
                        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                        using var scope = scopeFactory.CreateScope();
                        
                        // 尝试获取默认的本地存储服务实现（LocalStorageService）
                        // 在依赖注入配置中，LocalStorageService应该是IStorageService的默认实现
                        _currentStorageService = scope.ServiceProvider.GetService<LocalStorageService>();
                        
                        // 如果获取不到LocalStorageService，说明可能有插件替换了默认实现
                        if (_currentStorageService == null)
                        {
                            // 在临时作用域内获取当前注册的IStorageService实现
                            // 这样即使有插件激活，也能获取到正确的实现
                            _currentStorageService = scope.ServiceProvider.GetService<IStorageService>();
                            
                            // 如果获取到的仍然是StorageServiceProxy，说明还没有任何其他实现注册
                            // 这种情况不应该发生，但为了安全起见，创建一个LocalStorageService实例
                            if (_currentStorageService is StorageServiceProxy)
                            {
                                var options = scope.ServiceProvider.GetService<IOptions<StorageOptions>>();
                                _currentStorageService = new LocalStorageService(options);
                            }
                        }
                        
                        _initialized = true;
                    }
                }
            }
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? bucketName = null)
        {
            EnsureInitialized();
            return await _currentStorageService.UploadAsync(fileStream, fileName, bucketName);
        }

        public async Task<byte[]> DownloadAsync(string fileName, string? bucketName = null)
        {
            EnsureInitialized();
            return await _currentStorageService.DownloadAsync(fileName, bucketName);
        }

        public async Task<bool> DeleteAsync(string fileName, string? bucketName = null)
        {
            EnsureInitialized();
            return await _currentStorageService.DeleteAsync(fileName, bucketName);
        }

        public async Task<string> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            EnsureInitialized();
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
                _initialized = true; // 标记为已初始化，因为我们已经设置了服务
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
                
                // 尝试获取默认的本地存储服务实现
                var defaultStorageService = scope.ServiceProvider.GetService<LocalStorageService>();
                
                // 如果获取不到LocalStorageService，说明可能有插件替换了默认实现
                if (defaultStorageService == null)
                {
                    // 在临时作用域内获取当前注册的IStorageService实现
                    var service = scope.ServiceProvider.GetService<IStorageService>();
                    
                    // 如果获取到的仍然是StorageServiceProxy，说明还没有任何其他实现注册
                    if (!(service is StorageServiceProxy))
                    {
                        _currentStorageService = service;
                    }
                    else
                    {
                        // 创建默认的LocalStorageService实例
                        var options = scope.ServiceProvider.GetService<IOptions<StorageOptions>>();
                        _currentStorageService = new LocalStorageService(options);
                    }
                }
                else
                {
                    _currentStorageService = defaultStorageService;
                }
                
                _initialized = true; // 标记为已初始化
            }
        }
    }
}