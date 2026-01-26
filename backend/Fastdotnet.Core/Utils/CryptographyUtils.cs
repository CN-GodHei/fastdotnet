using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static System.Security.Cryptography.ECCurve;
using BigInteger = Org.BouncyCastle.Math.BigInteger;
using ECCurve = Org.BouncyCastle.Math.EC.ECCurve;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;


namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 加解密工具类 - 集成国密SM2及其他常见加密算法
    /// </summary>
    public static class CryptographyUtils
    {
        #region SM2 密钥对生成

        /// <summary>
        /// 生成SM2密钥对
        /// </summary>
        /// <returns>包含公钥和私钥的元组</returns>
        public static (string privateKey, string publicKey) GenerateSm2KeyPair()
        {
            try
            {
                // 使用SM2参数
                X9ECParameters sm2EcParameters = GMNamedCurves.GetByName("sm2p256v1");
                ECDomainParameters domainParameters = new ECDomainParameters(
                    sm2EcParameters.Curve,
                    sm2EcParameters.G,
                    sm2EcParameters.N,
                    sm2EcParameters.H);

                // 生成密钥对
                ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
                keyPairGenerator.Init(new ECKeyGenerationParameters(domainParameters, new SecureRandom()));
                AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();

                // 获取私钥
                ECPrivateKeyParameters privateKeyParams = (ECPrivateKeyParameters)keyPair.Private;
                // 获取公钥
                ECPublicKeyParameters publicKeyParams = (ECPublicKeyParameters)keyPair.Public;

                // 转换为十六进制字符串（与前端sm-crypto格式兼容）
                string privateKeyHex = privateKeyParams.D.ToString(16).PadLeft(64, '0');
                string publicKeyHex = EncodePublicKeyToHex(publicKeyParams.Q);

                return (publicKeyHex,privateKeyHex);
            }
            catch (Exception ex)
            {
                throw new Exception("生成SM2密钥对失败", ex);
            }
        }

        /// <summary>
        /// 从私钥生成公钥
        /// </summary>
        public static string GetPublicKeyFromPrivateKey(string privateKeyHex)
        {
            try
            {
                BigInteger d = new BigInteger(privateKeyHex, 16);
                X9ECParameters sm2EcParameters = GMNamedCurves.GetByName("sm2p256v1");
                ECPoint q = sm2EcParameters.G.Multiply(d);
                return EncodePublicKeyToHex(q);
            }
            catch (Exception ex)
            {
                throw new Exception("从私钥生成公钥失败", ex);
            }
        }

        #endregion

        #region SM2 加密

        /// <summary>
        /// SM2加密（与前端sm-crypto兼容）
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="publicKeyHex">公钥（十六进制）</param>
        /// <param name="cipherMode">加密模式：C1C3C2 或 C1C2C3（默认C1C3C2，与sm-crypto兼容）</param>
        /// <returns>Base64编码的密文</returns>
        public static string Sm2Encrypt(string plainText, string publicKeyHex, string cipherMode = "C1C3C2")
        {
            try
            {
                byte[] plainData = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = DoSm2Encrypt(plainData, publicKeyHex, cipherMode);
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception("SM2加密失败", ex);
            }
        }

        /// <summary>
        /// SM2加密（返回十六进制）
        /// </summary>
        public static string Sm2EncryptToHex(string plainText, string publicKeyHex, string cipherMode = "C1C3C2")
        {
            try
            {
                byte[] plainData = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = DoSm2Encrypt(plainData, publicKeyHex, cipherMode);
                return ByteArrayToHexString(encryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception("SM2加密失败", ex);
            }
        }

        private static byte[] DoSm2Encrypt(byte[] plainData, string publicKeyHex, string cipherMode)
        {
            X9ECParameters sm2EcParameters = GMNamedCurves.GetByName("sm2p256v1");
            ECDomainParameters domainParameters = new ECDomainParameters(
                sm2EcParameters.Curve,
                sm2EcParameters.G,
                sm2EcParameters.N,
                sm2EcParameters.H);

            // 解码公钥
            ECPoint publicKeyPoint = DecodePublicKeyFromHex(publicKeyHex, sm2EcParameters.Curve);
            ECPublicKeyParameters publicKey = new ECPublicKeyParameters(publicKeyPoint, domainParameters);

            // 创建SM2加密引擎
            SM2Engine sm2Engine = new SM2Engine(new SM3Digest());
            sm2Engine.Init(true, new ParametersWithRandom(publicKey, new SecureRandom()));

            // 加密数据
            byte[] encrypted = sm2Engine.ProcessBlock(plainData, 0, plainData.Length);

            // 根据模式重组数据
            return ReformatCipherData(encrypted, cipherMode);
        }

        #endregion

        #region SM2 解密

        /// <summary>
        /// SM2解密
        /// </summary>
        /// <param name="cipherTextBase64">Base64编码的密文</param>
        /// <param name="privateKeyHex">私钥（十六进制）</param>
        /// <param name="cipherMode">加密模式：C1C3C2 或 C1C2C3（默认C1C3C2）</param>
        /// <returns>解密后的明文</returns>
        public static string Sm2Decrypt(string cipherTextBase64, string privateKeyHex, string cipherMode = "C1C3C2")
        {
            try
            {
                byte[] cipherData = Convert.FromBase64String(cipherTextBase64);
                byte[] decryptedData = DoSm2Decrypt(cipherData, privateKeyHex, cipherMode);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception("SM2解密失败", ex);
            }
        }

        /// <summary>
        /// SM2解密（十六进制输入）
        /// </summary>
        public static string Sm2DecryptFromHex(string cipherTextHex, string privateKeyHex, string cipherMode = "C1C3C2")
        {
            try
            {
                byte[] cipherData = HexStringToByteArray(cipherTextHex);
                byte[] decryptedData = DoSm2Decrypt(cipherData, privateKeyHex, cipherMode);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception("SM2解密失败", ex);
            }
        }

        private static byte[] DoSm2Decrypt(byte[] cipherData, string privateKeyHex, string cipherMode)
        {
            X9ECParameters sm2EcParameters = GMNamedCurves.GetByName("sm2p256v1");
            ECDomainParameters domainParameters = new ECDomainParameters(
                sm2EcParameters.Curve,
                sm2EcParameters.G,
                sm2EcParameters.N,
                sm2EcParameters.H);

            // 解码私钥
            BigInteger d = new BigInteger(privateKeyHex, 16);
            ECPrivateKeyParameters privateKey = new ECPrivateKeyParameters(d, domainParameters);

            // 根据模式重新组织密文数据
            byte[] reformattedData = ReformatCipherData(cipherData, cipherMode);

            // 创建SM2解密引擎
            SM2Engine sm2Engine = new SM2Engine(new SM3Digest());
            sm2Engine.Init(false, privateKey);

            // 解密数据
            return sm2Engine.ProcessBlock(reformattedData, 0, reformattedData.Length);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 编码公钥为十六进制字符串（04 + X + Y）
        /// </summary>
        private static string EncodePublicKeyToHex(ECPoint publicKeyPoint)
        {
            byte[] x = publicKeyPoint.XCoord.GetEncoded();
            byte[] y = publicKeyPoint.YCoord.GetEncoded();

            // 确保长度为32字节（256位）
            x = PadTo32Bytes(x);
            y = PadTo32Bytes(y);

            return "04" + ByteArrayToHexString(x) + ByteArrayToHexString(y);
        }

        /// <summary>
        /// 从十六进制字符串解码公钥
        /// </summary>
        private static ECPoint DecodePublicKeyFromHex(string publicKeyHex, ECCurve curve)
        {
            if (publicKeyHex.StartsWith("04"))
            {
                publicKeyHex = publicKeyHex.Substring(2);
            }

            if (publicKeyHex.Length != 128) // 64字节 = 128个十六进制字符
            {
                throw new ArgumentException("无效的公钥长度");
            }

            string xHex = publicKeyHex.Substring(0, 64);
            string yHex = publicKeyHex.Substring(64, 64);

            BigInteger x = new BigInteger(xHex, 16);
            BigInteger y = new BigInteger(yHex, 16);

            return curve.CreatePoint(x, y);
        }

        /// <summary>
        /// 填充到32字节
        /// </summary>
        private static byte[] PadTo32Bytes(byte[] data)
        {
            if (data.Length == 32) return data;

            byte[] padded = new byte[32];
            if (data.Length > 32)
            {
                Array.Copy(data, data.Length - 32, padded, 0, 32);
            }
            else
            {
                Array.Copy(data, 0, padded, 32 - data.Length, data.Length);
            }
            return padded;
        }

        /// <summary>
        /// 重新组织密文数据格式
        /// </summary>
        private static byte[] ReformatCipherData(byte[] cipherData, string cipherMode)
        {
            // C1: 65字节（04 + X + Y）
            // C2: 明文长度
            // C3: 32字节（SM3哈希）

            if (cipherData.Length < 97) // 至少65 + 1 + 32
            {
                return cipherData;
            }

            byte[] c1 = new byte[65]; // 04 + 32字节X + 32字节Y
            byte[] c3 = new byte[32]; // SM3哈希
            byte[] c2 = new byte[cipherData.Length - 97]; // 剩余部分

            if (cipherMode == "C1C3C2")
            {
                // 默认格式：C1C3C2
                // BouncyCastle输出已经是这种格式
                return cipherData;
            }
            else if (cipherMode == "C1C2C3")
            {
                // 需要转换为C1C2C3格式
                Array.Copy(cipherData, 0, c1, 0, 65); // C1
                Array.Copy(cipherData, 65 + 32, c2, 0, c2.Length); // C2
                Array.Copy(cipherData, 65, c3, 0, 32); // C3

                // 组合为C1C2C3
                byte[] result = new byte[cipherData.Length];
                Array.Copy(c1, 0, result, 0, 65);
                Array.Copy(c2, 0, result, 65, c2.Length);
                Array.Copy(c3, 0, result, 65 + c2.Length, 32);

                return result;
            }
            else
            {
                throw new ArgumentException("不支持的加密模式，仅支持C1C3C2或C1C2C3");
            }
        }

        /// <summary>
        /// 验证SM2密钥对
        /// </summary>
        public static bool VerifyKeyPair(string privateKeyHex, string publicKeyHex)
        {
            try
            {
                string generatedPublicKey = GetPublicKeyFromPrivateKey(privateKeyHex);
                return generatedPublicKey.Equals(publicKeyHex, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        private static string ByteArrayToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        private static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return Array.Empty<byte>();

            if (hex.Length % 2 != 0)
                throw new ArgumentException("十六进制字符串长度必须是偶数");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }
            return bytes;
        }

        #endregion

        #region 与前端兼容的便捷方法

        /// <summary>
        /// 生成与sm-crypto兼容的密钥对
        /// </summary>
        public static dynamic GenerateSm2KeyPairForFrontend()
        {
            var keys = GenerateSm2KeyPair();
            return new
            {
                privateKey = keys.privateKey,
                publicKey = keys.publicKey,
                message = "使用私钥进行解密，使用公钥进行加密"
            };
        }

        /// <summary>
        /// 解密前端使用sm-crypto加密的数据
        /// </summary>
        public static string DecryptFromFrontend(string cipherTextBase64, string privateKeyHex)
        {
            // sm-crypto默认使用C1C3C2模式
            return Sm2Decrypt(cipherTextBase64, privateKeyHex, "C1C3C2");
        }

        /// <summary>
        /// 加密数据供前端sm-crypto解密
        /// </summary>
        public static string EncryptForFrontend(string plainText, string publicKeyHex)
        {
            // sm-crypto默认使用C1C3C2模式
            return Sm2Encrypt(plainText, publicKeyHex, "C1C3C2");
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

    
    #endregion
}