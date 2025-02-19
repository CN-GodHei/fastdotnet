using System;
using System.Linq;
using System.Reflection;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fastdotnet.Core.Attributes;

namespace Fastdotnet.Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            // 获取所有程序集
            var assemblies = new List<Assembly>();

            // 添加当前应用程序域中的Fastdotnet程序集
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName?.StartsWith("Fastdotnet.") == true));

            // 扫描插件目录
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugin");
            if (Directory.Exists(pluginPath))
            {
                foreach (var pluginDir in Directory.GetDirectories(pluginPath))
                {
                    var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
                    foreach (var dllFile in dllFiles)
                    {
                        try
                        {
                            var assembly = Assembly.LoadFrom(dllFile);
                            assemblies.Add(assembly);
                        }
                        catch (Exception ex)
                        {
                            // 记录加载失败的插件
                            Console.WriteLine($"Failed to load plugin: {dllFile}, Error: {ex.Message}");
                        }
                    }
                }
            }

            assemblies = assemblies.ToList();

            // 扫描并注册带有AutoInject特性的类
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<AutoInjectAttribute>() != null);

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<AutoInjectAttribute>();
                    var interfaces = type.GetInterfaces();

                    if (interfaces.Any())
                    {
                        // 如果类实现了接口，注册接口和实现
                        foreach (var interfaceType in interfaces)
                        {
                            switch (attribute.Lifetime)
                            {
                                case ServiceLifetime.Singleton:
                                    services.AddSingleton(interfaceType, type);
                                    break;
                                case ServiceLifetime.Scoped:
                                    services.AddScoped(interfaceType, type);
                                    break;
                                case ServiceLifetime.Transient:
                                    services.AddTransient(interfaceType, type);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // 如果类没有实现接口，直接注册类型
                        switch (attribute.Lifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(type);
                                break;
                            case ServiceLifetime.Scoped:
                                services.AddScoped(type);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(type);
                                break;
                        }
                    }
                }
            }

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 应用服务的注册已经在AddInfrastructureServices中通过自动扫描完成
            return services;
        }
    }
    }