using System.Net;
using Fastdotnet.WebApi.Tests.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Fastdotnet.WebApi.Tests;

/// <summary>
/// 中间件管道契约测试。
/// <para>
/// 背景：<c>EncryptionMiddleware</c> 曾在「端点存在但不是 MVC 控制器动作」时既不处理也不调用
/// <c>_next</c>，导致请求被静默中断——表现为 <b>HTTP 200 且响应体为空</b>。
/// 任何非 MVC 端点（最小 API、健康检查、MCP 端点）都会被它打断，而这类故障极难排查。
/// </para>
/// <para>
/// 本测试把「不得出现 200 + 空响应体」固化为契约：一旦有人再写出同类中间件，CI 立即变红。
/// </para>
/// </summary>
[TestClass]
public class MiddlewarePipelineContractTests
{
    [TestMethod]
    public async Task 所有GET端点都不得出现_200且响应体为空()
    {
        using var factory = new FastdotnetWebApplicationFactory();
        using var client = factory.CreateClient();

        var endpoints = factory.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(ds => ds.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();

        Assert.IsTrue(endpoints.Count > 0, "未发现任何已注册端点，说明路由未生效");

        var violations = new List<string>();
        var probed = 0;

        foreach (var endpoint in endpoints)
        {
            var methods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? [];
            if (!methods.Contains("GET", StringComparer.OrdinalIgnoreCase)) continue;

            var rawText = endpoint.RoutePattern.RawText;
            if (string.IsNullOrWhiteSpace(rawText)) continue;

            var url = BuildProbeUrl(rawText);
            if (url is null) continue;

            probed++;

            try
            {
                using var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK && string.IsNullOrEmpty(body))
                {
                    violations.Add($"{url} → 200 但响应体为空（疑似请求被中间件静默中断）");
                }
            }
            catch (Exception ex)
            {
                violations.Add($"{url} → 请求异常：{ex.GetType().Name}: {ex.Message}");
            }
        }

        Assert.IsTrue(probed > 0, "没有探测到任何 GET 端点，测试本身失效");

        Assert.AreEqual(
            0,
            violations.Count,
            "以下端点出现「200 + 空响应体」或请求异常，说明中间件管道存在静默中断：" +
            Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    /// <summary>把路由模板转成可请求的 URL；含无法构造的参数时返回 null（跳过）</summary>
    private static string? BuildProbeUrl(string rawText)
    {
        var url = "/" + rawText.TrimStart('/');

        // 路由参数替换为占位值；catch-all 与可选参数直接跳过，避免构造无意义的请求
        if (url.Contains('*')) return null;

        while (url.Contains('{'))
        {
            var start = url.IndexOf('{');
            var end = url.IndexOf('}', start);
            if (end < 0) return null;

            var token = url.Substring(start, end - start + 1);
            if (token.Contains('?')) return null; // 可选参数：跳过

            url = url.Replace(token, "probe");
        }

        return url;
    }
}
