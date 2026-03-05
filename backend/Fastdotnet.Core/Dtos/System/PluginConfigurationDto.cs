using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.System
{
    public class PluginConfigurationDto
    {
    }

    public class PluginConfigurationGetRawJsonDto
    {
        /// <summary>
        /// 存在记录
        /// </summary>
        public bool ExistRocord { get; set; }

        /// <summary>
        /// 原始JOSN
        /// </summary>
        public string? RawJson { get; set; }
    }
}
