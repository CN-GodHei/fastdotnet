using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Core.Attributes;
using Fastdotnet.Plugin.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Plugin.Controllers
{
    [ApiController]
    [Route("api/plugin/test")]
    [AutoInject(ServiceLifetime.Scoped)]
    public class TestPluginController : ControllerBase
    {
        private readonly ITestPluginService _testPluginService;

        public TestPluginController(ITestPluginService testPluginService)
        {
            _testPluginService = testPluginService;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var status = _testPluginService.GetPluginStatus();
            return Ok(new { status = status });
        }
    }
}