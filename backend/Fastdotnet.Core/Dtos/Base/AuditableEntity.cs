using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Dtos.Base
{
    /// <summary>
    /// 审计实体接口（包含操作人信息）
    /// </summary>
    public interface IAuditableEntity : IBaseEntity
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        string? CreatedBy { get; set; }

        /// <summary>
        /// 更新人ID
        /// </summary>
        string? UpdatedBy { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        string? DeletedBy { get; set; }
    }

    /// <summary>
    /// 可审计的基础实体类（包含操作人信息）
    /// </summary>
    public class AuditableEntity : BaseEntity, IAuditableEntity
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        [SugarColumn(ColumnName = "created_by", ColumnDescription = "创建人ID", IsNullable = true)]
        public string? CreatedBy { get; set; } = string.Empty; // 默认空字符串，或可通过构造函数/拦截器赋值

        /// <summary>
        /// 更新人ID
        /// </summary>
        [SugarColumn(ColumnName = "updated_by", ColumnDescription = "更新人ID", IsNullable = true)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        [SugarColumn(ColumnName = "deleted_by", ColumnDescription = "删除人ID", IsNullable = true)]
        public string? DeletedBy { get; set; }
    }
}
