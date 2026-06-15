using Fastdotnet.Core.Exceptions;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class BusinessExceptionTests
{
    [TestMethod]
    public void Constructor_Default_DoesNotThrow()
    {
        var ex = new BusinessException();
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Constructor_WithMessage_SetsMessage()
    {
        var ex = new BusinessException("业务处理失败");

        Assert.AreEqual("业务处理失败", ex.Message);
    }

    [TestMethod]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        var inner = new InvalidOperationException("内部错误");
        var ex = new BusinessException("包装异常", inner);

        Assert.AreEqual("包装异常", ex.Message);
        Assert.AreSame(inner, ex.InnerException);
    }

    [TestMethod]
    public void DefaultException_InheritsFromSystemException()
    {
        var ex = new BusinessException();

        Assert.IsInstanceOfType<Exception>(ex);
    }

    [TestMethod]
    public void Constructor_WithEmptyMessage_SetsEmptyMessage()
    {
        var ex = new BusinessException(string.Empty);

        Assert.AreEqual(string.Empty, ex.Message);
    }
}
