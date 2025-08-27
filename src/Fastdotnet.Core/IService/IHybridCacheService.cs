using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService
{
    /// <summary>
    /// 混合缓存服务接口
    /// </summary>
    public interface IHybridCacheService
    {
        /// <summary>
        /// 获取或创建缓存项
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">缓存项工厂函数</param>
        /// <param name="options">缓存选项</param>
        /// <param name="tags">标签数组</param>
        /// <returns>缓存项</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, HybridCacheEntryOptions options = null, string[] tags = null);

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="options">缓存选项</param>
        /// <param name="tags">标签数组</param>
        Task SetAsync<T>(string key, T value, HybridCacheEntryOptions options = null, string[] tags = null);

        /// <summary>
        /// 获取缓存项
        /// 注意：HybridCache没有直接的Get方法，此方法通过GetOrCreateAsync实现，
        /// 如果缓存中不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">缓存项类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存项，如果不存在则返回默认值</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 移除缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// 根据标签移除缓存项
        /// </summary>
        /// <param name="tag">标签</param>
        Task RemoveByTagAsync(string tag);
    }
}