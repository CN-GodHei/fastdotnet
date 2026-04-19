using Fastdotnet.Plugin.Contracts.Events;

namespace PluginA.EventHandlers;

/// <summary>
/// 插件安装事件处理器（PluginA 订阅系统事件）
/// </summary>
public class PluginInstalledEventHandler : IEventHandler<PluginInstalledEvent>
{
    private readonly ILogger<PluginInstalledEventHandler> _logger;
    
    public PluginInstalledEventHandler(ILogger<PluginInstalledEventHandler> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// 处理插件安装事件
    /// </summary>
    public async Task HandleAsync(PluginInstalledEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("【PluginA】检测到新插件安装：插件 ID={PluginId}, 版本={Version}", 
            @event.PluginId, @event.PluginVersion);
        
        // TODO: 这里可以添加业务逻辑，例如：
        // 1. 更新插件列表
        // 2. 初始化依赖关系
        // 3. 记录审计日志
        
        await Task.CompletedTask;
        
        _logger.LogInformation("【PluginA】插件安装事件处理完成");
    }
}
