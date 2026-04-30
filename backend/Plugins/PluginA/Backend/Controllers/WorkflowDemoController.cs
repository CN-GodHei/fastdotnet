using Fastdotnet.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace PluginA.Controllers
{
    /// <summary>
    /// 插件内工作流使用示例
    /// </summary>
    [ApiController]
    [Route("api/plugin-a/workflow")]
    public class WorkflowDemoController : ControllerBase
    {
        private readonly IWorkflowManager _workflowManager;

        public WorkflowDemoController(IWorkflowManager workflowManager)
        {
            _workflowManager = workflowManager;
        }

        /// <summary>
        /// 模拟提交一个请假申请并启动工作流
        /// </summary>
        /// <param name="leaveType">请假类型</param>
        /// <param name="days">天数</param>
        /// <returns></returns>
        [HttpPost("submit-leave")]
        public async Task<IActionResult> SubmitLeave(string leaveType, int days)
        {
            // 1. 业务逻辑：保存请假单到插件自己的数据库（此处省略）
            var businessId = Guid.NewGuid().ToString("N");

            // 2. 准备流程输入参数
            var input = new
            {
                BusinessId = businessId,
                LeaveType = leaveType,
                Days = days,
                Submitter = "张三",
                SubmitTime = DateTime.Now
            };

            // 3. 调用 IWorkflowManager 启动流程
            // 注意：PluginA 并没有引用 Elsa，它通过接口与主程序中的 Elsa 插件通信
            try 
            {
                var instanceId = await _workflowManager.StartWorkflowAsync("LeaveApproval", input, businessId);
                
                return Ok(new 
                { 
                    Message = "请假申请已提交，流程已启动", 
                    WorkflowInstanceId = instanceId,
                    BusinessId = businessId 
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"启动流程失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 模拟审批通过，触发流程信号
        /// </summary>
        /// <param name="instanceId">工作流实例ID</param>
        /// <param name="decision">决策结果</param>
        [HttpPost("approve")]
        public async Task<IActionResult> Approve(string instanceId, string decision)
        {
            // 触发名为 "ApproveSignal" 的信号
            // 流程图中对应的 "Event Received" 活动会捕获到这个信号并继续执行
            await _workflowManager.TriggerSignalAsync(instanceId, "ApproveSignal", new { Decision = decision });

            return Ok(new { Message = $"信号 {decision} 已发送" });
        }

        /// <summary>
        /// 检查流程引擎是否就绪
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { Ready = _workflowManager.IsReady });
        }
    }
}
