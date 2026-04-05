
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Service.IService.Sys
{
    /// <summary>
    /// 密码服务接口
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// 根据系统配置加密密码
        /// 自动从字典读取加密类型和密钥
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>加密后的密码</returns>
        Task<string> EncryptPasswordAsync(string password);

        /// <summary>
        /// 获取默认加密密码
        /// 从字典读取默认密码并使用系统配置加密
        /// </summary>
        /// <returns>默认加密密码</returns>
        Task<string> GetDefaultEncryptedPasswordAsync();

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="hashedPassword">加密后的密码</param>
        /// <returns>是否匹配</returns>
        Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
    }
}
