

namespace Fastdotnet.Core.Dtos.Base
{
    /// <summary>
    /// 基础实体类接口
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        string Id { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletedAt { get; set; }
    }
    
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseEntity : IBaseEntity, ISoftDelete
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, ColumnDescription="主键id")]
        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SplitField]
        [SugarColumn(ColumnName = "created_at", ColumnDescription = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "updated_at", ColumnDescription = "更新时间", IsNullable = true)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted", ColumnDescription = "是否删除")]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(ColumnName = "deleted_at", ColumnDescription = "删除时间", IsNullable = true)]
        public DateTime? DeletedAt { get; set; }
    }
}