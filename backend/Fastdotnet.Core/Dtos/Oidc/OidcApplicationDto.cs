namespace Fastdotnet.Core.Dtos.Oidc;

/// <summary>
/// OIDC 应用创建请求
/// </summary>
public class CreateOidcApplicationRequest
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 客户端类型 (confidential/public)
    /// </summary>
    public string ClientType { get; set; } = "confidential";

    /// <summary>
    /// 重定向 URI 列表
    /// </summary>
    public List<string> RedirectUris { get; set; } = new();

    /// <summary>
    /// 登出后重定向 URI 列表
    /// </summary>
    public List<string> PostLogoutRedirectUris { get; set; } = new();

    /// <summary>
    /// 授权类型 (authorization_code, implicit, etc.)
    /// </summary>
    public List<string> GrantTypes { get; set; } = new() { "authorization_code" };

    /// <summary>
    /// 作用域 (openid, profile, email, etc.)
    /// </summary>
    public List<string> Scopes { get; set; } = new() { "openid", "profile" };
}

/// <summary>
/// OIDC 应用信息响应
/// </summary>
public class OidcApplicationResponse
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public List<string> RedirectUris { get; set; } = new();
    public List<string> PostLogoutRedirectUris { get; set; } = new();
    public DateTime CreationDate { get; set; }
}
