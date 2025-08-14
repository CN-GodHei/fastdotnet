using Fastdotnet.Plugin.Contracts;
using System.Collections.Generic;

namespace PluginA
{
    public class PluginAPermissionProvider : IPermissionProvider
    {
        public IEnumerable<PermissionDefinition> GetPermissions()
        {
            return new List<PermissionDefinition>
            {
                new()
                {
                    Module = "PluginA",
                    Code = PluginAPermissions.DataView,
                    Name = "查看插件A数据",
                    Category = "User",
                    Type = "Api"
                },
                new()
                {
                    Module = "PluginA",
                    Code = PluginAPermissions.SettingsManage,
                    Name = "管理插件A设置",
                    Category = "Admin",
                    Type = "Api"
                }
            };
        }
    }
}