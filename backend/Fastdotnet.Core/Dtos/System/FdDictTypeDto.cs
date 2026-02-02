

namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdDictTypeDto
    {

        /// <summary>
        /// name
        /// </summary>
        [StringLength(64, ErrorMessage = "名称最多64个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(50, ErrorMessage = "编码最多50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [StringLength(256, ErrorMessage = "备注最多256个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// sys_flag
        /// </summary>
        public int SysFlag { get; set; }

        /// <summary>
        /// plugin_sys_flag
        /// </summary>
        public int PluginSysFlag { get; set; }

        /// <summary>
        /// plugin_id
        /// </summary>
        [Required(ErrorMessage = "插件Id不能为空")]
        [StringLength(50, ErrorMessage = "插件Id最多50个字符")]
        public string? PluginId { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdDictTypeDto
    {

        /// <summary>
        /// name
        /// </summary>
        [StringLength(64, ErrorMessage = "名称最多64个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(50, ErrorMessage = "编码最多50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [StringLength(256, ErrorMessage = "备注最多256个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// sys_flag
        /// </summary>
        public int SysFlag { get; set; }

        /// <summary>
        /// plugin_sys_flag
        /// </summary>
        public int PluginSysFlag { get; set; }

        /// <summary>
        /// plugin_id
        /// </summary>
        [StringLength(50, ErrorMessage = "插件Id最多50个字符")]
        public string? PluginId { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdDictTypeDto
    {

        /// <summary>
        /// name
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>

        public string Code { get; set; }

        /// <summary>
        /// order_no
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// remark
        /// </summary>

        public string Remark { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// sys_flag
        /// </summary>
        public int SysFlag { get; set; }

        /// <summary>
        /// plugin_sys_flag
        /// </summary>
        public int PluginSysFlag { get; set; }

        /// <summary>
        /// plugin_id
        /// </summary>

        public string? PluginId { get; set; }
    }
}
