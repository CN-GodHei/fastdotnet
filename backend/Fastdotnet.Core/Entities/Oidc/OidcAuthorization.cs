using SqlSugar;
using System;

namespace Fastdotnet.Core.Entities.Oidc
{
    /// <summary>
    /// OpenIddict 授权记录实体（SqlSugar 版本）
    /// </summary>
    [SugarTable("OpenIddictAuthorizations")]
    public class OidcAuthorization
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 应用 ID（外键）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? ApplicationId { get; set; }

        /// <summary>
        /// 并发令牌（用于乐观锁）
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? ConcurrencyToken { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// 自定义属性（JSON 格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Properties { get; set; }

        /// <summary>
        /// 作用域列表（JSON 数组格式）
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string? Scopes { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? Status { get; set; }

        /// <summary>
        /// 主题（用户标识）
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string? Subject { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string? Type { get; set; }

        /// <summary>
        /// 导航属性：关联的应用
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ApplicationId))]
        public OidcApplication? Application { get; set; }
    }
}
