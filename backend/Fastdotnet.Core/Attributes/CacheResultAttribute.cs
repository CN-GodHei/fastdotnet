using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 控制器方法缓存特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheResultAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// 缓存过期时间（秒）
        /// </summary>
        public int ExpirationSeconds { get; set; } = 0; // 0表示使用配置文件中的默认值

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool Enabled { get; set; } = true;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!Enabled)
            {
                await next();
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetService<IHybridCacheService>();
            var cacheSettings = context.HttpContext.RequestServices.GetService<IOptions<CacheSettings>>()?.Value;
            
            if (cacheService == null || cacheSettings == null)
            {
                await next();
                return;
            }

            // 生成缓存键
            var key = GenerateCacheKey(context);
            
            // 获取标签
            var tags = GetCacheTags(context);

            // 尝试从缓存获取结果数据（缓存字符串形式的JSON）
            var cachedJson = await cacheService.GetOrCreateAsync<string>(key, async () =>
            {
                return null; // 如果缓存中没有找到，返回null
            });

            // 如果缓存中有数据，直接构建响应返回
            if (!string.IsNullOrEmpty(cachedJson))
            {
                try
                {
                    // 使用Newtonsoft.Json反序列化JSON到对象
                    var cachedData = JsonConvert.DeserializeObject<CacheResultData>(cachedJson);
                    if (cachedData != null)
                    {
                        context.Result = new ObjectResult(cachedData.Data)
                        {
                            StatusCode = cachedData.StatusCode
                        };
                        return;
                    }
                }
                catch
                {
                    // 如果反序列化失败，继续执行正常流程
                }
            }

            // 执行实际操作
            var executedContext = await next();

            // 如果执行成功且有返回结果，缓存实际的数据
            if (executedContext.Result is ObjectResult objectResult && 
                objectResult.StatusCode >= 200 && 
                objectResult.StatusCode < 300)
            {
                // 使用配置文件中的过期时间，如果当前属性未设置
                var expirationSeconds = ExpirationSeconds > 0 ? ExpirationSeconds : cacheSettings.LocalCacheExpirationMinutes * 60;
                
                var options = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromSeconds(expirationSeconds * 2),
                    LocalCacheExpiration = TimeSpan.FromSeconds(expirationSeconds)
                };

                // 创建缓存数据对象
                var cacheData = new CacheResultData
                {
                    Data = objectResult.Value,
                    StatusCode = objectResult.StatusCode ?? 200
                };

                // 使用Newtonsoft.Json序列化为JSON字符串缓存
                var json = JsonConvert.SerializeObject(cacheData);
                await cacheService.SetAsync(key, json, options, tags);
            }
        }

        /// <summary>
        /// 缓存结果数据类
        /// </summary>
        private class CacheResultData
        {
            public object Data { get; set; }
            public int StatusCode { get; set; }
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="context">动作执行上下文</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey(ActionExecutingContext context)
        {
            // 使用控制器类型的全名作为基础
            var controllerType = context.Controller.GetType();
            var controllerFullName = controllerType.FullName;
            
            // 获取动作方法名
            var actionName = context.ActionDescriptor.DisplayName.Split('(')[0].Split('.').Last();
            
            // 构建基础键
            var baseKey = string.IsNullOrEmpty(KeyPrefix) ? $"{controllerFullName}:{actionName}" : KeyPrefix;
            
            // 处理参数，生成唯一的参数签名
            var parameters = context.ActionArguments;
            var parameterSignature = GenerateParameterSignature(parameters);
            
            return $"{baseKey}?{parameterSignature}";
        }

        /// <summary>
        /// 生成参数签名
        /// </summary>
        /// <param name="parameters">参数字典</param>
        /// <returns>参数签名</returns>
        private string GenerateParameterSignature(IDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return "no-params";

            // 按参数名排序以确保一致性
            var sortedParams = parameters.OrderBy(p => p.Key);
            
            var paramStrings = new List<string>();
            foreach (var param in sortedParams)
            {
                var value = param.Value?.ToString() ?? "null";
                paramStrings.Add($"{param.Key}={value}");
            }
            
            // 将参数字符串连接并计算哈希值，避免键过长
            var paramString = string.Join("&", paramStrings);
            var hash = paramString.GetHashCode().ToString("x8"); // 转换为8位十六进制字符串
            
            return hash;
        }

        /// <summary>
        /// 获取缓存标签
        /// </summary>
        /// <param name="context">动作执行上下文</param>
        /// <returns>标签数组</returns>
        private string[] GetCacheTags(ActionExecutingContext context)
        {
            var tags = new List<string>();

            // 获取方法级别的标签
            var methodTags = context.ActionDescriptor.EndpointMetadata
                .OfType<CacheTagAttribute>()
                .Select(attr => attr.Tag);

            tags.AddRange(methodTags);

            // 获取控制器级别的标签
            var controllerTags = context.Controller.GetType()
                .GetCustomAttributes(typeof(CacheTagAttribute), true)
                .OfType<CacheTagAttribute>()
                .Select(attr => attr.Tag);

            tags.AddRange(controllerTags);

            return tags.Distinct().ToArray();
        }
    }
}