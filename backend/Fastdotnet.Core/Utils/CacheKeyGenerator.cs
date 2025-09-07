using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 缓存键生成器
    /// </summary>
    public static class CacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="prefix">键前缀</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>缓存键</returns>
        public static string GenerateKey(string prefix, params object[] parameters)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

            if (parameters == null || parameters.Length == 0)
                return prefix;

            var paramStrings = parameters.Select(p => p?.ToString() ?? "null");
            return $"{prefix}:{string.Join(":", paramStrings)}";
        }

        /// <summary>
        /// 生成带标签的缓存键
        /// </summary>
        /// <param name="prefix">键前缀</param>
        /// <param name="tags">标签数组</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>缓存键</returns>
        public static string GenerateKeyWithTag(string prefix, string[] tags, params object[] parameters)
        {
            var key = GenerateKey(prefix, parameters);
            if (tags != null && tags.Length > 0)
            {
                return $"{key}@{string.Join(",", tags)}";
            }
            return key;
        }
    }
}