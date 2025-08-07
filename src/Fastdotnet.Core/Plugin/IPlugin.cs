using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.Core.Plugin
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
        /// 插件初始化，在此处可以访问主程序的服务
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于解析主程序的服务</param>
        Task InitializeAsync(IServiceProvider serviceProvider);

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
        /// <param name="serviceProvider">服务提供程序，用于解析主程序的服务以进行清理</param>
        Task UnloadAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// 配置插件服务
        /// </summary>
        /// <param name="builder">Autofac容器构建器</param>
        void ConfigureServices(ContainerBuilder builder);
    }
}
