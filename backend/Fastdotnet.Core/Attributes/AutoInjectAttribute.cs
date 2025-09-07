using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 用于标记需要自动注入的服务类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInjectAttribute : Attribute
    {
        /// <summary>
        /// 服务生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lifetime">服务生命周期，默认为Scoped</param>
        public AutoInjectAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
    }
}