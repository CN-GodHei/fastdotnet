using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Dto
{
    /// <summary>
    /// 新增传输对象
    /// </summary>
    public class PluginATestCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int TestValue { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string Creator { get; set; }
    }

    /// <summary>
    /// 修改传输对象
    /// </summary>
    public class PluginATestUpdateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TestValue { get; set; }
        public bool IsEnabled { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 输出传输对象
    /// </summary>
    public class PluginATestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TestValue { get; set; }
        public bool IsEnabled { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
