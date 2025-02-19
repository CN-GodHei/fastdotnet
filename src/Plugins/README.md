# Fastdotnet 插件系统

## 技术栈

插件系统基于以下核心技术构建：

- .NET Core 动态加载机制（AssemblyLoadContext）
- 依赖注入（Microsoft.Extensions.DependencyInjection）
- ASP.NET Core MVC（用于插件控制器的注册和路由）
- 文件系统监控（FileSystemWatcher，用于热加载）
- 反射（用于类型发现和服务注册）

## 调用流程

### 1. 插件加载

1. 系统启动时，PluginManager 初始化并监控插件目录
2. 检测到新的插件DLL时，通过 AssemblyLoadContext 动态加载程序集
3. 扫描程序集中的类型，识别：
   - 实现 IPlugin 接口的插件类
   - 带有 PluginService 特性的服务
   - 控制器类

### 2. 服务注册

1. 插件服务注册到 DI 容器
2. 控制器注册到 MVC 系统
3. 配置控制器路由（默认路由格式：/api/{controller}/{action}）

### 3. 插件生命周期

1. Initialize：插件初始化
2. Start：插件启动
3. Stop：插件停止（卸载时调用）

## 开发规范

### 1. 项目结构

```
src/Plugins/
  YourPlugin/
    Controllers/      # 控制器类
    Services/         # 业务服务
    Models/           # 数据模型
    YourPlugin.cs     # 插件主类
    YourPlugin.csproj # 项目文件
```

### 2. 插件实现要求

1. 插件主类必须实现 IPlugin 接口：

```csharp
public interface IPlugin
{
    string Id { get; }    // 插件唯一标识
    void Initialize();    // 初始化方法
    void Start();        // 启动方法
    void Stop();         // 停止方法
}
```

2. 服务类需要添加 PluginService 特性：

```csharp
[PluginService]
public interface IYourService
{
    // 服务接口定义
}

public class YourService : IYourService
{
    // 服务实现
}
```

3. 控制器命名必须以 "Controller" 结尾：

```csharp
public class YourController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> YourAction()
    {
        // 控制器方法实现
    }
}
```

### 3. 最佳实践

1. 插件ID命名规范：使用小写字母和连字符，如："your-plugin"
2. 控制器路由规范：使用 RESTful 风格的 API 设计
3. 依赖注入：优先使用构造函数注入
4. 异常处理：实现适当的错误处理和日志记录
5. 资源释放：在 Stop 方法中正确释放资源

### 4. 示例插件

参考 `src/Plugins/Fastdotnet.Demo` 项目，该项目展示了一个完整的插件实现示例。

## 注意事项

1. 插件DLL应放置在指定的插件目录中
2. 确保插件项目引用正确的框架版本
3. 避免在插件中使用全局状态
4. 正确处理插件的依赖关系
5. 遵循异步编程最佳实践

## 调试提示

1. 插件加载时会输出详细的日志信息
2. 可以通过 PluginManager 的 GetPlugin 方法获取插件实例
3. 使用 GetAllPlugins 方法查看所有已加载的插件