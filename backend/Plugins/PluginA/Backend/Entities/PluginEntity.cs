using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;

namespace PluginA.Entities
{
    /// <summary>
    /// 插件实体示例
    /// </summary>
    [SugarTable("plugin_entities")]
    public class PluginEntity: BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        // 添加无参数构造函数以满足new()约束
        public PluginEntity()
        {
        }
    }
}