namespace Fastdotnet.Service.IService.App
{
    public interface IFdNoticeService
    {
        Task<List<Fastdotnet.Core.Dtos.App.FdNoticeDto>> GetMyNoticesAsync();
        Task<string> SendAsync(Fastdotnet.Core.Dtos.App.CreateFdNoticeDto dto);
        Task<bool> MarkAsReadAsync(string id);
        Task<bool> MarkAllAsReadAsync();
        Task<bool> DeleteAsync(string id);
    }
}
