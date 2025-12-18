using System;
using Fastdotnet.Core.Enum;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 标记API使用范围的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiUsageScopeAttribute : Attribute
    {
        /// <summary>
        /// API使用范围
        /// </summary>
        public ApiUsageScopeEnum Scope { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scope">API使用范围</param>
        public ApiUsageScopeAttribute(ApiUsageScopeEnum scope)
        {
            Scope = scope;
        }
    }
}