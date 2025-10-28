using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Refit;
using System.IO;
using System.Reflection;

namespace Fastdotnet.Desktop.Api.Handlers
{
    public class UnifiedResponseContentSerializer : IHttpContentSerializer
    {
        private readonly JsonSerializerOptions _options;

        public UnifiedResponseContentSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
        {
            var jsonString = await content.ReadAsStringAsync(cancellationToken);
            
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (JsonException)
            {
                // 解析失败，返回默认值
                return default;
            }
        }

        public HttpContent ToHttpContent<T>(T item)
        {
            var jsonString = JsonSerializer.Serialize(item, _options);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return content;
        }

        public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
        {
            // 使用默认的字段名映射
            return propertyInfo?.Name;
        }
    }
}