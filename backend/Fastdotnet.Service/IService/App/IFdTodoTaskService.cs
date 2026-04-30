namespace Fastdotnet.Service.IService.App
{
    public interface IFdTodoTaskService
    {
        Task<List<Fastdotnet.Core.Dtos.App.FdTodoTaskDto>> GetMyTodoTasksAsync();
        Task<string> CreateAsync(Fastdotnet.Core.Dtos.App.CreateFdTodoTaskDto dto);
        Task<bool> CompleteAsync(string id);
        Task<bool> DeleteAsync(string id);
    }
}
