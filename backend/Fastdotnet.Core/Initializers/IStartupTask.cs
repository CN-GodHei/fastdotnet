using System.Threading.Tasks;

namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// 定义了一个接口，用于指定应用程序启动时应执行的任务。
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// 执行任务:外部统一处理异常日志，无需手动编写异常日志，除非捕捉异常后还需要自己的逻辑
        /// </summary>
        /// <returns>表示异步操作的任务。</returns>
        Task ExecuteAsync();
    }
}
