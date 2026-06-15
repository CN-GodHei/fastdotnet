using System.Security.Cryptography;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class CryptographyUtilsTest
{
    #region RSA 测试

    [TestMethod]
    public void GenerateRSAKeyPair_Default_ReturnsValidKeyPair()
    {
        var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();

        Assert.IsFalse(string.IsNullOrEmpty(publicKey));
        Assert.IsFalse(string.IsNullOrEmpty(privateKey));
        Assert.AreNotEqual(publicKey, privateKey);
    }

    [TestMethod]
    public void RSA_Encrypt_Decrypt_RoundTrip()
    {
        var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();
        var original = "Hello, RSA!";

        var encrypted = CryptographyUtils.RSAEncrypt(original, publicKey);
        Assert.IsNotNull(encrypted);
        Assert.AreNotEqual(original, encrypted);

        var decrypted = CryptographyUtils.RSADecrypt(encrypted, privateKey);
        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void RSA_Sign_Verify_WorksCorrectly()
    {
        var (publicKey, privateKey) = CryptographyUtils.GenerateRSAKeyPair();
        var data = "Data to sign";

        var signature = CryptographyUtils.RSASign(data, privateKey);
        Assert.IsNotNull(signature);

        var isValid = CryptographyUtils.RSAVerify(data, signature, publicKey);
        Assert.IsTrue(isValid);

        var isInvalid = CryptographyUtils.RSAVerify("Different data", signature, publicKey);
        Assert.IsFalse(isInvalid);
    }

    [TestMethod]
    public void RSAEncrypt_EmptyPlainText_ThrowsArgumentException()
    {
        var (publicKey, _) = CryptographyUtils.GenerateRSAKeyPair();
        Assert.ThrowsException<ArgumentException>(() =>
            CryptographyUtils.RSAEncrypt("", publicKey));
    }

    [TestMethod]
    public void RSADecrypt_WithWrongKey_ThrowsException()
    {
        var (pub1, _) = CryptographyUtils.GenerateRSAKeyPair();
        var (_, wrongPriv) = CryptographyUtils.GenerateRSAKeyPair();

        var encrypted = CryptographyUtils.RSAEncrypt("test", pub1);
        Assert.ThrowsException<CryptographicException>(() =>
            CryptographyUtils.RSADecrypt(encrypted, wrongPriv));
    }

    #endregion

    #region AES 测试

    [TestMethod]
    public void AES_Encrypt_Decrypt_RoundTrip()
    {
        var original = "Hello, AES!";
        var key = "1234567890123456";

        var encrypted = CryptographyUtils.AESEncrypt(original, key);
        Assert.IsNotNull(encrypted);
        Assert.AreNotEqual(original, encrypted);

        var decrypted = CryptographyUtils.AESDecrypt(encrypted, key);
        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void AES_Encrypt_WithIV_RoundTrip()
    {
        var original = "AES with IV!";
        var key = "1234567890123456";
        var iv = "abcdefghijklmnop";

        var encrypted = CryptographyUtils.AESEncrypt(original, key, iv);
        var decrypted = CryptographyUtils.AESDecrypt(encrypted, key);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void AES_Decrypt_WithWrongKey_ThrowsException()
    {
        var original = "Sensitive Data";
        var encrypted = CryptographyUtils.AESEncrypt(original, "1234567890123456");

        Assert.ThrowsException<CryptographicException>(() =>
            CryptographyUtils.AESDecrypt(encrypted, "6543210987654321"));
    }

    [TestMethod]
    public void AES_ECB_Encrypt_Decrypt_RoundTrip()
    {
        var original = "ECB Mode Test";
        var key = "key1234567890abc";

        var encrypted = CryptographyUtils.AESEncryptECB(original, key);
        Assert.IsNotNull(encrypted);

        var decrypted = CryptographyUtils.AESDecryptECB(encrypted, key);
        Assert.AreEqual(original, decrypted);
    }

    #endregion

    #region 密码哈希测试

    [TestMethod]
    public void HashPassword_ReturnsValidHash()
    {
        var password = "MySecurePassword";
        var hash = CryptographyUtils.HashPassword(password);

        Assert.IsNotNull(hash);
        Assert.IsTrue(hash.Length > 0);
    }

    [TestMethod]
    public void HashPassword_SameInput_ProducesDifferentHashes()
    {
        var hash1 = CryptographyUtils.HashPassword("password");
        var hash2 = CryptographyUtils.HashPassword("password");

        // 每次哈希应产生不同盐值
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "correct";
        var hash = CryptographyUtils.HashPassword(password);

        var result = CryptographyUtils.VerifyPassword(password, hash);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = CryptographyUtils.HashPassword("correct");

        var result = CryptographyUtils.VerifyPassword("wrong", hash);
        Assert.IsFalse(result);
    }

    #endregion

    #region 密码可逆加密测试

    [TestMethod]
    public void Encrypt_Decrypt_Password_RoundTrip()
    {
        var password = "SecretPassword123";
        var key = Convert.ToBase64String(new byte[32]); // 32 字节密钥

        var encrypted = CryptographyUtils.EncryptPassword(password, key);
        Assert.IsNotNull(encrypted);
        Assert.AreNotEqual(password, encrypted);

        var decrypted = CryptographyUtils.DecryptPassword(encrypted, key);
        Assert.AreEqual(password, decrypted);
    }

    [TestMethod]
    public void ProcessPassword_HashType_Correct()
    {
        var password = "testpass";
        var result = CryptographyUtils.ProcessPassword(password, CryptographyUtils.PasswordHashType.Irreversible);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(password, result);
    }

    [TestMethod]
    public void VerifyProcessedPassword_WorksCorrectly()
    {
        var password = "verifyMe";
        var hashed = CryptographyUtils.ProcessPassword(password, CryptographyUtils.PasswordHashType.Irreversible);

        var valid = CryptographyUtils.VerifyProcessedPassword(password, hashed, CryptographyUtils.PasswordHashType.Irreversible);
        Assert.IsTrue(valid);

        var invalid = CryptographyUtils.VerifyProcessedPassword("wrong", hashed, CryptographyUtils.PasswordHashType.Irreversible);
        Assert.IsFalse(invalid);
    }

    #endregion
}
