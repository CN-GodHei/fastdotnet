namespace Fastdotnet.Service.IService.Sys
{
    public interface IFdWorkbenchService
    {
        // 卡片定义管理
        Task<List<Fastdotnet.Core.Dtos.Sys.FdWorkbenchCardDto>> GetAllCardsAsync();
        Task<string> CreateCardAsync(Fastdotnet.Core.Dtos.Sys.CreateFdWorkbenchCardDto dto);
        Task<bool> UpdateCardAsync(Fastdotnet.Core.Dtos.Sys.UpdateFdWorkbenchCardDto dto);
        Task<bool> DeleteCardAsync(string id);

        // 权限分配
        Task<bool> AssignCardsToRoleAsync(string roleId, List<string> cardIds);
        Task<List<string>> GetRoleCardIdsAsync(string roleId);
    }
}
