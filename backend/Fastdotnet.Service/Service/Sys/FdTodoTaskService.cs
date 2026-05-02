using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    public class FdTodoTaskService : IFdTodoTaskService
    {
        private readonly IRawRepository<Fastdotnet.Core.Entities.Sys.FdTodoTask, string> _todoRepository;
        private readonly ICurrentUser _currentUser;

        public FdTodoTaskService(
            IRawRepository<Fastdotnet.Core.Entities.Sys.FdTodoTask, string> todoRepository,
            ICurrentUser currentUser)
        {
            _todoRepository = todoRepository;
            _currentUser = currentUser;
        }

        public async Task<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>> GetMyTodoTasksAsync()
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return new List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>();

            var tasks = await _todoRepository.GetListAsync(t => t.AssigneeId == userId && t.Status == 0);
            return tasks.OrderByDescending(t => t.CreatedAt).Adapt<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>>();
        }

        public async Task<string> CreateAsync(Fastdotnet.Core.Dtos.Sys.CreateFdTodoTaskDto dto)
        {
            var task = dto.Adapt<Fastdotnet.Core.Entities.Sys.FdTodoTask>();
            task.Id = Guid.NewGuid().ToString("N");
            task.Status = 0;
            task.CreatedAt = DateTime.Now;
            
            await _todoRepository.InsertAsync(task);
            return task.Id;
        }

        public async Task<bool> CompleteAsync(string id)
        {
            var task = await _todoRepository.GetByIdAsync(id);
            if (task == null) return false;

            task.Status = 1; // 已办
            return await _todoRepository.UpdateAsync(task);
        }

        public async Task<bool> TransferAsync(string id, string newAssigneeId)
        {
            var task = await _todoRepository.GetByIdAsync(id);
            if (task == null) return false;

            task.AssigneeId = newAssigneeId;
            return await _todoRepository.UpdateAsync(task);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _todoRepository.DeleteAsync(id);
        }
    }
}
