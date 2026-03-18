using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Initializers;
using PluginA.Entities;
using SqlSugar;
using System.Threading.Tasks;

namespace PluginA.Initializers
{
    public class PluginAUserExtensionInitializer : IStartupTask
    {
        private readonly ISqlSugarClient _db;

        public PluginAUserExtensionInitializer(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task ExecuteAsync()
        {
            // 创建PluginA用户扩展表（如果不存在）
            //await _db.CodeFirst.InitTablesAsync<PluginAUserExtension>();
        }
    }
}