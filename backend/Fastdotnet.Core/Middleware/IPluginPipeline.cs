
namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// 定义了一个通用的插件管道契约，用于各种非 HTTP 场景的中间件。
    /// 这是一个泛型接口，可以用于工作流、消息队列、后台任务等任何自定义上下文。
    /// </summary>
    /// <typeparam name="TContext">上下文类型</typeparam>
    public interface IPluginPipeline<TContext>
    {
        /// <summary>
        /// 处理请求。
        /// </summary>
        /// <param name="context">当前请求的上下文。</param>
        /// <param name="next">管道中的下一个中间件。</param>
        /// <returns>表示中间件执行完成的任务。</returns>
        Task InvokeAsync(TContext context, Func<TContext, Task> next);
    }

    /// <summary>
    /// 泛型插件管道的注册表，用于存储和管理特定上下文类型的中间件。
    /// </summary>
    /// <typeparam name="TContext">上下文类型</typeparam>
    public class PluginPipelineRegistry<TContext>
    {
        private readonly ConcurrentDictionary<string, WeakReference<Type>> _pipelineTypes 
            = new ConcurrentDictionary<string, WeakReference<Type>>();

        /// <summary>
        /// 注册一个管道中间件类型。
        /// </summary>
        /// <param name="pipelineType">要注册的中间件类型，必须实现 IPluginPipeline<TContext></param>
        public void Register(Type pipelineType)
        {
            if (pipelineType == null) return;
            
            // 验证是否实现了 IPluginPipeline<TContext>
            if (!typeof(IPluginPipeline<TContext>).IsAssignableFrom(pipelineType))
            {
                throw new ArgumentException(
                    $"类型 {pipelineType.Name} 必须实现 IPluginPipeline<{typeof(TContext).Name}> 接口");
            }

            if (pipelineType.FullName == null) return;
            _pipelineTypes[pipelineType.FullName] = new WeakReference<Type>(pipelineType);
        }

        /// <summary>
        /// 注销一个管道中间件类型。
        /// </summary>
        /// <param name="pipelineType">要注销的中间件类型</param>
        public void Unregister(Type pipelineType)
        {
            if (pipelineType?.FullName == null) return;
            _pipelineTypes.TryRemove(pipelineType.FullName, out _);
        }

        /// <summary>
        /// 获取当前已注册且处于活动状态的中间件类型快照。
        /// </summary>
        /// <returns>中间件类型的可枚举集合</returns>
        public IEnumerable<Type> GetPipelineTypes()
        {
            var activeTypes = new List<Type>();
            foreach (var pair in _pipelineTypes)
            {
                if (pair.Value.TryGetTarget(out Type targetType))
                {
                    activeTypes.Add(targetType);
                }
                else
                {
                    // 主动从字典中移除无效引用
                    _pipelineTypes.TryRemove(pair.Key, out _);
                }
            }
            return activeTypes;
        }
    }

    /// <summary>
    /// 泛型插件管道的调度器，负责执行注册的中间件。
    /// </summary>
    /// <typeparam name="TContext">上下文类型</typeparam>
    public class PluginPipelineDispatcher<TContext>
    {
        private readonly PluginPipelineRegistry<TContext> _registry;
        private readonly IServiceProvider _serviceProvider;

        public PluginPipelineDispatcher(
            PluginPipelineRegistry<TContext> registry,
            IServiceProvider serviceProvider)
        {
            _registry = registry;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 执行管道中的所有中间件。
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="finalAction">管道的最终执行动作（当所有中间件完成后执行）</param>
        /// <returns>表示管道执行完成的任务</returns>
        public async Task ExecuteAsync(TContext context, Func<TContext, Task> finalAction)
        {
            var pipelineTypes = _registry.GetPipelineTypes().ToList();

            if (pipelineTypes.Any())
            {
                // 构建并执行临时管道
                Func<TContext, Task> pipeline = BuildPipeline(pipelineTypes, finalAction);
                await pipeline(context);
            }
            else
            {
                // 如果没有注册的中间件，直接执行最终动作
                await finalAction(context);
            }
        }

        /// <summary>
        /// 构建管道链。
        /// </summary>
        /// <param name="pipelineTypes">中间件类型列表</param>
        /// <param name="final">管道的最终执行动作</param>
        /// <returns>构建好的管道委托</returns>
        private Func<TContext, Task> BuildPipeline(
            IReadOnlyList<Type> pipelineTypes,
            Func<TContext, Task> final)
        {
            // 从后向前构建责任链
            Func<TContext, Task> pipeline = final;
            
            for (int i = pipelineTypes.Count - 1; i >= 0; i--)
            {
                var middlewareType = pipelineTypes[i];
                var next = pipeline;
                
                pipeline = async context =>
                {
                    // 动态创建中间件实例，支持依赖注入
                    var middleware = (IPluginPipeline<TContext>)ActivatorUtilities.CreateInstance(
                        _serviceProvider,
                        middlewareType);
                    await middleware.InvokeAsync(context, next);
                };
            }
            
            return pipeline;
        }
    }
}
