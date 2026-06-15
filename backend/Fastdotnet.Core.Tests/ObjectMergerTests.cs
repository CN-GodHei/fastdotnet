using Newtonsoft.Json;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class ObjectMergerTests
{
    public class TestConfig
    {
        public string Name { get; set; } = string.Empty;
        public int MaxConnections { get; set; } = 10;
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public TestConfig? Nested { get; set; }
    }

    [TestMethod]
    public void ApplyOverrides_NullOverrides_ReturnsDefaultsClone()
    {
        var defaults = new TestConfig { Name = "default", MaxConnections = 5 };
        var result = ObjectMerger.ApplyOverrides(defaults, null!);

        Assert.AreEqual("default", result.Name);
        Assert.AreEqual(5, result.MaxConnections);
        Assert.AreNotSame(defaults, result); // 深拷贝
    }

    [TestMethod]
    public void ApplyOverrides_NullDefaults_ReturnsOverrides()
    {
        var overrides = new TestConfig { Name = "override", MaxConnections = 20 };
        var result = ObjectMerger.ApplyOverrides(null!, overrides);

        Assert.AreEqual("override", result.Name);
        Assert.AreEqual(20, result.MaxConnections);
    }

    [TestMethod]
    public void ApplyOverrides_OverridesApplied_Correctly()
    {
        var defaults = new TestConfig { Name = "default", MaxConnections = 10 };
        var overrides = new TestConfig { Name = "production", MaxConnections = 100 };

        var result = ObjectMerger.ApplyOverrides(defaults, overrides);

        Assert.AreEqual("production", result.Name);
        Assert.AreEqual(100, result.MaxConnections);
    }

    [TestMethod]
    public void ApplyOverrides_OverridesObject_NotMutateDefaults()
    {
        var defaults = new TestConfig { Name = "default" };
        var overrides = new TestConfig { Name = "production" };

        var result = ObjectMerger.ApplyOverrides(defaults, overrides);

        Assert.AreEqual("default", defaults.Name); // 原始对象未变
        Assert.AreEqual("production", result.Name);
    }

    [TestMethod]
    public void ApplyOverrides_DeepCopy_NoReferenceLeak()
    {
        var defaults = new TestConfig
        {
            Name = "default",
            Tags = new List<string> { "a", "b" }
        };
        var overrides = new TestConfig
        {
            Tags = new List<string> { "c" }
        };

        var result = ObjectMerger.ApplyOverrides(defaults, overrides);

        Assert.AreEqual(1, result.Tags.Count);
        Assert.AreEqual("c", result.Tags[0]);
    }
}
