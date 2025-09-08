using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件SignalR端点注册接口
    /// </summary>
    public interface IPluginSignalREndpoint
    {
        /// <summary>
        /// 注册SignalR端点
        /// </summary>
        /// <param name="endpoints">端点路由构建器</param>
        void RegisterSignalREndpoints(IEndpointRouteBuilder endpoints);
    }
}