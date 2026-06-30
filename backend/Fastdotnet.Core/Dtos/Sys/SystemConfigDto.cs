namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    ///��������ģ��
    /// </summary>
    public class CreateFdSystemInfoConfigDto
    {

        /// <summary>
        /// name
        /// </summary>
        [Required(ErrorMessage = "name����Ϊ��")]
        [StringLength(100, ErrorMessage = "name���100���ַ�")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// code
        /// </summary>
        [Required(ErrorMessage = "code����Ϊ��")]
        [StringLength(100, ErrorMessage = "code���100���ַ�")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(500, ErrorMessage = "description���500���ַ�")]
        public string? Description { get; set; }

        /// <summary>
        /// is_system
        /// </summary>
        [Required(ErrorMessage = "is_system����Ϊ��")]
        public bool IsSystem { get; set; }
    }

    /// <summary>
    ///�޸Ĵ���ģ��
    /// </summary>
    public class UpdateFdSystemInfoConfigDto
    {

        /// <summary>
        /// id
        /// </summary>
        [StringLength(255, ErrorMessage = "id���255���ַ�")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// name
        /// </summary>
        [StringLength(100, ErrorMessage = "name���100���ַ�")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// code
        /// </summary>
        [StringLength(100, ErrorMessage = "code���100���ַ�")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [StringLength(500, ErrorMessage = "description���500���ַ�")]
        public string? Description { get; set; }

        /// <summary>
        /// is_system
        /// </summary>
        public bool IsSystem { get; set; }
    }

    /// <summary>
    ///�������ģ��
    /// </summary>
    public class FdSystemInfoConfigDto
    {

        /// <summary>
        /// id
        /// </summary>

        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// name
        /// </summary>

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// code
        /// </summary>

        public string Code { get; set; } = string.Empty;

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
