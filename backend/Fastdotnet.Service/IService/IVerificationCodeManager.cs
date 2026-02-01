
namespace Fastdotnet.Service.IService
{
    /// <summary>
    /// 验证码管理器，负责根据业务码分发和执行对应的策略
    /// </summary>
    public interface IVerificationCodeManager
    {
        /// <summary>
        /// 根据业务码发送验证码
        /// </summary>
        /// <param name="recipient">接收者（如邮箱地址）</param>
        /// <param name="businessCode">业务码</param>
        /// <param name="expirationInMinutes">自定义过期时间（分钟）</param>
        Task SendCodeAsync(string recipient, string businessCode, int? expirationInMinutes = null);

        /// <summary>
        /// 根据业务码校验验证码
        /// </summary>
        /// <param name="recipient">接收者（如邮箱地址）</param>
        /// <param name="code">待校验的验证码</param>
        /// <param name="businessCode">业务码</param>
        /// <returns>校验是否成功</returns>
        Task<bool> VerifyCodeAsync(string recipient, string code, string businessCode);
    }
}
