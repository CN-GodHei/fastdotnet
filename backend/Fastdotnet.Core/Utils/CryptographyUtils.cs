using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;


namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 加解密工具类 - 集成国密SM2及其他常见加密算法
    /// </summary>
    public static class CryptographyUtils
    {
        #region SM2 国密算法
        
        /// <summary>
        /// SM2加密参数
        /// </summary>
        private static readonly ECDomainParameters sm2DomainParams;
        
        /// <summary>
        /// SM2曲线参数
        /// </summary>
        private static readonly FpCurve sm2Curve;
        
        /// <summary>
        /// SM2基点
        /// </summary>
        private static readonly Org.BouncyCastle.Math.EC.ECPoint sm2G;
        
        /// <summary>
        /// SM2公钥长度
        /// </summary>
        private const int Sm2PublicKeyLength = 64;
        
        /// <summary>
        /// 静态构造函数初始化SM2参数
        /// </summary>
        static CryptographyUtils()
        {
            // 初始化SM2椭圆曲线参数 (y^2 = x^3 + ax + b)
            // 使用推荐的素数域上的椭圆曲线参数
            var p = new BigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF", 16);
            var a = new BigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC", 16);
            var b = new BigInteger("28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93", 16);
            var n = new BigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123", 16);
            var gx = new BigInteger("32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7", 16);
            var gy = new BigInteger("BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0", 16);
            
            sm2Curve = new FpCurve(p, a, b);
            sm2G = sm2Curve.CreatePoint(gx, gy);
            sm2DomainParams = new ECDomainParameters(sm2Curve, sm2G, n);
        }
        
        /// <summary>
        /// 生成SM2密钥对
        /// </summary>
        /// <returns>包含公钥和私钥的元组</returns>
        public static (string publicKey, string privateKey) GenerateSM2KeyPair()
        {
            var generator = new ECKeyPairGenerator();
            var secureRandom = new SecureRandom();
            // 使用我们定义的SM2域参数
            var keyGenParam = new ECKeyGenerationParameters(sm2DomainParams, secureRandom);
            generator.Init(keyGenParam);
            
            var keyPair = generator.GenerateKeyPair();
            var privateKeyParams = (ECPrivateKeyParameters)keyPair.Private;
            var publicKeyParams = (ECPublicKeyParameters)keyPair.Public;
            
            var privateKeyHex = privateKeyParams.D.ToString(16).PadLeft(64, '0');
            var publicKeyHex = Hex.Encode(ECKeyConvertor.ConvertPublicKeyToBytes(publicKeyParams));
            
            return (publicKeyHex, privateKeyHex);
        }
        
        /// <summary>
        /// SM2加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="publicKey">公钥（十六进制格式）</param>
        /// <returns>加密后的数据（十六进制格式）</returns>
        public static string SM2Encrypt(string plainText, string publicKey)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));
                
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("公钥不能为空", nameof(publicKey));
            
            try
            {
                var publicKeyBytes = Hex.Decode(publicKey);
                
                // 使用Bouncy Castle内置的API来解析公钥点
                var ecPoint = sm2Curve.DecodePoint(publicKeyBytes);
                
                var publicKeyParams = new ECPublicKeyParameters(ecPoint, sm2DomainParams);
                
                var engine = new Org.BouncyCastle.Crypto.Engines.SM2Engine();
                var cipherText = Encoding.UTF8.GetBytes(plainText);
                
                engine.Init(true, new ParametersWithRandom(publicKeyParams, new SecureRandom()));
                var encryptedBytes = engine.ProcessBlock(cipherText, 0, cipherText.Length);
                
                return Hex.Encode(encryptedBytes);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"SM2加密失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// SM2解密
        /// </summary>
        /// <param name="cipherText">密文（十六进制格式）</param>
        /// <param name="privateKey">私钥（十六进制格式）</param>
        /// <returns>解密后的明文</returns>
        public static string SM2Decrypt(string cipherText, string privateKey)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));
                
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("私钥不能为空", nameof(privateKey));
            
            try
            {
                var privateKeyBigInt = new BigInteger(privateKey, 16);
                var privateKeyParams = new ECPrivateKeyParameters(privateKeyBigInt, sm2DomainParams);
                
                var engine = new Org.BouncyCastle.Crypto.Engines.SM2Engine();
                var cipherTextBytes = Hex.Decode(cipherText);
                
                engine.Init(false, privateKeyParams);
                var decryptedBytes = engine.ProcessBlock(cipherTextBytes, 0, cipherTextBytes.Length);
                
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"SM2解密失败: {ex.Message}", ex);
            }
        }
        
        #endregion
        
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
            using var rsa = RSA.Create(keySize);
            return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem());
        }
        
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="publicKey">公钥（PEM格式）</param>
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
            rsa.ImportFromPem(publicKey);
            
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainBytes, padding);
            
            return Convert.ToBase64String(encryptedBytes);
        }
        
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText">密文（Base64格式）</param>
        /// <param name="privateKey">私钥（PEM格式）</param>
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
            rsa.ImportFromPem(privateKey);
            
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = rsa.Decrypt(cipherBytes, padding);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data">待签名的数据</param>
        /// <param name="privateKey">私钥（PEM格式）</param>
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
            rsa.ImportFromPem(privateKey);
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signature = rsa.SignData(dataBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
            
            return Convert.ToBase64String(signature);
        }
        
        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="signature">签名（Base64格式）</param>
        /// <param name="publicKey">公钥（PEM格式）</param>
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
            rsa.ImportFromPem(publicKey);
            
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);
            
            return rsa.VerifyData(dataBytes, signatureBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
        }
        
        #endregion
        
        #region SM3 哈希算法
        
        /// <summary>
        /// 计算SM3哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>哈希值（十六进制格式）</returns>
        public static string SM3Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("输入不能为空", nameof(input));
            
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var digest = new SM3Digest();
            digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
            
            var hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);
            
            return Hex.Encode(hash);
        }
        
        #endregion
        
        #region SM4 加密算法
        
        /// <summary>
        /// SM4加密（基于BouncyCastle实现）
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（16字节）</param>
        /// <param name="mode">加密模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns>加密结果（Base64格式）</returns>
        public static string SM4Encrypt(string plainText, string key, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("明文不能为空", nameof(plainText));
                
            if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetBytes(key).Length != 16)
                throw new ArgumentException("SM4密钥必须为16字节", nameof(key));
            
            try
            {
                // 使用BouncyCastle实现SM4算法
                var engine = new Org.BouncyCastle.Crypto.Engines.SM4Engine();
                
                // 准备密钥
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var keyParam = new KeyParameter(keyBytes);
                
                // 根据模式设置参数
                ICipherParameters parameters;
                byte[] ivBytes = null;
                
                if (mode == CipherMode.CBC)
                {
                    ivBytes = new byte[16];
                    RandomNumberGenerator.Fill(ivBytes); // 生成随机IV
                    parameters = new ParametersWithIV(keyParam, ivBytes);
                }
                else
                {
                    parameters = keyParam;
                }
                
                // 初始化加密器
                engine.Init(true, parameters); // true表示加密模式
                
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                
                // 按16字节块处理数据
                var blockSize = engine.GetBlockSize();
                var output = new byte[plainBytes.Length + blockSize]; // 预留空间用于填充
                var totalLen = 0;
                
                for (int i = 0; i < plainBytes.Length; i += blockSize)
                {
                    var block = new byte[blockSize];
                    var len = Math.Min(blockSize, plainBytes.Length - i);
                    Array.Copy(plainBytes, i, block, 0, len);
                    
                    // 如果不是完整块，需要进行填充
                    if (len < blockSize)
                    {
                        if (padding == PaddingMode.PKCS7)
                        {
                            var padLen = (byte)(blockSize - len);
                            for (int j = len; j < blockSize; j++)
                                block[j] = padLen;
                        }
                        else
                        {
                            // 对于非完整块，用0填充
                            for (int j = len; j < blockSize; j++)
                                block[j] = 0;
                        }
                    }
                    
                    var processedLen = engine.ProcessBlock(block, 0, output, totalLen);
                    totalLen += processedLen;
                }
                
                // 构建最终输出（如果使用CBC模式，需要包含IV）
                if (mode == CipherMode.CBC)
                {
                    var finalOutput = new byte[ivBytes.Length + totalLen];
                    Buffer.BlockCopy(ivBytes, 0, finalOutput, 0, ivBytes.Length);
                    Buffer.BlockCopy(output, 0, finalOutput, ivBytes.Length, totalLen);
                    return Convert.ToBase64String(finalOutput);
                }
                else
                {
                    var finalOutput = new byte[totalLen];
                    Array.Copy(output, 0, finalOutput, 0, totalLen);
                    return Convert.ToBase64String(finalOutput);
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"SM4加密失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// SM4解密（基于BouncyCastle实现）
        /// </summary>
        /// <param name="cipherText">密文（Base64格式）</param>
        /// <param name="key">密钥（16字节）</param>
        /// <param name="mode">加密模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns>解密后的明文</returns>
        public static string SM4Decrypt(string cipherText, string key, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("密文不能为空", nameof(cipherText));
                
            if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetBytes(key).Length != 16)
                throw new ArgumentException("SM4密钥必须为16字节", nameof(key));
            
            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);
                
                // 使用BouncyCastle实现SM4算法
                var engine = new Org.BouncyCastle.Crypto.Engines.SM4Engine();
                
                // 准备密钥
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var keyParam = new KeyParameter(keyBytes);
                
                // 处理IV（如果是CBC模式）
                ICipherParameters parameters;
                int offset = 0;
                
                if (mode == CipherMode.CBC)
                {
                    if (cipherBytes.Length < 16)
                        throw new ArgumentException("密文长度不足，无法提取IV");
                    
                    var ivBytes = new byte[16];
                    Buffer.BlockCopy(cipherBytes, 0, ivBytes, 0, 16);
                    offset = 16;
                    parameters = new ParametersWithIV(keyParam, ivBytes);
                }
                else
                {
                    parameters = keyParam;
                }
                
                // 初始化解密器
                engine.Init(false, parameters); // false表示解密模式
                
                // 获取密文数据部分
                var cipherData = new byte[cipherBytes.Length - offset];
                Buffer.BlockCopy(cipherBytes, offset, cipherData, 0, cipherData.Length);
                
                var blockSize = engine.GetBlockSize();
                var output = new byte[cipherData.Length];
                var totalLen = 0;
                
                for (int i = 0; i < cipherData.Length; i += blockSize)
                {
                    var block = new byte[blockSize];
                    var len = Math.Min(blockSize, cipherData.Length - i);
                    Array.Copy(cipherData, i, block, 0, len);
                    
                    var processedLen = engine.ProcessBlock(block, 0, output, totalLen);
                    totalLen += processedLen;
                }
                
                // 如果使用PKCS7填充，需要移除填充
                if (padding == PaddingMode.PKCS7 && totalLen > 0)
                {
                    var padLen = output[totalLen - 1];
                    if (padLen <= blockSize && padLen > 0)
                    {
                        // 验证填充
                        bool isValidPad = true;
                        for (int i = totalLen - padLen; i < totalLen; i++)
                        {
                            if (output[i] != padLen)
                            {
                                isValidPad = false;
                                break;
                            }
                        }
                        
                        if (isValidPad)
                        {
                            totalLen -= padLen;
                        }
                    }
                }
                
                return Encoding.UTF8.GetString(output, 0, totalLen);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"SM4解密失败: {ex.Message}", ex);
            }
        }
        
        #endregion
    }
    
    #region 辅助类
    
    /// <summary>
    /// 十六进制编码/解码辅助类
    /// </summary>
    internal static class Hex
    {
        private static readonly char[] HexChars = "0123456789ABCDEF".ToCharArray();
        
        public static string Encode(byte[] bytes)
        {
            if (bytes == null) return null;
            
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                result[i * 2] = HexChars[(bytes[i] & 0xF0) >> 4];
                result[i * 2 + 1] = HexChars[bytes[i] & 0x0F];
            }
            return new string(result);
        }
        
        public static byte[] Decode(string hex)
        {
            if (hex == null) return null;
            
            var result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                result[i / 2] = (byte)((HexToByte(hex[i]) << 4) | HexToByte(hex[i + 1]));
            }
            return result;
        }
        
        private static byte HexToByte(char c)
        {
            if (c >= '0' && c <= '9') return (byte)(c - '0');
            if (c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
            if (c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
            throw new ArgumentException($"Invalid hex character: {c}");
        }
    }
    
    /// <summary>
    /// EC密钥转换辅助类
    /// </summary>
    internal static class ECKeyConvertor
    {
        public static byte[] ConvertPublicKeyToBytes(ECPublicKeyParameters publicKeyParams)
        {
            var q = publicKeyParams.Q.Normalize();
            var x = q.AffineXCoord.GetEncoded();
            var y = q.AffineYCoord.GetEncoded();
            
            var result = new byte[1 + x.Length + y.Length]; // 04标识符 + x坐标 + y坐标
            result[0] = 0x04; // 未压缩格式标识符
            Buffer.BlockCopy(x, 0, result, 1, x.Length);
            Buffer.BlockCopy(y, 0, result, 1 + x.Length, y.Length);
            
            return result;
        }
    }
    
    #endregion
}