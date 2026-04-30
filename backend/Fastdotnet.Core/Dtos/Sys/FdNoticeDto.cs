using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.Sys
{
    public class CreateFdNoticeDto
    {
        [Required(ErrorMessage = "接收人不能为空")]
        public string ReceiverId { get; set; } = default!;
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(255, ErrorMessage = "标题最多255个字符")]
        public string Title { get; set; } = default!;
        public string? Content { get; set; }
    }

    public class UpdateFdNoticeDto
    {
        [Required(ErrorMessage = "Id不能为空")]
        public string Id { get; set; } = default!;
        public int IsRead { get; set; }
    }

    public class FdNoticeDto
    {
        public string Id { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? Content { get; set; }
        public int IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
