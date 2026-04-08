using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.Oidc
{
    /// <summary>
    /// OpenIddict 作用域实体（SqlSugar 版本）
    /// </summary>
    [SugarTable("OpenIddictScopes")]
    public class OidcScope
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 并发令牌（用于乐观锁）
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ConcurrencyToken { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// 描述（多语言，JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Descriptions { get; set; }

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
        /// 作用域名称（唯一）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? Name { get; set; }

        /// <summary>
        /// 自定义属性（JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Properties { get; set; }

        /// <summary>
        /// 资源列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Resources { get; set; }
    }
}
