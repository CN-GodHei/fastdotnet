using Microsoft.AspNetCore.Mvc;
using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Enum;
using Fastdotnet.Service.IService;

namespace Fastdotnet.WebApi.Controllers.App
{
    /// <summary>
    /// 应用端待办任务控制器
    /// </summary>
    [Route("api/app/[controller]")]
    [ApiUsageScope(ApiUsageScopeEnum.Both)]
    public class FdAppTodoTaskController : AppGenericDtoControllerBase<Fastdotnet.Core.Entities.App.FdTodoTask, string, Fastdotnet.Core.Dtos.App.CreateFdTodoTaskDto, Fastdotnet.Core.Dtos.App.UpdateFdTodoTaskDto, Fastdotnet.Core.Dtos.App.FdTodoTaskDto>
    {
        private readonly Fastdotnet.Service.IService.App.IFdTodoTaskService _todoTaskService;

        public FdAppTodoTaskController(
            Fastdotnet.Service.IService.App.IFdTodoTaskService todoTaskService,
            IBaseService<Fastdotnet.Core.Entities.App.FdTodoTask, string> service) : base(service)
        {
            _todoTaskService = todoTaskService;
        }

        /// <summary>
        /// 获取当前用户的待办任务
        /// </summary>
        [HttpGet("my")]
        public async Task<List<Fastdotnet.Core.Dtos.App.FdTodoTaskDto>> GetMyTodoTasks()
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
    }
}
