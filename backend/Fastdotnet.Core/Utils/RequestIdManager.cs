using System;
using System.Threading;

namespace Fastdotnet.Core.Utils;

/// <summary>
/// 请求ID管理器，用于生成和管理请求ID
/// </summary>
public static class RequestIdManager
{
    private static readonly AsyncLocal<string> _asyncLocalRequestId = new();

    /// <summary>
    /// 获取当前请求的ID（可能为 null）
    /// </summary>
    public static string? CurrentRequestId
    {
        get => _asyncLocalRequestId.Value;
        set => _asyncLocalRequestId.Value = value;
    }

    /// <summary>
    /// 生成新的请求ID（不自动设置到 CurrentRequestId）
    /// </summary>
    public static string GenerateNewRequestId()
    {
        return Guid.NewGuid().ToString("N"); // 只生成，不赋值
    }

    /// <summary>
    /// 清除当前请求ID
    /// </summary>
    public static void Clear()
    {
        _asyncLocalRequestId.Value = null;
    }
}