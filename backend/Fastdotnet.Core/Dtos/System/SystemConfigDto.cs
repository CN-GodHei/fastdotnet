
namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdSystemInfoConfigDto
    {

        /// <summary>
        /// name
        /// </summary>
        [Required(ErrorMessage = "name不能为空")]
        [StringLength(100, ErrorMessage = "name最多100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [Required(ErrorMessage = "code不能为空")]
        [StringLength(100, ErrorMessage = "code最多100个字符")]
        public string Code { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(500, ErrorMessage = "description最多500个字符")]
        public string? Description { get; set; }

        /// <summary>
        /// is_system
        /// </summary>
        [Required(ErrorMessage = "is_system不能为空")]
        public bool IsSystem { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdSystemInfoConfigDto
    {

        /// <summary>
        /// id
        /// </summary>
        [StringLength(255, ErrorMessage = "id最多255个字符")]
        public string Id { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [StringLength(100, ErrorMessage = "name最多100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(100, ErrorMessage = "code最多100个字符")]
        public string Code { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(500, ErrorMessage = "description最多500个字符")]
        public string? Description { get; set; }

        /// <summary>
        /// is_system
        /// </summary>
        public bool IsSystem { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdSystemInfoConfigDto
    {

        /// <summary>
        /// id
        /// </summary>

        public string Id { get; set; }

        /// <summary>
        /// name
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>

        public string Code { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// description
        /// </summary>

        public string? Description { get; set; }

        /// <summary>
        /// is_system
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// created_at
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// updated_at
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
