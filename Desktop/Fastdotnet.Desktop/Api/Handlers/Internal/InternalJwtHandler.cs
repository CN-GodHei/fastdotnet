using System;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fastdotnet.Desktop.Api.Handlers.Internal
{
    /// <summary>
    /// Internal handler to add JWT token to the Authorization header
    /// </summary>
    public class InternalJwtHandler : DelegatingHandler
    {
        private readonly Func<string?> _getTokenCallback;

        public InternalJwtHandler(Func<string?> getTokenCallback, HttpMessageHandler? innerHandler = null) 
            : base(innerHandler ?? new HttpClientHandler())
        {
            _getTokenCallback = getTokenCallback;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get the JWT token from the callback
            var token = _getTokenCallback();

            // Add the Authorization header if a token is available
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Call the next handler in the pipeline
            var response = await base.SendAsync(request, cancellationToken);

            // Example: Implement custom logic based on the response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Handle 401 Unauthorized - e.g., refresh token, redirect to login
                Console.WriteLine("Received 401 Unauthorized, you might need to refresh the token or redirect to login.");
                // Add your token refresh or logout logic here
            }

            return response;
        }
    }
}