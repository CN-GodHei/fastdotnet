namespace Fastdotnet.Service.IService.Sys
{
    public interface IFdNoticeService
    {
        Task<List<Fastdotnet.Core.Dtos.Sys.FdNoticeDto>> GetMyNoticesAsync();
        Task<string> SendAsync(Fastdotnet.Core.Dtos.Sys.CreateFdNoticeDto dto);
        Task<bool> MarkAsReadAsync(string id);
        Task<bool> MarkAllAsReadAsync();
        Task<bool> DeleteAsync(string id);
    }
}
