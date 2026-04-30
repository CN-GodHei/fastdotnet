using System;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 用于标记在 AutoSignalRProvider 中需要自动注册的 SignalR 方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SignalRMethodAttribute : Attribute
    {
        /// <summary>
        /// 注册到前端调用的方法名。如果为空，则默认使用原函数名。
        /// </summary>
        public string MethodName { get; }

        public SignalRMethodAttribute(string methodName = null)
        {
            MethodName = methodName;
        }
    }
}
