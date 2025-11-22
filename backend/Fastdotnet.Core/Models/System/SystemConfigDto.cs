namespace Fastdotnet.Core.Models.System
{
    public class FdSystemInfoConfigDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }

    public class CreateFdSystemInfoConfigDto
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }

    public class UpdateFdSystemInfoConfigDto
    {
        public string Id { get; set; }
        //public string Code { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }
}
