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
            
            try
            {
                // 检查公钥格式，如果是Base64格式则转换
                if (IsBase64String(publicKey))
                {
                    // 尝试导入Base64格式的公钥
                    byte[] keyBytes = Convert.FromBase64String(publicKey);
                    rsa.ImportRSAPublicKey(keyBytes, out _);
                }
                else
                {
                    // 尝试导入PEM格式的公钥
                    rsa.ImportFromPem(publicKey);
                }
            }
            catch (CryptographicException)
            {
                // 如果上面的方法失败，尝试另一种Base64导入方式
                if (IsBase64String(publicKey))
                {
                    // 尝试不同的导入方法
                    using var tempRsa = RSA.Create();
                    byte[] keyBytes = Convert.FromBase64String(publicKey);
                    tempRsa.ImportRSAPublicKey(keyBytes, out _);
                    
                    // 将临时RSA实例的参数复制到主RSA实例
                    var parameters = tempRsa.ExportParameters(false);
                    rsa.ImportParameters(parameters);
                }
                else
                {
                    throw; // 重新抛出原始异常
                }
            }
            
            // 使用分段加密处理可能较大的数据
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            
            // 计算每段的最大大小
            int maxLength = rsa.KeySize / 8 - 2 * SHA256.Create().HashSize / 8 - 2; // OAEP填充的计算公式
            
            if (plainBytes.Length <= maxLength)
            {
                // 数据较小，直接加密
                var encryptedBytes = rsa.Encrypt(plainBytes, padding);
                return Convert.ToBase64String(encryptedBytes);
            }
            else
            {
                // 数据较大，需要分段加密
                var encryptedBlocks = new List<string>();
                
                for (int i = 0; i < plainBytes.Length; i += maxLength)
                {
                    int currentBlockSize = Math.Min(maxLength, plainBytes.Length - i);
                    byte[] currentBlock = new byte[currentBlockSize];
                    Array.Copy(plainBytes, i, currentBlock, 0, currentBlockSize);
                    
                    byte[] encryptedBlock = rsa.Encrypt(currentBlock, padding);
                    
                    // 将每个加密块转换为Base64并添加到列表
                    encryptedBlocks.Add(Convert.ToBase64String(encryptedBlock));
                }
                
                // 使用特殊分隔符连接所有加密块
                return string.Join("|SPLIT|", encryptedBlocks);
            }
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
            
            try
            {
                // 检查私钥格式，如果是Base64格式则转换
                if (IsBase64String(privateKey))
                {
                    // 尝试导入Base64格式的私钥
                    byte[] keyBytes = Convert.FromBase64String(privateKey);
                    rsa.ImportRSAPrivateKey(keyBytes, out _);
                }
                else
                {
                    // 尝试导入PEM格式的私钥
                    rsa.ImportFromPem(privateKey);
                }
            }
            catch (CryptographicException ex)
            {
                // 如果上面的方法失败，先检查是否是由于传入了错误的参数
                // 如果privateKey看起来像是加密后的数据而非私钥，则抛出更明确的错误
                if (IsBase64String(privateKey) && privateKey.Length > 512) // 典型的加密数据会比较长
                {
                    throw new ArgumentException("传入的私钥参数可能不是有效的私钥，而是加密后的数据。请确认传入了正确的私钥。", nameof(privateKey), ex);
                }
                
                // 如果上面的方法失败，尝试另一种Base64导入方式
                if (IsBase64String(privateKey))
                {
                    try
                    {
                        // 尝试不同的导入方法
                        using var tempRsa = RSA.Create();
                        byte[] keyBytes = Convert.FromBase64String(privateKey);
                        tempRsa.ImportRSAPrivateKey(keyBytes, out _);
                        
                        // 将临时RSA实例的参数复制到主RSA实例
                        var parameters = tempRsa.ExportParameters(true);
                        rsa.ImportParameters(parameters);
                    }
                    catch (CryptographicException)
                    {
                        // 如果所有方法都失败，抛出原始异常
                        throw ex;
                    }
                }
                else
                {
                    throw; // 重新抛出原始异常
                }
            }
            
            // 检查是否为分段加密的数据
            // 分段加密的数据使用特殊分隔符 "|SPLIT|" 连接
            bool isSegmentedEncrypted = cipherText.Contains("|SPLIT|");
            
            // 如果不是分段加密，直接转换为字节数组
            byte[] cipherBytes = !isSegmentedEncrypted ? Convert.FromBase64String(cipherText) : null;
            
            if (isSegmentedEncrypted)
            {
                // 分段解密
                try
                {
                    // 使用分隔符 "|SPLIT|" 分割密文
                    string[] encryptedBlocks = cipherText.Split(new string[] { "|SPLIT|" }, StringSplitOptions.None);
                    
                    var decryptedBlocks = new List<byte[]>();
                    
                    foreach (string encryptedBlockBase64 in encryptedBlocks)
                    {
                        if (!string.IsNullOrEmpty(encryptedBlockBase64))
                        {
                            // 将Base64字符串转换为字节数组
                            byte[] encryptedBlock = Convert.FromBase64String(encryptedBlockBase64);
                            
                            // 解密当前块
                            byte[] decryptedBlock = rsa.Decrypt(encryptedBlock, padding);
                            decryptedBlocks.Add(decryptedBlock);
                        }
                    }
                    
                    // 合并所有解密块
                    var result = new List<byte>();
                    foreach (var block in decryptedBlocks)
                    {
                        result.AddRange(block);
                    }
                    
                    return Encoding.UTF8.GetString(result.ToArray());
                }
                catch (Exception)
                {
                    // 如果分段解密失败，尝试单次解密
                    // 例如，如果密文看起来像包含分隔符但实际上不是分段加密
                }
            }
            
            // 单次解密
            var decryptedBytes = rsa.Decrypt(Convert.FromBase64String(cipherText), padding);
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
            
            try
            {
                // 检查私钥格式，如果是Base64格式则转换
                if (IsBase64String(privateKey))
                {
                    // 尝试导入Base64格式的私钥
                    byte[] keyBytes = Convert.FromBase64String(privateKey);
                    rsa.ImportRSAPrivateKey(keyBytes, out _);
                }
                else
                {
                    // 尝试导入PEM格式的私钥
                    rsa.ImportFromPem(privateKey);
                }
            }
            catch (CryptographicException)
            {
                // 如果上面的方法失败，尝试另一种Base64导入方式
                if (IsBase64String(privateKey))
                {
                    // 尝试不同的导入方法
                    using var tempRsa = RSA.Create();
                    byte[] keyBytes = Convert.FromBase64String(privateKey);
                    tempRsa.ImportRSAPrivateKey(keyBytes, out _);
                    
                    // 将临时RSA实例的参数复制到主RSA实例
                    var parameters = tempRsa.ExportParameters(true);
                    rsa.ImportParameters(parameters);
                }
                else
                {
                    throw; // 重新抛出原始异常
                }
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
            
            try
            {
                // 检查公钥格式，如果是Base64格式则转换
                if (IsBase64String(publicKey))
                {
                    // 尝试导入Base64格式的公钥
                    byte[] keyBytes = Convert.FromBase64String(publicKey);
                    rsa.ImportRSAPublicKey(keyBytes, out _);
                }
                else
                {
                    // 尝试导入PEM格式的公钥
                    rsa.ImportFromPem(publicKey);
                }
            }
            catch (CryptographicException)
            {
                // 如果上面的方法失败，尝试另一种Base64导入方式
                if (IsBase64String(publicKey))
                {
                    // 尝试不同的导入方法
                    using var tempRsa = RSA.Create();
                    byte[] keyBytes = Convert.FromBase64String(publicKey);
                    tempRsa.ImportRSAPublicKey(keyBytes, out _);
                    
                    // 将临时RSA实例的参数复制到主RSA实例
                    var parameters = tempRsa.ExportParameters(false);
                    rsa.ImportParameters(parameters);
                }
                else
                {
                    throw; // 重新抛出原始异常
                }
            }
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);
            
            return rsa.VerifyData(dataBytes, signatureBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
        }
        
        #endregion
        
    }
}