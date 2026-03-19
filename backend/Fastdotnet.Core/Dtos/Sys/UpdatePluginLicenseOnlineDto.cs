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
        public string Token { get; set; }

        /// <summary>
        /// 插件Id
        /// </summary>
        [Required]
        public string PluginId { get; set; }

    }
}
