using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;



namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 加解密工具类 -
    /// </summary>
    public static class CryptographyUtils
    {
        /// <summary>
        /// 检查字符串是否为有效的Base64格式
        /// </summary>
        /// <param name="base64String">待检查的字符串</param>
        /// <returns>如果是有效的Base64格式返回true，否则返回false</returns>
        private static bool IsBase64String(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;
            
            // 检查字符串长度是否为4的倍数
            if (base64String.Length % 4 != 0)
                return false;
            
            try
            {
                // 尝试解码Base64字符串
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #region AES 加密算法

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（必须是16、24或32字节）</param>
        /// <param name="iv">初始化向量（可选，如果不提供则自动生成）</param>
        /// <returns>加密结果（Base64格式）</returns>
        public static string AESEncrypt(string plainText, string key, string iv = null)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));
                
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));
            
            using var aes = Aes.Create();
            aes.KeySize = 256; // 使用256位密钥
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            // 根据密钥长度调整KeySize
            if (key.Length == 16)
                aes.KeySize = 128;
            else if (key.Length == 24)
                aes.KeySize = 192;
            else if (key.Length != 32)
                throw new ArgumentException("AES密钥长度必须为16、24或32字节", nameof(key));
            
            aes.Key = Encoding.UTF8.GetBytes(key);
            
            if (!string.IsNullOrEmpty(iv))
            {
                if (iv.Length != 16)
                    throw new ArgumentException("IV长度必须为16字节", nameof(iv));
                aes.IV = Encoding.UTF8.GetBytes(iv);
            }
            else
            {
                // 如果没有提供IV，则生成随机IV
                aes.GenerateIV();
            }
            
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            
            // 将IV和加密数据一起返回，以确保解密时能正确使用IV
            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);
            
            return Convert.ToBase64String(result);
        }
        
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">密文（Base64格式）</param>
        /// <param name="key">密钥（必须与加密时使用的相同）</param>
        /// <returns>解密后的明文</returns>
        public static string AESDecrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));
                
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));
            
            var cipherBytes = Convert.FromBase64String(cipherText);
            
            if (cipherBytes.Length < 16)
                throw new ArgumentException("密文长度不足", nameof(cipherText));
            
            using var aes = Aes.Create();
            aes.KeySize = 256; // 默认使用256位密钥
            
            // 根据密钥长度调整KeySize
            if (key.Length == 16)
                aes.KeySize = 128;
            else if (key.Length == 24)
                aes.KeySize = 192;
            else if (key.Length != 32)
                throw new ArgumentException("AES密钥长度必须为16、24或32字节", nameof(key));
            
            // 从密文中提取IV（前16字节）
            var iv = new byte[16];
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, 16);
            aes.IV = iv;
            
            // 提取实际的加密数据
            var encryptedData = new byte[cipherBytes.Length - 16];
            Buffer.BlockCopy(cipherBytes, 16, encryptedData, 0, encryptedData.Length);
            
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        
        #endregion
        
        #region RSA 加密算法
        
        /// <summary>
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="keySize">密钥长度，默认2048位</param>
        /// <returns>包含公钥和私钥的元组</returns>
        public static (string publicKey, string privateKey) GenerateRSAKeyPair(int keySize = 2048)
        {

            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                // 导出公钥和私钥
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

                return (publicKey, privateKey);

            }
        }
        
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="publicKey">公钥（Base64或PEM格式）</param>
        /// <param name="padding">填充模式</param>
        /// <returns>加密后的数据（Base64格式）</returns>
        public static string RSAEncrypt(string plainText, string publicKey, RSAEncryptionPadding padding = null)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));
                
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("公钥不能为空", nameof(publicKey));
            
            padding = padding ?? RSAEncryptionPadding.OaepSHA256;
            
            using var rsa = RSA.Create();
            
            // 检查公钥格式，如果是Base64格式则转换为PEM格式
            if (IsBase64String(publicKey))
            {
                // Base64格式的公钥，需要转换成PEM格式
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            }
            else
            {
                // PEM格式的公钥
                rsa.ImportFromPem(publicKey);
            }
            
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainBytes, padding);
            
            return Convert.ToBase64String(encryptedBytes);
        }
        
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText">密文（Base64格式）</param>
        /// <param name="privateKey">私钥（Base64或PEM格式）</param>
        /// <param name="padding">填充模式</param>
        /// <returns>解密后的明文</returns>
        public static string RSADecrypt(string cipherText, string privateKey, RSAEncryptionPadding padding = null)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));
                
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("私钥不能为空", nameof(privateKey));
            
            padding = padding ?? RSAEncryptionPadding.OaepSHA256;
            
            using var rsa = RSA.Create();
            
            // 检查私钥格式，如果是Base64格式则转换为PEM格式
            if (IsBase64String(privateKey))
            {
                // Base64格式的私钥，需要转换成PEM格式
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            }
            else
            {
                // PEM格式的私钥
                rsa.ImportFromPem(privateKey);
            }
            
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = rsa.Decrypt(cipherBytes, padding);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data">待签名的数据</param>
        /// <param name="privateKey">私钥（Base64或PEM格式）</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        /// <returns>签名结果（Base64格式）</returns>
        public static string RSASign(string data, string privateKey, HashAlgorithmName hashAlgorithm = default)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("数据不能为空", nameof(data));
                
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("私钥不能为空", nameof(privateKey));
            
            hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;
            
            using var rsa = RSA.Create();
            
            // 检查私钥格式，如果是Base64格式则转换为PEM格式
            if (IsBase64String(privateKey))
            {
                // Base64格式的私钥，需要转换成PEM格式
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            }
            else
            {
                // PEM格式的私钥
                rsa.ImportFromPem(privateKey);
            }
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signature = rsa.SignData(dataBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
            
            return Convert.ToBase64String(signature);
        }
        
        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="signature">签名（Base64格式）</param>
        /// <param name="publicKey">公钥（Base64或PEM格式）</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        /// <returns>验证结果</returns>
        public static bool RSAVerify(string data, string signature, string publicKey, HashAlgorithmName hashAlgorithm = default)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("数据不能为空", nameof(data));
                
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentException("签名不能为空", nameof(signature));
                
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("公钥不能为空", nameof(publicKey));
            
            hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;
            
            using var rsa = RSA.Create();
            
            // 检查公钥格式，如果是Base64格式则转换为PEM格式
            if (IsBase64String(publicKey))
            {
                // Base64格式的公钥，需要转换成PEM格式
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            }
            else
            {
                // PEM格式的公钥
                rsa.ImportFromPem(publicKey);
            }
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);
            
            return rsa.VerifyData(dataBytes, signatureBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
        }
        
        #endregion
        
    }
}