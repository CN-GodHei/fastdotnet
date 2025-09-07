namespace Fastdotnet.Core.Constants
{
    /// <summary>
    /// 系统常量配置
    /// </summary>
    public static class SystemConstants
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public const string SystemName = "Fastdotnet管理系统";

        /// <summary>
        /// 系统版本
        /// </summary>
        public const string Version = "1.0.0";

        /// <summary>
        /// JWT密钥
        /// </summary>
        public const string JwtSecretKey = "FastdotnetSecretKey";

        /// <summary>
        /// JWT过期时间（分钟）
        /// </summary>
        public const int JwtExpiresMinutes = 120;

        /// <summary>
        /// 默认分页大小
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// 最大分页大小
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// 密码加密密钥
        /// </summary>
        public const string PasswordKey = "Fastdotnet@2024";

        /// <summary>
        /// 超级管理员角色编码
        /// </summary>
        public const string SuperAdminRoleCode = "SUPER_ADMIN";
    }
}