using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class DictValueConverterTests
{
    [TestMethod]
    public void ConvertToDotNetType_NullDictData_ReturnsNull()
    {
        var result = DictValueConverter.ConvertToDotNetType(null!);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ConvertToDotNetType_StringType_ReturnsString()
    {
        var dictData = new FdDictData { Value = "hello", ValueType = DictValueType.String };
        var result = DictValueConverter.ConvertToDotNetType(dictData);
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void ConvertByValueType_Int_ReturnsInt()
    {
        var result = DictValueConverter.ConvertByValueType("42", DictValueType.Int);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void ConvertByValueType_Long_ReturnsLong()
    {
        var result = DictValueConverter.ConvertByValueType("9999999999", DictValueType.Long);
        Assert.AreEqual(9999999999L, result);
    }

    [TestMethod]
    public void ConvertByValueType_Double_ReturnsDouble()
    {
        var result = DictValueConverter.ConvertByValueType("3.14159", DictValueType.Double);
        var delta = Math.Abs(3.14159 - (double)result!);
        Assert.IsTrue(delta < 0.00001);
    }

    [TestMethod]
    public void ConvertByValueType_Decimal_ReturnsDecimal()
    {
        var result = DictValueConverter.ConvertByValueType("99.99", DictValueType.Decimal);
        Assert.AreEqual(99.99m, result);
    }

    [TestMethod]
    public void ConvertByValueType_Boolean_True_ReturnsTrue()
    {
        var result = DictValueConverter.ConvertByValueType("true", DictValueType.Boolean);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void ConvertByValueType_Boolean_False_ReturnsFalse()
    {
        var result = DictValueConverter.ConvertByValueType("false", DictValueType.Boolean);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void ConvertByValueType_DateTime_ReturnsDateTime()
    {
        var result = DictValueConverter.ConvertByValueType("2024-01-15", DictValueType.DateTime);
        Assert.AreEqual(new DateTime(2024, 1, 15), result);
    }

    [TestMethod]
    public void ConvertByValueType_Json_ReturnsJObject()
    {
        var result = DictValueConverter.ConvertByValueType("{\"key\":\"val\"}", DictValueType.Json);
        Assert.IsNotNull(result);
        Assert.AreEqual("val", (string)((Newtonsoft.Json.Linq.JObject)result!)["key"]!);
    }

    [TestMethod]
    public void ConvertByValueType_JsonArray_ReturnsJArray()
    {
        var result = DictValueConverter.ConvertByValueType("[\"a\",\"b\"]", DictValueType.JsonArray);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, ((Newtonsoft.Json.Linq.JArray)result!).Count);
    }

    [TestMethod]
    public void ConvertByValueType_StringOverload_Int_Correct()
    {
        var result = DictValueConverter.ConvertByValueType("42", "int");
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void ConvertByValueType_StringOverload_Bool_Correct()
    {
        var result = DictValueConverter.ConvertByValueType("true", "boolean");
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void ConvertByValueType_StringOverload_UnknownType_ReturnsString()
    {
        var result = DictValueConverter.ConvertByValueType("data", "unknown");
        Assert.AreEqual("data", result);
    }

    [TestMethod]
    public void ConvertByValueType_InvalidInt_ReturnsDefaultZero()
    {
        var result = DictValueConverter.ConvertByValueType("not_a_number", DictValueType.Int);
        Assert.AreEqual(0, result);
    }
}
