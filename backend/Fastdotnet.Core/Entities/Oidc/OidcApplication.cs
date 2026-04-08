using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.Oidc
{
    /// <summary>
    /// OpenIddict 应用/客户端实体（SqlSugar 版本）
    /// </summary>
    [SugarTable("OpenIddictApplications")]
    public class OidcApplication
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 应用类型
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ApplicationType { get; set; }

        /// <summary>
        /// 客户端 ID
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? ClientId { get; set; }

        /// <summary>
        /// 客户端密钥
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ClientType { get; set; }

        /// <summary>
        /// 并发令牌（用于乐观锁）
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ConcurrencyToken { get; set; }

        /// <summary>
        /// 同意类型
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ConsentType { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 显示名称（多语言，JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? DisplayNames { get; set; }

        /// <summary>
        /// JWKS（JSON Web Key Set）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? JsonWebKeySet { get; set; }

        /// <summary>
        /// 权限列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Permissions { get; set; }

        /// <summary>
        /// 登出后重定向 URI 列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// 自定义属性（JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Properties { get; set; }

        /// <summary>
        /// 重定向 URI 列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? RedirectUris { get; set; }

        /// <summary>
        /// 要求列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Requirements { get; set; }

        /// <summary>
        /// 设置（JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Settings { get; set; }
    }
}
