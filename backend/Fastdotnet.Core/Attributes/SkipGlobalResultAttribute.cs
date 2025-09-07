using System;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 跳过全局返回结果处理的特性
    /// 添加此特性的控制器或方法将不会被全局结果过滤器处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SkipGlobalResultAttribute : Attribute
    {
    }
}