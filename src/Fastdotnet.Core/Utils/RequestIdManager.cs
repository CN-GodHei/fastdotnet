using System;
using System.Threading;

namespace Fastdotnet.Core.Utils;

/// <summary>
/// 请求ID管理器，用于生成和管理请求ID
/// </summary>
public static class RequestIdManager
{
    private static readonly AsyncLocal<string> _asyncLocalRequestId = new AsyncLocal<string>();

    /// <summary>
    /// 获取当前请求的ID
    /// </summary>
    public static string CurrentRequestId
    {
        get
        {
            return _asyncLocalRequestId.Value ??= Guid.NewGuid().ToString("N");
        }
        set
        {
            _asyncLocalRequestId.Value = value;
        }
    }

    /// <summary>
    /// 生成新的请求ID
    /// </summary>
    /// <returns></returns>
    public static string GenerateNewRequestId()
    {
        var requestId = Guid.NewGuid().ToString("N"); // 不带连字符的GUID
        CurrentRequestId = requestId;
        return requestId;
    }

    /// <summary>
    /// 清除当前请求ID
    /// </summary>
    public static void Clear()
    {
        _asyncLocalRequestId.Value = null;
    }
}