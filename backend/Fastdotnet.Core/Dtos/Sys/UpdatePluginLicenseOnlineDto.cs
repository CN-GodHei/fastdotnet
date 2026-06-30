using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Sys
{
    public class UpdatePluginLicenseOnlineDto
    {
        /// <summary>
        /// Token
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 插件Id
        /// </summary>
        [Required]
        public string PluginId { get; set; } = string.Empty;

    }
}
