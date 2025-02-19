using Fastdotnet.Demo.IService;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IDemoService _demoService;

        public DemoController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            return Ok(_demoService.GetMessage());
        }
    }
}