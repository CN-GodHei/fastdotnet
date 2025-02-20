using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fastdotnet.Core.Plugin
{
    public class PluginManager
    {
        private readonly IServiceCollection _services;
        private readonly string _pluginPath;
        private IEndpointRouteBuilder _endpointRouteBuilder;
        private readonly ConcurrentDictionary<string, IPlugin> _loadedPlugins;
        private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies;
        private FileSystemWatcher _watcher;
        private IEndpointRouteBuilder _app;

        public PluginManager(IServiceCollection services, string pluginPath)
        {
            _services = services;
            _pluginPath = pluginPath;
            _loadedPlugins = new ConcurrentDictionary<string, IPlugin>();
            _loadedAssemblies = new ConcurrentDictionary<string, Assembly>();
            InitializeFileWatcher();
        }

        private void InitializeFileWatcher()
        {
            _watcher = new FileSystemWatcher(_pluginPath)
            {
                Filter = "*.dll",
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            _watcher.Created += OnPluginChanged;
            _watcher.Changed += OnPluginChanged;
            _watcher.Deleted += OnPluginDeleted;
        }

        private void OnPluginChanged(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                LoadPlugin(e.FullPath);
            }
        }

        private void OnPluginDeleted(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                UnloadPlugin(e.FullPath);
            }
        }

        public void LoadPlugin(string pluginPath)
        {
            try
            {
                var assemblyName = Path.GetFileNameWithoutExtension(pluginPath);

                // 检查程序集是否已经加载
                if (_loadedAssemblies.TryGetValue(assemblyName, out var existingAssembly))
                {
                    Console.WriteLine($"Assembly {assemblyName} is already loaded, skipping...");
                    return;
                }

                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
                _loadedAssemblies.TryAdd(assemblyName, assembly);

                Console.WriteLine($"开始加载程序集 {assemblyName}");
                var allTypes = assembly.GetTypes();
                Console.WriteLine($"程序集 {assemblyName} 中包含 {allTypes.Length} 个类型");

                // 注册带有PluginService特性的服务
                var serviceTypes = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.GetCustomAttribute<PluginServiceAttribute>() != null));
                foreach (var serviceType in serviceTypes)
                {
                    var serviceInterface = serviceType.GetInterfaces()
                        .FirstOrDefault(i => i.GetCustomAttribute<PluginServiceAttribute>() != null);
                    if (serviceInterface != null)
                    {
                        _services.AddScoped(serviceInterface, serviceType);
                    }
                }

                // 注册插件中的所有控制器
                var controllerTypes = assembly.GetTypes()
                    .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract && !t.IsInterface);
                var controllerList = controllerTypes.ToList();
                Console.WriteLine($"找到 {controllerList.Count} 个控制器类型");
                foreach (var controllerType in controllerList)
                {
                    Console.WriteLine($"正在注册控制器: {controllerType.FullName}");
                    _services.AddTransient(controllerType);

                    // 注册控制器的路由
                    Console.WriteLine($"准备为控制器 {controllerType.FullName} 注册路由，_endpointRouteBuilder是否为null: {_endpointRouteBuilder == null}");
                    if (_endpointRouteBuilder != null)
                    {
                        Console.WriteLine($"开始为控制器 {controllerType.FullName} 注册路由");
                        var controllerName = controllerType.Name.Replace("Controller", "");
                        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => m.DeclaringType == controllerType);

                        foreach (var method in methods)
                        {
                            var httpMethodAttribute = method.GetCustomAttributes()
                                .FirstOrDefault(a => a is HttpMethodAttribute) as HttpMethodAttribute;

                            if (httpMethodAttribute != null)
                            {
                                var template = $"/api/{controllerName}/{method.Name}";
                                var methodInfo = method;
                                var httpMethod = httpMethodAttribute.HttpMethods.First();

                                Console.WriteLine($"注册路由: {httpMethod} {template} -> {controllerType.Name}.{method.Name}");
                                Console.WriteLine($"路由注册时的_endpointRouteBuilder实例: {_endpointRouteBuilder.GetType().FullName}");

                                _endpointRouteBuilder.MapMethods(
                                    template,
                                    new[] { httpMethod },
                                    async context =>
                                    {
                                        var controller = ActivatorUtilities.CreateInstance(context.RequestServices, controllerType) as ControllerBase;
                                        if (controller != null)
                                        {
                                            controller.ControllerContext = new ControllerContext
                                            {
                                                HttpContext = context
                                            };
                                            var result = methodInfo.Invoke(controller, new object[] { });
                                            if (result is Task<IActionResult> taskResult)
                                            {
                                                var actionResult = await taskResult;
                                                await actionResult.ExecuteResultAsync(new ActionContext
                                                {
                                                    HttpContext = context,
                                                    RouteData = context.GetRouteData(),
                                                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                });
                                            }
                                            else if (result is IActionResult actionResult)
                                            {
                                                await actionResult.ExecuteResultAsync(new ActionContext
                                                {
                                                    HttpContext = context,
                                                    RouteData = context.GetRouteData(),
                                                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                });
                                            }
                                            else if (result != null)
                                            {
                                                await new JsonResult(result).ExecuteResultAsync(new ActionContext
                                                {
                                                    HttpContext = context,
                                                    RouteData = context.GetRouteData(),
                                                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                });
                                            }
                                        }
                                    });
                            }
                        }
                    }
                }
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    if (plugin != null)
                    {
                        if (_loadedPlugins.TryAdd(plugin.Id, plugin))
                        {
                            // 注册插件服务
                            _services.AddSingleton(pluginType, plugin);
                            plugin.Initialize();
                            plugin.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录加载插件时的错误
                Console.WriteLine($"Error loading plugin {pluginPath}: {ex.Message}");
            }
        }

        public void UnloadPlugin(string pluginPath)
        {
            var pluginId = Path.GetFileNameWithoutExtension(pluginPath);
            if (_loadedPlugins.TryRemove(pluginId, out var plugin))
            {
                try
                {
                    plugin.Stop();
                    // 从程序集缓存中移除
                    _loadedAssemblies.TryRemove(pluginId, out _);

                    // 尝试进行垃圾回收以清理未使用的资源
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error unloading plugin {pluginId}: {ex.Message}");
                }
            }
        }

        public void LoadAllPlugins()
        {
            var pluginFiles = Directory.GetFiles(_pluginPath, "*.dll", SearchOption.AllDirectories);
            foreach (var file in pluginFiles)
            {
                LoadPlugin(file);
            }
        }

        public void SetEndpointRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
        {
            Console.WriteLine("开始设置EndpointRouteBuilder...");
            _endpointRouteBuilder = endpointRouteBuilder;
            _app = endpointRouteBuilder;
            Console.WriteLine($"EndpointRouteBuilder设置完成，类型为: {endpointRouteBuilder.GetType().FullName}");

            // 重新注册已加载插件的路由
            Console.WriteLine("开始重新注册已加载插件的路由...");
            foreach (var assembly in _loadedAssemblies.Values)
            {
                var controllerTypes = assembly.GetTypes()
                    .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract && !t.IsInterface);

                foreach (var controllerType in controllerTypes)
                {
                    var controllerName = controllerType.Name.Replace("Controller", "");
                    var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.DeclaringType == controllerType);
                    foreach (var method in methods)
                    {
                        var httpMethodAttribute = method.GetCustomAttributes()
                            .FirstOrDefault(a => a is HttpMethodAttribute) as HttpMethodAttribute;
                        {
                            if (httpMethodAttribute != null)
                            {
                                string template = null;
                                if (!string.IsNullOrEmpty(httpMethodAttribute.Template))
                                {
                                    template = $"/api/{controllerName}/{httpMethodAttribute.Template}";
                                }
                                else
                                {
                                    template = $"/api/{controllerName}/{method.Name}";
                                }
                                var methodInfo = method;
                                var httpMethod = httpMethodAttribute.HttpMethods.First();
                                {
                                    Console.WriteLine($"注册路由: {httpMethod} {template} -> {controllerType.Name}.{method.Name}");
                                    {
                                        try
                                        {
                                            _endpointRouteBuilder.MapMethods(
                                                template,
                                                new[] { httpMethod },
                                                async context =>
                                                {
                                                    var controller = ActivatorUtilities.CreateInstance(context.RequestServices, controllerType) as ControllerBase;
                                                    if (controller != null)
                                                    {
                                                        controller.ControllerContext = new ControllerContext
                                                        {
                                                            HttpContext = context
                                                        };
                                                        var result = methodInfo.Invoke(controller, new object[] { });
                                                        if (result is Task<IActionResult> taskResult)
                                                        {
                                                            var actionResult = await taskResult;
                                                            await actionResult.ExecuteResultAsync(new ActionContext
                                                            {
                                                                HttpContext = context,
                                                                RouteData = context.GetRouteData(),
                                                                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                            });
                                                        }
                                                        else if (result is IActionResult actionResult)
                                                        {
                                                            await actionResult.ExecuteResultAsync(new ActionContext
                                                            {
                                                                HttpContext = context,
                                                                RouteData = context.GetRouteData(),
                                                                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                            });
                                                        }
                                                        else if (result != null)
                                                        {
                                                            await new JsonResult(result).ExecuteResultAsync(new ActionContext
                                                            {
                                                                HttpContext = context,
                                                                RouteData = context.GetRouteData(),
                                                                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                                            });
                                                        }
                                                    }
                                                });
                                            Console.WriteLine($"路由 {template} 注册成功");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"注册路由 {template} 失败: {ex.Message}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void RegisterControllerRoutes(Type controllerType)
        {
            var controllerName = controllerType.Name.Replace("Controller", "");
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == controllerType);

            foreach (var method in methods)
            {
                var httpMethodAttribute = method.GetCustomAttributes()
                    .FirstOrDefault(a => a is HttpMethodAttribute) as HttpMethodAttribute;

                if (httpMethodAttribute != null)
                {
                    var template = $"/api/{controllerName}/{method.Name}";
                    var methodInfo = method;
                    var httpMethod = httpMethodAttribute.HttpMethods.First();

                    Console.WriteLine($"注册路由: {httpMethod} {template} -> {controllerType.Name}.{method.Name}");

                    try
                    {
                        _endpointRouteBuilder.MapMethods(
                            template,
                            new[] { httpMethod },
                            async context =>
                            {
                                var controller = ActivatorUtilities.CreateInstance(context.RequestServices, controllerType) as ControllerBase;
                                if (controller != null)
                                {
                                    controller.ControllerContext = new ControllerContext
                                    {
                                        HttpContext = context
                                    };
                                    var result = methodInfo.Invoke(controller, new object[] { });
                                    if (result is Task<IActionResult> taskResult)
                                    {
                                        var actionResult = await taskResult;
                                        await actionResult.ExecuteResultAsync(new ActionContext
                                        {
                                            HttpContext = context,
                                            RouteData = context.GetRouteData(),
                                            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                        });
                                    }
                                    else if (result is IActionResult actionResult)
                                    {
                                        await actionResult.ExecuteResultAsync(new ActionContext
                                        {
                                            HttpContext = context,
                                            RouteData = context.GetRouteData(),
                                            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                        });
                                    }
                                    else if (result != null)
                                    {
                                        await new JsonResult(result).ExecuteResultAsync(new ActionContext
                                        {
                                            HttpContext = context,
                                            RouteData = context.GetRouteData(),
                                            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                                        });
                                    }
                                }
                            });
                        Console.WriteLine($"路由 {template} 注册成功");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"注册路由 {template} 失败: {ex.Message}");
                    }
                }
            }
        }
        public IPlugin GetPlugin(string pluginId)
        {
            _loadedPlugins.TryGetValue(pluginId, out var plugin);
            return plugin;
        }

        public IEnumerable<IPlugin> GetAllPlugins()
        {
            return _loadedPlugins.Values;
        }
    }
}