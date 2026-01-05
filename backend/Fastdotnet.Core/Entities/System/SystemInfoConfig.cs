using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.Enum;
using SqlSugar;

namespace Fastdotnet.Core.Entities.System
{
    /// <summary>
    /// 系统配置表(键值对)
    /// </summary>
    [SugarTable("fd_system_info_config", "系统信息配置")]
    [SugarIndex("idx_config_code", nameof(Code), OrderByType.Asc, nameof(Belong), OrderByType.Asc, true)]
    public class SystemInfoConfig : BaseEntity
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, Length = 100, ColumnDescription = "配置名称")]
        public string Name { get; set; }

        /// <summary>
        /// 配置编码 (Key)
        /// </summary>
        [SugarColumn(ColumnName = "code", IsNullable = false, Length = 100, ColumnDescription = "配置编码 (Key)")]
        public string Code { get; set; }

        /// <summary>
        /// 配置值 (Value)
        /// </summary>
        [SugarColumn(ColumnName = "value", IsNullable = true, IsJson = true, ColumnDataType = "text", ColumnDescription = "配置值 (Value)")]
        public object Value { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        [SugarColumn(ColumnName = "description", IsNullable = true, Length = 500, ColumnDescription = "配置描述")]
        public string Description { get; set; }

        /// <summary>
        /// 是否为系统内置
        /// </summary>
        [SugarColumn(ColumnName = "is_system", ColumnDescription = "是否为系统内置")]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// 属于
        /// </summary>
        [SugarColumn(ColumnName = "belong", IsNullable = false, ColumnDescription = "属于管理端还是应用端")]
        public SystemCategory Belong { get; set; } 
    }
}
