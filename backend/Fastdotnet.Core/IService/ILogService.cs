using Fastdotnet.Core.Dtos.LogModels;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService
{
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
        /// 添加操作日志（同步）
        /// </summary>
        /// <param name="log"></param>
        void AddOperationLog(OperationLog log);

        /// <summary>
        /// 添加异常日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task AddExceptionLogAsync(ExceptionLog log);
        
        /// <summary>
        /// 添加异常日志（同步）
        /// </summary>
        /// <param name="log"></param>
        void AddExceptionLog(ExceptionLog log);

        /// <summary>
        /// 添加调试日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task AddDebugLogAsync(DebugLog log);
        
        /// <summary>
        /// 添加调试日志（同步）
        /// </summary>
        /// <param name="log"></param>
        void AddDebugLog(DebugLog log);
    }
}