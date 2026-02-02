namespace Fastdotnet.Core.Exceptions;

/// <summary>
/// 自定义业务异常
/// 此类异常被认为是可预期的，不会被日志系统记录。
/// </summary>
public class BusinessException : Exception
{
    public BusinessException()
    {
    }

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
