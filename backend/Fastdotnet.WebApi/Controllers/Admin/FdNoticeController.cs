using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 管理端系统通知控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    public class FdNoticeController : GenericDtoControllerBase<Fastdotnet.Core.Entities.Sys.FdNotice, string, Fastdotnet.Core.Dtos.Sys.CreateFdNoticeDto, Fastdotnet.Core.Dtos.Sys.UpdateFdNoticeDto, Fastdotnet.Core.Dtos.Sys.FdNoticeDto>
    {
        private readonly Fastdotnet.Service.IService.Sys.IFdNoticeService _noticeService;

        public FdNoticeController(
            Fastdotnet.Service.IService.Sys.IFdNoticeService noticeService,
            IBaseService<Fastdotnet.Core.Entities.Sys.FdNotice, string> service) : base(service)
        {
            _noticeService = noticeService;
        }

        /// <summary>
        /// 获取当前用户的通知
        /// </summary>
        [HttpGet("my")]
        public async Task<List<Fastdotnet.Core.Dtos.Sys.FdNoticeDto>> GetMyNotices()
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
