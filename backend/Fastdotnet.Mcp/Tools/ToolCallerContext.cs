using Fastdotnet.Plugin.Contracts;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// 调用者身份与权限快照（每个 MCP 请求构造一次）。
/// </summary>
public sealed record ToolCallerContext(
    string? UserId,
    string? UserName,
    string UserType,
    bool IsSuperAdmin,
    string? TenantId,
    string? RequestId,
    IReadOnlySet<string> Permissions)
{
    /// <summary>
    /// 是否允许调用该工具：无权限要求 → 允许；超管 → 允许；否则按权限码精确匹配。
    /// </summary>
    public bool CanInvoke(PluginToolDescriptor descriptor)
        => descriptor.RequiredPermission is null
           || IsSuperAdmin
           || Permissions.Contains(descriptor.RequiredPermission);

    /// <summary>匿名调用者（未认证时使用，实际不会走到这里——端点已要求鉴权）</summary>
    public static ToolCallerContext Anonymous { get; } = new(
        null, null, "Admin", false, null, null,
        new HashSet<string>(StringComparer.OrdinalIgnoreCase));
}

/// <summary>已注册的工具：对外全限定名 → 插件内声明的映射</summary>
public sealed record RegisteredPluginTool(
    string QualifiedName,
    string PluginId,
    string PluginDisplayName,
    PluginToolDescriptor Descriptor);
