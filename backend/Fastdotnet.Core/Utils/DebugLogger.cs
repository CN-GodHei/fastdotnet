

namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 调试信息输出工具类，供主框架和插件使用
    /// 主要用于生产环境的问题追踪和信息输出，替代断点调试
    /// </summary>
    public static class DebugLogger
    {
        // 由于是静态工具类，需要通过依赖注入设置服务
        private static ILogService _logService;

        /// <summary>
        /// 初始化DebugLogger，通常在程序启动时调用
        /// </summary>
        /// <param name="logService"></param>
        public static void Initialize(ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="key">业务标识</param>
        public static async Task PrintAsync(string message, string key = null)
        {
            if (_logService == null)
                throw new InvalidOperationException("DebugLogger未初始化，请先调用Initialize方法");

            var debugLog = new DebugLog
            {
                Message = message,
                Key = key
            };

            await _logService.AddDebugLogAsync(debugLog);
        }

        /// <summary>
        /// 输出调试信息（同步）
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="key">业务标识</param>
        public static void Print(string message, string key = null)
        {
            if (_logService == null)
                throw new InvalidOperationException("DebugLogger未初始化，请先调用Initialize方法");

            var debugLog = new DebugLog
            {
                Message = message,
                Key = key
            };

            _logService.AddDebugLog(debugLog);
        }
    }
}