

namespace Fastdotnet.Core.IService.App
{
    public interface IFdAppUserService
    {
        /// <summary>
        /// 获取用户基础信息
        /// </summary>
        Task<FdAppUser?> GetFdAppUserAsync(string FdAppUserId, CancellationToken ct = default);

        /// <summary>
        /// 获取用户 + 指定插件扩展数据
        /// </summary>
        Task<(FdAppUser Base, TExtension? Extension)> GetFdAppUserWithExtensionAsync<TExtension>(
            string FdAppUserId,
            CancellationToken ct = default) where TExtension : class;

        /// <summary>
        /// 更新用户基础信息（无扩展）
        /// </summary>
        Task UpdateFdAppUserAsync(
            string FdAppUserId,
            Action<FdAppUser> updateAction,
            CancellationToken ct = default);

        /// <summary>
        /// 更新用户基础信息 + 插件扩展数据（事务安全）
        /// </summary>
        Task UpdateFdAppUserWithExtensionAsync<TExtension>(
            string FdAppUserId,
            Action<FdAppUser> updateBaseAction,
            TExtension extensionData,
            CancellationToken ct = default) where TExtension : class;

        /// <summary>
        /// 创建新用户 + 可选扩展数据（事务安全）
        /// </summary>
        Task<string> CreateFdAppUserWithExtensionAsync<TExtension>(
            Action<FdAppUser> initBaseAction,
            TExtension extensionData,
            CancellationToken ct = default) where TExtension : class;
    }
}
