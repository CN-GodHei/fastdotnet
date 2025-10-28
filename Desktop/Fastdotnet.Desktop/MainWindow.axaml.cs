using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Fastdotnet.Desktop.Api;
using Fastdotnet.Desktop.ViewModels;
using Fastdotnet.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider? _serviceProvider;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            // 如果没有服务提供者，创建一个没有API的ViewModel
            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel; // 设置数据上下文为 ViewModel
        }

        public MainWindow(IServiceProvider serviceProvider) : this()
        {
            _serviceProvider = serviceProvider;

            // 尝试从服务提供者获取菜单API并创建ViewModel
            var menuApi = _serviceProvider.GetService<Fastdotnet.Desktop.Api.IFastdotnetApi>();
            _viewModel = new MainWindowViewModel(menuApi);
            this.DataContext = _viewModel;

            // User is already logged in at this point, initialize UI and call APIs
            CallApiExampleAsync();
        }

        private void OnSubMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var menuItemId = button.Tag?.ToString();
                Console.WriteLine($"Sub menu item with ID '{menuItemId}' was clicked.");
                // 在实际应用中，这里可以用来导航到不同的页面或执行其他操作
            }
        }

        private async void CallApiExampleAsync()
        {
            if (_serviceProvider == null) return;

            // Get the service (this will now work with the generated interface)
            if (_serviceProvider.GetService<IFastdotnetApi>() is IFastdotnetApi api)
            {
                try
                {
                    // Call the All() endpoint to get system configuration items
                    var response = await api.All();
                    if (response.IsSuccessStatusCode && response.Content != null)
                    {
                        Console.WriteLine("Retrieved system configuration:");
                        foreach (var item in response.Content)
                        {
                            Console.WriteLine($"{item.Key}: {item.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve system configuration: {response?.Error?.Message}");
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error calling API: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("API service not available - check backend and code generation");
            }
        }
    }
}