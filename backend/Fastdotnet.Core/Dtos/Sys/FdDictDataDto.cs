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
        [StringLength(50, ErrorMessage = "字典类型Id最多50个字符")]
        public string DictTypeId { get; set; }

        /// <summary>
        /// label
        /// </summary>
        [StringLength(256, ErrorMessage = "显示文本最多256个字符")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [StringLength(140, ErrorMessage = "值最多140个字符")]
        public string Value { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(50, ErrorMessage = "编码最多50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(256, ErrorMessage = "名称最多256个字符")]
        public string? Name { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        [Required(ErrorMessage = "显示样式-标签颜色不能为空")]
        [StringLength(16, ErrorMessage = "显示样式-标签颜色最多16个字符")]
        public string? TagType { get; set; }

        /// <summary>
        /// style_setting
        /// </summary>
        [Required(ErrorMessage = "显示样式-Style不能为空")]
        [StringLength(512, ErrorMessage = "显示样式-Style最多512个字符")]
        public string? StyleSetting { get; set; }

        /// <summary>
        /// class_setting
        /// </summary>
        [Required(ErrorMessage = "显示样式-Class不能为空")]
        [StringLength(512, ErrorMessage = "显示样式-Class最多512个字符")]
        public string? ClassSetting { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [Required(ErrorMessage = "备注不能为空")]
        [StringLength(2048, ErrorMessage = "备注最多2048个字符")]
        public string? Remark { get; set; }

        /// <summary>
        /// ext_data
        /// </summary>
        [Required(ErrorMessage = "拓展数据(保存业务功能的配置项)不能为空")]
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdDictDataDto
    {

        /// <summary>
        /// dict_type_id
        /// </summary>
        [StringLength(50, ErrorMessage = "字典类型Id最多50个字符")]
        public string DictTypeId { get; set; }

        /// <summary>
        /// label
        /// </summary>
        [StringLength(256, ErrorMessage = "显示文本最多256个字符")]
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [StringLength(140, ErrorMessage = "值最多140个字符")]
        public string Value { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(50, ErrorMessage = "编码最多50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [StringLength(256, ErrorMessage = "名称最多256个字符")]
        public string? Name { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        [StringLength(16, ErrorMessage = "显示样式-标签颜色最多16个字符")]
        public string? TagType { get; set; }

        /// <summary>
        /// style_setting
        /// </summary>
        [StringLength(512, ErrorMessage = "显示样式-Style最多512个字符")]
        public string? StyleSetting { get; set; }

        /// <summary>
        /// class_setting
        /// </summary>
        [StringLength(512, ErrorMessage = "显示样式-Class最多512个字符")]
        public string? ClassSetting { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [StringLength(2048, ErrorMessage = "备注最多2048个字符")]
        public string? Remark { get; set; }

        /// <summary>
        /// ext_data
        /// </summary>
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdDictDataDto
    {

        /// <summary>
        /// dict_type_id
        /// </summary>
        public string DictTypeId { get; set; }

        /// <summary>
        /// label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// tag_type
        /// </summary>
        public string? TagType { get; set; }

        /// <summary>
        /// style_setting
        /// </summary>

        public string? StyleSetting { get; set; }

        /// <summary>
        /// class_setting
        /// </summary>

        public string? ClassSetting { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// ext_data
        /// </summary>
        public string? ExtData { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }
    }
}
