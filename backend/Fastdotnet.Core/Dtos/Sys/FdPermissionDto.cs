namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    ///��������ģ��
    /// </summary>
    public class CreateFdPermissionDto
    {

        /// <summary>
        /// name
        /// </summary>
        [Required(ErrorMessage = "name����Ϊ��")]
        [StringLength(255, ErrorMessage = "name���255���ַ�")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// code
        /// </summary>
        [Required(ErrorMessage = "code����Ϊ��")]
        [StringLength(255, ErrorMessage = "code���255���ַ�")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// description
        /// </summary>
        [StringLength(255, ErrorMessage = "description���255���ַ�")]
        public string? Description { get; set; }

        /// <summary>
        /// module
        /// </summary>
        [StringLength(255, ErrorMessage = "module���255���ַ�")]
        public string? Module { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [Required(ErrorMessage = "type����Ϊ��")]
        public long Type { get; set; }

        /// <summary>
        /// category
        /// </summary>
        [Required(ErrorMessage = "category����Ϊ��")]
        [StringLength(50, ErrorMessage = "category���50���ַ�")]
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    ///�޸Ĵ���ģ��
    /// </summary>
    public class UpdateFdPermissionDto
    {

        /// <summary>
        /// id
        /// </summary>
        [StringLength(255, ErrorMessage = "id���255���ַ�")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// name
        /// </summary>
        [StringLength(255, ErrorMessage = "name���255���ַ�")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// code
        /// </summary>
        [StringLength(255, ErrorMessage = "code���255���ַ�")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// description
        /// </summary>
        [StringLength(255, ErrorMessage = "description���255���ַ�")]
        public string? Description { get; set; }

        /// <summary>
        /// module
        /// </summary>
        [StringLength(255, ErrorMessage = "module���255���ַ�")]
        public string? Module { get; set; }

        /// <summary>
        /// type
        /// </summary>
        public long Type { get; set; }

        /// <summary>
        /// category
        /// </summary>
        [StringLength(50, ErrorMessage = "category���50���ַ�")]
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    ///�������ģ��
    /// </summary>
    public class FdPermissionDto
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

        public string Category { get; set; } = string.Empty;

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
