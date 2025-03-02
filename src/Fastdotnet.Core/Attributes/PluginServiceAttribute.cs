namespace Fastdotnet.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class PluginServiceAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public PluginServiceAttribute(string name, string version, string description, string author)
        {
            Name = name;
            Version = version;
            Description = description;
            Author = author;
        }
    }
}