
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

        #region AES加密算法

        /// <summary>
        /// 生成 AES 密钥
        /// </summary>
        /// <param name="keySize">密钥长度（位），支持 128、192 或 256 位，默认 256 位</param>
        /// <returns>Base64 编码的密钥</returns>
        public static string GenerateAESKey(int keySize = 256)
        {
            using var aes = Aes.Create();
            aes.KeySize = keySize;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        /// <summary>
        /// 生成初始化向量 (IV)
        /// </summary>
        /// <returns>Base64 编码的 IV</returns>
        public static string GenerateAESIV()
        {
            using var aes = Aes.Create();
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（Base64 格式）</param>
        /// <param name="iv">初始化向量（Base64 格式，可选）</param>
        /// <param name="cipherMode">加密模式，默认 CBC</param>
        /// <param name="paddingMode">填充模式，默认 PKCS7</param>
        /// <returns>加密后的数据（Base64 格式）</returns>
        public static string AESEncrypt(string plainText, string key, string iv = null, 
            CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;

            // 如果提供了 IV 则使用，否则使用自动生成的 IV
            if (!string.IsNullOrEmpty(iv))
            {
                aes.IV = Convert.FromBase64String(iv);
            }

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // 将 IV 和密文一起返回（IV 在前 16 字节）
            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="cipherText">密文（Base64 格式，前 16 字节为 IV）</param>
        /// <param name="key">密钥（Base64 格式）</param>
        /// <param name="cipherMode">加密模式，默认 CBC</param>
        /// <param name="paddingMode">填充模式，默认 PKCS7</param>
        /// <returns>解密后的明文</returns>
        public static string AESDecrypt(string cipherText, string key,
            CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;

            var cipherBytes = Convert.FromBase64String(cipherText);

            // 提取 IV（前 16 字节）
            var iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // 提取实际的密文（去除 IV）
            var encryptedBytes = new byte[cipherBytes.Length - iv.Length];
            Buffer.BlockCopy(cipherBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// AES加密（使用 ECB 模式，不需要 IV）
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（Base64 格式）</param>
        /// <param name="paddingMode">填充模式，默认 PKCS7</param>
        /// <returns>加密后的数据（Base64 格式）</returns>
        public static string AESEncryptECB(string plainText, string key, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = paddingMode;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// AES 解密（使用 ECB 模式，不需要 IV）
        /// </summary>
        /// <param name="cipherText">密文（Base64 格式）</param>
        /// <param name="key">密钥（Base64 格式）</param>
        /// <param name="paddingMode">填充模式，默认 PKCS7</param>
        /// <returns>解密后的明文</returns>
        public static string AESDecryptECB(string cipherText, string key, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("密钥不能为空", nameof(key));

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = paddingMode;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        #endregion

        #region 密码哈希与验证

        /// <summary>
        /// 密码加密类型枚举
        /// </summary>
        public enum PasswordHashType
        {
            /// <summary>
            /// 不可逆加密（默认，推荐）- 使用PBKDF2算法
            /// </summary>
            Irreversible = 0,
            
            /// <summary>
            /// 可逆加密 - 使用AES加密
            /// </summary>
            Reversible = 1
        }

        /// <summary>
        /// 对密码进行哈希处理（不可逆加密）
        /// 使用PBKDF2算法，生成包含盐值和哈希值的字符串
        /// 格式："pbkdf2:iterations:salt:hash"
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="iterations">迭代次数，默认10000次</param>
        /// <returns>哈希后的密码字符串</returns>
        public static string HashPassword(string password, int iterations = 10000)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            // 生成随机盐值（16字节）
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // 使用PBKDF2生成哈希
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); // 32字节哈希值

                // 组合格式：pbkdf2:iterations:salt:hash
                return $"pbkdf2:{iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
            }
        }

        /// <summary>
        /// 验证密码是否匹配哈希值
        /// </summary>
        /// <param name="password">待验证的明文密码</param>
        /// <param name="hashedPassword">存储的哈希密码</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                // 解析哈希字符串
                var parts = hashedPassword.Split(':');
                if (parts.Length != 4 || parts[0] != "pbkdf2")
                    return false;

                int iterations = int.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] storedHash = Convert.FromBase64String(parts[3]);

                // 使用相同的参数重新计算哈希
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    byte[] computedHash = pbkdf2.GetBytes(32);

                    // 比较哈希值（恒定时间比较，防止时序攻击）
                    return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 加密密码（可逆加密）
        /// 使用AES加密，需要提供一个密钥
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="encryptionKey">加密密钥（Base64格式），如果为空则使用默认密钥</param>
        /// <returns>加密后的密码（Base64格式）</returns>
        public static string EncryptPassword(string password, string encryptionKey = null)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            // 如果没有提供密钥，使用一个默认密钥（生产环境应该从配置中读取）
            if (string.IsNullOrEmpty(encryptionKey))
            {
                // 警告：这是示例密钥，生产环境必须使用安全的密钥管理
                encryptionKey = GenerateAESKey();
            }

            // 使用AES加密
            return AESEncrypt(password, encryptionKey);
        }

        /// <summary>
        /// 解密密码（可逆解密）
        /// </summary>
        /// <param name="encryptedPassword">加密后的密码（Base64格式）</param>
        /// <param name="encryptionKey">解密密钥（Base64格式），必须与加密时使用的密钥相同</param>
        /// <returns>解密后的明文密码</returns>
        public static string DecryptPassword(string encryptedPassword, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                throw new ArgumentException("加密密码不能为空", nameof(encryptedPassword));

            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentException("解密密钥不能为空", nameof(encryptionKey));

            // 使用AES解密
            return AESDecrypt(encryptedPassword, encryptionKey);
        }

        /// <summary>
        /// 根据加密类型处理密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="hashType">加密类型</param>
        /// <param name="encryptionKey">可逆加密时的密钥（可选）</param>
        /// <returns>处理后的密码</returns>
        public static string ProcessPassword(string password, PasswordHashType hashType, string encryptionKey = null)
        {
            return hashType switch
            {
                PasswordHashType.Irreversible => HashPassword(password),
                PasswordHashType.Reversible => EncryptPassword(password, encryptionKey),
                _ => HashPassword(password) // 默认使用不可逆加密
            };
        }

        /// <summary>
        /// 验证密码（自动识别加密类型）
        /// </summary>
        /// <param name="password">待验证的明文密码</param>
        /// <param name="storedPassword">存储的密码（可能是哈希或加密后的）</param>
        /// <param name="hashType">加密类型</param>
        /// <param name="encryptionKey">可逆加密时的密钥（可选）</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyProcessedPassword(string password, string storedPassword, PasswordHashType hashType, string encryptionKey = null)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
                return false;

            switch (hashType)
            {
                case PasswordHashType.Irreversible:
                    return VerifyPassword(password, storedPassword);
                
                case PasswordHashType.Reversible:
                    try
                    {
                        var decryptedPassword = DecryptPassword(storedPassword, encryptionKey);
                        return password == decryptedPassword;
                    }
                    catch
                    {
                        return false;
                    }
                
                default:
                    return VerifyPassword(password, storedPassword);
            }
        }

        #endregion

        
    }
}