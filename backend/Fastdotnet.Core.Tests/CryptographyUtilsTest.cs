using System.Security.Cryptography;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class CryptographyUtilsTest
{
    // AES-256 需要 32 字节密钥，编码为 Base64
    private static readonly string AesKey = Convert.ToBase64String(new byte[32]
        { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32 });

    private static readonly string AesIv = Convert.ToBase64String(new byte[16]
        { 101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116 });

    // ECB 模式用 16 字节密钥
    private static readonly string AesEcbKey = Convert.ToBase64String(new byte[16]
        { 50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65 });

    private static readonly string WrongAesKey = Convert.ToBase64String(new byte[32]
        { 99,98,97,96,95,94,93,92,91,90,89,88,87,86,85,84,83,82,81,80,79,78,77,76,75,74,73,72,71,70,69,68 });

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

        var encrypted = CryptographyUtils.AESEncrypt(original, AesKey);
        Assert.IsNotNull(encrypted);
        Assert.AreNotEqual(original, encrypted);

        var decrypted = CryptographyUtils.AESDecrypt(encrypted, AesKey);
        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void AES_Encrypt_WithIV_RoundTrip()
    {
        var original = "AES with IV!";

        var encrypted = CryptographyUtils.AESEncrypt(original, AesKey, AesIv);
        var decrypted = CryptographyUtils.AESDecrypt(encrypted, AesKey);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void AES_Decrypt_WithWrongKey_ThrowsException()
    {
        var original = "Sensitive Data";
        var encrypted = CryptographyUtils.AESEncrypt(original, AesKey);

        Assert.ThrowsException<CryptographicException>(() =>
            CryptographyUtils.AESDecrypt(encrypted, WrongAesKey));
    }

    [TestMethod]
    public void AES_ECB_Encrypt_Decrypt_RoundTrip()
    {
        var original = "ECB Mode Test";

        var encrypted = CryptographyUtils.AESEncryptECB(original, AesEcbKey);
        Assert.IsNotNull(encrypted);

        var decrypted = CryptographyUtils.AESDecryptECB(encrypted, AesEcbKey);
        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void AESEncrypt_EmptyString_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CryptographyUtils.AESEncrypt("", AesKey));
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

    [TestMethod]
    public void HashPassword_EmptyString_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CryptographyUtils.HashPassword(""));
    }

    #endregion

    #region 密码可逆加密测试

    [TestMethod]
    public void Encrypt_Decrypt_Password_RoundTrip()
    {
        var password = "SecretPassword123";
        var key = Convert.ToBase64String(new byte[32]);

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
