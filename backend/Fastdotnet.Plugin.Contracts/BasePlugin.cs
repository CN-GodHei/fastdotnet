
namespace Fastdotnet.Plugin.Contracts
{
/// <summary>
    /// 插件基类，提供默认的生命周期状态管理实现
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        private volatile PluginLifecycleState _lifecycleState = PluginLifecycleState.Uninitialized;

        /// <summary>
        /// 获取插件 ID
        /// </summary>
        public abstract string PluginId { get; }

        /// <summary>
        /// 获取插件名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取插件版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 获取插件当前生命周期状态
        /// </summary>
        public PluginLifecycleState LifecycleState => _lifecycleState;

        /// <summary>
        /// 更新插件生命周期状态
        /// </summary>
        protected void UpdateLifecycleState(PluginLifecycleState newState)
        {
            _lifecycleState = newState;
        }

        /// <summary>
        /// 插件初始化（此方法不可被重写，确保状态管理逻辑不被跳过）
        /// </summary>
        /// <remarks>
        /// 子类应该重写 <see cref="OnInitializeAsync"/> 而不是此方法
        /// </remarks>
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            await OnInitializeAsync(serviceProvider);
            UpdateLifecycleState(PluginLifecycleState.Initialized);
        }

        /// <summary>
        /// 插件启动（此方法不可被重写，确保状态管理逻辑不被跳过）
        /// </summary>
        /// <remarks>
        /// 子类应该重写 <see cref="OnStartAsync"/> 而不是此方法
        /// </remarks>
        public async Task StartAsync()
        {
            await OnStartAsync();
            UpdateLifecycleState(PluginLifecycleState.Started);
        }

        /// <summary>
        /// 插件停止（此方法不可被重写，确保状态管理逻辑不被跳过）
        /// </summary>
        /// <remarks>
        /// 子类应该重写 <see cref="OnStopAsync"/> 而不是此方法
        /// </remarks>
        public async Task StopAsync()
        {
            await OnStopAsync();
            UpdateLifecycleState(PluginLifecycleState.Stopped);
        }

        /// <summary>
        /// 插件卸载前的清理工作（此方法不可被重写，确保状态管理逻辑不被跳过）
        /// </summary>
        /// <remarks>
        /// 子类应该重写 <see cref="OnUnloadAsync"/> 而不是此方法
        /// </remarks>
        public async Task UnloadAsync(IServiceProvider serviceProvider)
        {
            await OnUnloadAsync(serviceProvider);
            UpdateLifecycleState(PluginLifecycleState.Unloaded);
        }

        /// <summary>
        /// 配置插件服务
        /// </summary>
        /// <param name="builder">Autofac 容器构建器</param>
        public abstract void ConfigureServices(ContainerBuilder builder);

        /// <summary>
        /// 初始化时的钩子方法，子类可重写
        /// </summary>
        protected virtual Task OnInitializeAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动时的钩子方法，子类可重写
        /// </summary>
        protected virtual Task OnStartAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止时的钩子方法，子类可重写
        /// </summary>
        protected virtual Task OnStopAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 卸载时的钩子方法，子类可重写
        /// </summary>
        protected virtual Task OnUnloadAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }
    }
}