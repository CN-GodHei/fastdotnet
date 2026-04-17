namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdDictDataDto
    {
        /// <summary>
        /// dict_type_id
        /// </summary>
        [StringLength(50, ErrorMessage = "字典类型 Id 最多 50 个字符")]
        public string DictTypeId { get; set; }

        /// <summary>
        /// dict_type_code（冗余字段，便于查询）
        /// </summary>
        [StringLength(100, ErrorMessage = "字典类型编码最多 100 个字符")]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// label
        /// </summary>
        [StringLength(128, ErrorMessage = "显示文本最多 128 个字符")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [StringLength(500, ErrorMessage = "值最多 500 个字符")]
        public string Value { get; set; }

        /// <summary>
        /// 值的数据类型（支持自动转换：枚举<->字符串）
        /// </summary>
        public int ValueType { get; set; }  // 默认 0=String

        /// <summary>
        /// code
        /// </summary>
        [StringLength(100, ErrorMessage = "编码最多 100 个字符")]
        public string? Code { get; set; }

        /// <summary>
        /// parent_id（支持树形结构）
        /// </summary>
        [StringLength(50, ErrorMessage = "父级 ID 最多 50 个字符")]
        public string ParentId { get; set; } = "";

        /// <summary>
        /// level（层级深度）
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// remark
        /// </summary>
        [StringLength(500, ErrorMessage = "备注最多 500 个字符")]
        public string? Remark { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        [StringLength(20, ErrorMessage = "状态标识最多 20 个字符")]
        public string? TagType { get; set; }

        /// <summary>
        /// css_class
        /// </summary>
        [StringLength(100, ErrorMessage = "CSS 类名最多 100 个字符")]
        public string? CssClass { get; set; }

        /// <summary>
        /// list_class
        /// </summary>
        [StringLength(100, ErrorMessage = "列表样式最多 100 个字符")]
        public string? ListClass { get; set; }

        /// <summary>
        /// is_default
        /// </summary>
        public int IsDefault { get; set; } = 0;

        /// <summary>
        /// ext_data
        /// </summary>
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; } = 1;
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdDictDataDto
    {
        /// <summary>
        /// id
        /// </summary>
        [Required(ErrorMessage = "ID 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// dict_type_id
        /// </summary>
        [StringLength(50, ErrorMessage = "字典类型 Id 最多 50 个字符")]
        public string DictTypeId { get; set; }

        /// <summary>
        /// dict_type_code
        /// </summary>
        [StringLength(100, ErrorMessage = "字典类型编码最多 100 个字符")]
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// label
        /// </summary>
        [StringLength(128, ErrorMessage = "显示文本最多 128 个字符")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [StringLength(500, ErrorMessage = "值最多 500 个字符")]
        public string Value { get; set; }

        /// <summary>
        /// 值的数据类型
        /// </summary>
        public int ValueType { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(100, ErrorMessage = "编码最多 100 个字符")]
        public string? Code { get; set; }

        /// <summary>
        /// parent_id
        /// </summary>
        [StringLength(50, ErrorMessage = "父级 ID 最多 50 个字符")]
        public string ParentId { get; set; } = "";

        /// <summary>
        /// level
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; } = 100;

        /// <summary>
        /// remark
        /// </summary>
        [StringLength(500, ErrorMessage = "备注最多 500 个字符")]
        public string? Remark { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        [StringLength(20, ErrorMessage = "状态标识最多 20 个字符")]
        public string? TagType { get; set; }

        /// <summary>
        /// css_class
        /// </summary>
        [StringLength(100, ErrorMessage = "CSS 类名最多 100 个字符")]
        public string? CssClass { get; set; }

        /// <summary>
        /// list_class
        /// </summary>
        [StringLength(100, ErrorMessage = "列表样式最多 100 个字符")]
        public string? ListClass { get; set; }

        /// <summary>
        /// is_default
        /// </summary>
        public int IsDefault { get; set; } = 0;

        /// <summary>
        /// ext_data
        /// </summary>
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; } = 1;
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdDictDataDto
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// dict_type_id
        /// </summary>
        public string DictTypeId { get; set; }

        /// <summary>
        /// dict_type_code
        /// </summary>
        public string? DictTypeCode { get; set; }

        /// <summary>
        /// label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 值的数据类型（0-String,1-Int,2-Long,3-Double,4-Decimal,5-Boolean,6-DateTime,7-Json,8-JsonArray）
        /// </summary>
        public int ValueType { get; set; }

        /// <summary>
        /// code
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// parent_id
        /// </summary>
        public string ParentId { get; set; } = "";

        /// <summary>
        /// level
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        public string? TagType { get; set; }

        /// <summary>
        /// css_class
        /// </summary>
        public string? CssClass { get; set; }

        /// <summary>
        /// list_class
        /// </summary>
        public string? ListClass { get; set; }

        /// <summary>
        /// is_default
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// ext_data
        /// </summary>
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// children（子节点）
        /// </summary>
        public List<FdDictDataDto>? Children { get; set; }
    }

    /// <summary>
    /// 简化版字典数据（用于前端下拉选择等场景）
    /// </summary>
    public class FdDictDataSimple
    {
        /// <summary>
        /// 编码（程序内部标识，可选）
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 值（实际提交的值）
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 显示文本（下拉框显示的标签）
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// 最简版字典数据（仅 label 和 value，适用于简单下拉场景）
    /// </summary>
    public class FdDictDataMinimal
    {
        /// <summary>
        /// 值（实际提交的值）
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 显示文本（下拉框显示的标签）
        /// </summary>
        public string Label { get; set; }
    }
}
