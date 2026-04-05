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

            // Console.WriteLine($"========== 开始执行初始化器 (共 {initializers.Count()} 个) ==========");
            
            foreach (var initializer in initializers)
            {
                var typeName = initializer.GetType().Name;
                var order = initializer.Order;
                // Console.WriteLine($"[初始化器] 开始执行: {typeName} (Order={order})");
                
                await initializer.InitializeAsync();
                
                // Console.WriteLine($"[初始化器] 完成执行: {typeName}");
            }
            
            // Console.WriteLine("========== 所有初始化器执行完成 ==========");
        }
    }
}
