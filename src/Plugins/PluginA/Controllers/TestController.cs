using Microsoft.AspNetCore.Mvc;
using PluginA.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "在2025-03-02 10点49分由GodHei写下了这行改变dotnet框架插件化生态建设里程碑式进程的代码", testMessage = _testService.GetTestMessage() });
        }
        [HttpGet]
        public IActionResult Test()
        {
            return Ok(new { message = "在2025-03-02 10点49分由GodHei写下了这行改变dotnet框架插件化生态建设里程碑式进程的代码", testMessage = _testService.GetTestMessage() });
        }

    }
}
