namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdNationalStandardDictDto
    {

        /// <summary>
        /// 国标编号
        /// </summary>
        [StringLength(30, ErrorMessage = "国标编号最多30个字符")]
        public string StandardCode { get; set; }

        /// <summary>
        /// 国标名称
        /// </summary>
        [Required(ErrorMessage = "国标名称不能为空")]
        [StringLength(100, ErrorMessage = "国标名称最多100个字符")]
        public string? StandardName { get; set; }

        /// <summary>
        /// 标准编码
        /// </summary>
        [StringLength(50, ErrorMessage = "标准编码最多50个字符")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 标准名称
        /// </summary>
        [StringLength(200, ErrorMessage = "标准名称最多200个字符")]
        public string ItemName { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        [Required(ErrorMessage = "父级编码不能为空")]
        [StringLength(50, ErrorMessage = "父级编码最多50个字符")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度
        /// </summary>
        [Required(ErrorMessage = "层级深度不能为空")]
        public byte? Level { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [StringLength(20, ErrorMessage = "版本最多20个字符")]
        public string Version { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [Required(ErrorMessage = "生效日期不能为空")]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 有效版本
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 属性扩展
        /// </summary>
        public Dictionary<string, object>? ExtraObject { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdNationalStandardDictDto
    {

        /// <summary>
        /// 国标编号
        /// </summary>
        [StringLength(30, ErrorMessage = "国标编号最多30个字符")]
        public string StandardCode { get; set; }

        /// <summary>
        /// 国标名称
        /// </summary>
        [StringLength(100, ErrorMessage = "国标名称最多100个字符")]
        public string? StandardName { get; set; }

        /// <summary>
        /// 标准编码
        /// </summary>
        [StringLength(50, ErrorMessage = "标准编码最多50个字符")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 标准名称
        /// </summary>
        [StringLength(200, ErrorMessage = "标准名称最多200个字符")]
        public string ItemName { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        [StringLength(50, ErrorMessage = "父级编码最多50个字符")]
        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度
        /// </summary>
        public byte? Level { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [StringLength(20, ErrorMessage = "版本最多20个字符")]
        public string Version { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 有效版本
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 属性扩展
        /// </summary>
        public Dictionary<string, object>? ExtraObject { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdNationalStandardDictDto
    {

        /// <summary>
        /// 国标编号
        /// </summary>

        public string StandardCode { get; set; }

        /// <summary>
        /// 国标名称
        /// </summary>

        public string? StandardName { get; set; }

        /// <summary>
        /// 标准编码
        /// </summary>

        public string ItemCode { get; set; }

        /// <summary>
        /// 标准名称
        /// </summary>

        public string ItemName { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>

        public string? ParentCode { get; set; }

        /// <summary>
        /// 层级深度
        /// </summary>
        public byte? Level { get; set; }

        /// <summary>
        /// 版本
        /// </summary>

        public string Version { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 有效版本
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 属性扩展
        /// </summary>
        public Dictionary<string, object>? ExtraObject { get; set; }
    }
}