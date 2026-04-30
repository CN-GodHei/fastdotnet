using AutoMapper;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    public class FdTodoTaskService : IFdTodoTaskService
    {
        private readonly IRawRepository<Fastdotnet.Core.Entities.Sys.FdTodoTask, string> _todoRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public FdTodoTaskService(
            IRawRepository<Fastdotnet.Core.Entities.Sys.FdTodoTask, string> todoRepository,
            ICurrentUser currentUser,
            IMapper mapper)
        {
            _todoRepository = todoRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>> GetMyTodoTasksAsync()
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return new List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>();

            var tasks = await _todoRepository.GetListAsync(t => t.AssigneeId == userId && t.Status == 0);
            return _mapper.Map<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>>(tasks.OrderByDescending(t => t.CreatedAt));
        }

        public async Task<string> CreateAsync(Fastdotnet.Core.Dtos.Sys.CreateFdTodoTaskDto dto)
        {
            var task = _mapper.Map<Fastdotnet.Core.Entities.Sys.FdTodoTask>(dto);
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
