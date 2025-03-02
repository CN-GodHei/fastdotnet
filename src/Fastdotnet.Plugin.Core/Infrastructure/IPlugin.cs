using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public interface IPlugin
    {
        /// <summary>
        /// 获取插件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取插件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 插件初始化
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// 插件启动
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// 插件停止
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// 插件卸载前的清理工作
        /// </summary>
        Task UnloadAsync();
    }
}
