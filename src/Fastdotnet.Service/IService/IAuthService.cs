using Fastdotnet.Core.Models.Auth;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService
{
    public interface IAuthService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="dto">登录信息</param>
        /// <param name="userCategory">用户类别: "Admin" or "App"</param>
        /// <returns>JWT Token</returns>
        Task<string> LoginAsync(LoginDto dto, string userCategory);

        /// <summary>
        /// App端用户注册
        /// </summary>
        /// <param name="dto">注册信息</param>
        Task AppRegisterAsync(AppRegisterDto dto);
    }
}
