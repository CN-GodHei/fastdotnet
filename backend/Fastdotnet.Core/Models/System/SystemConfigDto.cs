namespace Fastdotnet.Core.Models.System
{
    public class SystemConfigDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }

    public class CreateSystemConfigDto
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }

    public class UpdateSystemConfigDto
    {
        public string Id { get; set; }
        //public string Code { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
    }
}
