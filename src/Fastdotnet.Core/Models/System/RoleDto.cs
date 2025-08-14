using System;

namespace Fastdotnet.Core.Models.System
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
