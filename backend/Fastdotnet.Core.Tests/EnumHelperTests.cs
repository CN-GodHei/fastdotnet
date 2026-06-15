using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class EnumHelperTests
{
    // 测试用枚举
    private enum TestEnum
    {
        [Description("未知状态")]
        Unknown = 0,

        [Description("激活状态")]
        Active = 1,

        Disabled = 2
    }

    private enum EmptyEnum { }

    [TestMethod]
    public void GetDescription_ValidEnum_ReturnsDescription()
    {
        var desc = EnumHelper.GetDescription(TestEnum.Active);
        Assert.AreEqual("激活状态", desc);
    }

    [TestMethod]
    public void GetDescription_NoDescriptionAttribute_ReturnsEnumName()
    {
        var desc = EnumHelper.GetDescription(TestEnum.Disabled);
        Assert.AreEqual("Disabled", desc);
    }

    [TestMethod]
    public void GetDescription_NullEnum_ReturnsEmptyString()
    {
        var desc = EnumHelper.GetDescription(null!);
        Assert.AreEqual(string.Empty, desc);
    }

    [TestMethod]
    public void TryGetEnumFromDescription_ValidDescription_ReturnsTrue()
    {
        var result = EnumHelper.TryGetEnumFromDescription<TestEnum>("激活状态", out var value);
        Assert.IsTrue(result);
        Assert.AreEqual(TestEnum.Active, value);
    }

    [TestMethod]
    public void TryGetEnumFromDescription_InvalidDescription_ReturnsFalse()
    {
        var result = EnumHelper.TryGetEnumFromDescription<TestEnum>("不存在的描述", out var value);
        Assert.IsFalse(result);
        Assert.AreEqual(default(TestEnum), value);
    }

    [TestMethod]
    public void ToDescriptionDictionary_WithEnum_ReturnsPopulatedDictionary()
    {
        var dict = EnumHelper.ToDescriptionDictionary<TestEnum>();

        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual("未知状态", dict[0]);
        Assert.AreEqual("激活状态", dict[1]);
        Assert.AreEqual("Disabled", dict[2]);
    }

    [TestMethod]
    public void GetAllMembers_WithEnum_ReturnsAllMembers()
    {
        var members = EnumHelper.GetAllMembers<TestEnum>();

        Assert.AreEqual(3, members.Count);
        Assert.AreEqual(TestEnum.Unknown, members[0].Value);
        Assert.AreEqual(TestEnum.Active, members[1].Value);
        Assert.AreEqual(TestEnum.Disabled, members[2].Value);
    }

    [TestMethod]
    public void IsEqual_SameStringValue_ReturnsTrue()
    {
        var result = EnumHelper.IsEqual("Active", TestEnum.Active);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsEqual_DifferentStringValue_ReturnsFalse()
    {
        var result = EnumHelper.IsEqual("Unknown", TestEnum.Active);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ParseEnum_ValidString_ReturnsEnumValue()
    {
        var result = EnumHelper.ParseEnum<TestEnum>("Active");

        Assert.AreEqual(TestEnum.Active, result);
    }

    [TestMethod]
    public void ParseEnum_InvalidString_ReturnsDefault()
    {
        var result = EnumHelper.ParseEnum<TestEnum>("Invalid");

        Assert.AreEqual(default(TestEnum), result);
    }
}
