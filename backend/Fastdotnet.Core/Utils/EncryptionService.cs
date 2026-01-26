using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 加密服务类 - 提供统一的加密操作接口
    /// </summary>
    public class EncryptionService
    {
        /// <summary>
        /// 加密算法类型枚举
        /// </summary>
        public enum AlgorithmType
        {
            AES,
            RSA
        }

        /// <summary>
        /// 加密配置选项
        /// </summary>
        public class EncryptionOptions
        {
            public AlgorithmType Algorithm { get; set; }
            public string Key { get; set; }
            public string Iv { get; set; }
            public int RsaKeySize { get; set; } = 2048;
            public RSAEncryptionPadding RsaPadding { get; set; } = RSAEncryptionPadding.OaepSHA256;
            public CipherMode Mode { get; set; } = CipherMode.CBC;
            public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
        }

        /// <summary>
        /// 执行加密操作
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="options">加密选项</param>
        /// <returns>加密后的数据</returns>
        public string Encrypt(string plainText, EncryptionOptions options)
        {
            switch (options.Algorithm)
            {

                case AlgorithmType.AES:
                    if (string.IsNullOrEmpty(options.Key))
                        throw new ArgumentException("AES加密需要提供密钥", nameof(options));
                    return CryptographyUtils.AESEncrypt(plainText, options.Key, options.Iv);

                case AlgorithmType.RSA:
                    if (string.IsNullOrEmpty(options.Key))
                        throw new ArgumentException("RSA加密需要提供公钥", nameof(options));
                    return CryptographyUtils.RSAEncrypt(plainText, options.Key, options.RsaPadding);

                default:
                    throw new NotSupportedException($"不支持的加密算法: {options.Algorithm}");
            }
        }

        /// <summary>
        /// 执行解密操作
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="options">解密选项</param>
        /// <returns>解密后的明文</returns>
        public string Decrypt(string cipherText, EncryptionOptions options)
        {
            switch (options.Algorithm)
            {

                case AlgorithmType.AES:
                    if (string.IsNullOrEmpty(options.Key))
                        throw new ArgumentException("AES解密需要提供密钥", nameof(options));
                    return CryptographyUtils.AESDecrypt(cipherText, options.Key);

                case AlgorithmType.RSA:
                    if (string.IsNullOrEmpty(options.Key))
                        throw new ArgumentException("RSA解密需要提供私钥", nameof(options));
                    return CryptographyUtils.RSADecrypt(cipherText, options.Key, options.RsaPadding);

                default:
                    throw new NotSupportedException($"不支持的解密算法: {options.Algorithm}");
            }
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <param name="algorithm">哈希算法类型</param>
        /// <returns>哈希值</returns>
        //public string Hash(string input, AlgorithmType algorithm)
        //{
        //    switch (algorithm)
        //    {
        //        case AlgorithmType.SM3:
        //            return CryptographyUtils.SM3Hash(input);

        //        default:
        //            throw new NotSupportedException($"不支持的哈希算法: {algorithm}");
        //    }
        //}

        /// <summary>
        /// 生成指定算法的密钥对
        /// </summary>
        /// <param name="algorithm">算法类型</param>
        /// <param name="keySize">密钥大小（仅对RSA有效）</param>
        /// <returns>包含公钥和私钥的元组</returns>
        public (string publicKey, string privateKey) GenerateKeyPair(AlgorithmType algorithm, int keySize = 2048)
        {
            switch (algorithm)
            {
                case AlgorithmType.RSA:
                    return CryptographyUtils.GenerateRSAKeyPair(keySize);

                default:
                    throw new NotSupportedException($"不支持生成密钥对的算法: {algorithm}");
            }
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        /// <returns>签名结果</returns>
        public string Sign(string data, string privateKey, HashAlgorithmName hashAlgorithm = default)
        {
            return CryptographyUtils.RSASign(data, privateKey, hashAlgorithm);
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        /// <returns>验证结果</returns>
        public bool Verify(string data, string signature, string publicKey, HashAlgorithmName hashAlgorithm = default)
        {
            return CryptographyUtils.RSAVerify(data, signature, publicKey, hashAlgorithm);
        }
    }

    /// <summary>
    /// 加密服务扩展方法
    /// </summary>
    public static class EncryptionServiceExtensions
    {
        private static EncryptionService _defaultService;

        /// <summary>
        /// 获取默认加密服务实例
        /// </summary>
        /// <returns>加密服务实例</returns>
        public static EncryptionService GetEncryptionService()
        {
            if (_defaultService == null)
            {
                _defaultService = new EncryptionService();
            }
            return _defaultService;
        }

        

        /// <summary>
        /// 使用AES加密
        /// </summary>
        /// <param name="service">加密服务</param>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>加密结果</returns>
        public static string EncryptWithAES(this EncryptionService service, string plainText, string key, string iv = null)
        {
            var options = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.AES,
                Key = key,
                Iv = iv
            };
            return service.Encrypt(plainText, options);
        }

        /// <summary>
        /// 使用AES解密
        /// </summary>
        /// <param name="service">加密服务</param>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>解密结果</returns>
        public static string DecryptWithAES(this EncryptionService service, string cipherText, string key)
        {
            var options = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.AES,
                Key = key
            };
            return service.Decrypt(cipherText, options);
        }

        /// <summary>
        /// 使用RSA加密
        /// </summary>
        /// <param name="service">加密服务</param>
        /// <param name="plainText">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="padding">填充模式</param>
        /// <returns>加密结果</returns>
        public static string EncryptWithRSA(this EncryptionService service, string plainText, string publicKey, 
            RSAEncryptionPadding padding = null)
        {
            var options = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.RSA,
                Key = publicKey,
                RsaPadding = padding ?? RSAEncryptionPadding.OaepSHA256
            };
            return service.Encrypt(plainText, options);
        }

        /// <summary>
        /// 使用RSA解密
        /// </summary>
        /// <param name="service">加密服务</param>
        /// <param name="cipherText">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="padding">填充模式</param>
        /// <returns>解密结果</returns>
        public static string DecryptWithRSA(this EncryptionService service, string cipherText, string privateKey, 
            RSAEncryptionPadding padding = null)
        {
            var options = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.RSA,
                Key = privateKey,
                RsaPadding = padding ?? RSAEncryptionPadding.OaepSHA256
            };
            return service.Decrypt(cipherText, options);
        }

    }
}