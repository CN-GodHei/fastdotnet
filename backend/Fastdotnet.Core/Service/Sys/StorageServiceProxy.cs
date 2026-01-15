using Fastdotnet.Core.Dto.Storage;
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

        public StorageServiceProxy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IStorageService GetCurrentStorageService()
        {
            // 检查当前服务是否为StorageServiceProxy自身（即未初始化或需要更新的情况）
            // 如果是，则获取当前注册的默认实现
            if (_currentStorageService == null || _currentStorageService is StorageServiceProxy)
            {
                lock (_lock)
                {
                    // 双重检查锁定
                    if (_currentStorageService == null || _currentStorageService is StorageServiceProxy)
                    {
                        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                        using var scope = scopeFactory.CreateScope();
                        
                        // 尝试获取默认的本地存储服务实现（LocalStorageService）
                        var localStorageService = scope.ServiceProvider.GetService<LocalStorageService>();
                        
                        if (localStorageService != null)
                        {
                            // 如果存在LocalStorageService实例，使用它
                            _currentStorageService = localStorageService;
                        }
                        else
                        {
                            // 否则获取当前注册的IStorageService实现（可能是插件提供的）
                            var service = scope.ServiceProvider.GetService<IStorageService>();
                            
                            // 确保获取到的不是StorageServiceProxy自身
                            if (service != null && !(service is StorageServiceProxy))
                            {
                                _currentStorageService = service;
                            }
                            else
                            {
                                // 如果还是获取不到合适的实现，创建默认的LocalStorageService
                                var options = scope.ServiceProvider.GetService<IOptions<StorageOptions>>();
                                _currentStorageService = new LocalStorageService(options);
                            }
                        }
                    }
                }
            }
            
            return _currentStorageService;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? bucketName = null)
        {
            var service = GetCurrentStorageService();
            Console.WriteLine($"Current storage service implementation: {service.GetType().FullName}");
            return await service.UploadAsync(fileStream, fileName, bucketName);
        }

        public async Task<byte[]> DownloadAsync(string fileName, string? bucketName = null)
        {
            var service = GetCurrentStorageService();
            return await service.DownloadAsync(fileName, bucketName);
        }

        public async Task<bool> DeleteAsync(string fileName, string? bucketName = null)
        {
            var service = GetCurrentStorageService();
            return await service.DeleteAsync(fileName, bucketName);
        }

        public async Task<string> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            var service = GetCurrentStorageService();
            return await service.GetFileUrlAsync(fileName, bucketName);
        }

        public string StorageType
        {
            get
            {
                var service = GetCurrentStorageService();
                // 如果当前服务实现了IStorageService接口的StorageType属性，则返回其值
                if (service is IStorageService storageService)
                {
                    return storageService.StorageType;
                }
                // 否则返回默认值
                return "unknown";
            }
        }

        public async Task<UploadCredentialResponse> GenerateUploadCredentialAsync(UploadCredentialRequest request)
        {
            var service = GetCurrentStorageService();
            // 如果当前服务实现了IStorageService接口的GenerateUploadCredentialAsync方法，则调用它
            if (service is IStorageService storageService)
            {
                return await storageService.GenerateUploadCredentialAsync(request);
            }
            // 否则抛出异常
            throw new NotImplementedException("当前存储服务不支持生成上传凭证");
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
                
                // 尝试获取默认的本地存储服务实现
                var defaultStorageService = scope.ServiceProvider.GetService<LocalStorageService>();
                
                if (defaultStorageService != null)
                {
                    _currentStorageService = defaultStorageService;
                }
                else
                {
                    // 获取当前注册的IStorageService实现（可能是插件提供的）
                    var service = scope.ServiceProvider.GetService<IStorageService>();
                    
                    // 确保获取到的不是StorageServiceProxy自身
                    if (service != null && !(service is StorageServiceProxy))
                    {
                        _currentStorageService = service;
                    }
                    else
                    {
                        // 如果还是获取不到合适的实现，创建默认的LocalStorageService
                        var options = scope.ServiceProvider.GetService<IOptions<StorageOptions>>();
                        _currentStorageService = new LocalStorageService(options);
                    }
                }
            }
        }
    }
}