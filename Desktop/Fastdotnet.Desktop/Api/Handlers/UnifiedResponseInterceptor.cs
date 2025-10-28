using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fastdotnet.Desktop.Api.Handlers
{
    public class UnifiedResponseInterceptor : DelegatingHandler
    {
        public event EventHandler<ApiSuccessEventArgs>? OnApiSuccess;
        public event EventHandler<ApiErrorEventArgs>? OnApiError;

        public UnifiedResponseInterceptor(HttpMessageHandler? innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                // 检查 HTTP 状态码
                if (!response.IsSuccessStatusCode)
                {
                    // HTTP 错误（4xx, 5xx 等）
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    
                    OnApiError?.Invoke(this, new ApiErrorEventArgs
                    {
                        RequestUrl = request.RequestUri?.ToString() ?? "",
                        HttpMethod = request.Method.ToString(),
                        ErrorCode = (int)response.StatusCode,
                        ErrorMessage = $"HTTP Error: {(int)response.StatusCode} - {response.ReasonPhrase}",
                        RawResponse = errorContent,
                        HttpStatusCode = response.StatusCode
                    });

                    return response;
                }

                // HTTP 成功，检查业务状态码
                var originalContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                if (!string.IsNullOrEmpty(originalContent))
                {
                    try
                    {
                        // 解析统一响应格式
                        using var jsonDocument = JsonDocument.Parse(originalContent);
                        var rootElement = jsonDocument.RootElement;

                        // 检查是否是统一响应格式（包含 Code 字段）
                        if (rootElement.TryGetProperty("Code", out var codeElement))
                        {
                            var code = codeElement.GetInt32();
                            
                            // 触发成功事件（即使 Code != 0，这也可能是业务层面的成功/失败）
                            OnApiSuccess?.Invoke(this, new ApiSuccessEventArgs
                            {
                                RequestUrl = request.RequestUri?.ToString() ?? "",
                                HttpMethod = request.Method.ToString(),
                                ResponseCode = code,
                                ResponseMessage = rootElement.TryGetProperty("Msg", out var msgElement) ? msgElement.GetString() : null,
                                RawResponse = originalContent,
                                HttpStatusCode = response.StatusCode
                            });

                            // 如果 Code != 0，这是一个业务错误
                            if (code != 0)
                            {
                                var errorMsg = rootElement.TryGetProperty("Msg", out msgElement) ? msgElement.GetString() : "Unknown business error";
                                
                                OnApiError?.Invoke(this, new ApiErrorEventArgs
                                {
                                    RequestUrl = request.RequestUri?.ToString() ?? "",
                                    HttpMethod = request.Method.ToString(),
                                    ErrorCode = code,
                                    ErrorMessage = errorMsg,
                                    RawResponse = originalContent,
                                    HttpStatusCode = response.StatusCode
                                });

                                // 根据需要决定是否修改响应状态码来反映业务错误
                                // 注意：这取决于您的具体需求
                                /*
                                if (response.IsSuccessStatusCode)
                                {
                                    // 创建一个新的响应，设置为错误状态
                                    response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                                    {
                                        Content = new StringContent(originalContent)
                                    };
                                }
                                */
                            }
                            else
                            {
                                // Code == 0，业务成功，如果需要可以修改响应内容只保留 Data 部分
                                if (rootElement.TryGetProperty("Data", out var dataElement))
                                {
                                    var dataJson = dataElement.GetRawText();
                                    
                                    // 创建新的响应，只包含 Data 部分
                                    var newResponse = new HttpResponseMessage(response.StatusCode)
                                    {
                                        Content = new StringContent(dataJson),
                                        StatusCode = response.StatusCode
                                    };
                                    
                                    // 复制重要的头部信息
                                    foreach (var header in response.Headers)
                                    {
                                        newResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
                                    }
                                    
                                    response = newResponse;
                                }
                            }
                        }
                        else
                        {
                            // 不是统一响应格式，直接触发成功事件
                            OnApiSuccess?.Invoke(this, new ApiSuccessEventArgs
                            {
                                RequestUrl = request.RequestUri?.ToString() ?? "",
                                HttpMethod = request.Method.ToString(),
                                ResponseCode = 0,
                                ResponseMessage = "Success",
                                RawResponse = originalContent,
                                HttpStatusCode = response.StatusCode
                            });
                        }
                    }
                    catch (JsonException)
                    {
                        // 如果不是有效的 JSON 格式，保持原样
                        // 这可能是一些非标准的响应
                        OnApiSuccess?.Invoke(this, new ApiSuccessEventArgs
                        {
                            RequestUrl = request.RequestUri?.ToString() ?? "",
                            HttpMethod = request.Method.ToString(),
                            ResponseCode = 0,
                            ResponseMessage = "Success",
                            RawResponse = originalContent,
                            HttpStatusCode = response.StatusCode
                        });
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                // 处理网络错误或其他异常
                OnApiError?.Invoke(this, new ApiErrorEventArgs
                {
                    RequestUrl = request.RequestUri?.ToString() ?? "",
                    HttpMethod = request.Method.ToString(),
                    ErrorCode = -1,
                    ErrorMessage = ex.Message,
                    Exception = ex,
                    HttpStatusCode = 0 // 表示网络错误
                });

                throw; // 重新抛出异常
            }
        }
    }

    public class ApiSuccessEventArgs : EventArgs
    {
        public string RequestUrl { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public int ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
        public string RawResponse { get; set; } = "";
        public HttpStatusCode HttpStatusCode { get; set; }
    }

    public class ApiErrorEventArgs : EventArgs
    {
        public string RequestUrl { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string RawResponse { get; set; } = "";
        public Exception? Exception { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}