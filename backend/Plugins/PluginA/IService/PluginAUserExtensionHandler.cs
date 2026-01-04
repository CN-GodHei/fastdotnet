using Fastdotnet.Core.Entities.App;
using Fastdotnet.Plugin.Contracts;
using Fastdotnet.Plugin.Contracts.Extensibility.Users;
using PluginA.Entities;
using SqlSugar;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace PluginA.IService
{
    /// <summary>
    /// PluginA 用户扩展数据处理器
    /// </summary>
    public class PluginAUserExtensionHandler : IFdAppUserExtensionHandler<PluginAUserExtension>
    {
        private readonly ISqlSugarClient _db;

        public PluginAUserExtensionHandler(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 保存用户扩展数据
        /// </summary>
        public async Task SaveAsync(string userId, PluginAUserExtension data, IStorageContext context, CancellationToken ct = default)
        {
            // 设置用户ID
            data.FdAppUserId = userId;

            // 使用IStorageContext接口进行存储操作
            await context.SaveEntityAsync(data, ct);
        }

        /// <summary>
        /// 加载用户扩展数据
        /// </summary>
        public async Task<PluginAUserExtension?> LoadAsync(string userId, IStorageContext context, CancellationToken ct = default)
        {
            // 使用IStorageContext接口进行查询操作
            var sql = "SELECT * FROM Fd_PluginAUserExtension WHERE FdAppUserId = @userId";
            var parameters = new { userId = userId };

            return await context.QuerySingleOrDefaultAsync<PluginAUserExtension>(sql, parameters, ct);
        }
    }
}