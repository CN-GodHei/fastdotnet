using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Fastdotnet.Core.Settings;
using Microsoft.Extensions.Logging;
using Fastdotnet.Core.Initializers;

namespace Fastdotnet.Core.Service.Oidc
{
    /// <summary>
    /// OpenIddict 应用初始化器
    /// 用于注册默认的 OIDC 客户端（如 Elsa Workflows）
    /// </summary>
    public class OidcApplicationInitializer : IApplicationInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly OidcSettings _oidcSettings;
        private readonly ILogger<OidcApplicationInitializer> _logger;

        public OidcApplicationInitializer(
            IServiceProvider serviceProvider,
            IOptions<OidcSettings> oidcSettings,
            ILogger<OidcApplicationInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _oidcSettings = oidcSettings.Value;
            _logger = logger;
        }

        public int Order => 100; // 在数据库初始化之后执行

        public async Task InitializeAsync()
        {
            if (!_oidcSettings.Enabled)
            {
                _logger.LogInformation("OIDC 功能未启用，跳过初始化");
                return;
            }

            // 尝试获取 IOpenIddictApplicationManager（如果 OIDC 已正确配置）
            var applicationManager = _serviceProvider.GetService<IOpenIddictApplicationManager>();
            if (applicationManager == null)
            {
                _logger.LogWarning("IOpenIddictApplicationManager 未注册，跳过 OIDC 初始化");
                return;
            }

            _logger.LogInformation("开始初始化 OIDC 应用和客户端...");

            // 使用 SqlSugar Store，数据库表由 SqlSugar 自动创建
            _logger.LogInformation("OIDC 数据库表将由 SqlSugar 自动管理");

            // 注册 Elsa Workflows 作为 OIDC 客户端
            await RegisterElsaClientAsync(applicationManager);

            _logger.LogInformation("OIDC 应用初始化完成");
        }

        /// <summary>
        /// 注册 Elsa Workflows 作为 OIDC 客户端
        /// </summary>
        private async Task RegisterElsaClientAsync(IOpenIddictApplicationManager applicationManager)
        {
            const string clientId = "elsa-workflows";
            
            // 检查是否已存在
            if (await applicationManager.FindByClientIdAsync(clientId) != null)
            {
                _logger.LogInformation("Elsa OIDC 客户端已存在，跳过注册");
                return;
            }

            _logger.LogInformation("注册 Elsa OIDC 客户端...");

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = "elsa-secret-key-change-in-production", // 生产环境应使用强密码
                DisplayName = "Elsa Workflows",
                Permissions =
                {
                    // 允许的授权类型
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    // 允许的响应类型
                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    // 允许的作用域
                    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access",

                    // 允许的端点
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                },
                Requirements =
                {
                    // 要求使用 PKCE
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange,
                },
                RedirectUris =
                {
                    // Elsa Studio 的回调地址（通过反向代理访问）
                    new Uri("http://localhost:18889/fdelsa/signin-oidc"),
                    new Uri("https://localhost:18889/fdelsa/signin-oidc"),
                    // 如果是直接访问（非代理），添加直连地址
                    // new Uri("http://localhost:5000/signin-oidc"),
                    // new Uri("https://localhost:5001/signin-oidc"),
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:18889/fdelsa/signout-callback-oidc"),
                    new Uri("https://localhost:18889/fdelsa/signout-callback-oidc"),
                }
            };

            await applicationManager.CreateAsync(descriptor);
            _logger.LogInformation("Elsa OIDC 客户端注册成功");
        }
    }
}
