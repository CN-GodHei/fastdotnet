
namespace Fastdotnet.Core.Initializers
{
    /// <summary>
    /// 定义一个接口，用于在应用服务配置完成后需要初始化的服务。
    /// </summary>
    public interface IApplicationInitializer
    {
        /// <summary>
        /// 获取此初始化器的执行顺序。值越小越先执行。
        /// 默认值为 1001。字典初始化器应使用较小的值（如 1000），用户初始化器应使用较大的值（如 2000）。；1-1000系统级的初始化，1001-2000字典等的初始化，2001-3000插件级的初始化
        /// </summary>
        int Order => 1001;

        /// <summary>
        /// 执行服务的初始化逻辑。
        /// </summary>
        /// <returns>表示异步初始化操作的任务。</returns>
        Task InitializeAsync();
    }
}
