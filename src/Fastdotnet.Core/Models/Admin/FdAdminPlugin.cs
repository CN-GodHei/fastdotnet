using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 插件信息表
    /// </summary>
    public class FdAdminPlugin : BaseEntity
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 插件编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 插件程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 插件入口类型
        /// </summary>
        public string EntryType { get; set; }

        /// <summary>
        /// 插件配置（JSON格式）
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 加载顺序
        /// </summary>
        public int LoadOrder { get; set; }

        /// <summary>
        /// 最后加载时间
        /// </summary>
        public DateTime? LastLoadTime { get; set; }
    }
}