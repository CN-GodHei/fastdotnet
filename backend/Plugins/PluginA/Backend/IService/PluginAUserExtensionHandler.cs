using Fastdotnet.Core.Entities.App;
using PluginA.Entities;
using SqlSugar;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Fastdotnet.Core.Extensibility.Users;

namespace PluginA.IService
{
    /// <summary>
    /// PluginA 用户扩展数据处理器
    /// </summary>
    public class PluginAUserExtensionHandler : FdAppUserExtensionHandlerBase<PluginAUserExtension>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storageContext">存储上下文</param>
        public PluginAUserExtensionHandler(IStorageContext storageContext) 
            : base(storageContext, "p_pluginA_user_extensions")
        {
        }

        /// <summary>
        /// 设置用户ID到扩展数据中
        /// </summary>
        protected override void SetUserId(PluginAUserExtension data, string userId)
        {
            data.FdAppUserId = userId;
        }
    }
}