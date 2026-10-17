using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Fastdotnet.WebApi.Tests.Infrastructure;

/// <summary>
/// 测试用 JWT 签发器：复用 appsettings.Development.json 中的 JwtSettings，签出与宿主同构的令牌。
/// <para>
/// 之所以自己签而不是走登录接口：登录接口带 <c>[EncryptRequest]</c>/<c>[EncryptResponse]</c>，
/// 走通它需要实现完整的 RSA 加解密，成本远高于收益，且与本测试目标（管道契约）无关。
/// </para>
/// </summary>
internal static class TestJwtFactory
{
    private const string SecretKey = "a-very-string-and-secure-secret-key-that-is-at-least-32-bytes-string";
    private const string Issuer = "Fastdotnet";
    private const string Audience = "Fastdotnet.Api";

    /// <summary>超级管理员令牌（同时给出短名与完整 URI 形式的声明，规避入站 Claim 映射差异）</summary>
    public static string CreateSuperAdminToken(
        string userId = "11438163155878918",
        string userName = "superadmin")
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Name, userName),
            new(ClaimTypes.Name, userName),
            new("category", "Admin"),
            new(ClaimTypes.Role, "SUPER_ADMIN"),
            new("role", "SUPER_ADMIN"),
        };

        return Create(claims);
    }

    /// <summary>普通应用端用户令牌（无超管角色）</summary>
    public static string CreateAppUserToken(
        string userId = "12055423131517957",
        string userName = "admintest")
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Name, userName),
            new(ClaimTypes.Name, userName),
            new("category", "App"),
        };

        return Create(claims);
    }

    private static string Create(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
