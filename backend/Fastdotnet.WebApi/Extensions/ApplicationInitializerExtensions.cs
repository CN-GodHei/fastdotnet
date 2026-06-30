namespace Fastdotnet.WebApi.Extensions
{
    /// <summary>
    /// 提供应用初始化器的扩展方法。
    /// </summary>
    public static class ApplicationInitializerExtensions
    {
        /// <summary>
        /// 发现并按顺序执行所有注册的 IApplicationInitializer 服务。
        /// </summary>
        /// <param name="app">WebApplication 实例。</param>
        public static async Task UseApplicationInitializers(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializers = scope.ServiceProvider.GetServices<IApplicationInitializer>()
                .OrderBy(i => i.Order);
        
            foreach (var initializer in initializers)
            {
                await initializer.InitializeAsync();
            }
        }
    }
}
