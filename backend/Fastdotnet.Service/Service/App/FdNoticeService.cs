using AutoMapper;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Service.IService.App;

namespace Fastdotnet.Service.Service.App
{
    public class FdNoticeService : IFdNoticeService
    {
        private readonly IRawRepository<Fastdotnet.Core.Entities.App.FdNotice, string> _noticeRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public FdNoticeService(
            IRawRepository<Fastdotnet.Core.Entities.App.FdNotice, string> noticeRepository,
            ICurrentUser currentUser,
            IMapper mapper)
        {
            _noticeRepository = noticeRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<List<Fastdotnet.Core.Dtos.App.FdNoticeDto>> GetMyNoticesAsync()
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return new List<Fastdotnet.Core.Dtos.App.FdNoticeDto>();

            var notices = await _noticeRepository.GetListAsync(n => n.ReceiverId == userId);
            return _mapper.Map<List<Fastdotnet.Core.Dtos.App.FdNoticeDto>>(notices.OrderByDescending(n => n.CreatedAt));
        }

        public async Task<string> SendAsync(Fastdotnet.Core.Dtos.App.CreateFdNoticeDto dto)
        {
            var notice = _mapper.Map<Fastdotnet.Core.Entities.App.FdNotice>(dto);
            notice.Id = Guid.NewGuid().ToString("N");
            notice.IsRead = 0;
            notice.CreatedAt = DateTime.Now;

            await _noticeRepository.InsertAsync(notice);
            return notice.Id;
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var notice = await _noticeRepository.GetByIdAsync(id);
            if (notice == null) return false;

            notice.IsRead = 1;
            return await _noticeRepository.UpdateAsync(notice);
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return false;

            var unreadNotices = await _noticeRepository.GetListAsync(n => n.ReceiverId == userId && n.IsRead == 0);
            foreach (var notice in unreadNotices)
            {
                notice.IsRead = 1;
            }
            await _noticeRepository.UpdateRangeAsync(unreadNotices);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _noticeRepository.DeleteAsync(id);
        }
    }
}
