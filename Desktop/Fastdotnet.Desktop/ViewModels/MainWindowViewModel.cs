using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Fastdotnet.Desktop.Api;
using Fastdotnet.Desktop.Api.Models;
using Fastdotnet.Desktop.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Fastdotnet.Desktop.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set => this.RaiseAndSetIfChanged(ref _menuItems, value);
        }

        public ReactiveCommand<string, Unit> MenuItemClickCommand { get; }

        private readonly IFastdotnetApi? _menuApi;

        public MainWindowViewModel(IFastdotnetApi? menuApi = null)
        {
            _menuApi = menuApi;
            MenuItemClickCommand = ReactiveCommand.Create<string>(OnMenuItemClick);
            LoadMenuDataAsync();
        }

        private async Task LoadMenuDataAsync()
        {
            try
            {
                if (_menuApi != null)
                {
                    // 从后端获取菜单数据 - 调用获取菜单树的API
                    var response = await _menuApi.Tree2(); // 使用正确的API方法获取菜单树
                    
                    // 清空现有菜单
                    MenuItems.Clear();
                    
                    // 将API返回的菜单数据转换为用于UI显示的结构
                    if (response.IsSuccessStatusCode && response.Content != null)
                    {
                        foreach (var apiMenuItem in response.Content)
                        {
                            var uiMenuItem = ConvertToUIMenuItem(apiMenuItem);
                            MenuItems.Add(uiMenuItem);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to load menu data: {response?.Error?.Message}");
                    }
                }
                else
                {
                    // 模拟从后端获取菜单数据（如果API实例未提供）
                    var sampleMenus = new List<MenuItem>
                    {
                        new Fastdotnet.Desktop.Models.MenuItem { Id = "1", Name = "Dashboard" },
                        new Fastdotnet.Desktop.Models.MenuItem { Id = "2", Name = "Products" },
                        new Fastdotnet.Desktop.Models.MenuItem { Id = "3", Name = "Users" }
                    };

                    // 添加子菜单
                    sampleMenus[1].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "2-1", Name = "List Products" });
                    sampleMenus[1].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "2-2", Name = "Categories" });

                    sampleMenus[2].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "3-1", Name = "List Users" });
                    sampleMenus[2].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "3-2", Name = "Roles" });

                    // 清空并加载新数据
                    MenuItems.Clear();
                    foreach (var item in sampleMenus)
                    {
                        MenuItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading menu data: {ex.Message}");
                
                // 如果加载失败，使用示例数据
                var sampleMenus = new List<MenuItem>
                {
                    new Fastdotnet.Desktop.Models.MenuItem { Id = "1", Name = "Dashboard" },
                    new Fastdotnet.Desktop.Models.MenuItem { Id = "2", Name = "Products" },
                    new Fastdotnet.Desktop.Models.MenuItem { Id = "3", Name = "Users" }
                };

                // 添加子菜单
                sampleMenus[1].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "2-1", Name = "List Products" });
                sampleMenus[1].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "2-2", Name = "Categories" });

                sampleMenus[2].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "3-1", Name = "List Users" });
                sampleMenus[2].Children.Add(new Fastdotnet.Desktop.Models.MenuItem { Id = "3-2", Name = "Roles" });

                // 清空并加载新数据
                MenuItems.Clear();
                foreach (var item in sampleMenus)
                {
                    MenuItems.Add(item);
                }
            }
        }

        private MenuItem ConvertToUIMenuItem(MenuDto apiItem)
        {
            var uiItem = new Fastdotnet.Desktop.Models.MenuItem
            {
                Id = apiItem.Id,
                Name = apiItem.Name
            };

            if (apiItem.Children != null)
            {
                foreach (var child in apiItem.Children)
                {
                    uiItem.Children.Add(ConvertToUIMenuItem(child));
                }
            }

            return uiItem;
        }

        private void OnMenuItemClick(string menuItemId)
        {
            // 处理菜单项点击逻辑
            Console.WriteLine($"Menu item with ID '{menuItemId}' was clicked.");
            // 在实际应用中，这里可以用来导航到不同的页面或执行其他操作
        }
    }
}