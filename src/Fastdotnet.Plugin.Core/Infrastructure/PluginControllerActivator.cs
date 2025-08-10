using Autofac;
using Fastdotnet.Plugin.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;

public class PluginControllerActivator : IControllerActivator
{
    private readonly IPluginLoadService _pluginLoadService;
    private readonly PluginManager _pluginManager;

    public PluginControllerActivator(IPluginLoadService pluginLoadService, PluginManager pluginManager)
    {
        _pluginLoadService = pluginLoadService;
        _pluginManager = pluginManager;
    }

    public object Create(ControllerContext controllerContext)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException(nameof(controllerContext));
        }

        var controllerType = controllerContext.ActionDescriptor.ControllerTypeInfo.AsType();

        // 检查控制器是否来自插件
        if (_pluginManager.TryGetPluginId(controllerType, out var pluginId))
        {
            // 如果是，则从该插件的隔离子容器中解析
            if (_pluginLoadService.TryGetPluginScope(pluginId, out var pluginScope))
            {
                return pluginScope.Resolve(controllerType);
            }
        }

        // 对于非插件的控制器，使用默认的请求作用域来创建
        return ActivatorUtilities.CreateInstance(controllerContext.HttpContext.RequestServices, controllerType);
    }

    public void Release(ControllerContext context, object controller)
    {
        // 控制器的释放由创建它的DI容器（无论是请求作用域还是插件作用域）自动处理
        // 我们只需要确保可释放的控制器被调用Dispose即可
        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
