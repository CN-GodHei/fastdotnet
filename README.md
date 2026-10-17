<div align="center">

# Fastdotnet

**AI Agent 时代的企业级动态插件化底座**

基于 .NET 10 与 `AssemblyLoadContext`，实现真正的运行时热插拔：插件加载、卸载、热更新都无需重启宿主，后端插件与 qiankun 微前端同步交付。

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Vue 3](https://img.shields.io/badge/Vue-3-42B883?logo=vuedotjs&logoColor=white)](https://vuejs.org/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/CN-GodHei/fastdotnet/actions/workflows/backend-ci.yml/badge.svg)](https://github.com/CN-GodHei/fastdotnet/actions/workflows/backend-ci.yml)
[![Docs](https://img.shields.io/badge/docs-docs.fastdotnet.top-success)](https://docs.fastdotnet.top)
[![QQ Group](https://img.shields.io/badge/QQ%E7%BE%A4-779454817-1EBAFC)](https://qm.qq.com/q/779454817)

[快速开始](#-3-分钟跑起来) · [在线文档](https://docs.fastdotnet.top) · [架构设计](docs/02-核心概念/架构设计.md) · [插件开发](docs/05-插件开发/index.md) · [在线体验](http://admin.demo.fastdotnet.top/) · [插件商城](https://fastdotnet.top/marketplace)

</div>

![Fastdotnet 插件管理界面](docs/.vitepress/public/images/plugin-marketplace.png)

---

## 目录

- [为什么是 Fastdotnet](#为什么是-fastdotnet)
- [和其他框架的取舍](#和其他框架的取舍)
- [3 分钟跑起来](#-3-分钟跑起来)
- [核心能力](#核心能力)
- [三层能力模型](#三层能力模型内置--插件--商城)
- [架构设计](#架构设计)
- [插件系统](#插件系统)
- [路线图](#路线图)
- [参与贡献](#参与贡献)
- [生态与交流](#生态与交流)
- [许可证](#许可证)

---

## 为什么是 Fastdotnet

**"像搭积木一样构建企业应用"** —— 底座内置完整的企业级基础能力，行业功能通过插件按需组装。

| 你遇到的痛点 | Fastdotnet 的做法 |
| --- | --- |
| 不同客户要不同功能组合，单体应用难裁剪 | 插件按需加载/卸载，一套底座支撑多种交付形态 |
| 业务要快速迭代，发版却要停服 | 运行时热插拔，插件更新无需重启宿主 |
| 多团队并行开发，DLL 版本互相打架 | 每个插件独立 `AssemblyLoadContext` + 独立依赖上下文 |
| 想要微服务的模块化，又不想背分布式复杂度 | 单体部署即可获得模块级隔离与解耦 |
| 外包项目重复造轮子，交付周期长 | 底座开箱即用 + 插件商城复用现成能力 |
| 项目代码做完就闲置 | 通用能力封装为插件，可上架商城持续复用 |

它**不是空壳脚手架**：认证授权、用户与权限、菜单字典、日志审计、缓存、代码生成等都已内置（见[核心能力](#核心能力)），你可以直接在上面写业务。

## 和其他框架的取舍

| 维度 | Fastdotnet | ABP Framework | Orchard Core |
| --- | --- | --- | --- |
| **插件隔离** | ✅ 独立 `AssemblyLoadContext`，各插件依赖互不干扰 | 模块共享同一应用域，靠统一版本规避冲突 | 模块化成熟，但共享同一运行时 |
| **热插拔** | ✅ 运行时加载 / 卸载 / 热更新 | 重新加载模块通常需要重启应用 | 支持模块启停，配置相对繁琐 |
| **前端集成** | ✅ 内置 qiankun 微前端，前后端插件同步交付 | 需自行设计前端模块化方案 | 以 CMS 场景为主 |
| **架构约束** | 底座分层清晰，插件内部可自由选型（DDD / Clean 均可） | 重度 DDD 约定，学习曲线较陡 | 面向内容管理的约定较强 |
| **典型场景** | 中小型 SaaS、企业信息化、外包快速交付 | 大型复杂业务系统、大团队协作 | 内容型站点 |

> 选型建议：需要**运行时模块隔离 + 快速交付**选 Fastdotnet；需要**重型 DDD 治理**选 ABP；做**内容站点**选 Orchard Core。

---

## 🚀 3 分钟跑起来

### 环境要求

- .NET 10 SDK（本项目在 `10.0.402` 上验证通过）
- Node.js 20+ 与 pnpm（运行前端时需要）

### 方式一：源码运行（推荐，SQLite 零配置）

`Development` 环境默认使用 SQLite，**不需要预先准备数据库**。

```bash
# 1) 后端：自动建表并初始化数据
cd backend/Fastdotnet.WebApi
dotnet run
# 打开 http://localhost:18889/swagger

# 2) 前端依赖：在 workspace 根安装一次即可（管理端 / 应用端 / 公共层统一管理）
cd Web
pnpm install

# 3) 管理端（新开一个终端）
pnpm dev:admin
# 打开 http://localhost:18888

# 4) 应用端（可选）
pnpm dev:app
# 打开 http://localhost:18887
```

首次启动会自动创建超级管理员：`superadmin` / `123456`
（默认密码取自字典 `PASSWORD_CONFIG.DefaultUserPassword`，**请首次登录后立即修改**）。

### 方式二：Docker 运行后端 + MySQL

```bash
git clone https://github.com/CN-GodHei/fastdotnet.git
cd fastdotnet/docker
docker compose --profile internal-db up -d
# 打开 http://localhost:18889/swagger
```

> Docker Compose 当前只包含**后端 + MySQL**（端口 `18889`），不含前端容器；前端请按方式一本地运行或自行打包部署。
> 更详细的部署说明见 [docker/QUICKSTART.md](docker/QUICKSTART.md) 与 [docker/DEPLOYMENT_GUIDE.md](docker/DEPLOYMENT_GUIDE.md)。

### 验证插件热插拔

启动后端后，用 `superadmin` 登录管理端，进入**插件管理**：可在线扫描、启用、停用、卸载插件；示例插件位于 `backend/Plugins/PluginA`，其前端子应用位于 `backend/Plugins/PluginA/Frontend`。

---

## 核心能力

> 以下能力均已内置于框架底座，开箱即用。

### 🔌 插件系统
- **运行时热插拔**：基于 `AssemblyLoadContext` 加载/卸载，配合 `FileSystemWatcher` 自动感知插件变更
- **独立依赖上下文**：插件可自带不同版本的第三方库，互不冲突
- **插件分支网关**：插件可注册私有请求处理管道
- **反向代理逃生舱**：插件可自带 Kestrel 等微型 Web 服务器，由 YARP 透明转发
- **静态资源映射**：自动处理插件 `wwwroot`，支持 SPA History 模式
- **容器化 DI**：基于 Autofac 为每个插件分配独立 `LifetimeScope`
- **插件间 EventBus**：跨插件发布/订阅，低耦合联动 → [事件总线文档](docs/05-插件开发/事件总线.md)

### 🔐 认证授权
- OIDC/SSO 单点登录（基于 OpenIddict）
- JWT 认证 + RBAC 权限（支持按钮级）
- 多租户数据隔离
- 验证码（图形/滑块）、IP 与用户黑名单、API 限流
- 请求加密传输（AES + RSA）与防重放校验

### 💾 数据与缓存
- SqlSugar ORM，支持 CodeFirst / DbFirst
- 多数据库：SQLite / MySQL / PostgreSQL / SQL Server / 达梦
- SQL 执行日志：AOP 拦截记录含参数值的 SQL 到 `log_sql_execution`，按天分表（开关 `SqlSugar.EnableSqlExecutionLogging`）
- HybridCache 双层缓存：本地内存 + Redis，支持 `[CacheResult]` / `[CacheTag]` 特性
- 仓储模式、事务管理、批量操作

### 🎨 前端与微前端
- Vue 3 + TypeScript + Element Plus + Vite
- qiankun 微前端：主应用按后端元数据动态加载插件子应用
- 动态路由、标签页 KeepAlive、响应式布局、明暗主题、i18n

### 📡 实时通信与可观测
- SignalR（`UniversalHub`）+ 插件级 SignalR 端点
- Outbox 模式可靠推送：SignalR 实时推送 + Webhook 回调 → [对外推送文档](docs/05-插件开发/对外推送.md)
- 操作日志审计（操作人、IP、请求头/体、状态码、耗时，按天分表）
- 异常日志入库 `log_exception`、RequestId 链路追踪、优雅停机

### 🛠️ 开发体验
- Swagger/OpenAPI 自动文档、代码生成器一键 CRUD
- 统一响应格式、自动模型校验、全局异常处理
- 插件项目模板与 CLI 工具、开发期热重载

## 三层能力模型（内置 + 插件 + 商城）

| 类别 | 底座内置（✅ 已实现） | 可插件扩展（🔌） |
| --- | --- | --- |
| 认证授权 | OIDC/SSO、JWT、RBAC、多租户 | 社交登录（微信/QQ/钉钉）、LDAP |
| 用户管理 | 管理员、应用用户、角色权限 | 会员等级、积分、实名认证 |
| 数据存储 | SqlSugar ORM、多数据库、缓存 | 数据同步、ETL、备份 |
| 支付 | — | 支付宝、微信支付、银联 |
| 工作流 | 待办任务、审批流转 | Elsa 工作流引擎、可视化设计器 |
| 对象存储 | 本地存储 | 阿里云 OSS、腾讯云 COS、MinIO、AWS S3 |
| 消息通知 | 站内信 | 邮件、短信、企业微信、钉钉机器人 |
| 报表 | — | 数据报表、图表分析、打印模板 |
| 行业方案 | — | CRM、ERP、OA、电商、教育 |

**需要什么功能？** 先看底座是否内置，缺的再去[插件商城](https://fastdotnet.top/marketplace)找现成插件，最后才考虑自己写。

---

## 架构设计

### 分层与依赖方向

```
Fastdotnet.WebApi           宿主启动层：HTTP 入口、中间件、插件加载引擎
        ↓
Fastdotnet.Service          业务逻辑层：用户、权限、系统配置等底座服务
        ↓
Fastdotnet.Orm              数据访问层：SqlSugar 仓储封装、多库适配
        ↓
Fastdotnet.Core             核心基础层：常量/枚举/异常/工具/中间件（零外部依赖）
        ↓
Fastdotnet.Plugin.Contracts 插件契约层：IPlugin、PluginBase、事件总线契约
        ↓
Fastdotnet.Plugin.Shared    插件运行时共享组件：AOT 适配器与通用工具
```

依赖方向严格自上而下；`Fastdotnet.Core` 不依赖任何其他项目，`Plugin.Contracts` 是插件开发的唯一必需依赖。完整说明见 [架构设计文档](docs/02-核心概念/架构设计.md)。

### 项目结构

```
backend/
├── Fastdotnet.Core/               # 核心基础层
├── Fastdotnet.Core.Notification/  # Outbox / 通知基础设施
├── Fastdotnet.Orm/                # 数据访问层
├── Fastdotnet.Plugin.Contracts/   # 插件契约（IPlugin / PluginBase）
├── Fastdotnet.Plugin.Shared/      # 插件运行时共享组件
├── Fastdotnet.Service/            # 业务服务层
├── Fastdotnet.WebApi/             # 宿主（含 plugins/ 插件部署目录）
├── Fastdotnet.Core.Tests/         # 单元测试（74 个用例）
└── Plugins/PluginA/               # 示例插件：Backend/ + Frontend/
Web/                               # pnpm workspace（前端统一依赖管理）
├── fastdotnet-admin/              # 管理端前端（18888）
├── fastdotnet-app/                # 应用端前端（18887）
└── packages/shared/               # 公共层 @fastdotnet/shared（两个前端共享的工具与类型，只有一份）
docs/                              # VitePress 文档站源码
docker/                            # 容器化部署
```

### 代码知识图谱

我们用知识图谱梳理过模块关系：**4,349 个节点、10,146 条关系边、294 个功能社区**（2026-05 快照，`PluginManager` 与 `IFastdotnetApi` 是跨社区的关键桥接点）。图谱产物体积较大，未随仓库分发，可在本地用 [graphify](https://github.com/safishamsi/graphify) 生成。

---

## 插件系统

### 插件接口

每个插件实现 `IPlugin`，或继承 `PluginBase`（推荐，已提供生命周期状态管理）：

```csharp
public interface IPlugin
{
    string PluginId { get; }                      // 插件唯一标识
    string Name { get; }                          // 插件名称
    string Version { get; }                       // 插件版本
    PluginLifecycleState LifecycleState { get; }  // 生命周期状态

    Task InitializeAsync(IServiceProvider serviceProvider); // 初始化
    Task StartAsync();                                      // 启动
    Task StopAsync();                                       // 停止
    Task UnloadAsync(IServiceProvider serviceProvider);     // 卸载前清理
    void ConfigureServices(ContainerBuilder builder);       // 注册插件服务
}
```

### 最小示例：Hello World 插件

```csharp
public class HelloWorldPlugin : PluginBase
{
    public override string PluginId => "hello-world";
    public override string Name => "Hello World Plugin";
    public override string Version => "1.0.0";

    // 注册到插件的独立容器，不会污染宿主
    public override void ConfigureServices(ContainerBuilder builder)
    {
        builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
    }

    protected override Task OnStartAsync()
    {
        Console.WriteLine("Hello World Plugin is running!");
        return Task.CompletedTask;
    }
}
```

`PluginBase` 还提供 `OnInitializeAsync` / `OnStopAsync` / `OnUnloadAsync` 钩子，按需重写即可。

### 插件清单 plugin.json

```json
{
  "id": "11375910391972869",
  "name": "演示插件A",
  "description": "从开发到使用的全生命周期演示",
  "version": "1.0.0",
  "enabled": true,
  "author": "Fastdotnet Team",
  "dependencies": [],
  "tags": ["demo", "example"],
  "entryPoint": "PluginA.dll"
}
```

### 开发与打包

1. 在 `backend/Plugins` 下新建插件项目，实现 `IPlugin` 或继承 `PluginBase`
2. 在 `.csproj` 中配置唯一 `<PluginId>`（缺失会在编译期报错）
3. 构建后插件自动输出到 `Plugins/<插件名>/publish/<PluginId>/`，其中 `plugin.json` 一并复制
4. 将产物放入宿主 `Fastdotnet.WebApi/plugins/<PluginId>/`，或在管理后台在线安装

> 更多内容：[创建插件](docs/05-插件开发/创建插件.md) · [前端开发](docs/05-插件开发/前端开发.md) · [微前端集成](docs/05-插件开发/微前端集成.md) · [打包发布](docs/05-插件开发/打包发布.md) · [调试与测试](docs/05-插件开发/调试与测试.md) · [PluginA 演示插件](docs/05-插件开发/PluginA演示插件.md)

### AI 集成规划

Fastdotnet 正在向 AI Agent 时代演进，已产出基于 Microsoft Semantic Kernel 的集成方案（智能对话与 RAG、Agent 编排、Function Calling——**插件即工具**）。详见 [AI 智能体集成方案](backend/AI智能体集成方案/README.md)。

---

## 路线图

| 阶段 | 内容 | 状态 |
| --- | --- | --- |
| v1.0 | 核心插件系统、OIDC 认证、基础权限管理 | ✅ 已完成 |
| v1.1 | 插件商城、支付/工作流官方插件 | 🚀 进行中 |
| v1.2 | AI Agent 集成、智能体编排引擎 | 📋 规划中 |
| v1.3 | 多租户隔离增强、性能优化与 AOT 支持 | ⏳ 待定 |

有想做的方向，欢迎在 [Discussions](https://github.com/CN-GodHei/fastdotnet/discussions) 或 Issues 里认领。

## 参与贡献

我们欢迎任何形式的贡献：报告 Bug、提功能建议、修文档、提交代码、贡献插件。

- 📖 贡献流程与开发环境：[CONTRIBUTING.md](CONTRIBUTING.md)
- 📜 社区规范：[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- 🔒 安全问题：请勿公开提 Issue，按 [SECURITY.md](SECURITY.md) 私下报告
- 🐛 [Bug Report](https://github.com/CN-GodHei/fastdotnet/issues/new?template=bug_report.yml) · 💡 [Feature Request](https://github.com/CN-GodHei/fastdotnet/issues/new?template=feature_request.yml)

```bash
# 快速自测
cd backend && dotnet build && dotnet test     # 74 个单元测试
cd Web && pnpm install && pnpm dev:admin
```

## 生态与交流

### 🏪 插件商城与开发者分成

底座免费开源（MIT），插件商城承载增值能力：官方插件提供支付、工作流、社交登录、对象存储等常用功能；第三方开发者可将通用能力封装为插件上架，获得持续分成。

- 浏览插件：[fastdotnet.top/marketplace](https://fastdotnet.top/marketplace)
- 入驻上架：[商城入驻指南](https://fastdotnet.top/marketplace/seller)
- 开发文档：[插件开发指南](docs/05-插件开发/index.md)

### 🎮 在线体验

| 环境 | 地址 | 账号 |
| --- | --- | --- |
| 管理端 | http://admin.demo.fastdotnet.top/ | `superadmin` / `123456`（`mktest` / `123456` 可体验商城与授权） |
| 应用端 | http://app.demo.fastdotnet.top/ | `admintest` / `123456` |

> 演示环境数据会定期重置，请勿存放重要数据。

### 🤝 赞助商

<div align="center">
  <a href="https://www.netzyun.com/" target="_blank">
    <img src="sponsorship/网臻云_logo.png" alt="网臻云" width="200" />
  </a>
  <p><strong>网臻云</strong> — 演示服务器赞助商</p>
</div>

### 📬 联系我们

- 🌐 官网：[https://fastdotnet.top](https://fastdotnet.top) · 📚 文档：[https://docs.fastdotnet.top](https://docs.fastdotnet.top)
- 🐛 Issues：[GitHub Issues](https://github.com/CN-GodHei/fastdotnet/issues) · 🗣️ [Discussions](https://github.com/CN-GodHei/fastdotnet/discussions)
- 📧 邮箱：yunnanzuyuankeji@163.com · 💬 QQ 群：[779454817](https://qm.qq.com/q/779454817)
- 📦 依赖清单：[后端](DEPENDENCIES_BACKEND.md) · [前端](DEPENDENCIES_FRONTEND.md)

## 许可证

本项目采用 [MIT 许可证](LICENSE)。插件可独立授权，详见商城说明。

---

<div align="center">

**如果 Fastdotnet 对你有帮助，欢迎 Star ⭐ 支持我们**

[![Star History Chart](https://api.star-history.com/svg?repos=CN-GodHei/fastdotnet&type=Date)](https://star-history.com/#CN-GodHei/fastdotnet&Date)

</div>
