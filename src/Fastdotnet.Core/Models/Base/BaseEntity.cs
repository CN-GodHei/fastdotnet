using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using System;

namespace Fastdotnet.Core.Models.Base
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
        DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? UpdateTime { get; set; }
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
        [SugarColumn(ColumnName = "create_time", ColumnDescription = "创建时间", ColumnDataType = "datetime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", ColumnDescription = "更新时间", IsNullable = true, ColumnDataType = "datetime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted", ColumnDescription = "是否删除")]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(ColumnName = "delete_time", ColumnDescription = "删除时间", IsNullable = true, ColumnDataType = "datetime")]
        public DateTime? DeleteTime { get; set; }
    }
}