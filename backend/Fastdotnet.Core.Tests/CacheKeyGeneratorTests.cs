using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class CacheKeyGeneratorTests
{
    [TestMethod]
    public void GenerateKey_ValidPrefixOnly_ReturnsPrefix()
    {
        var key = CacheKeyGenerator.GenerateKey("user");

        Assert.AreEqual("user", key);
    }

    [TestMethod]
    public void GenerateKey_ValidPrefixWithParams_ReturnsCombinedKey()
    {
        var key = CacheKeyGenerator.GenerateKey("user", 42, "profile");

        Assert.AreEqual("user:42:profile", key);
    }

    [TestMethod]
    public void GenerateKey_EmptyPrefix_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CacheKeyGenerator.GenerateKey(""));
    }

    [TestMethod]
    public void GenerateKey_NullPrefix_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CacheKeyGenerator.GenerateKey(null!));
    }

    [TestMethod]
    public void GenerateKeyWithTag_NullTags_ReturnsKeyWithoutTags()
    {
        var key = CacheKeyGenerator.GenerateKeyWithTag("user", null!, 99);

        Assert.AreEqual("user:99", key);
    }

    [TestMethod]
    public void GenerateKeyWithTag_WithTags_ReturnsKeyWithTags()
    {
        var key = CacheKeyGenerator.GenerateKeyWithTag("user", new[] { "vip", "active" }, 88);

        Assert.AreEqual("user:88@vip,active", key);
    }
}
