using Fastdotnet.Core.Models.Logs;
using System.Threading.Tasks;

namespace Fastdotnet.Service.IService;

/// <summary>
/// 日志服务接口
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 添加操作日志
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    Task AddOperationLogAsync(OperationLog log);

    /// <summary>
    /// 添加异常日志
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    Task AddExceptionLogAsync(ExceptionLog log);

    /// <summary>
    /// 添加调试日志
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    Task AddDebugLogAsync(DebugLog log);
}
