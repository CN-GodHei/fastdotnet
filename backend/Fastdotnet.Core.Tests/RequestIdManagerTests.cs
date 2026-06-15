using Fastdotnet.Core.Utils;

namespace Fastdotnet.Core.Tests;

[TestClass]
public class RequestIdManagerTests
{
    [TestCleanup]
    public void Cleanup()
    {
        RequestIdManager.Clear();
    }

    [TestMethod]
    public void GenerateNewRequestId_ReturnsNonEmptyGuidString()
    {
        var id = RequestIdManager.GenerateNewRequestId();

        Assert.IsFalse(string.IsNullOrEmpty(id));
        Assert.AreEqual(32, id.Length); // Guid "N" 格式
    }

    [TestMethod]
    public void GenerateNewRequestId_GeneratesUniqueIds()
    {
        var id1 = RequestIdManager.GenerateNewRequestId();
        var id2 = RequestIdManager.GenerateNewRequestId();

        Assert.AreNotEqual(id1, id2);
    }

    [TestMethod]
    public void GenerateNewRequestId_DoesNotSetCurrentRequestId()
    {
        var previous = RequestIdManager.CurrentRequestId;
        RequestIdManager.GenerateNewRequestId();

        Assert.AreEqual(previous, RequestIdManager.CurrentRequestId);
    }

    [TestMethod]
    public void SetAndGetCurrentRequestId_WorksCorrectly()
    {
        RequestIdManager.CurrentRequestId = "test-id-123";

        Assert.AreEqual("test-id-123", RequestIdManager.CurrentRequestId);
    }

    [TestMethod]
    public void Clear_RemovesRequestId()
    {
        RequestIdManager.CurrentRequestId = "test-id";
        RequestIdManager.Clear();

        Assert.IsNull(RequestIdManager.CurrentRequestId);
    }
}
