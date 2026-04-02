using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PluginA.Contexts
{
    /// <summary>
    /// 业务操作上下文 - 用于演示 IPluginPipeline 泛型接口
    /// </summary>
    public class BusinessOperationContext
    {
        /// <summary>
        /// 操作 ID
        /// </summary>
        public string OperationId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 操作类型（Create、Update、Delete、Query）
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作的数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 操作的数据 ID
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// 操作用户 ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 附加数据
        /// </summary>
        public IDictionary<string, object> ExtraData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 是否已验证（由验证中间件设置）
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// 是否有权限（由权限中间件设置）
        /// </summary>
        public bool HasPermission { get; set; }

        /// <summary>
        /// 执行耗时（毫秒，由性能监控中间件设置）
        /// </summary>
        public long? ElapsedMilliseconds { get; set; }
    }
}
