namespace Fastdotnet.Core.Dtos.Sys
{
    public class FdWorkbenchCardDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string Type { get; set; } = "List";
        public string DataSourceUrl { get; set; } = null!;
        public string? ConfigJson { get; set; }
        public int DefaultWidth { get; set; }
        public int DefaultHeight { get; set; }
    }

    public class CreateFdWorkbenchCardDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string Type { get; set; } = "List";
        public string DataSourceUrl { get; set; } = null!;
        public string? ConfigJson { get; set; }
        public int DefaultWidth { get; set; } = 4;
        public int DefaultHeight { get; set; } = 2;
    }

    public class UpdateFdWorkbenchCardDto : CreateFdWorkbenchCardDto
    {
        public string Id { get; set; } = null!;
    }
}
