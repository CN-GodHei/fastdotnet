using Fastdotnet.Core.Constants;
using Fastdotnet.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fastdotnet.Service.Service.System
{
    /// <summary>
    /// 主框架的权限提供者
    /// </summary>
    public class FrameworkPermissionProvider : IPermissionProvider
    {
        public IEnumerable<PermissionDefinition> GetPermissions()
        {
            // 使用反射来自动扫描Permissions静态类
            return typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .SelectMany(scopeType => GetPermissionsFromScope(scopeType));
        }

        private IEnumerable<PermissionDefinition> GetPermissionsFromScope(Type scopeType)
        {
            var scopeName = scopeType.Name.ToLower(); // e.g., "admin"

            return scopeType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .SelectMany(moduleType => GetPermissionsFromModule(scopeName, moduleType));
        }

        private IEnumerable<PermissionDefinition> GetPermissionsFromModule(string scopeName, Type moduleType)
        {
            var moduleName = moduleType.Name.ToLower(); // e.g., "users"

            return moduleType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi =>
                {
                    var code = (string)fi.GetValue(null);
                    var name = fi.Name; // e.g., "View", "Create"
                    return new PermissionDefinition
                    {
                        Module = moduleName,
                        Code = code,
                        Name = $"{scopeName}.{moduleName}.{name}", // You might want a more sophisticated naming system
                        Category = scopeName.Substring(0, 1).ToUpper() + scopeName.Substring(1), // "admin" -> "Admin"
                        Type = "Api"
                    };
                });
        }
    }
}
