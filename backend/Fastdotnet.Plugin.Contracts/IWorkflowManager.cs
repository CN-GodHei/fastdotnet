namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 工作流管理接口
    /// 允许其他业务插件无需直接引用 Elsa 即可启动和管理流程
    /// </summary>
    public interface IWorkflowManager
    {
        /// <summary>
        /// 启动一个工作流
        /// </summary>
        /// <param name="definitionId">工作流定义ID</param>
        /// <param name="input">可选的输入参数</param>
        /// <param name="correlationId">可选的关联ID（用于业务匹配）</param>
        /// <returns>工作流实例ID</returns>
        Task<string> StartWorkflowAsync(string definitionId, object? input = null, string? correlationId = null);

        /// <summary>
        /// 发送信号以唤醒挂起的流程
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <param name="signalName">信号名称</param>
        /// <param name="input">可选输入参数</param>
        Task TriggerSignalAsync(string workflowInstanceId, string signalName, object? input = null);

        /// <summary>
        /// 终止工作流
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <param name="reason">终止原因</param>
        Task TerminateWorkflowAsync(string workflowInstanceId, string? reason = null);

        /// <summary>
        /// 检查工作流插件是否已就绪
        /// </summary>
        bool IsReady { get; }
    }
}
