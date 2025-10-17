namespace Fastdotnet.Core.Constants
{
    /// <summary>
    /// 定义系统中所有权限的静态常量
    /// 格式：范围.模块.操作
    /// </summary>
    public static class Permissions
    {
        public static class Admin
        {
            public static class Users
            {
                public const string View = "admin.users.view";
                public const string Create = "admin.users.create";
                public const string Edit = "admin.users.edit";
                public const string Delete = "admin.users.delete";
                public const string ResetPassword = "admin.users.resetpassword";
            }

            public static class Roles
            {
                public const string View = "admin.roles.view";
                public const string Create = "admin.roles.create";
                public const string Edit = "admin.roles.edit";
                public const string Delete = "admin.roles.delete";
                public const string AssignPermissions = "admin.roles.assignpermissions";
            }
            
            public static class Permissions
            {
                public const string View = "admin.permissions.view";
            }

            public static class Menus
            {
                public const string View = "admin.menus.view";
                public const string Create = "admin.menus.create";
                public const string Edit = "admin.menus.edit";
                public const string Delete = "admin.menus.delete";
            }

            public static class MenuButtons
            {
                public const string View = "admin.menubuttons.view";
                public const string Create = "admin.menubuttons.create";
                public const string Edit = "admin.menubuttons.edit";
                public const string Delete = "admin.menubuttons.delete";
            }
        }

        /*
        // 示例：未来前台用户的权限可以这样扩展
        public static class App
        {
            public static class Profile
            {
                public const string View = "app.profile.view";
                public const string Edit = "app.profile.edit";
            }
        }
        */

        public static class System
        {
            public static class CodeGen
            {
                public const string View = "system.codegen.view";
                public const string Create = "system.codegen.create";
                public const string Edit = "system.codegen.edit";
                public const string Delete = "system.codegen.delete";
            }
        }
    }
}