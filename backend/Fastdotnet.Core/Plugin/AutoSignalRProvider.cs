using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Plugin
{
    /// <summary>
    /// 基于特性的自动 SignalR Provider 基类。
    /// 继承此类后，只需在方法上打上 [SignalRMethod] 标签即可自动注册，
    /// 并且支持强类型的参数自动绑定。
    /// </summary>
    public abstract class AutoSignalRProvider : IPluginSignalRProvider
    {
        /// <summary>
        /// 自动从上下文获取插件 ID，如果有特殊需求可被子类重写
        /// </summary>
        public virtual string PluginId => PluginContext.GetCurrentPluginInfo()?.id;

        public virtual void RegisterMethods(ISignalRMethodRegistry registry)
        {
            // 扫描当前类中所有带有 [SignalRMethod] 特性的公共实例方法
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttribute<SignalRMethodAttribute>() != null);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SignalRMethodAttribute>();
                // 如果没有指定名称，默认使用函数名
                var methodName = string.IsNullOrEmpty(attr.MethodName) ? method.Name : attr.MethodName;

                // 注册到字典中
                registry.Register(methodName, async (args, context) =>
                {
                    return await InvokeMethodAsync(method, args, context);
                });
                
                Console.WriteLine($"[{PluginId}] 自动发现并注册 SignalR 强类型方法: {methodName}");
            }
        }

        private async Task<object?> InvokeMethodAsync(MethodInfo method, object[] args, HubCallerContext context)
        {
            var parameters = method.GetParameters();
            var invokeArgs = new object[parameters.Length];
            int argIndex = 0;

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                
                // 处理内置类型参数
                if (p.ParameterType == typeof(HubCallerContext))
                {
                    invokeArgs[i] = context;
                }
                else if (p.ParameterType == typeof(object[]))
                {
                    invokeArgs[i] = args;
                }
                // 处理从前端传过来的业务参数
                else
                {
                    if (args != null && argIndex < args.Length)
                    {
                        var rawValue = args[argIndex];
                        
                        // 如果传入的是 JsonElement，则利用 System.Text.Json 尝试将其反序列化为目标实体类
                        if (rawValue is System.Text.Json.JsonElement element)
                        {
                            invokeArgs[i] = System.Text.Json.JsonSerializer.Deserialize(
                                element.GetRawText(), 
                                p.ParameterType, 
                                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        else if (rawValue != null)
                        {
                            try
                            {
                                // 对于数字/字符串等基本类型，尝试安全转换
                                invokeArgs[i] = Convert.ChangeType(rawValue, p.ParameterType);
                            }
                            catch
                            {
                                invokeArgs[i] = rawValue;
                            }
                        }
                        
                        argIndex++;
                    }
                    else
                    {
                        // 前端少传参数时的默认值补齐
                        invokeArgs[i] = p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null;
                    }
                }
            }

            // 执行目标方法
            var result = method.Invoke(this, invokeArgs);

            // 如果方法返回 Task 或 Task<T>，则 await 它并提取结果
            if (result is Task task)
            {
                await task;
                var taskType = task.GetType();
                if (taskType.IsGenericType)
                {
                    return taskType.GetProperty("Result")?.GetValue(task);
                }
                return null;
            }

            return result;
        }
    }
}
