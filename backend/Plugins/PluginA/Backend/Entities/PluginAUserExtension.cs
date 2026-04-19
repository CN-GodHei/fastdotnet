using Fastdotnet.Core.Dtos.Base;
using SqlSugar;

namespace PluginA.Entities
{
    /// <summary>
    /// PluginA 插件的用户扩展数据实体
    /// </summary>
    [SugarTable("p_pluginA_user_extensions")]
    public class PluginAUserExtension
    {
        /// <summary>
        /// 关联的用户ID（主键）
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "用户ID")]
        public string FdAppUserId { get; set; }

        /// <summary>
        /// 用户偏好设置
        /// </summary>
        [SugarColumn(ColumnDescription = "用户偏好设置", IsNullable = true)]
        public string Preferences { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [SugarColumn(ColumnDescription = "用户积分")]
        public int Points { get; set; }

    }
}