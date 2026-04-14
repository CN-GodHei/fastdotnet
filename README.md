# Fastdotnet 框架

## 项目简介
Fastdotnet是一个基于.NET Core的模块化开发框架，采用插件化架构设计，支持功能模块的热插拔，具有高度的可扩展性和灵活性。框架基于.NET 10.0构建，适用于构建企业级应用程序。框架底座采用分层架构设计，插件开发可以自由选择架构模式（如DDD、Clean Architecture等）。

## 项目结构
```
├── backend/
│   ├── Fastdotnet.Core/           # 核心领域层
│   │   ├── Attributes/            # 特性定义
│   │   ├── Constants/             # 常量定义
│   │   ├── Controllers/           # 控制器基类
│   │   ├── Dtos/                  # 数据传输对象
│   │   ├── Entities/              # 实体模型
│   │   ├── Enum/                  # 枚举定义
│   │   ├── Exceptions/            # 异常处理
│   │   ├── Extensions/            # 扩展方法
│   │   ├── Hubs/                  # SignalR Hub
│   │   ├── IService/              # 服务接口
│   │   ├── Middleware/            # 中间件
│   │   ├── Plugin/                # 插件系统核心接口
│   │   ├── Service/               # 服务实现
│   │   ├── Settings/              # 配置设置
│   │   ├── Utils/                 # 工具类
│   │   └── Version/               # 版本信息
│   ├── Fastdotnet.Orm/            # ORM数据访问层
│   ├── Fastdotnet.Plugin.Contracts/ # 插件契约层
│   ├── Fastdotnet.Plugin.Shared/  # 插件共享组件
│   ├── Fastdotnet.Service/        # 应用服务层
│   │   ├── IService/              # 服务接口
│   │   └── Service/               # 服务实现
│   ├── Fastdotnet.WebApi/         # Web API层
│   │   ├── Controllers/           # API控制器
│   │   ├── Middleware/            # 中间件
│   │   └── plugins/               # 插件部署目录
│   └── Plugins/                   # 插件源码目录
│       ├── PluginA/               # 示例插件
│       └── README.md              # 插件系统说明
├── Web/                           # 前端项目
│   ├── fastdotnet-admin/          # 管理后台前端
│   ├── fastdotnet-app/            # 应用前端
│   └── plugin-a-admin/            # 插件A管理前端
```

## 架构说明

### 分层架构
1. **Fastdotnet.Core**
   - 框架核心基础层，提供通用的基础设施与底层支持
   - 定义系统级的常量、枚举、异常处理及工具类
   - 承载插件系统的核心契约（Contracts）与扩展点接口
   - 保持零外部依赖，确保整个框架的稳定性和纯净度

2. **Fastdotnet.Orm**
   - 数据访问基础设施层，基于 SqlSugar 封装
   - 提供通用的仓储模式（Repository）与数据库连接管理
   - 支持多数据库适配（SQLite、MySQL、PostgreSQL、达梦等）

3. **Fastdotnet.Plugin.Contracts**
   - 插件通信契约层，定义主程序与插件交互的标准接口
   - 包含插件生命周期（IPlugin）、权限扩展及存储上下文等核心定义
   - 作为插件开发的唯一依赖项，确保插件与底座的松耦合

4. **Fastdotnet.Plugin.Shared**
   - 插件运行时共享组件，提供 AOT 适配器与通用工具
   - 协助插件在独立上下文中安全地访问系统资源

5. **Fastdotnet.Service**
   - 业务逻辑实现层，承载底座的核心功能模块
   - 封装用户管理、权限控制、系统配置等通用业务服务
   - 协调 ORM 数据操作与上层 API 请求

6. **Fastdotnet.WebApi**
   - 宿主启动层，负责应用程序的组装与运行
   - 集成认证授权、全局中间件、Swagger 及插件加载引擎
   - 提供统一的 HTTP 入口点与动态路由分发能力

### 依赖关系
- Fastdotnet.Core: 不依赖其他项目
- Fastdotnet.Orm: 依赖 Core
- Fastdotnet.Plugin.Contracts: 依赖 Core
- Fastdotnet.Plugin.Shared: 依赖 Contracts
- Fastdotnet.Service: 依赖 Core, Orm
- Fastdotnet.WebApi: 依赖 Core, Orm, Plugin.Contracts, Plugin.Shared, Service

## 插件系统

### 核心优势
Fastdotnet 的插件化架构专为高扩展性、动态性和企业级 SaaS 场景设计，具备以下显著优势：

1. **强隔离性 (Strong Isolation)**
   - 采用 `AssemblyLoadContext` (ALC) 技术，为每个插件创建独立的运行上下文。
   - 彻底解决 DLL 版本冲突问题，不同插件可以使用同一库的不同版本而互不干扰。
   - 插件崩溃不会影响主程序的稳定性，实现了真正的故障隔离。

2. **动态热插拔 (Dynamic Hot-Swapping)**
   - 支持在运行时动态加载、启动、停止和卸载插件，无需重启服务器。
   - 配合文件系统监控（FileSystemWatcher），可实现插件文件的自动检测与热更新。
   - 插件状态实时更新，支持优雅停机（Graceful Shutdown）和资源清理。

3. **微前端深度集成 (Micro-Frontends Integration)**
   - 后端插件与前端 qiankun 微应用无缝对接。
   - 主前端根据后端返回的元数据，动态加载远程插件的 Vue/React 子应用。
   - 实现了前后端功能模块的完全解耦与同步交付。

4. **灵活的网关与路由 (Flexible Gateway & Routing)**
   - **插件分支网关**：允许插件注册私有的请求处理管道，实现类似“子应用”的独立逻辑。
   - **反向代理逃生舱**：支持插件自带微型 Web 服务器（如 Kestrel），主程序通过 YARP 进行透明转发，兼容异构技术栈。
   - **静态资源映射**：自动处理插件 `wwwroot` 下的文件服务，完美支持 SPA 应用的 History 模式。

5. **容器化依赖注入 (Scoped DI Container)**
   - 基于 Autofac 为每个插件分配独立的 `LifetimeScope`。
   - 插件内部的服务注册高度自治，同时可通过接口与主程序或其他插件安全交互。
   - 避免了全局容器的污染，提升了系统的可维护性。

6. **事件驱动通信 (Event-Driven Communication)**
   - 内置强大的 `IEventBus`，支持跨插件的发布/订阅模式。
   - 插件间通过事件进行低耦合联动，便于构建复杂的业务流转体系。

### 技术栈
插件系统基于以下核心技术构建：

- .NET Core 动态加载机制（AssemblyLoadContext）
- 依赖注入（Microsoft.Extensions.DependencyInjection和Autofac）
- ASP.NET Core MVC（用于插件控制器的注册和路由）
- 文件系统监控（FileSystemWatcher，用于热加载）
- 反射（用于类型发现和服务注册）

### 插件目录结构
```
backend/Plugins/
├── PluginA/                # 插件A项目
│   ├── Controllers/        # 控制器
│   ├── IService/           # 服务接口
│   ├── Services/           # 服务实现
│   ├── PluginA.cs          # 插件主类
│   ├── PluginA.csproj      # 项目文件
│   └── plugin.json         # 插件配置文件
└── README.md               # 插件系统说明
```

### 插件接口
每个插件必须实现`IPlugin`接口，该接口定义了插件的基本生命周期方法：

```csharp
public interface IPlugin
{
    string PluginId { get; }     // 插件唯一标识符
    string Name { get; }         // 插件名称
    string Version { get; }      // 插件版本
    PluginLifecycleState LifecycleState { get; } // 插件生命周期状态
    
    Task InitializeAsync(IServiceProvider serviceProvider); // 插件初始化
    Task StartAsync();          // 插件启动
    Task StopAsync();           // 插件停止
    Task UnloadAsync(IServiceProvider serviceProvider); // 插件卸载前清理
    void ConfigureServices(ContainerBuilder builder); // 配置插件服务
}
```

### 插件配置文件
每个插件需要包含一个`plugin.json`配置文件，用于描述插件的基本信息：

```json
{
  "id": "PluginA",
  "name": "Fastdotnet Demo Plugin",
  "description": "演示插件，用于展示插件系统的基本功能",
  "version": "1.0.0",
  "enabled": true,
  "author": "Fastdotnet Team",
  "dependencies": [],
  "tags": ["demo", "example"]
}
```

### 插件开发规范
1. 插件项目命名规范：建议使用有意义的名称
2. 每个插件需要实现`IPlugin`接口或继承`PluginBase`基类（推荐）
   - `PluginBase`提供了默认的生命周期状态管理，简化插件开发
   - 继承`PluginBase`后只需重写`OnInitializeAsync`、`OnStartAsync`等钩子方法
3. 插件必须在.csproj文件中配置唯一的`<PluginId>`属性
4. 插件之间应保持独立，避免相互依赖
5. 插件可以包含自己的控制器、服务和模型
6. 插件控制器应继承自`GenericDtoControllerBase`或`AppGenericDtoControllerBase`基类

### 热插拔机制
- 插件管理器负责插件的加载、卸载和生命周期管理
- 支持运行时动态加载和卸载插件
- 文件系统监控自动检测插件变更
- 插件状态实时更新，支持优雅停止

## 缓存系统

### 技术栈
缓存系统基于Microsoft HybridCache构建，具有以下特点：

- **双层缓存架构**：本地内存缓存 + 分布式缓存（Redis/MemoryCache）
- **高性能**：本地缓存提供最快的访问速度
- **高可用**：分布式缓存确保多实例间的数据一致性
- **灵活配置**：支持MemoryCache和Redis两种后端存储

### 核心组件
1. **IHybridCacheService** - 核心缓存服务接口
2. **CacheResultAttribute** - 控制器方法缓存特性
3. **CacheTagAttribute** - 缓存标签特性

### 使用方式

#### 控制器层使用
```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [CacheResult(ExpirationSeconds = 600)]
    [CacheTag("user")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        // 实际业务逻辑
    }
}
```

#### 服务层使用
```csharp
public class UserService : IUserService
{
    private readonly IHybridCacheService _cacheService;
    
    public async Task<UserDto> GetUserAsync(int userId)
    {
        var key = $"user:{userId}";
        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            // 从数据库获取数据
        });
    }
}
```

### 安全性说明
缓存机制与JWT认证完全兼容，认证检查始终在缓存检查之前执行，确保安全性。

更多详细信息请参阅 [缓存使用指南](docs/HybridCache使用指南.md)

## 设计原则
1. **依赖倒置原则(DIP)**
   - 高层模块不应依赖低层模块，两者都应依赖抽象
   - 抽象不应依赖细节，细节应依赖抽象

2. **单一职责原则(SRP)**
   - 每个类都应该有一个单一的职责
   - 每个模块的功能要高内聚，低耦合

3. **开放封闭原则(OCP)**
   - 对扩展开放，对修改关闭
   - 通过插件系统支持功能扩展

4. **接口隔离原则(ISP)**
   - 客户端不应依赖它不需要的接口
   - 接口应该小而精确

## 快速开始

### 环境要求
- .NET 10.0 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS

### 构建项目
```bash
# 克隆仓库
git clone https://github.com/yourusername/fastdotnet.git
cd fastdotnet/backend

# 构建解决方案
dotnet build

# 运行WebApi项目
cd Fastdotnet.WebApi
dotnet run
```

### 开发插件
1. 在`backend/Plugins`目录下创建新的插件项目
2. 实现`IPlugin`接口或继承`PluginBase`基类（推荐），并在.csproj中配置`<PluginId>`
3. 构建插件并将输出复制到`backend/Fastdotnet.WebApi/plugins`目录

#### 示例：继承 PluginBase 基类
```csharp
public class MyPlugin : PluginBase
{
    public override string PluginId => "your-plugin-id";
    public override string Name => "My Plugin";
    public override string Version => "1.0.0";
    
    protected override Task OnInitializeAsync(IServiceProvider serviceProvider)
    {
        // 初始化逻辑
        return Task.CompletedTask;
    }
    
    protected override Task OnStartAsync()
    {
        // 启动逻辑
        return Task.CompletedTask;
    }
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 注册服务
    }
}
```

### 前端开发
项目包含多个前端应用：
- `Web/fastdotnet-admin/`: 管理后台前端应用
- `Web/fastdotnet-app/`: 用户应用前端
- `Web/plugin-a-admin/`: 插件A的管理前端

## 贡献指南
欢迎贡献代码、报告问题或提出改进建议。请遵循以下步骤：

1. Fork 项目
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建 Pull Request

## 联系方式
- QQ交流群: 779454817

## 许可证
本项目采用 MIT 许可证 - 详情请参阅 LICENSE 文件