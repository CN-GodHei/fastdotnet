using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fastdotnet.Core.Dtos.Base;

namespace Fastdotnet.Core.Entities.System
{
    [SugarTable("fd_plugin_configurations", "插件配置信息")]
    public class PluginConfiguration
    {
        /// <summary>
        /// 插件Id
        /// </summary>
        [SugarColumn(ColumnName = "plugin_id", IsPrimaryKey = true, ColumnDescription = "插件Id")]
        public string PluginId { get; set; } = null!;

        /// <summary>
        /// 配置信息：
        /// </summary>
        [SugarColumn(ColumnName = "config_json", ColumnDescription = "插件配置", ColumnDataType = "TEXT")]
        public string ConfigJson { get; set; } = "{}";
    }
}