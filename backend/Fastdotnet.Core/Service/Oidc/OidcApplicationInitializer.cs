using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

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

            // 注册 OIDC Scopes（必须在使用前注册）
            await RegisterScopesAsync();

            // 注册 Elsa Workflows 作为 OIDC 客户端
            await RegisterElsaClientAsync(applicationManager);

            _logger.LogInformation("OIDC 应用初始化完成");
        }

        /// <summary>
        /// 注册 OIDC Scopes（服务端必须预先注册）
        /// </summary>
        private async Task RegisterScopesAsync()
        {
            var scopeManager = _serviceProvider.GetService<IOpenIddictScopeManager>();
            if (scopeManager == null)
            {
                _logger.LogWarning("IOpenIddictScopeManager 未注册，跳过 Scope 注册");
                return;
            }

            var scopes = new[] { "fastdotnet-api" };
            foreach (var scopeName in scopes)
            {
                if (await scopeManager.FindByNameAsync(scopeName) == null)
                {
                    var descriptor = new OpenIddictScopeDescriptor
                    {
                        Name = scopeName,
                        DisplayName = scopeName,
                        Resources = { "fastdotnet-api" }
                    };

                    await scopeManager.CreateAsync(descriptor);
                    _logger.LogInformation("已注册 OIDC Scope: {ScopeName}", scopeName);
                }
            }
        }

        /// <summary>
        /// 注册 Elsa Workflows 作为 OIDC 客户端
        /// </summary>
        private async Task RegisterElsaClientAsync(IOpenIddictApplicationManager applicationManager)
        {
            const string clientId = "elsa-workflows";

            // 检查是否已存在
            var existingApp = await applicationManager.FindByClientIdAsync(clientId);
            if (existingApp != null)
            {
                _logger.LogInformation("Elsa OIDC 客户端已存在，更新配置...");

                // 获取现有的 descriptor 并更新 RedirectUris
                var updateDescriptor = new OpenIddictApplicationDescriptor();
                await applicationManager.PopulateAsync(updateDescriptor, existingApp);

                // 清除旧的 RedirectUris
                updateDescriptor.RedirectUris.Clear();
                updateDescriptor.PostLogoutRedirectUris.Clear();

                // 添加新的 RedirectUris
                updateDescriptor.RedirectUris.Add(new Uri("http://localhost:18889/fdelsa/signin-oidc"));
                updateDescriptor.RedirectUris.Add(new Uri("https://localhost:18889/fdelsa/signin-oidc"));
                updateDescriptor.RedirectUris.Add(new Uri("http://localhost:5000/authentication/login-callback"));
                updateDescriptor.RedirectUris.Add(new Uri("https://localhost:5000/authentication/login-callback"));
                updateDescriptor.RedirectUris.Add(new Uri("http://localhost:5001/authentication/login-callback"));
                updateDescriptor.RedirectUris.Add(new Uri("https://localhost:5001/authentication/login-callback"));
                // 插件路径的 redirect_uri（Elsa Studio Blazor WASM 作为插件部署）
                updateDescriptor.RedirectUris.Add(new Uri("http://localhost:18889/plugins/11365281228127623/publish/authentication/login-callback"));
                updateDescriptor.RedirectUris.Add(new Uri("https://localhost:18889/plugins/11365281228127623/publish/authentication/login-callback"));
                // 远程访问的 redirect_uri
                updateDescriptor.RedirectUris.Add(new Uri("http://100.118.249.35:18888/plugins/11365281228127623/publish/authentication/login-callback"));
                updateDescriptor.RedirectUris.Add(new Uri("https://100.118.249.35:18888/plugins/11365281228127623/publish/authentication/login-callback"));
                // 添加新的 PostLogoutRedirectUris
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("http://localhost:18889/fdelsa/signout-callback-oidc"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:18889/fdelsa/signout-callback-oidc"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("http://localhost:5000/"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:5000/"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("http://localhost:5001/"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:5001/"));
                // 插件路径的登出回调
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("http://localhost:18889/plugins/11365281228127623/publish/"));
                updateDescriptor.PostLogoutRedirectUris.Add(new Uri("https://localhost:18889/plugins/11365281228127623/publish/"));
                await applicationManager.UpdateAsync(existingApp, updateDescriptor);
                _logger.LogInformation("Elsa OIDC 客户端配置已更新");
                return;
            }

            _logger.LogInformation("注册 Elsa OIDC 客户端...");

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                //ClientSecret = "elsa-secret-key-change-in-production", // 生产环境应使用强密码
                // 关键点：设置为 public。不要设置为 confidential（机密型）。
                ClientType = ClientTypes.Public,

                // 建议设置为 explicit，这样用户登录后会弹出一个“是否允许该应用访问”的确认页面
                ConsentType = ConsentTypes.Explicit,
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
                    OpenIddictConstants.Permissions.Prefixes.Scope + "fastdotnet-api", // Elsa API 访问权限

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
                    // Elsa Studio 的回调地址（通过反向代理访问 - 后端 OIDC 模式）
                    new Uri("http://localhost:18889/fdelsa/signin-oidc"),
                    new Uri("https://localhost:18889/fdelsa/signin-oidc"),
                    
                    // Elsa Studio Blazor WASM 的回调地址（前端直接 OIDC 模式）
                    new Uri("http://localhost:5000/authentication/login-callback"),
                    new Uri("https://localhost:5000/authentication/login-callback"),
                    
                    // OIDC 测试项目的回调地址
                    new Uri("http://localhost:5001/authentication/login-callback"),
                    new Uri("https://localhost:5001/authentication/login-callback"),
                    
                    // 插件路径的 redirect_uri（Elsa Studio Blazor WASM 作为插件部署）
                    new Uri("http://localhost:18889/plugins/11365281228127623/publish/authentication/login-callback"),
                    new Uri("https://localhost:18889/plugins/11365281228127623/publish/authentication/login-callback"),
                    
                    // 远程访问的 redirect_uri
                    new Uri("http://100.118.249.35:18888/plugins/11365281228127623/publish/authentication/login-callback"),
                    new Uri("https://100.118.249.35:18888/plugins/11365281228127623/publish/authentication/login-callback"),
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:18889/fdelsa/signout-callback-oidc"),
                    new Uri("https://localhost:18889/fdelsa/signout-callback-oidc"),
                    
                    // Elsa Studio Blazor WASM 登出回调
                    new Uri("http://localhost:5000/"),
                    new Uri("https://localhost:5000/"),
                    new Uri("http://localhost:5001/"),
                    new Uri("https://localhost:5001/"),
                    
                    // 插件路径的登出回调（Elsa Studio Blazor WASM 作为插件部署）
                    new Uri("http://localhost:18889/plugins/11365281228127623/publish/"),
                    new Uri("https://localhost:18889/plugins/11365281228127623/publish/"),
                }
            };

            await applicationManager.CreateAsync(descriptor);
            _logger.LogInformation("Elsa OIDC 客户端注册成功");
        }
    }
}
