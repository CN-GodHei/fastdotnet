using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fastdotnet.Desktop.Api.Models;

namespace Fastdotnet.Desktop.Api.Handlers
{
    public class ApiResponseHandler : DelegatingHandler
    {
        public ApiResponseHandler(HttpMessageHandler? innerHandler = null) 
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // 如果状态码成功，但内容是统一响应格式，我们需要特殊处理
            if (response.IsSuccessStatusCode)
            {
                var originalContent = await response.Content.ReadAsStringAsync();
                
                try
                {
                    using var document = JsonDocument.Parse(originalContent);
                    if (document.RootElement.TryGetProperty("Data", out var dataElement))
                    {
                        // 这是统一响应格式，提取 Data 部分
                        var actualData = dataElement.GetRawText();
                        
                        // 如果是登录请求，且包含 Token
                        if (dataElement.TryGetProperty("Token", out var tokenElement))
                        {
                            // 保持原响应但修改内容处理逻辑
                            // 这里我们返回原始响应，但实际使用时需要特别处理
                        }
                    }
                }
                catch (JsonException)
                {
                    // 如果不是 JSON 格式或不是统一响应格式，继续正常处理
                }
            }

            return response;
        }
    }
}