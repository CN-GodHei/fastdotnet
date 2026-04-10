using System.Text.Json;
using Fastdotnet.Core.Dtos.Oidc;
using Fastdotnet.Core.Entities.Oidc;
using Fastdotnet.Service.IService;
using OpenIddict.Abstractions;
using SqlSugar;
using Permissions = OpenIddict.Abstractions.OpenIddictConstants.Permissions;

namespace Fastdotnet.Service.Service;

/// <summary>
/// OIDC 应用管理服务实现
/// </summary>
public class OidcAppService : IOidcAppService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ISqlSugarClient _db;

    public OidcAppService(IOpenIddictApplicationManager applicationManager, ISqlSugarClient db)
    {
        _applicationManager = applicationManager;
        _db = db;
    }

    public async Task<OidcApplicationResponse> CreateApplicationAsync(CreateOidcApplicationRequest request)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            // 生成唯一的 ClientId (类似微信的 AppID)
            ClientId = Guid.NewGuid().ToString("N"),
            DisplayName = request.DisplayName,
            ClientType = request.ClientType,
            ClientSecret = request.ClientType == "confidential" ? Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") : null,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Revocation,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code
            }
        };

        // 添加重定向 URI
        foreach (var uri in request.RedirectUris)
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        foreach (var uri in request.PostLogoutRedirectUris)
        {
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
        }

        // 添加权限和作用域
        foreach (var grantType in request.GrantTypes)
        {
            descriptor.Permissions.Add(Permissions.Prefixes.GrantType + grantType);
        }

        foreach (var scope in request.Scopes)
        {
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + scope);
        }

        await _applicationManager.CreateAsync(descriptor);

        // 重新获取以返回完整信息（包含生成的 ClientId）
        var app = await _applicationManager.FindByClientIdAsync(descriptor.ClientId!) as OpenIddictSqlSugarApplication;
        return MapToResponse(app!, descriptor.ClientSecret);
    }

    public async Task<List<OidcApplicationResponse>> GetApplicationsAsync()
    {
        var apps = await _db.Queryable<OpenIddictSqlSugarApplication>().ToListAsync();
        return apps.Select(a => MapToResponse(a, null)).ToList();
    }

    public async Task<OidcApplicationResponse?> GetApplicationByClientIdAsync(string clientId)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId) as OpenIddictSqlSugarApplication;
        if (app == null) return null;

        // 注意：出于安全考虑，查询时通常不返回 ClientSecret，除非是重置操作
        return MapToResponse(app, null);
    }

    public async Task<string> ResetClientSecretAsync(string clientId)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId) as OpenIddictSqlSugarApplication;
        if (app == null) throw new InvalidOperationException("Application not found.");

        // 公开应用（Public Client）不允许拥有密钥
        if (app.ClientType?.Equals("public", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("公开应用（Public Client）不支持设置客户端密钥。");
        }

        var newSecret = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        
        // 直接使用 Store 设置密钥，因为 Manager 的扩展方法可能未正确加载或签名不匹配
        var store = _applicationManager.GetType().GetProperty("Store")?.GetValue(_applicationManager) as IOpenIddictApplicationStore<OpenIddictSqlSugarApplication>;
        if (store != null)
        {
            await store.SetClientSecretAsync(app, newSecret, CancellationToken.None);
            await _applicationManager.UpdateAsync(app);
        }
        else
        {
            // 降级方案：直接修改实体并更新
            app.ClientSecret = newSecret;
            await _applicationManager.UpdateAsync(app);
        }
        
        return newSecret;
    }

    public async Task DeleteApplicationAsync(string clientId)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId);
        if (app != null)
        {
            await _applicationManager.DeleteAsync(app);
        }
    }

    private OidcApplicationResponse MapToResponse(OpenIddictSqlSugarApplication app, string? secret)
    {
        return new OidcApplicationResponse
        {
            Id = app.Id.ToString(),
            ClientId = app.ClientId ?? string.Empty,
            ClientSecret = secret, // 仅在创建或重置时返回
            DisplayName = app.DisplayName ?? string.Empty,
            ClientType = app.ClientType ?? string.Empty,
            RedirectUris = ParseJsonArray(app.RedirectUris),
            PostLogoutRedirectUris = ParseJsonArray(app.PostLogoutRedirectUris),
            CreationDate = DateTime.UtcNow
        };
    }

    private List<string> ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<string>();
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
