using Fastdotnet.Plugin.Marketplace.Dto;

namespace Fastdotnet.Plugin.Marketplace.IService
{
    /// <summary>
    /// 用户授权信息查询服务接口。
    /// </summary>
    public interface IUserLicenseLookupService
    {
        /// <summary>
        /// 根据用户ID和插件ID获取用户的授权信息。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="pluginId">插件ID。</param>
        /// <returns>如果找到授权信息则返回 UserLicenseInfoDto 对象，否则返回 null。</returns>
        UserLicenseInfoDto GetUserLicenseInfo(string userId, string pluginId);
    }
}