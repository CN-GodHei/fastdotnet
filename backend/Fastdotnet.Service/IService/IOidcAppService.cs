using Fastdotnet.Core.Dtos.Oidc;

namespace Fastdotnet.Service.IService;

/// <summary>
/// OIDC 应用管理服务接口
/// </summary>
public interface IOidcAppService
{
    /// <summary>
    /// 创建 OIDC 应用
    /// </summary>
    Task<OidcApplicationResponse> CreateApplicationAsync(CreateOidcApplicationRequest request);

    /// <summary>
    /// 获取所有 OIDC 应用列表
    /// </summary>
    Task<List<OidcApplicationResponse>> GetApplicationsAsync();

    /// <summary>
    /// 根据 ClientId 获取应用信息
    /// </summary>
    Task<OidcApplicationResponse?> GetApplicationByClientIdAsync(string clientId);

    /// <summary>
    /// 重置应用密钥
    /// </summary>
    Task<string> ResetClientSecretAsync(string clientId);

    /// <summary>
    /// 删除应用
    /// </summary>
    Task DeleteApplicationAsync(string clientId);
}
