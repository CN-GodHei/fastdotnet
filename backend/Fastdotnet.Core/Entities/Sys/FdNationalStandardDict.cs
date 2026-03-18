namespace Fastdotnet.Core.Entities.Sys
{
    /// <summary>
    /// 国家标准数据字典（统一存储各类 GB/T 标准）
    /// </summary>
    [SugarTable("fd_national_standard_dict", "国家标准数据字典")]
    public class FdNationalStandardDict : BaseEntity
    {

        /// <summary>
        /// 国标编号（唯一标识），如 "GB/T 2260", "GB/T 14396"
        /// </summary>
        [SugarColumn(ColumnName = "standard_code", Length = 30, IsNullable = false)]
        public string StandardCode { get; set; } = string.Empty;

        /// <summary>
        /// 国标中文名称（可选，用于界面展示），如 "疾病分类与代码"
        /// </summary>
        [SugarColumn(ColumnName = "standard_name", Length = 100, IsNullable = true)]
        public string? StandardName { get; set; }

        /// <summary>
        /// 标准项编码（国标中的正式代码）
        /// </summary>
        [SugarColumn(ColumnName = "item_code", Length = 50, IsNullable = false)]
        public string ItemCode { get; set; } = string.Empty;

        /// <summary>
        /// 标准项名称
        /// </summary>
        [SugarColumn(ColumnName = "item_name", Length = 200, IsNullable = false)]
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// 父级编码（用于树形结构）
        /// </summary>
        [SugarColumn(ColumnName = "parent_code", Length = 50, IsNullable = true)]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度（可选）
        /// </summary>
        [SugarColumn(ColumnName = "level", IsNullable = true)]
        public byte? Level { get; set; }

        /// <summary>
        /// 标准版本，如 "2023", "2024-clinical"
        /// </summary>
        [SugarColumn(ColumnName = "version", Length = 20, IsNullable = false, DefaultValue = "latest")]
        public string Version { get; set; } = "latest";

        /// <summary>
        /// 生效日期
        /// </summary>
        [SugarColumn(ColumnName = "effective_date", IsNullable = true)]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 是否为当前有效版本
        /// </summary>
        [SugarColumn(ColumnName = "is_current", IsNullable = false, DefaultValue = "1")]
        public bool IsCurrent { get; set; } = true;

        /// <summary>
        /// 状态：1=启用，0=废止
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = false, DefaultValue = "1")]
        public bool Status { get; set; }

        /// <summary>
        /// 排序值，数值越小越靠前。默认可设为 9999（表示按原始顺序）
        /// </summary>
        [SugarColumn(ColumnName = "sort", IsNullable = false, DefaultValue = "9999")]
        public int Sort { get; set; } = 9999;

        /// <summary>
        /// 扩展属性（JSON格式）
        /// </summary>
        [SugarColumn(ColumnName = "extra", IsNullable = true)]
        public string? Extra { get; set; }

        // 非映射属性：方便操作 JSON
        //[SugarColumn(IsIgnore = true)]
        //public Dictionary<string, object>? ExtraObject
        //{
        //    get => string.IsNullOrEmpty(Extra) ? null :
        //           Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Extra);
        //    set => Extra = value == null ? null :
        //           Newtonsoft.Json.JsonConvert.SerializeObject(value);
        //}

    }
}
