using Fastdotnet.Core.Dtos.Base;
using SqlSugar;

namespace PluginA.Entities
{
    /// <summary>
    /// PluginA 插件的用户扩展数据实体
    /// </summary>
    [SugarTable("Fd_PluginAUserExtension")]
    public class PluginAUserExtension : BaseEntity
    {
        /// <summary>
        /// 关联的用户ID
        /// </summary>
        [SugarColumn]
        public string FdAppUserId { get; set; }

        /// <summary>
        /// 用户偏好设置
        /// </summary>
        [SugarColumn]
        public string Preferences { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [SugarColumn]
        public int Points { get; set; }

    }
}