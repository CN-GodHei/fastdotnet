namespace Fastdotnet.Service.IService.Sys
{
    public interface IFdTodoTaskService
    {
        Task<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>> GetMyTodoTasksAsync();
        Task<string> CreateAsync(Fastdotnet.Core.Dtos.Sys.CreateFdTodoTaskDto dto);
        Task<bool> CompleteAsync(string id);
        Task<bool> TransferAsync(string id, string newAssigneeId);
        Task<bool> DeleteAsync(string id);
    }
}
