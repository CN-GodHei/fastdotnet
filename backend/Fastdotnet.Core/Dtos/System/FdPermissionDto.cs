
namespace Fastdotnet.Core.Dtos.System
{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdPermissionDto
    {

        /// <summary>
        /// name
        /// </summary>
        [Required(ErrorMessage = "name不能为空")]
        [StringLength(255, ErrorMessage = "name最多255个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [Required(ErrorMessage = "code不能为空")]
        [StringLength(255, ErrorMessage = "code最多255个字符")]
        public string Code { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(255, ErrorMessage = "description最多255个字符")]
        public string? Description { get; set; }

        /// <summary>
        /// module
        /// </summary>
        [StringLength(255, ErrorMessage = "module最多255个字符")]
        public string? Module { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [Required(ErrorMessage = "type不能为空")]
        public long Type { get; set; }

        /// <summary>
        /// category
        /// </summary>
        [Required(ErrorMessage = "category不能为空")]
        [StringLength(50, ErrorMessage = "category最多50个字符")]
        public string Category { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdPermissionDto
    {

        /// <summary>
        /// id
        /// </summary>
        [StringLength(255, ErrorMessage = "id最多255个字符")]
        public string Id { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [StringLength(255, ErrorMessage = "name最多255个字符")]
        public string Name { get; set; }

        /// <summary>
        /// code
        /// </summary>
        [StringLength(255, ErrorMessage = "code最多255个字符")]
        public string Code { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(255, ErrorMessage = "description最多255个字符")]
        public string? Description { get; set; }

        /// <summary>
        /// module
        /// </summary>
        [StringLength(255, ErrorMessage = "module最多255个字符")]
        public string? Module { get; set; }

        /// <summary>
        /// type
        /// </summary>
        public long Type { get; set; }

        /// <summary>
        /// category
        /// </summary>
        [StringLength(50, ErrorMessage = "category最多50个字符")]
        public string Category { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdPermissionDto
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
        /// description
        /// </summary>

        public string? Description { get; set; }

        /// <summary>
        /// module
        /// </summary>

        public string? Module { get; set; }

        /// <summary>
        /// type
        /// </summary>
        public long Type { get; set; }

        /// <summary>
        /// category
        /// </summary>

        public string Category { get; set; }

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
