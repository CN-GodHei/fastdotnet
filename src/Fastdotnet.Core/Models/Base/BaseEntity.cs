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
        long Id { get; set; }
        
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
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDataType = "datetime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime? DeleteTime { get; set; }
    }
}