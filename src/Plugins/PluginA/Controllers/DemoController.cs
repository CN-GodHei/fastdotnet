using Fastdotnet.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using PluginA.Entities;
using System.Collections.Generic;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DemoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetData()
        {
            var data = new PluginEntity
            {
                Id = 1,
                Name = "测试数据",
                Description = "这是一个测试实体"
            };
            return Ok(data);
        }

        [HttpGet("list")]
        public IActionResult GetList()
        {
            var list = new List<PluginEntity>
            {
                new PluginEntity
                {
                    Id = 1,
                    Name = "测试数据1",
                    Description = "这是第一个测试实体"
                },
                new PluginEntity
                {
                    Id = 2,
                    Name = "测试数据2",
                    Description = "这是第二个测试实体"
                }
            };
            return Ok(list);
        }

        [HttpGet("string")]
        public IActionResult GetString()
        {
            return Ok("这是一个字符串结果");
        }

        [HttpGet("number")]
        public IActionResult GetNumber()
        {
            return Ok(12345);
        }

        [HttpGet("void")]
        public IActionResult VoidMethod()
        {
            // 无返回值的方法
            return Ok();
        }

        [HttpGet("skip")]
        [SkipGlobalResult] // 这个方法将跳过全局结果处理
        public IActionResult SkipResult()
        {
            return Ok(new { Message = "这个结果不会被全局处理包装" });
        }
    }
}