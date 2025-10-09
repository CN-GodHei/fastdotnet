

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 标记在 Controller 或 Action 上，表示跳过操作日志记录。
    /// 适用于健康检查、心跳、频繁调用或敏感接口等场景。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SkipOperationLogAttribute : Attribute
    {
        /// <summary>
        /// 是否跳过操作日志
        /// </summary>
        public bool IsSkipped { get; set; } = true;
    }
}
