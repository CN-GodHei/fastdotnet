using System.Collections.Generic;

namespace Fastdotnet.Plugin.Contracts
{
    public class PluginInfo
    {
        /// <summary>
        /// 插件ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 插件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 插件是否启用
        /// </summary>
        public bool enabled { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 插件依赖
        /// </summary>
        public List<string> dependencies { get; set; }

        /// <summary>
        /// 插件标签
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// 插件入口DLL文件名
        /// </summary>
        public string entryPoint { get; set; }
    }
}