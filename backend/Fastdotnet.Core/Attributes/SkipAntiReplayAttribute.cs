
namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 跳过防重放验证的特性
    /// 标记此特性的 API 接口将不进行防重放攻击验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SkipAntiReplayAttribute : Attribute
    {
        /// <summary>
        /// 跳过原因（可选）
        /// </summary>
        public string Reason { get; set; } = "业务需要";
    }
}
