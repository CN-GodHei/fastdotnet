
namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 一个线程安全的注册表，用于存储和检索动态加载的中间件类型。
    /// 此服务作为单例注册，并作为 DynamicMiddlewareDispatcher 的中心事实点。
    /// 它使用 WeakReference 来持有中间件类型，防止其阻止插件程序集卸载。
    /// </summary>
    public class DynamicMiddlewareRegistry
    {
        private readonly ConcurrentDictionary<string, WeakReference<Type>> _middlewareTypes = new ConcurrentDictionary<string, WeakReference<Type>>();

        /// <summary>
        /// 注册一个中间件类型。
        /// </summary>
        /// <param name="middlewareType">要注册的中间件类型。必须实现 IDynamicMiddleware 接口。</param>
        public void Register(Type middlewareType)
        {
            if (middlewareType?.FullName == null) return;
            _middlewareTypes[middlewareType.FullName] = new WeakReference<Type>(middlewareType);
        }

        /// <summary>
        /// 注销一个中间件类型。
        /// </summary>
        /// <param name="middlewareType">要注销的中间件类型。</param>
        public void Unregister(Type middlewareType)
        {
            if (middlewareType?.FullName == null) return;
            _middlewareTypes.TryRemove(middlewareType.FullName, out _);
        }

        /// <summary>
        /// 获取当前已注册且处于活动状态的中间件类型的快照。
        /// 在此过程中会清理对垃圾回收类型的无效引用。
        /// </summary>
        /// <returns>中间件类型的可枚举集合。</returns>
        public IEnumerable<Type> GetMiddlewareTypes()
        {
            var activeTypes = new List<Type>();
            foreach (var pair in _middlewareTypes)
            {
                if (pair.Value.TryGetTarget(out Type targetType))
                {
                    activeTypes.Add(targetType);
                }
                else
                {
                    // 主动从字典中移除无效引用
                    _middlewareTypes.TryRemove(pair.Key, out _);
                }
            }
            return activeTypes;
        }
    }
}
