# Fastdotnet 框架

## 项目简介
Fastdotnet是一个基于.NET Core的模块化开发框架，采用插件化架构设计，支持功能模块的热插拔，具有高度的可扩展性和灵活性。

## 项目结构
```
├── src/
│   ├── Fastdotnet.Core/           # 核心领域层
│   │   ├── Models/                # 领域模型
│   │   └── Plugin/                # 插件系统核心接口
│   ├── Fastdotnet.Infrastructure/ # 基础设施层
│   │   ├── Common/                # 通用功能
│   │   └── Utils/                 # 工具类
│   ├── Fastdotnet.Service/        # 应用服务层
│   │   ├── IService/             # 服务接口
│   │   └── Service/              # 服务实现
│   ├── Fastdotnet.Plugin/         # 插件实现层
│   │   └── Implementations/      # 插件实现
│   └── Fastdotnet.WebApi/         # Web API层
│       ├── Controllers/          # API控制器
│       └── Middleware/           # 中间件
```

## 架构说明

### 分层架构
1. **Fastdotnet.Core**
   - 核心领域层，包含领域模型和核心业务逻辑
   - 定义领域实体、值对象和领域服务
   - 提供插件系统的核心接口定义

2. **Fastdotnet.Infrastructure**
   - 基础设施层，提供技术实现和支持
   - 包含通用工具、辅助类、扩展方法
   - 实现跨切面关注点（日志、缓存等）

3. **Fastdotnet.Service**
   - 应用服务层，实现业务用例
   - 协调领域对象和基础设施服务
   - 处理业务规则和数据转换

4. **Fastdotnet.Plugin**
   - 插件实现层，提供可插拔的功能扩展
   - 实现Core层定义的插件接口
   - 支持功能的动态扩展

5. **Fastdotnet.WebApi**
   - 表现层，提供HTTP API接口
   - 处理请求路由和响应格式化
   - 集成认证和其他中间件

### 依赖关系
- Fastdotnet.Core: 不依赖其他项目
- Fastdotnet.Infrastructure: 依赖 Core
- Fastdotnet.Service: 依赖 Core, Infrastructure
- Fastdotnet.Plugin: 依赖 Core
- Fastdotnet.WebApi: 依赖 Core, Infrastructure, Service, Plugin

## 插件系统

### 插件目录结构
```
Plugin/
├── Fastdotnet.Plugin/    # 基础插件项目
├── Fastdotnet.A/         # A插件项目
├── Fastdotnet.B/         # B插件项目
└── Fastdotnet.Test/      # 测试插件项目
```

### 插件开发规范
1. 插件项目命名规范：Fastdotnet.{PluginName}
2. 每个插件项目需要实现IPlugin接口
3. 插件的编译输出路径配置为Plugin目录下对应的子文件夹
4. 插件之间应保持独立，避免相互依赖

### 热插拔机制
- 插件管理器(PluginManager)负责插件的加载、卸载和生命周期管理
- 支持运行时动态加载和卸载插件
- 文件系统监控自动检测插件变更
- 插件状态实时更新，支持优雅停止

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

## 版本管理
- 遵循语义化版本 2.0.0 (SemVer)
- 版本格式：主版本号.次版本号.修订号
- 主版本号：做了不兼容的API修改
- 次版本号：做了向下兼容的功能性新增
- 修订号：做了向下兼容的问题修正