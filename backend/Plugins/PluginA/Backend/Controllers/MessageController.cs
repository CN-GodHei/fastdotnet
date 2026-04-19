using Microsoft.AspNetCore.Mvc;
using PluginA.IService;
using PluginA.Services;
using System.Threading.Tasks;

namespace PluginA.Controllers
{
    [Route("api/plugins/plugin-a/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IPluginAMessageService _messageService;

        public MessageController(IPluginAMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// 发送通知到插件前端
        /// </summary>
        /// <param name="message">要发送的消息</param>
        /// <returns></returns>
        [HttpPost("send-notification")]
        public async Task<string> SendNotification([FromBody] SendMessageRequest request)
        {
            await _messageService.SendNotificationToClientsAsync(request.Message);
            return "通知已发送";
        }

        /// <summary>
        /// 发送数据到插件前端
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns></returns>
        [HttpPost("send-data")]
        public async Task<string> SendData([FromBody] SendMessageRequest request)
        {
            await _messageService.SendDataToClientsAsync(request.Message);
            return "数据已发送";
        }
    }

    public class SendMessageRequest
    {
        public string Message { get; set; }
    }
}