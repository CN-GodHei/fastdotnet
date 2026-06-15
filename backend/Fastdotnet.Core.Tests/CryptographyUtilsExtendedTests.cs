using System.Security.Cryptography;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class CryptographyUtilsExtendedTests
{
    [TestMethod]
    public void AESEncrypt_EmptyString_ReturnsValidCipher()
    {
        var encrypted = CryptographyUtils.AESEncrypt("", "1234567890123456");
        Assert.IsNotNull(encrypted);
        var decrypted = CryptographyUtils.AESDecrypt(encrypted, "1234567890123456");
        Assert.AreEqual("", decrypted);
    }

    [TestMethod]
    public void AESEncrypt_WrongKey_ThrowsOrProducesWrongDecryption()
    {
        var original = "Sensitive Data";
        var encrypted = CryptographyUtils.AESEncrypt(original, "1234567890123456");

        Assert.ThrowsException<CryptographicException>(() =>
            CryptographyUtils.AESDecrypt(encrypted, "6543210987654321"));
    }

    [TestMethod]
    public void RSA_KeyGeneration_ProducesUniqueKeys()
    {
        var (pub1, priv1) = CryptographyUtils.GenerateRSAKeyPair();
        var (pub2, priv2) = CryptographyUtils.GenerateRSAKeyPair();

        Assert.AreNotEqual(pub1, pub2);
        Assert.AreNotEqual(priv1, priv2);
    }

    [TestMethod]
    public void RSA_Decrypt_WithWrongPrivateKey_Throws()
    {
        var (pub1, _) = CryptographyUtils.GenerateRSAKeyPair();
        var (_, wrongPriv) = CryptographyUtils.GenerateRSAKeyPair();

        var encrypted = CryptographyUtils.RSAEncrypt("data", pub1);

        Assert.ThrowsException<CryptographicException>(() =>
            CryptographyUtils.RSADecrypt(encrypted, wrongPriv));
    }

    [TestMethod]
    public void HashPassword_EmptyString_ReturnsValidHash()
    {
        var hash = CryptographyUtils.HashPassword("");
        Assert.IsFalse(string.IsNullOrEmpty(hash));
    }

    [TestMethod]
    public void HybridEncryption_RoundTrip_WorksCorrectly()
    {
        var (pub, priv) = CryptographyUtils.GenerateRSAKeyPair();
        var original = "混合加密测试数据";

        var encrypted = HybridEncryptionUtils.Encrypt(original, pub);
        var decrypted = HybridEncryptionUtils.Decrypt(encrypted, priv);

        Assert.AreEqual(original, decrypted);
    }

    [TestMethod]
    public void HybridEncryption_WrongKey_Throws()
    {
        var (pub1, _) = CryptographyUtils.GenerateRSAKeyPair();
        var (_, wrongPriv) = CryptographyUtils.GenerateRSAKeyPair();

        var encrypted = HybridEncryptionUtils.Encrypt("test", pub1);

        Assert.ThrowsException<CryptographicException>(() =>
            HybridEncryptionUtils.Decrypt(encrypted, wrongPriv));
    }

    [TestMethod]
    public void HybridEncryption_DifferentInputs_DifferentOutputs()
    {
        var (pub, _) = CryptographyUtils.GenerateRSAKeyPair();

        var enc1 = HybridEncryptionUtils.Encrypt("data1", pub);
        var enc2 = HybridEncryptionUtils.Encrypt("data2", pub);

        Assert.AreNotEqual(enc1, enc2);
    }

    [TestMethod]
    public void RSAEncrypt_NullPlainText_ThrowsArgumentException()
    {
        var (pub, _) = CryptographyUtils.GenerateRSAKeyPair();
        Assert.ThrowsException<ArgumentException>(() =>
            CryptographyUtils.RSAEncrypt(null!, pub));
    }

    [TestMethod]
    public void AESDecrypt_EmptyCipher_ThrowsOrReturnsEmpty()
    {
        // 空密文应该触发异常或返回空
        Assert.ThrowsException<ArgumentException>(() =>
            CryptographyUtils.AESDecrypt("", "1234567890123456"));
    }
}
