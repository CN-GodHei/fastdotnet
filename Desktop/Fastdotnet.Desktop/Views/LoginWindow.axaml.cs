using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Fastdotnet.Desktop.Api;
using Fastdotnet.Desktop.Api.Models;
using Fastdotnet.Desktop.Services;

namespace Fastdotnet.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFastdotnetApi _apiClient;
        private readonly AuthService _authService;

        public LoginWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _apiClient = serviceProvider.GetRequiredService<IFastdotnetApi>();
            _authService = serviceProvider.GetRequiredService<AuthService>();
            InitializeComponent();
            
            // Subscribe to token change event to close the window after successful login
            _authService.OnTokenChanged += OnTokenChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Get controls
            var loginButton = this.FindControl<Button>("LoginButton");
            var usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
            var passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");

            // Set default values for testing
            usernameTextBox.Text = "superadmin";
            passwordTextBox.Text = "123456";

            // Subscribe to login button click
            loginButton.Click += async (sender, e) => await OnLoginClick(usernameTextBox.Text, passwordTextBox.Text);
        }

        private async Task OnLoginClick(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                SetErrorMessage("Please enter both username and password.");
                return;
            }

            try
            {
                // Create login DTO
                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };

                try
                {
                    // Call the login API using the IFastdotnetApi interface
                    var response = await _apiClient.Login(loginDto);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Login successful.");
                        
                        // The API returns a wrapped response in the format:
                        // { "Data": { "Token": "actual-token" }, "Code": 0, "Msg": null }
                        // Since Refit generated the interface to return IApiResponse<LoginResultDto>,
                        // the deserialization has already processed the data from the Data field.
                        // response.Content will be the LoginResultDto which contains the token.
                        
                        string? token = null;
                        if (response.Content != null && !string.IsNullOrEmpty(response.Content.Token))
                        {
                            token = response.Content.Token;
                        }
                        else
                        {
                            Console.WriteLine("Response does not contain token in Content.");
                        }
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            _authService.Token = token;
                            Console.WriteLine("Token successfully obtained and stored.");
                        }
                        else
                        {
                            SetErrorMessage("Login successful but no token found in response.");
                            Console.WriteLine("Warning: Login successful but no token found in the response.");
                        }
                    }
                    else
                    {
                        SetErrorMessage($"Login failed: {response.StatusCode} - {response.Error?.Message}");
                        Console.WriteLine($"Login failed with status: {response.StatusCode}, Error: {response.Error?.Message}");
                    }
                }
                catch (ApiException apiEx)
                {
                    // Handle Refit-specific API errors
                    SetErrorMessage($"API Error: {apiEx.Message}");
                    Console.WriteLine($"API error during login: {apiEx}");
                }
                catch (Exception generalEx)
                {
                    // Handle other errors
                    SetErrorMessage($"Error: {generalEx.Message}");
                    Console.WriteLine($"General error during login: {generalEx}");
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage($"Login failed: {ex.Message}");
                Console.WriteLine($"Login error: {ex}");
            }
        }

        private void SetErrorMessage(string message)
        {
            var errorTextBlock = this.FindControl<TextBlock>("ErrorMessageTextBlock");
            if (errorTextBlock != null)
            {
                errorTextBlock.Text = message;
            }
        }

        private void OnTokenChanged(object? sender, EventArgs e)
        {
            // Only proceed if the token is valid (not null/empty)
            if (!string.IsNullOrEmpty(_authService.Token))
            {
                // Update the main window in the application lifetime
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // Show the main window
                    var mainWindow = new MainWindow(_serviceProvider);
                    desktop.MainWindow = mainWindow;
                    mainWindow.Show();
                }
                
                // Close the login window when token is set
                this.Close();
            }
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            // Unsubscribe to prevent memory leaks
            _authService.OnTokenChanged -= OnTokenChanged;
        }
    }
}