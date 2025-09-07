# Fastdotnet 项目架构说明

## 项目结构

```
├── Fastdotnet.Core/           # 核心领域层
│   ├── Models/                # 领域模型
│   └── Plugin/                # 核心插件接口
├── Fastdotnet.Infrastructure/ # 基础设施层
│   ├── Common/                # 通用功能和基础设施
│   │   ├── Constants/         # 常量定义
│   │   ├── Enums/            # 枚举定义
│   │   └── Helpers/          # 辅助类
│   └── Utils/                # 工具类
│       ├── Extensions/       # 扩展方法
│       └── Helpers/          # 辅助工具
├── Fastdotnet.Service/       # 应用服务层
│   ├── IService/             # 服务接口
│   │   ├── Admin/            # 管理员相关服务接口
│   │   └── User/             # 用户相关服务接口
│   └── Service/              # 服务实现
│       ├── Admin/            # 管理员相关服务实现
│       └── User/             # 用户相关服务实现
├── Fastdotnet.Plugin/        # 插件实现层
│   └── Implementations/      # 具体插件实现
└── Fastdotnet.WebApi/        # Web API层
    ├── Controllers/          # API控制器
    └── Middleware/           # 中间件
        ├── Authentication/   # 身份认证中间件
        └── Logging/          # 日志中间件
```

## 项目分层说明

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

## 依赖关系

- Fastdotnet.Core: 不依赖其他项目
- Fastdotnet.Infrastructure: 依赖 Core
- Fastdotnet.Service: 依赖 Core, Infrastructure
- Fastdotnet.Plugin: 依赖 Core
- Fastdotnet.WebApi: 依赖 Core, Infrastructure, Service, Plugin

## 设计原则

- 遵循依赖倒置原则，通过接口进行依赖
- 保持层级之间的清晰边界
- 避免跨层级直接访问
- 合理使用依赖注入
- 插件化设计提供良好的扩展性
- 领域驱动设计思想指导架构设计