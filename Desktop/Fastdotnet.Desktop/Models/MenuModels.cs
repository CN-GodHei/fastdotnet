using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fastdotnet.Desktop.Models
{
    public class MenuResponse
    {
        public int Code { get; set; }
        public string? Msg { get; set; }
        public List<MenuApiItem>? Data { get; set; }
    }

    public class MenuApiItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Component { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public string? ParentId { get; set; }
        public List<MenuApiItem>? Children { get; set; }
    }
    
    // 用于UI显示的菜单项
    public class MenuItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        // 可以根据需要添加更多属性，如图标、路由路径等
        public ObservableCollection<MenuItem> Children { get; set; } = new ObservableCollection<MenuItem>();
    }
}