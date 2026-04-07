using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Extensions
{
    /// <summary>
    /// 插件分支网关扩展方法
    /// </summary>
    public static class PluginBranchGateExtensions
    {
        /// <summary>
        /// 启用插件分支网关 - 允许插件动态注册请求处理管道
        /// </summary>
        public static IApplicationBuilder UsePluginBranchGate(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PluginBranchGateMiddleware>();
        }
    }
}
