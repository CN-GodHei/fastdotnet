using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Enum
{
    /// <summary>
    /// 是否枚举
    /// </summary>
    [Description("是否枚举")]
    public enum YesNoEnum
    {
        /// <summary>
        /// 是
        /// </summary>
        [Description("是"), Theme("success")]
        Y = 1,

        /// <summary>
        /// 否
        /// </summary>
        [Description("否"), Theme("danger")]
        N = 2
    }
}
