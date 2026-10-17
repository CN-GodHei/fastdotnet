using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Fastdotnet.WebApi.Tests.Infrastructure;

namespace Fastdotnet.WebApi.Tests;

/// <summary>
/// MCP 端点契约测试。
/// <para>
/// 覆盖两类曾经真实发生过的事故：
/// ① 非 MVC 端点被 <c>EncryptionMiddleware</c> 静默中断（返回 200 + 空响应体）；
/// ② MCP 客户端不带防重放签名头，在 Release（防重放开启）下被 409 拦截。
/// </para>
/// </summary>
[TestClass]
public class McpEndpointTests
{
    private const string McpPath = "/mcp";

    private static string ToolsListPayload =>
        """{"jsonrpc":"2.0","id":1,"method":"tools/list"}""";

    [TestMethod]
    public async Task 未认证请求MCP端点应被拒绝且不得是_200空响应()
    {
        using var factory = new FastdotnetWebApplicationFactory();
        using var client = factory.CreateClient();

        using var response = await PostMcpAsync(client, ToolsListPayload, token: null);
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreNotEqual(HttpStatusCode.OK, response.StatusCode,
            "未认证的 MCP 请求不应返回 200");
        Assert.IsFalse(
            response.StatusCode == HttpStatusCode.OK && string.IsNullOrEmpty(body),
            "出现「200 + 空响应体」，说明请求被中间件静默中断");
    }

    [TestMethod]
    public async Task 未知路径应返回404而不是被静默吞掉()
    {
        using var factory = new FastdotnetWebApplicationFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/this-path-does-not-exist-12345");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task 已认证的tools_list应返回工具清单()
    {
        using var factory = new FastdotnetWebApplicationFactory();
        using var client = factory.CreateClient();
        var token = TestJwtFactory.CreateSuperAdminToken();

        using var response = await PostMcpAsync(client, ToolsListPayload, token);
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"响应体：{body}");
        Assert.IsFalse(string.IsNullOrEmpty(body), "MCP 响应体为空——端点被静默中断");

        Assert.IsTrue(body.Contains("fd_capabilities_list"),
            $"工具清单中缺少稳定通道工具 fd_capabilities_list。响应体：{body}");
        Assert.IsTrue(body.Contains("fd_capability_invoke"),
            $"工具清单中缺少稳定通道工具 fd_capability_invoke。响应体：{body}");
    }

    [TestMethod]
    public async Task 开启防重放后MCP端点仍应可用()
    {
        // 这是「本地关闭、线上开启」那条分叉的回归防线：
        // MCP 客户端不会发送 X-Timestamp / X-Nonce / X-Signature，必须被显式放行。
        using var factory = new FastdotnetWebApplicationFactory(antiReplayEnabled: true);
        using var client = factory.CreateClient();
        var token = TestJwtFactory.CreateSuperAdminToken();

        using var response = await PostMcpAsync(client, ToolsListPayload, token);
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreNotEqual(HttpStatusCode.Conflict, response.StatusCode,
            "防重放中间件拦截了 MCP 请求（409），说明 /mcp 放行规则失效");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"响应体：{body}");
        Assert.IsTrue(body.Contains("fd_capabilities_list"), $"响应体：{body}");
    }

    [TestMethod]
    public async Task 开启防重放后普通接口缺少签名头应被拦截()
    {
        // 反向验证：开关本身必须真的生效，否则「放行 /mcp」就失去了意义
        using var factory = new FastdotnetWebApplicationFactory(antiReplayEnabled: true);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/Plugin/active");

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode,
            "防重放开启后，缺少签名头的普通接口应返回 409");
    }

    private static async Task<HttpResponseMessage> PostMcpAsync(
        HttpClient client,
        string json,
        string? token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, McpPath)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // MCP Streamable HTTP 要求同时接受 JSON 与 SSE
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await client.SendAsync(request);
    }
}
