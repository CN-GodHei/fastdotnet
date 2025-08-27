# Fastdotnet 框架

## 项目简介
Fastdotnet是一个基于.NET Core的模块化开发框架，采用插件化架构设计，支持功能模块的热插拔，具有高度的可扩展性和灵活性。框架基于.NET 8.0构建，遵循领域驱动设计(DDD)原则，适用于构建企业级应用程序。

## 项目结构
```
├── src/
│   ├── Fastdotnet.Core/           # 核心领域层
│   │   ├── Attributes/            # 特性定义
│   │   ├── Constants/             # 常量定义
│   │   ├── Models/                # 领域模型
│   │   ├── Plugin/                # 插件系统核心接口
│   │   ├── Utils/                 # 核心工具类
│   │   └── Version/               # 版本信息
│   ├── Fastdotnet.Plugin.Core/    # 插件核心层
│   │   └── Infrastructure/        # 插件基础设施
│   ├── Fastdotnet.Service/        # 应用服务层
│   │   ├── IService/              # 服务接口
│   │   └── Service/               # 服务实现
│   ├── Fastdotnet.WebApi/         # Web API层
│   │   ├── Controllers/           # API控制器
│   │   ├── Middleware/            # 中间件
│   │   └── plugins/               # 插件部署目录
│   └── Plugins/                   # 插件源码目录
│       ├── Fastdotnet.Demo/       # 示例插件
│       ├── MyPlugin/              # 自定义插件
│       ├── PluginA/               # 插件A示例
│       └── README.md              # 插件系统说明
```

## 架构说明

### 分层架构
1. **Fastdotnet.Core**
   - 核心领域层，包含领域模型和核心业务逻辑
   - 定义领域实体、值对象和领域服务
   - 提供插件系统的核心接口定义
   - 不依赖其他项目，是整个框架的基础

2. **Fastdotnet.Plugin.Core**
   - 插件核心层，提供插件系统的基础实现
   - 依赖于Fastdotnet.Core
   - 使用Autofac进行依赖注入管理
   - 集成ASP.NET Core MVC核心功能

3. **Fastdotnet.Service**
   - 应用服务层，实现业务用例
   - 协调领域对象和基础设施服务
   - 处理业务规则和数据转换
   - 提供面向应用的服务接口

4. **Fastdotnet.WebApi**
   - 表现层，提供HTTP API接口
   - 处理请求路由和响应格式化
   - 集成认证和其他中间件
   - 作为应用程序的入口点

### 依赖关系
- Fastdotnet.Core: 不依赖其他项目
- Fastdotnet.Plugin.Core: 依赖 Core
- Fastdotnet.Service: 依赖 Core
- Fastdotnet.WebApi: 依赖 Core, Plugin.Core, Service

## 插件系统

### 技术栈
插件系统基于以下核心技术构建：

- .NET Core 动态加载机制（AssemblyLoadContext）
- 依赖注入（Microsoft.Extensions.DependencyInjection和Autofac）
- ASP.NET Core MVC（用于插件控制器的注册和路由）
- 文件系统监控（FileSystemWatcher，用于热加载）
- 反射（用于类型发现和服务注册）

### 插件目录结构
```
src/Plugins/
├── PluginA/                # 插件A项目
│   ├── Controllers/        # 控制器
│   ├── IService/           # 服务接口
│   ├── Services/           # 服务实现
│   ├── PluginA.cs          # 插件主类
│   ├── PluginA.csproj      # 项目文件
│   └── plugin.json         # 插件配置文件
├── MyPlugin/               # 自定义插件
├── Fastdotnet.Demo/        # 示例插件
└── README.md               # 插件系统说明
```

### 插件接口
每个插件必须实现`IPlugin`接口，该接口定义了插件的基本生命周期方法：

```csharp
public interface IPlugin
{
    string Name { get; }        // 插件名称
    string Version { get; }      // 插件版本
    Task InitializeAsync();      // 插件初始化
    Task StartAsync();          // 插件启动
    Task StopAsync();           // 插件停止
    Task UnloadAsync();         // 插件卸载前清理
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
1. 插件项目命名规范：建议使用`Fastdotnet.{PluginName}`或有意义的名称
2. 每个插件项目需要实现`IPlugin`接口
3. 插件必须包含`plugin.json`配置文件
4. 插件之间应保持独立，避免相互依赖
5. 插件可以包含自己的控制器、服务和模型

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
- .NET 8.0 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS

### 构建项目
```bash
# 克隆仓库
git clone https://github.com/yourusername/fastdotnet.git
cd fastdotnet

# 构建解决方案
dotnet build

# 运行WebApi项目
cd src/Fastdotnet.WebApi
dotnet run
```

### 开发插件
1. 在`src/Plugins`目录下创建新的插件项目
2. 实现`IPlugin`接口
3. 创建`plugin.json`配置文件
4. 构建插件并将输出复制到`src/Fastdotnet.WebApi/plugins`目录

## 贡献指南
欢迎贡献代码、报告问题或提出改进建议。请遵循以下步骤：

1. Fork 项目
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建 Pull Request

## 许可证
本项目采用 MIT 许可证 - 详情请参阅 LICENSE 文件