using Fastdotnet.Core.Middleware;
using Microsoft.Extensions.Logging;
using PluginA.Contexts;
using System;
using System.Threading.Tasks;

namespace PluginA.Services
{
    /// <summary>
    /// 业务操作执行器 - 演示如何使用 IPluginPipeline 管道
    /// </summary>
    public class BusinessOperationExecutor
    {
        private readonly PluginPipelineDispatcher<BusinessOperationContext> _dispatcher;
        private readonly ILogger<BusinessOperationExecutor> _logger;

        public BusinessOperationExecutor(
            PluginPipelineDispatcher<BusinessOperationContext> dispatcher,
            ILogger<BusinessOperationExecutor> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        /// <summary>
        /// 执行一个业务操作（会经过所有注册的中间件）
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="dataId">数据 ID</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="extraData">附加数据</param>
        /// <returns>执行后的上下文</returns>
        public async Task<BusinessOperationContext> ExecuteAsync(
            string operationType,
            string dataType,
            string dataId,
            string userId,
            IDictionary<string, object> extraData = null)
        {
            // 创建业务操作上下文
            var context = new BusinessOperationContext
            {
                OperationType = operationType,
                DataType = dataType,
                DataId = dataId,
                UserId = userId,
                OperationTime = DateTime.UtcNow,
                ExtraData = extraData ?? new Dictionary<string, object>()
            };

            _logger.LogInformation(
                "开始通过管道执行业务操作：{OperationType} - {DataType} - {DataId}",
                operationType,
                dataType,
                dataId);

            // 通过管道执行真正的业务逻辑
            await _dispatcher.ExecuteAsync(context, async (ctx) =>
            {
                // ====================================================
                // 这里是真正的业务逻辑，会在所有中间件之后执行
                // ====================================================
                
                _logger.LogInformation(
                    "正在执行核心业务逻辑...\n" +
                    "操作 ID: {OperationId}\n" +
                    "操作类型：{OperationType}\n" +
                    "数据类型：{DataType}\n" +
                    "数据 ID: {DataId}",
                    ctx.OperationId,
                    ctx.OperationType,
                    ctx.DataType,
                    ctx.DataId);

                // TODO: 在这里实现你的业务逻辑
                // 例如：
                // - 数据库操作（增删改查）
                // - 调用外部 API
                // - 发送消息到队列
                // - 触发领域事件
                // 等等...

                // 模拟业务逻辑执行
                await Task.Delay(100); // 模拟耗时操作

                _logger.LogInformation("核心业务逻辑执行完成");
            });

            return context;
        }

        /// <summary>
        /// 便捷方法：创建操作
        /// </summary>
        public Task<BusinessOperationContext> CreateAsync(
            string dataType,
            string dataId,
            string userId,
            IDictionary<string, object> extraData = null)
        {
            return ExecuteAsync("Create", dataType, dataId, userId, extraData);
        }

        /// <summary>
        /// 便捷方法：更新操作
        /// </summary>
        public Task<BusinessOperationContext> UpdateAsync(
            string dataType,
            string dataId,
            string userId,
            IDictionary<string, object> extraData = null)
        {
            return ExecuteAsync("Update", dataType, dataId, userId, extraData);
        }

        /// <summary>
        /// 便捷方法：删除操作
        /// </summary>
        public Task<BusinessOperationContext> DeleteAsync(
            string dataType,
            string dataId,
            string userId)
        {
            return ExecuteAsync("Delete", dataType, dataId, userId);
        }

        /// <summary>
        /// 便捷方法：查询操作
        /// </summary>
        public Task<BusinessOperationContext> QueryAsync(
            string dataType,
            string dataId,
            string userId)
        {
            return ExecuteAsync("Query", dataType, dataId, userId);
        }
    }
}
