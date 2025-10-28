using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Fastdotnet.Desktop.Api;
using Fastdotnet.Desktop.Api.Handlers;
using Fastdotnet.Desktop.Services;
using Fastdotnet.Desktop.Api.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net;

namespace Fastdotnet.Desktop
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Configure services
            var services = new ServiceCollection();

            // Register AuthService as a singleton
            services.AddSingleton<Services.AuthService>();

            // Create the unified response interceptor
            var interceptor = new UnifiedResponseInterceptor();

            // Subscribe to events for global notification
            interceptor.OnApiSuccess += (sender, e) =>
            {
                Console.WriteLine($"API Success - {e.HttpMethod} {e.RequestUrl}: HTTP Status={(int)e.HttpStatusCode}, Code={e.ResponseCode}, Msg={e.ResponseMessage}");

                // 可以在这里添加更多的成功处理逻辑
                // 例如：性能监控、日志记录等
            };

            interceptor.OnApiError += (sender, e) =>
            {
                Console.WriteLine($"API Error - {e.HttpMethod} {e.RequestUrl}: HTTP Status={(int)e.HttpStatusCode}, Code={e.ErrorCode}, Msg={e.ErrorMessage}");

                // 根据 HTTP 状态码进行不同的错误处理
                switch (e.HttpStatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        // 未授权错误处理
                        Console.WriteLine("Unauthorized access - token may be invalid");
                        break;
                    case HttpStatusCode.Forbidden:
                        // 禁止访问错误处理
                        Console.WriteLine("Access forbidden - insufficient permissions");
                        break;
                    case HttpStatusCode.NotFound:
                        // 资源未找到错误处理
                        Console.WriteLine("Resource not found");
                        break;
                    default:
                        // 其他错误处理
                        if ((int)e.HttpStatusCode >= 500)
                        {
                            Console.WriteLine("Server error occurred");
                        }
                        else if ((int)e.HttpStatusCode >= 400)
                        {
                            Console.WriteLine("Client error occurred");
                        }
                        break;
                }

                // 这里你可以显示全局通知、记录错误日志等
            };

            // Register Refit client with the generated interface
            services.AddRefitClient<IFastdotnetApi>(new RefitSettings
            {
                ContentSerializer = new UnifiedResponseContentSerializer()
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:18889/")) // Updated base URL - removed /api/ as it's included in the generated interface
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var authService = provider.GetRequiredService<Services.AuthService>();
                // Chain the handlers: interceptor -> JWT handler -> HTTP handler
                interceptor.InnerHandler = new Api.Handlers.Internal.InternalJwtHandler(() => authService.Token);
                return interceptor;
            });

            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var authService = Services.GetRequiredService<Services.AuthService>();

                if (!authService.IsLoggedIn)
                {
                    // Show login window if not logged in
                    var loginWindow = new Views.LoginWindow(Services);
                    desktop.MainWindow = loginWindow;
                    desktop.MainWindow.Show();
                }
                else
                {
                    // User is already logged in, show main window
                    desktop.MainWindow = new MainWindow(Services);
                    desktop.MainWindow.Show();
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}