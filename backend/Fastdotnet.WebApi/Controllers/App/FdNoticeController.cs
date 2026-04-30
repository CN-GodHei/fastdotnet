using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.App
{
    /// <summary>
    /// 应用端系统通知控制器
    /// </summary>
    [Route("api/app/[controller]")]
    public class FdNoticeController : AppGenericDtoControllerBase<Fastdotnet.Core.Entities.App.FdNotice, string, Fastdotnet.Core.Dtos.App.CreateFdNoticeDto, Fastdotnet.Core.Dtos.App.UpdateFdNoticeDto, Fastdotnet.Core.Dtos.App.FdNoticeDto>
    {
        private readonly Fastdotnet.Service.IService.App.IFdNoticeService _noticeService;

        public FdNoticeController(
            Fastdotnet.Service.IService.App.IFdNoticeService noticeService,
            IBaseService<Fastdotnet.Core.Entities.App.FdNotice, string> service,
            IMapper mapper) : base(service, mapper)
        {
            _noticeService = noticeService;
        }

        /// <summary>
        /// 获取当前用户的通知
        /// </summary>
        [HttpGet("my")]
        public async Task<List<Fastdotnet.Core.Dtos.App.FdNoticeDto>> GetMyNotices()
        {
            return await _noticeService.GetMyNoticesAsync();
        }

        /// <summary>
        /// 标记为已读
        /// </summary>
        [HttpPost("read/{id}")]
        public async Task<bool> MarkAsRead(string id)
        {
            return await _noticeService.MarkAsReadAsync(id);
        }

        /// <summary>
        /// 全部标记为已读
        /// </summary>
        [HttpPost("readAll")]
        public async Task<bool> MarkAllAsRead()
        {
            return await _noticeService.MarkAllAsReadAsync();
        }
    }
}
