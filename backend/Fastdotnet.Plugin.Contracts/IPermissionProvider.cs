
namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 用于在提供者之间传递权限定义的标准模型
    /// </summary>
    public class PermissionDefinition
    {
        /// <summary>
        /// 权限所属模块 (主框架应为"System", 插件应为插件ID)
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 权限代码 (e.g., "admin.users.view")
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限显示名称 (e.g., "查看用户")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限分类: "Admin" 或 "User"
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 权限类型: "Api", "Menu", "Data" 等
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// 权限提供者接口，所有需要动态注册权限的模块都应实现此接口
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// 获取本模块定义的所有权限
        /// </summary>
        /// <returns>权限定义列表</returns>
        IEnumerable<PermissionDefinition> GetPermissions();
    }
}
