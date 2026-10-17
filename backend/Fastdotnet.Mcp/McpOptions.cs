namespace Fastdotnet.Mcp;

/// <summary>
/// MCP 暴露层配置（appsettings.json 的 "Mcp" 节）
/// </summary>
public sealed class McpOptions
{
    public const string SectionName = "Mcp";

    /// <summary>是否启用 MCP 服务端</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>MCP 端点路径（Streamable HTTP）</summary>
    public string EndpointPath { get; set; } = "/mcp";

    /// <summary>对 Agent 通告的服务名</summary>
    public string ServerName { get; set; } = "fastdotnet";

    public string ServerVersion { get; set; } = "1.0.0";

    /// <summary>给 Agent 的服务级说明（影响它如何使用这些能力）</summary>
    public string ServerInstructions { get; set; } =
        "Fastdotnet 插件能力网关。用 fd_capabilities_list 查看当前可用的能力，" +
        "用 fd_capability_invoke 或直接按工具名调用。工具集合会随插件装载/卸载动态变化。";

    /// <summary>单次工具调用默认超时（秒）</summary>
    public int DefaultTimeoutSeconds { get; set; } = 30;

    /// <summary>单次调用返回文本的大小上限（字节），超出则截断</summary>
    public int MaxResultBytes { get; set; } = 256 * 1024;

    /// <summary>tools/list 最多返回的工具数</summary>
    public int MaxToolsPerList { get; set; } = 200;
}
