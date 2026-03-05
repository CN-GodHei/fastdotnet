

namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// 插件配置服务接口
    /// </summary>
    public interface IPluginConfigurationService
    {
        /// <summary>
        /// 获取插件的强类型配置（推荐插件使用）
        /// </summary>
        /// <typeparam name="T">配置类型，必须有无参构造函数</typeparam>
        /// <param name="pluginId">插件唯一标识</param>
        /// <returns>配置对象，若不存在则返回 null</returns>
        Task<T?> GetSettingsAsync<T>(string pluginId) where T : class, new();

        /// <summary>
        /// 保存插件的强类型配置（推荐插件使用）
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="pluginId">插件唯一标识</param>
        /// <param name="settings">配置对象</param>
        Task SaveSettingsAsync<T>(string pluginId, T settings) where T : class;

        // ----------------------------
        // 以下为可选高级 API（用于管理后台等）
        // ----------------------------

        /// <summary>
        /// 获取插件的原始 JSON 配置字符串
        /// </summary>
        Task<PluginConfigurationGetRawJsonDto> GetRawJsonAsync(string pluginId);

        /// <summary>
        /// 保存插件的原始 JSON 配置（需确保 JSON 有效）
        /// </summary>
        Task SaveRawJsonAsync(string pluginId, string rawJson);
    }
}
