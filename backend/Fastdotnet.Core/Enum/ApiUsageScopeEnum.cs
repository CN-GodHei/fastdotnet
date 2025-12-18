using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Enum
{
    /// <summary>
    /// API使用范围枚举
    /// </summary>
    [Description("API使用范围枚举")]
    public enum ApiUsageScopeEnum
    {
        /// <summary>
        /// Admin管理端专用
        /// </summary>
        [Description("Admin管理端专用")]
        AdminOnly = 1,

        /// <summary>
        /// App用户端专用
        /// </summary>
        [Description("App用户端专用")]
        AppOnly = 2,

        /// <summary>
        /// 两端通用
        /// </summary>
        [Description("两端通用")]
        Both = 3
    }
}