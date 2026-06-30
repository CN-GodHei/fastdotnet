using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 混合加密工具类（RSA + AES）
    /// 结合 RSA 的密钥管理优势和 AES 的性能优势
    /// </summary>
    public static class HybridEncryptionUtils
    {
        /// <summary>
        /// 加密数据（RSA + AES 混合）
        /// </summary>
        /// <param name="plainText">明文数据</param>
        /// <param name="rsaPublicKey">RSA 公钥</param>
        /// <returns>加密后的 JSON 字符串，包含 encryptedData 和 encryptedKey</returns>
        public static string Encrypt(string plainText, string rsaPublicKey)
        {
            // 1. 生成随机的 AES 密钥和 IV（每次调用都不同）✅
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            // 2. 用 AES 加密数据
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedData;

            using (var encryptor = aes.CreateEncryptor())
            {
                encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            // 3. 用 RSA 加密 AES 密钥（包含 IV）
            // 将 Key 和 IV 组合在一起加密
            byte[] keyAndIV = new byte[aesKey.Length + aesIV.Length];
            Array.Copy(aesKey, 0, keyAndIV, 0, aesKey.Length);
            Array.Copy(aesIV, 0, keyAndIV, aesKey.Length, aesIV.Length);

            byte[] encryptedKey;
            using (var rsa = RSA.Create())
            {
                // 支持 PEM 或 Base64 格式的公钥
                string publicKeyBase64 = rsaPublicKey;
                if (rsaPublicKey.StartsWith("-----BEGIN"))
                {
                    // PEM 格式，提取 Base64 部分
                    var lines = rsaPublicKey.Split('\n');
                    var base64Lines = new System.Collections.Generic.List<string>();
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (!trimmed.StartsWith("-----") && !string.IsNullOrEmpty(trimmed))
                        {
                            base64Lines.Add(trimmed);
                        }
                    }
                    publicKeyBase64 = string.Join("", base64Lines);
                }
                
                byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
                rsa.ImportRSAPublicKey(publicKeyBytes, out _);
                encryptedKey = rsa.Encrypt(keyAndIV, RSAEncryptionPadding.Pkcs1);
            }

            // 4. 组合结果
            var result = new
            {
                encryptedData = Convert.ToBase64String(encryptedData),
                encryptedKey = Convert.ToBase64String(encryptedKey),
                algorithm = "AES-256-CBC+RSA"
            };

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 解密数据（RSA + AES 混合）
        /// </summary>
        /// <param name="encryptedJson">加密的 JSON 字符串</param>
        /// <param name="rsaPrivateKey">RSA 私钥</param>
        /// <returns>解密后的明文</returns>
        public static string Decrypt(string encryptedJson, string rsaPrivateKey)
        {
            // 1. 解析 JSON
            dynamic result = JsonConvert.DeserializeObject(encryptedJson)!;
            string encryptedDataBase64 = result.encryptedData;
            string encryptedKeyBase64 = result.encryptedKey;

            byte[] encryptedData = Convert.FromBase64String(encryptedDataBase64);
            byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);

            // 2. 用 RSA 解密 AES 密钥
            string keyAndIVString;
            try
            {
                using (var rsa = RSA.Create())
                {
                    // 支持 Base64 格式的私钥
                    byte[] privateKeyBytes = Convert.FromBase64String(rsaPrivateKey);
                    rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                    byte[] decryptedBytes = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);
                    keyAndIVString = Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (CryptographicException)
            {
                // 密钥不匹配，说明使用了错误的私钥（公钥已过期）
                throw;
            }

            // 3. 提取 AES Key 和 IV（支持两种格式）
            byte[] aesKey;
            byte[] aesIV;
            
            if (keyAndIVString.Contains("|"))
            {
                // 前端格式：Base64Key|Base64IV
                var parts = keyAndIVString.Split('|');
                if (parts.Length != 2)
                {
                    throw new CryptographicException("无效的密钥格式");
                }
                aesKey = Convert.FromBase64String(parts[0]);
                aesIV = Convert.FromBase64String(parts[1]);
            }
            else
            {
                // 后端格式：二进制拼接
                byte[] keyAndIV = Convert.FromBase64String(keyAndIVString);
                aesKey = new byte[32]; // AES-256
                aesIV = new byte[16];  // AES block size
                Array.Copy(keyAndIV, 0, aesKey, 0, aesKey.Length);
                Array.Copy(keyAndIV, aesKey.Length, aesIV, 0, aesIV.Length);
            }

            // 4. 用 AES 解密数据
            byte[] decryptedData;
            using (var aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                decryptedData = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }

            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// 生成 RSA 密钥对
        /// </summary>
        public static (string publicKey, string privateKey) GenerateRsaKeyPair()
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            string publicKey = rsa.ToXmlString(false);  // 只包含公钥
            string privateKey = rsa.ToXmlString(true);  // 包含公私钥
            return (publicKey, privateKey);
        }
    }
}
