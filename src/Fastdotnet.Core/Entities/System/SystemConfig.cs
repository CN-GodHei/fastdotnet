using Fastdotnet.Core.Models.Base;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 系统配置表(键值对)
    /// </summary>
    [SugarTable("FdSystemConfig")]
    [SugarIndex("idx_config_code", nameof(Code), OrderByType.Asc, IsUnique = true)]
    public class SystemConfig : BaseEntity
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 100)]
        public string Name { get; set; }

        /// <summary>
        /// 配置编码 (Key)
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 100)]
        public string Code { get; set; }

        /// <summary>
        /// 配置值 (Value)
        /// </summary>
        [SugarColumn(IsNullable = true, IsJson = true, ColumnDataType = "text")]
        public object Value { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500)]
        public string Description { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}
