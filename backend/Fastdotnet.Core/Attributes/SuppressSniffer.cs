using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 不被扫描和发现的特性
    /// </summary>
    /// <remarks>用于程序集扫描类型或方法时候</remarks>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Struct)]
    public class SuppressSnifferAttribute : Attribute
    {
    }
}
