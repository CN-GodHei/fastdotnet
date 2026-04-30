namespace Fastdotnet.Core.Dtos.App
{
    public class FdUserLayoutDto
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string LayoutData { get; set; } = null!;
        public string Name { get; set; } = "Default";
        public int IsActive { get; set; }
    }

    public class SaveFdUserLayoutDto
    {
        public string LayoutData { get; set; } = null!;
        public string Name { get; set; } = "Default";
    }

    public class UpdateFdUserLayoutDto : SaveFdUserLayoutDto
    {
        public string Id { get; set; } = null!;
    }
}
