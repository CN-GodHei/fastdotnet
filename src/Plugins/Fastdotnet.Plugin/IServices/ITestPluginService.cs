using System;

namespace Fastdotnet.Plugin.IServices
{
    public interface ITestPluginService
    {
        /// <summary>
        /// 获取插件状态
        /// </summary>
        /// <returns>插件当前状态</returns>
        string GetPluginStatus();

        /// <summary>
        /// 启动插件
        /// </summary>
        void Start();

        /// <summary>
        /// 停止插件
        /// </summary>
        void Stop();
    }
}