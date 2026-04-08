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
                    "openid",
                    "profile",
                    "email",
                    "roles",

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
                    // Elsa Studio 的回调地址（根据实际部署地址修改）
                    new Uri("http://localhost:5000/signin-oidc"),
                    new Uri("https://localhost:5001/signin-oidc"),
                    // 如果是前端分离架构，添加前端地址
                    // new Uri("http://localhost:3000/callback"),
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:5000/signout-callback-oidc"),
                    new Uri("https://localhost:5001/signout-callback-oidc"),
                }
            };

            await applicationManager.CreateAsync(descriptor);
            _logger.LogInformation("Elsa OIDC 客户端注册成功");
        }
    }
}
