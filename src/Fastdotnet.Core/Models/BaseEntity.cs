using System;

namespace Fastdotnet.Core.Models
{
    /// <summary>
    /// 基础实体类，所有实体都应继承此类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatorId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新人ID
        /// </summary>
        public long? UpdaterId { get; set; }

        /// <summary>
        /// 更新人名称
        /// </summary>
        public string UpdaterName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        public long? DeleterId { get; set; }

        /// <summary>
        /// 删除人名称
        /// </summary>
        public string DeleterName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}