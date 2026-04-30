namespace Fastdotnet.Service.IService.App
{
    public interface IFdUserWorkbenchService
    {
        // 获取用户当前可见的所有授权卡片定义
        Task<List<Fastdotnet.Core.Dtos.Sys.FdWorkbenchCardDto>> GetAvailableCardsAsync();

        // 布局管理
        Task<Fastdotnet.Core.Dtos.App.FdUserLayoutDto?> GetMyLayoutAsync(string name = "Default");
        Task<bool> SaveMyLayoutAsync(Fastdotnet.Core.Dtos.App.SaveFdUserLayoutDto dto);
    }
}
