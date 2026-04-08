namespace Fastdotnet.Core.Settings
{
    /// <summary>
    /// OpenIddict/OIDC 配置选项
    /// </summary>
    public class OidcSettings
    {
        /// <summary>
        /// 是否启用 OIDC Identity Provider 功能
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Issuer URI（颁发者地址），通常是基础 URL
        /// 例如：https://yourdomain.com
        /// </summary>
        public string IssuerUri { get; set; } = string.Empty;

        /// <summary>
        /// JWT 签名密钥（用于签发 ID Token 和 Access Token）
        /// 建议使用至少 256 位的密钥
        /// </summary>
        public string SigningKey { get; set; } = string.Empty;

        /// <summary>
        /// JWT 加密密钥（用于加密 Token，可选）
        /// </summary>
        public string EncryptionKey { get; set; } = string.Empty;

        /// <summary>
        /// Access Token 有效期（分钟）
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 60;

        /// <summary>
        /// Refresh Token 有效期（天）
        /// </summary>
        public int RefreshTokenLifetime { get; set; } = 30;

        /// <summary>
        /// ID Token 有效期（分钟）
        /// </summary>
        public int IdTokenLifetime { get; set; } = 60;

        /// <summary>
        /// 是否允许刷新令牌
        /// </summary>
        public bool AllowRefreshTokens { get; set; } = true;

        /// <summary>
        /// 是否要求 PKCE（Proof Key for Code Exchange）
        /// 建议生产环境启用
        /// </summary>
        public bool RequirePkce { get; set; } = true;

        /// <summary>
        /// 是否要求 HTTPS（生产环境必须启用）
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;
    }
}
