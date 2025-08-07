# Fastdotnet 动态插件系统开发文档

## 1. 系统概述

本系统是一个基于 .NET 和 Autofac 实现的动态插件化（热插拔）框架。其核心目标是允许开发人员创建独立的插件模块，这些模块可以在主应用程序运行时被加载、执行和卸载，而无需重新启动主程序。

**核心特性:** 

*   **模块化开发**: 将不同功能开发成独立的插件，降低系统耦合度。
*   **独立部署**: 插件可以独立编译、打包和部署。
*   **热插拔**: 在应用程序运行时，动态地加载新插件或卸载现有插件，实现真正的热更新。
*   **依赖注入集成**: 插件内的服务和控制器能够无缝集成到主程序的Autofac依赖注入容器中。

## 2. 核心组件与调用关系

系统的稳定运行依赖于以下几个关键组件的协同工作。

*   `PluginManager`
    *   **职责**: 插件系统的核心中枢。它负责管理所有插件的生命周期。
    *   **关键操作**: 
        1.  为每个插件创建独立的 `AssemblyLoadContext`，这是实现插件隔离和卸载的关键。
        2.  加载插件的程序集（Assembly），并将其注册为ASP.NET Core的 `ApplicationPart`，以便MVC框架能发现其中的控制器。
        3.  扫描并缓存插件中所有的服务类型（接口与实现类的映射关系）。
        4.  提供一个 `IsTypeFromPluginAssembly` 方法，用于判断一个给定的类型是否来源于任何一个已加载的插件。

*   `PluginLoadService`
    *   **职责**: 插件加载和卸载的具体执行者。
    *   **关键操作**: 
        1.  扫描 `plugins` 物理目录，发现可用的插件。
        2.  调用 `PluginManager` 来执行实际的程序集加载和类型扫描。
        3.  实例化插件主类（实现 `IPlugin` 接口的类），并调用其生命周期方法（如 `InitializeAsync`, `StartAsync` 等）。

*   `PluginRegistrationSource`
    *   **职责**: 连接插件与Autofac依赖注入容器的桥梁，是实现动态DI注册的**核心**。
    *   **关键操作**: 
        1.  它被注册为Autofac的 `IRegistrationSource` (动态注册源)。
        2.  当Autofac被请求解析一个它不认识的类型时（例如插件中的控制器或服务），它会询问 `PluginRegistrationSource`。
        3.  `PluginRegistrationSource` 会使用 `PluginManager` 来判断该类型是否属于某个插件。如果是，它会即时地（On-the-fly）为该类型创建一个有效的注册信息，并返回给Autofac，从而完成动态解析。

*   `DynamicControllerFeatureProvider`
    *   **职责**: 让ASP.NET Core MVC框架能够识别并路由到插件中的控制器。
    *   **关键操作**: 当 `PluginManager` 加载一个新的插件程序集后，此提供者会被触发，它会扫描该程序集，找到所有控制器类，并将其信息添加到MVC的特性列表中。

## 3. 系统工作流程图

下面的流程图清晰地展示了从应用启动到处理一个插件接口请求的全过程。

```mermaid
graph TD
    subgraph 应用启动阶段
        A[应用启动] --> B(配置Autofac & 注册核心服务);
        B --> C(手动创建唯一的<br>PluginManager和PluginSource实例);
        C --> D{注册核心实例到Autofac<br>RegisterInstance(pluginManager)<br>RegisterSource(pluginSource)};
        D --> E[应用启动后, 扫描/plugins目录];
    end

    subgraph 插件加载阶段 (PluginLoadService)
        E --> F[循环处理每个插件DLL];
        F --> G[调用 PluginManager.LoadPlugin];
        G --> H(创建独立的<br>AssemblyLoadContext);
        H --> I[加载插件Assembly];
        I --> J[注册控制器到MVC框架];
        J --> K[缓存插件内的服务类型];
        K --> F;
    end

    subgraph HTTP请求处理阶段
        L[HTTP请求到达<br>/api/plugin-a/test] --> M{MVC路由系统<br>匹配到PluginA.TestController};
        M --> N[向Autofac请求<br>TestController实例];
        N --> O{Autofac: 静态注册表中<br>是否存在TestController?};
        O -- 否 --> P[询问 PluginRegistrationSource];
        P --> Q{PluginSource: TestController<br>是否来自已知插件?};
        Q -- 是 --> R[动态创建<br>TestController的注册信息];
        R --> S[Autofac使用该信息<br>创建实例并注入其依赖];
        S --> T[返回Controller实例<br>处理请求];
        T --> U[返回HTTP响应];
    end
```

## 4. 如何开发一个新插件

请遵循以下步骤来创建一个与本系统兼容的新插件。

#### 第1步: 创建插件项目

创建一个新的 **.NET 类库（Class Library）** 项目。例如，命名为 `PluginA`。

#### 第2步: 添加项目引用

在你的插件项目中，添加对以下核心项目的引用，以获取必要的接口和基类：

*   `Fastdotnet.Core`
*   `Fastdotnet.Plugin.Core`

#### 第3步: 实现 `IPlugin` 接口

创建一个公开的类作为插件的入口点，并实现 `IPlugin` 接口。

```csharp
// PluginA/PluginA.cs
using Fastdotnet.Core.Plugin;
using System.Threading.Tasks;

namespace PluginA
{
    public class PluginA : IPlugin
    {
        public Task InitializeAsync()
        {
            // 插件初始化逻辑，例如数据库迁移
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            // 插件启动逻辑
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            // 插件停止逻辑
            return Task.CompletedTask;
        }

        public Task UnloadAsync()
        {
            // 插件卸载逻辑
            return Task.CompletedTask;
        }
    }
}
```

#### 第4步: 编写服务和控制器

像在普通ASP.NET Core项目中一样，编写你的接口、服务实现和控制器。

**服务示例:** 
```csharp
// IService/ITestService.cs
namespace PluginA.IService
{
    public interface ITestService
    {
        string GetTestMessage();
    }
}

// Services/TestService.cs
using PluginA.IService;

namespace PluginA.Services
{
    public class TestService : ITestService
    {
        public string GetTestMessage()
        {
            return "消息来自插件A的服务!";
        }
    }
}
```

**控制器示例:** 
```csharp
// Controllers/TestController.cs
using Microsoft.AspNetCore.Mvc;
using PluginA.IService;

namespace PluginA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = _testService.GetTestMessage() });
        }
    }
}
```

#### 第5步: 创建 `plugin.json` 配置文件

在插件项目的根目录下，创建一个名为 `plugin.json` 的文件。确保将其设置为 **"如果较新则复制"** 到输出目录。

```json
{
  "id": "PluginA",
  "name": "示例插件A",
  "version": "1.0.0",
  "author": "Your Name",
  "description": "这是一个用于演示的插件。",
  "enabled": true
}
```

#### 第6步: 配置生成路径

为了方便开发和调试，建议直接将插件的编译输出路径指向主程序的 `plugins` 目录。编辑你的插件项目文件 (`.csproj`)，添加以下 `OutputPath` 配置：

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <!-- 将输出路径指向主Web项目的plugins目录下的对应插件文件夹 -->
    <OutputPath>..\Fastdotnet.WebApi\bin\Debug\net8.0\plugins\PluginA</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  </PropertyGroup>

  <!-- 其他引用等 -->

</Project>
```
*注意：请根据你的实际项目结构调整相对路径 `..\Fastdotnet.WebApi\`。*

## 5. 如何部署插件

1.  编译你的插件项目。
2.  将编译好的插件文件夹（例如 `PluginA`，其中包含 `PluginA.dll`, `plugin.json` 等所有文件）**整体**复制到主应用程序运行目录下的 `plugins` 文件夹中。
3.  如果主程序正在运行，它应该会自动加载新插件（此功能需在`PluginLoadService`中实现文件监控来触发）。如果程序未运行，则下次启动时会自动加载。
