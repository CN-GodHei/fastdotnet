using System;
using Fastdotnet.Desktop.Api;

namespace Fastdotnet.Desktop.Services
{
    /// <summary>
    /// Service to manage authentication state (token, user info, etc.)
    /// </summary>
    public class AuthService
    {
        private string? _token;

        public string? Token 
        { 
            get => _token; 
            set 
            { 
                _token = value;
                // Optionally notify other parts of the app about token change
                OnTokenChanged?.Invoke(this, EventArgs.Empty);
            } 
        }

        public event EventHandler? OnTokenChanged;

        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public void Logout()
        {
            Token = null;
        }
    }
}