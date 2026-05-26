using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 加密密钥管理服务
    /// 统一管理 RSA 密钥对的生成、缓存和获取
    /// </summary>
    public interface IEncryptionKeyService
    {
        /// <summary>
        /// 获取或生成 RSA 公钥（PEM 格式）
        /// </summary>
        Task<string> GetOrCreatePublicKeyAsync();

        /// <summary>
        /// 获取或生成 RSA 私钥
        /// </summary>
        Task<string> GetOrCreatePrivateKeyAsync();

        /// <summary>
        /// 强制刷新密钥对
        /// </summary>
        Task RefreshKeyPairAsync();
    }

    public class EncryptionKeyService : IEncryptionKeyService
    {
        private readonly HybridCache _hybridCache;
        private const string PublicKeyCacheKey = "Fastdotnet_Encryption_Rsa_PublicKey";
        private const string PrivateKeyCacheKey = "Fastdotnet_Encryption_Rsa_PrivateKey";
        private static readonly TimeSpan KeyExpiration = TimeSpan.FromDays(7);
        private static readonly SemaphoreSlim _keyGenerationLock = new SemaphoreSlim(1, 1);

        public EncryptionKeyService(HybridCache hybridCache)
        {
            _hybridCache = hybridCache;
        }

        /// <summary>
        /// 获取或生成 RSA 公钥（PEM 格式）
        /// </summary>
        public async Task<string> GetOrCreatePublicKeyAsync()
        {
            // 确保密钥对已生成
            await EnsureKeyPairExists();
            
            var options = new HybridCacheEntryOptions
            {
                Expiration = KeyExpiration,
                LocalCacheExpiration = KeyExpiration
            };

            return await _hybridCache.GetOrCreateAsync(
                PublicKeyCacheKey,
                factory: async (ct) =>
                {
                    // 理论上不会走到这里，因为 EnsureKeyPairExists 已经生成了
                    //Console.WriteLine("[EncryptionKeyService] WARNING: 缓存中没有公钥，重新生成...");
                    await GenerateAndCacheKeyPairAsync();
                    var result = await _hybridCache.GetOrCreateAsync<string>(PublicKeyCacheKey, ct => default);
                    return result ?? throw new InvalidOperationException("无法获取 RSA 公钥");
                }
            );
        }

        /// <summary>
        /// 获取或生成 RSA 私钥
        /// </summary>
        public async Task<string> GetOrCreatePrivateKeyAsync()
        {
            // 确保密钥对已生成
            await EnsureKeyPairExists();
            
            var options = new HybridCacheEntryOptions
            {
                Expiration = KeyExpiration,
                LocalCacheExpiration = KeyExpiration
            };

            return await _hybridCache.GetOrCreateAsync(
                PrivateKeyCacheKey,
                factory: async (ct) =>
                {
                    // 理论上不会走到这里，因为 EnsureKeyPairExists 已经生成了
                    //Console.WriteLine("[EncryptionKeyService] WARNING: 缓存中没有私钥，重新生成...");
                    await GenerateAndCacheKeyPairAsync();
                    var result = await _hybridCache.GetOrCreateAsync<string>(PrivateKeyCacheKey, ct => default);
                    return result ?? throw new InvalidOperationException("无法获取 RSA 私钥");
                }
            );
        }

        /// <summary>
        /// 确保密钥对存在
        /// </summary>
        private async Task EnsureKeyPairExists()
        {
            var existingPublicKey = await _hybridCache.GetOrCreateAsync<string>(
                PublicKeyCacheKey,
                ct => default
            );
            
            if (string.IsNullOrEmpty(existingPublicKey))
            {
                await GenerateAndCacheKeyPairAsync();
            }
        }

        /// <summary>
        /// 强制刷新密钥对
        /// </summary>
        public async Task RefreshKeyPairAsync()
        {
            // 先清除缓存中的旧密钥
            await _hybridCache.RemoveAsync(PublicKeyCacheKey);
            await _hybridCache.RemoveAsync(PrivateKeyCacheKey);
            
            // 强制生成新的密钥对
            await GenerateAndCacheKeyPairAsync();
        }

        /// <summary>
        /// 生成密钥对并缓存
        /// </summary>
        private async Task GenerateAndCacheKeyPairAsync()
        {
            // 使用锁确保只有一个线程能生成密钥对
            await _keyGenerationLock.WaitAsync();
            try
            {
                // 双重检查：获取锁后再检查一次缓存
                var existingPublicKey = await _hybridCache.GetOrCreateAsync<string>(
                    PublicKeyCacheKey,
                    _ => default
                );
                
                if (!string.IsNullOrEmpty(existingPublicKey))
                {
                    //Console.WriteLine("[EncryptionKeyService] 密钥对已存在，跳过生成");
                    return;
                }
                
                //Console.WriteLine("[EncryptionKeyService] 正在生成新的 RSA 密钥对...");
                
                // 生成密钥对
                var (pubKeyBase64, privKey) = CryptographyUtils.GenerateRSAKeyPair();
                
                // 将 Base64 格式的公钥转换为 PEM 格式（前端 JSEncrypt 需要）
                var publicKeyPem = ConvertBase64ToPemPublicKey(pubKeyBase64);
                
                // 存入缓存
                var options = new HybridCacheEntryOptions
                {
                    Expiration = KeyExpiration,
                    LocalCacheExpiration = KeyExpiration
                };
                
                await _hybridCache.SetAsync(PublicKeyCacheKey, publicKeyPem, options);
                await _hybridCache.SetAsync(PrivateKeyCacheKey, privKey, options);
                
                //Console.WriteLine("[EncryptionKeyService] RSA 密钥对已生成并缓存");
                //Console.WriteLine($"[EncryptionKeyService] 公钥: {publicKeyPem}");
                //Console.WriteLine($"[EncryptionKeyService] 私钥: {privKey}");
            }
            finally
            {
                _keyGenerationLock.Release();
            }
        }

        /// <summary>
        /// 将 Base64 格式的 RSA 公钥转换为 PEM 格式
        /// </summary>
        private string ConvertBase64ToPemPublicKey(string base64PublicKey)
        {
            try
            {
                // 解码 Base64
                byte[] keyBytes = Convert.FromBase64String(base64PublicKey);
                
                // 转换为 PEM 格式
                var pemBuilder = new StringBuilder();
                pemBuilder.AppendLine("-----BEGIN PUBLIC KEY-----");
                
                // Base64 编码并每 64 个字符换行
                string base64Pem = Convert.ToBase64String(keyBytes);
                for (int i = 0; i < base64Pem.Length; i += 64)
                {
                    int length = Math.Min(64, base64Pem.Length - i);
                    pemBuilder.AppendLine(base64Pem.Substring(i, length));
                }
                
                pemBuilder.AppendLine("-----END PUBLIC KEY-----");
                
                return pemBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EncryptionKeyService] 公钥格式转换失败: {ex.Message}");
                throw;
            }
        }
    }
}
