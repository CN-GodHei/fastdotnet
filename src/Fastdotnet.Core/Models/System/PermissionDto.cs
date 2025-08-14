namespace Fastdotnet.Core.Models.System
{
    public class PermissionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? Module { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
