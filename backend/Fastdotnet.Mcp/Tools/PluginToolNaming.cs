using System.Text;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// MCP 工具命名规范。
/// <para>
/// MCP 规范要求工具名 1–128 字符、仅含 <c>A-Z a-z 0-9 _ - .</c>，且唯一性只在单个服务端内保证。
/// 因此对外名称统一加 <c>fd_</c> 前缀并做字符规整，避免与聚合网关上的其他 MCP 服务冲突。
/// </para>
/// </summary>
public static class PluginToolNaming
{
    /// <summary>对外前缀</summary>
    public const string Prefix = "fd_";

    /// <summary>全限定名总长上限（规范上限 128，留足余量）</summary>
    private const int MaxLength = 64;

    /// <summary>把任意字符串规整为 MCP 允许的字符集：小写字母、数字、下划线</summary>
    public static string Sanitize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "unnamed";

        var sb = new StringBuilder(raw.Length);
        var lastUnderscore = false;
        foreach (var ch in raw.Trim())
        {
            char c = char.ToLowerInvariant(ch);
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                sb.Append(c);
                lastUnderscore = false;
            }
            else if (!lastUnderscore)
            {
                sb.Append('_');
                lastUnderscore = true;
            }
        }

        var result = sb.ToString().Trim('_');
        return result.Length == 0 ? "unnamed" : result;
    }

    /// <summary>构造对外全限定工具名：fd_{pluginId}_{toolName}</summary>
    public static string Qualify(string pluginId, string toolName)
    {
        var name = Prefix + Sanitize(pluginId) + "_" + Sanitize(toolName);
        return name.Length <= MaxLength ? name : name[..MaxLength];
    }
}
