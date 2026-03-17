using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    /// 设置插件授权传输模型
    /// </summary>
    public class SetPluginLicenseDto
    {
        /// <summary>
        /// 类型：0-单个;1-多个
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 授权内容
        /// </summary>
        [Required]
        public string LicenseStr { get; set; }
    }
}
