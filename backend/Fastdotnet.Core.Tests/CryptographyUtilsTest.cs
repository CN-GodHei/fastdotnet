using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests
{
    [TestClass]
    public class CryptographyUtilsTest
    {
        [TestMethod]
        public void Test_SM2_Encryption_Decryption()
        {
            // 生成SM2密钥对
            var (publicKey, privateKey) = CryptographyUtils.GenerateSM2KeyPair();
            
            string originalText = "Hello, SM2!";
            
            // 加密
            string encrypted = CryptographyUtils.SM2Encrypt(originalText, publicKey);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.SM2Decrypt(encrypted, privateKey);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_SM2_KeyGeneration()
        {
            var (publicKey, privateKey) = CryptographyUtils.GenerateSM2KeyPair();
            
            Assert.IsFalse(string.IsNullOrEmpty(publicKey));
            Assert.IsFalse(string.IsNullOrEmpty(privateKey));
            Assert.IsTrue(publicKey.Length > 0);
            Assert.IsTrue(privateKey.Length > 0);
        }

        [TestMethod]
        public void Test_AES_Encryption_Decryption()
        {
            string originalText = "Hello, AES!";
            string key = "1234567890123456"; // 16字节密钥
            
            // 加密
            string encrypted = CryptographyUtils.AESEncrypt(originalText, key);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.AESDecrypt(encrypted, key);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_AES_With_IV()
        {
            string originalText = "Hello, AES with IV!";
            string key = "1234567890123456"; // 16字节密钥
            string iv = "1234567890123456"; // 16字节IV
            
            // 加密
            string encrypted = CryptographyUtils.AESEncrypt(originalText, key, iv);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.AESDecrypt(encrypted, key);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_RSA_Encryption_Decryption()
        {
            string originalText = "Hello, RSA!";
            
            // 生成RSA密钥对
            var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();
            
            // 加密
            string encrypted = CryptographyUtils.RSAEncrypt(originalText, publicKey);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.RSADecrypt(encrypted, privateKey);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_RSA_Sign_Verify()
        {
            string data = "Data to sign";
            
            // 生成RSA密钥对
            var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();
            
            // 签名
            string signature = CryptographyUtils.RSASign(data, privateKey);
            Assert.IsNotNull(signature);
            
            // 验签
            bool isValid = CryptographyUtils.RSAVerify(data, signature, publicKey);
            Assert.IsTrue(isValid);
            
            // 验证错误数据无法通过验证
            bool isInvalid = CryptographyUtils.RSAVerify("Different data", signature, publicKey);
            Assert.IsFalse(isInvalid);
        }

        [TestMethod]
        public void Test_SM3_Hash()
        {
            string input = "Hello, SM3!";
            string hash1 = CryptographyUtils.SM3Hash(input);
            string hash2 = CryptographyUtils.SM3Hash(input);
            
            Assert.IsNotNull(hash1);
            Assert.AreEqual(hash1, hash2); // 相同输入应产生相同哈希
            
            string differentInput = "Hello, SM3!!";
            string hash3 = CryptographyUtils.SM3Hash(differentInput);
            
            Assert.AreNotEqual(hash1, hash3); // 不同输入应产生不同哈希
        }

        [TestMethod]
        public void Test_SM4_Encryption_Decryption()
        {
            string originalText = "Hello, SM4!";
            string key = "1234567890123456"; // 16字节密钥
            
            // 加密
            string encrypted = CryptographyUtils.SM4Encrypt(originalText, key);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.SM4Decrypt(encrypted, key);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_SM4_Encryption_Decryption_CBC_Mode()
        {
            string originalText = "Hello, SM4 CBC Mode!";
            string key = "1234567890123456"; // 16字节密钥
            
            // 加密
            string encrypted = CryptographyUtils.SM4Encrypt(originalText, key, System.Security.Cryptography.CipherMode.CBC);
            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(originalText, encrypted);
            
            // 解密
            string decrypted = CryptographyUtils.SM4Decrypt(encrypted, key, System.Security.Cryptography.CipherMode.CBC);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_EncryptionService_SM2()
        {
            var service = new EncryptionService();
            var (publicKey, privateKey) = service.GenerateKeyPair(EncryptionService.AlgorithmType.SM2);
            
            string originalText = "Hello from EncryptionService!";
            
            // 加密
            var encryptOptions = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.SM2,
                Key = publicKey
            };
            string encrypted = service.Encrypt(originalText, encryptOptions);
            Assert.IsNotNull(encrypted);
            
            // 解密
            var decryptOptions = new EncryptionService.EncryptionOptions
            {
                Algorithm = EncryptionService.AlgorithmType.SM2,
                Key = privateKey
            };
            string decrypted = service.Decrypt(encrypted, decryptOptions);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_EncryptionService_Extension_Methods()
        {
            var service = EncryptionServiceExtensions.GetEncryptionService();
            var (publicKey, privateKey) = service.GenerateKeyPair(EncryptionService.AlgorithmType.SM2);
            
            string originalText = "Testing extension methods!";
            
            // 使用扩展方法加密
            string encrypted = service.EncryptWithSM2(originalText, publicKey);
            Assert.IsNotNull(encrypted);
            
            // 使用扩展方法解密
            string decrypted = service.DecryptWithSM2(encrypted, privateKey);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        public void Test_EncryptionService_SM3_Hash()
        {
            var service = EncryptionServiceExtensions.GetEncryptionService();
            string input = "Test SM3 Hash with Service";
            string hash = service.HashWithSM3(input);
            
            Assert.IsNotNull(hash);
            Assert.IsTrue(hash.Length > 0);
            
            // 验证相同的输入产生相同的哈希
            string hash2 = service.HashWithSM3(input);
            Assert.AreEqual(hash, hash2);
        }

        [TestMethod]
        public void Test_EncryptionService_SM4()
        {
            var service = EncryptionServiceExtensions.GetEncryptionService();
            string originalText = "Testing SM4 with Service";
            string key = "1234567890123456";
            
            // 使用扩展方法加密
            string encrypted = service.EncryptWithSM4(originalText, key);
            Assert.IsNotNull(encrypted);
            
            // 使用扩展方法解密
            string decrypted = service.DecryptWithSM4(encrypted, key);
            Assert.AreEqual(originalText, decrypted);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_SM2_Empty_PlainText_Throws_Exception()
        {
            var (publicKey, _) = CryptographyUtils.GenerateSM2KeyPair();
            CryptographyUtils.SM2Encrypt("", publicKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_SM2_Empty_PublicKey_Throws_Exception()
        {
            CryptographyUtils.SM2Encrypt("test", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_SM2_Empty_CipherText_Throws_Exception()
        {
            var (_, privateKey) = CryptographyUtils.GenerateSM2KeyPair();
            CryptographyUtils.SM2Decrypt("", privateKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_SM2_Empty_PrivateKey_Throws_Exception()
        {
            CryptographyUtils.SM2Decrypt("test", "");
        }
    }
}