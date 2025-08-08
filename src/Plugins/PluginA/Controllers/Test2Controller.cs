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
    public class Test2Controller : ControllerBase
    {
        private readonly ITestService _testService;

        public Test2Controller(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public IActionResult Hello()
        {
            return Ok(new { message = "你好", testMessage = _testService.GetTestMessage() });
        }
    }
}
