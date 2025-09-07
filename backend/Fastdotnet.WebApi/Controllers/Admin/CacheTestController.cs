using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 缓存测试控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/cache-test")]
    [CacheTag("cache-test")]
    [CacheTag("admin")]
    public class CacheTestController : ControllerBase
    {
        private readonly IHybridCacheService _cacheService;

        public CacheTestController(IHybridCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// 测试基本缓存功能
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("user/{id}")]
        [CacheResult(ExpirationSeconds = 600)]
        [CacheTag("user-data")]
        public async Task<ActionResult<string>> GetUserById(int id)
        {
            // 模拟数据库查询延迟
            await Task.Delay(500);
            
            var result = $"User data for ID: {id}, generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return Ok(result);
        }

        /// <summary>
        /// 测试带多个参数的缓存功能
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>产品列表</returns>
        [HttpGet("products")]
        [CacheResult(ExpirationSeconds = 30)]
        [CacheTag("product-data")]
        public async Task<ActionResult<List<string>>> GetProducts(
            [FromQuery] int categoryId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            // 模拟数据库查询延迟
            await Task.Delay(800);
            
            var products = new List<string>();
            for (int i = 0; i < pageSize; i++)
            {
                var index = (page - 1) * pageSize + i + 1;
                products.Add($"Product {index} in category {categoryId}, generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            
            return Ok(products);
        }

        /// <summary>
        /// 手动设置缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <returns>操作结果</returns>
        [HttpPost("set")]
        public async Task<ActionResult> SetCache([FromQuery] string key, [FromQuery] string value)
        {
            await _cacheService.SetAsync(key, value, null, new[] { "manual-cache" });
            return Ok($"Cache set: {key} = {value}");
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        [HttpGet("get")]
        public async Task<ActionResult<string>> GetCache([FromQuery] string key)
        {
            // 使用GetOrCreateAsync但提供一个返回默认值的工厂函数
            var value = await _cacheService.GetOrCreateAsync<string>(key, async () =>
            {
                return "Cache not found";
            });
            
            // 如果返回的是默认值，说明缓存中没有找到
            if (value == "Cache not found")
            {
                return Ok("Cache not found");
            }
            
            return Ok(value);
        }

        /// <summary>
        /// 移除特定标签的缓存
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>操作结果</returns>
        [HttpDelete("clear-by-tag")]
        public async Task<ActionResult> ClearCacheByTag([FromQuery] string[] tags)
        {
            if (tags.Length==0)
            {
                return BadRequest("标签不能为空");
            }
            //string[] strings = ["cache-test", "admin", "user-data"];
            await _cacheService.RemoveByTagAsync(tags);
            return Ok($"Cache cleared for tag: {tags}");
        }
    }
}