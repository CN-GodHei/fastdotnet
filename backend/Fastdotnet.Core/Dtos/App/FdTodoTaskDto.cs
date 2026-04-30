using System.ComponentModel.DataAnnotations;

namespace Fastdotnet.Core.Dtos.App
{
    public class CreateFdTodoTaskDto
    {
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(255, ErrorMessage = "标题最多255个字符")]
        public string Title { get; set; } = default!;
        public string? BusinessId { get; set; }
        public string? Route { get; set; }
        [Required(ErrorMessage = "办理人不能为空")]
        public string AssigneeId { get; set; } = default!;
        public string? WorkflowInstanceId { get; set; }
    }

    public class UpdateFdTodoTaskDto
    {
        [Required(ErrorMessage = "Id不能为空")]
        public string Id { get; set; } = default!;
        public string? Title { get; set; }
        public int? Status { get; set; }
    }

    public class FdTodoTaskDto
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? BusinessId { get; set; }
        public string? Route { get; set; }
        public string AssigneeId { get; set; } = default!;
        public int Status { get; set; }
        public string? WorkflowInstanceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
