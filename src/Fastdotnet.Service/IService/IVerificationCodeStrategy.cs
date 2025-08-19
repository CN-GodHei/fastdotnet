using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    /// <summary>
    /// 定义了验证码处理策略的接口
    /// </summary>
    public interface IVerificationCodeStrategy
    {
        /// <summary>
        /// 此策略能处理的唯一业务码 (e.g., "UserRegister", "ResetPassword")
        /// </summary>
        string BusinessCode { get; }

        /// <summary>
        /// 发送验证码到指定的接收者
        /// </summary>
        /// <param name="recipient">接收者（如邮箱地址）</param>
        /// <param name="expirationInMinutes">自定义的过期时间（分钟）</param>
        Task SendAsync(string recipient, int? expirationInMinutes = null);

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="recipient">接收者（如邮箱地址）</param>
        /// <param name="code">待校验的验证码</param>
        /// <returns>校验是否成功</returns>
        Task<bool> VerifyAsync(string recipient, string code);
    }
}
