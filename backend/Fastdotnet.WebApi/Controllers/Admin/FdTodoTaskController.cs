using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 管理端待办任务控制器
    /// </summary>
    [Route("api/admin/[controller]")]
    public class FdTodoTaskController : GenericDtoControllerBase<Fastdotnet.Core.Entities.Sys.FdTodoTask, string, Fastdotnet.Core.Dtos.Sys.CreateFdTodoTaskDto, Fastdotnet.Core.Dtos.Sys.UpdateFdTodoTaskDto, Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>
    {
        private readonly Fastdotnet.Service.IService.Sys.IFdTodoTaskService _todoTaskService;

        public FdTodoTaskController(
            Fastdotnet.Service.IService.Sys.IFdTodoTaskService todoTaskService,
            IBaseService<Fastdotnet.Core.Entities.Sys.FdTodoTask, string> service,
            IMapper mapper) : base(service, mapper)
        {
            _todoTaskService = todoTaskService;
        }

        /// <summary>
        /// 获取当前用户的待办任务
        /// </summary>
        [HttpGet("my")]
        public async Task<List<Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>> GetMyTodoTasks()
        {
            return await _todoTaskService.GetMyTodoTasksAsync();
        }

        /// <summary>
        /// 完成待办任务
        /// </summary>
        [HttpPost("complete/{id}")]
        public async Task<bool> Complete(string id)
        {
            return await _todoTaskService.CompleteAsync(id);
        }

        /// <summary>
        /// 转办任务
        /// </summary>
        [HttpPost("transfer/{id}/{newAssigneeId}")]
        public async Task<bool> Transfer(string id, string newAssigneeId)
        {
            return await _todoTaskService.TransferAsync(id, newAssigneeId);
        }
    }
}
