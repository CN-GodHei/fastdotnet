using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Options
{

    /// <summary>
    /// 应用程序根配置
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// SqlSugar 数据库配置
        /// </summary>
        public SqlSugarOptions SqlSugar { get; set; } = new();

        /// <summary>
        /// JWT 认证配置
        /// </summary>
        public JwtSettings JwtSettings { get; set; } = new();

        /// <summary>
        /// OIDC/OAuth2 身份提供商配置
        /// </summary>
        public OidcSettings OidcSettings { get; set; } = new();

        /// <summary>
        /// 缓存配置
        /// </summary>
        public CacheSettings CacheSettings { get; set; } = new();

        /// <summary>
        /// 验证码配置
        /// </summary>
        public CaptchaOptions CaptchaOptions { get; set; } = new();

        /// <summary>
        /// 请求参数加密配置
        /// </summary>
        public RequestParamEncryption RequestParamEncryption { get; set; } = new();

        /// <summary>
        /// 插件配置
        /// </summary>
        public PluginSettings PluginSettings { get; set; } = new();

        /// <summary>
        /// 存储配置
        /// </summary>
        public StorageOptions Storage { get; set; } = new();

        /// <summary>
        /// Kestrel 服务器配置
        /// </summary>
        public KestrelOptions Kestrel { get; set; } = new();
    }



    /// <summary>
    /// SqlSugar 配置
    /// </summary>
    public class SqlSugarOptions
    {
        /// <summary>
        /// 是否启用 SQL 执行日志
        /// </summary>
        public bool EnableSqlExecutionLogging { get; set; }

        /// <summary>
        /// 数据库连接列表
        /// </summary>
        public List<ConnectionConfig> Connections { get; set; } = new();
    }

    /// <summary>
    /// JWT 配置
    /// </summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; }
    }

    /// <summary>
    /// OIDC/OpenIddict 配置
    /// </summary>
    public class OidcSettings
    {
        public bool Enabled { get; set; }
        public string IssuerUri { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
        public string EncryptionKey { get; set; } = string.Empty;
        public int AccessTokenLifetime { get; set; }
        public int RefreshTokenLifetime { get; set; }
        public int IdTokenLifetime { get; set; }
        public bool AllowRefreshTokens { get; set; }
        public bool RequirePkce { get; set; }
        public bool RequireHttpsMetadata { get; set; }
    }



    /// <summary>
    /// 缓存配置设置
    /// </summary>
    public class CacheSettings
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        public const string SectionName = "CacheSettings";

        /// <summary>
        /// 缓存类型：MemoryCache 或 Redis
        /// </summary>
        public string CacheType { get; set; } = "MemoryCache";

        /// <summary>
        /// Redis连接字符串（仅在使用Redis时需要）
        /// </summary>
        public string RedisConnectionString { get; set; } = "";

        /// <summary>
        /// Redis实例名称前缀
        /// </summary>
        public string RedisInstanceName { get; set; } = "Fastdotnet:";

        /// <summary>
        /// 本地缓存过期时间（分钟）
        /// </summary>
        public int LocalCacheExpirationMinutes { get; set; } = 5;

        /// <summary>
        /// 分布式缓存过期时间（分钟）
        /// </summary>
        public int DistributedCacheExpirationMinutes { get; set; } = 30;
    }
    /// <summary>
    /// 验证码配置
    /// </summary>
    public class CaptchaOptions
    {
        public int CaptchaType { get; set; }
        public int CodeLength { get; set; }
        public int ExpirySeconds { get; set; }
        public bool IgnoreCase { get; set; }
        public string StorageKeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 频率限制配置
        /// </summary>
        public CaptchaRateLimit RateLimit { get; set; } = new();

        /// <summary>
        /// 图片生成选项
        /// </summary>
        public CaptchaImageOption ImageOption { get; set; } = new();
    }

    public class CaptchaRateLimit
    {
        public bool Enabled { get; set; }
        public int WindowSeconds { get; set; }
        public int MaxRequests { get; set; }
    }

    public class CaptchaImageOption
    {
        public bool Animation { get; set; }
        public int FontSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BubbleMinRadius { get; set; }
        public int BubbleMaxRadius { get; set; }
        public int BubbleCount { get; set; }
        public double BubbleThickness { get; set; }
        public int InterferenceLineCount { get; set; }
        public string FontFamily { get; set; } = string.Empty;
        public int FrameDelay { get; set; }
        public string BackgroundColor { get; set; } = string.Empty;
        public string ForegroundColors { get; set; } = string.Empty;
        public int Quality { get; set; }
        public bool TextBold { get; set; }
    }

    /// <summary>
    /// 请求参数加密配置
    /// </summary>
    public class RequestParamEncryption
    {
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
    }

    /// <summary>
    /// 插件配置
    /// </summary>
    public class PluginSettings
    {
        public bool LoadDependenciesFromMemory { get; set; }
        public bool EnableDeveloperMode { get; set; }
    }

    /// <summary>
    /// 本地存储配置选项
    /// </summary>
    public class StorageOptions
    {
        /// <summary>
        /// 本地存储路径
        /// </summary>
        public string LocalStoragePath { get; set; } = "wwwroot/uploads";

        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; } = "/uploads";

        /// <summary>
        /// 默认存储桶
        /// </summary>
        public string DefaultBucket { get; set; } = "default";
    }

    /// <summary>
    /// Kestrel 服务器配置选项
    /// </summary>
    public class KestrelOptions
    {
        /// <summary>
        /// 端点配置
        /// </summary>
        public KestrelEndpointsOptions Endpoints { get; set; } = new();
    }

    /// <summary>
    /// Kestrel 端点配置
    /// </summary>
    public class KestrelEndpointsOptions
    {
        /// <summary>
        /// HTTP 端点配置
        /// </summary>
        public KestrelEndpointOptions Http { get; set; } = new();
    }

    /// <summary>
    /// Kestrel 端点选项
    /// </summary>
    public class KestrelEndpointOptions
    {
        /// <summary>
        /// 监听 URL
        /// </summary>
        public string Url { get; set; } = "http://*:18889";
    }
}
