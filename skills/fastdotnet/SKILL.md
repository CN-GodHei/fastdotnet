---
name: fastdotnet
description: Use when working on the Fastdotnet repository itself — reading or changing the host (WebApi/Service/Orm/Core), writing backend controllers/services/entities, changing the admin or app Vue frontend, building, testing, or diagnosing build/runtime problems. Also use when you need the framework's layering, conventions, guard scripts, ports, or command cheat sheet before touching code. For creating or changing a plugin, load fastdotnet-plugin-dev as well (and prefer it for plugin work).
---

# Fastdotnet 框架开发

Fastdotnet = **.NET 10 插件化底座**（`backend/`）+ **Vue 3 双前端**（`Web/`，qiankun 微前端）。宿主提供认证、RBAC、菜单、缓存、事件总线、MCP；业务功能做成插件，运行时热插拔。

## 30 秒定位

| 你要改的东西 | 去哪里 |
| --- | --- |
| HTTP 入口、中间件、DI 装配 | `backend/Fastdotnet.WebApi/`（`Program.cs`、`Modules/ApplicationModule.cs`） |
| 业务服务、初始化器 | `backend/Fastdotnet.Service/` |
| 数据访问、SqlSugar 装配 | `backend/Fastdotnet.Orm/` |
| 实体、DTO、枚举、工具、通用控制器基类 | `backend/Fastdotnet.Core/` |
| 插件契约（生命周期/事件/工具/工作流） | `backend/Fastdotnet.Plugin.Contracts/`（另有部分契约在 Core：权限、存储、插件上下文） |
| 插件 MVC 适配（控制器发现/激活/路由重写） | `backend/Fastdotnet.Plugin.Shared/` |
| MCP 暴露层（AI Agent 工具） | `backend/Fastdotnet.Mcp/` |
| 事件推送基础设施（Outbox/SignalR/Webhook） | `backend/Fastdotnet.Core.Notification/` |
| 示例插件 | `backend/Plugins/PluginA/`、`backend/Plugins/AiToolDemo/` |
| 管理端 / 应用端 / 公共层 | `Web/fastdotnet-admin/`、`Web/fastdotnet-app/`、`Web/packages/shared/` |
| 文档站 | `docs/`（VitePress） |

## 铁律（会破坏架构或 CI 的事）

1. **优先做插件**：新业务放 `backend/Plugins/<Name>/`，别往宿主里堆。宿主只放跨插件的基础能力。
2. **依赖单向**：`WebApi → Service → Orm → Core`；`Plugin.Contracts` 不依赖任何项目。Controller 不直接调 `IRepository`，走 Service。
3. **可空性即错误**：`CS8604`/`CS8625` 是编译错误（`backend/Directory.Build.props`）。用 `required`、可空标注、`ArgumentNullException.ThrowIfNull` 解决，不要 `null!`。
4. **不要新增 `<Compile Remove>`**；删掉无用文件，别让它"存在但不编译"。
5. **文档守门**：`docs/` 与根 README 中代码块的 `Add*/Use*/Map*/With*` 必须在 C# 源码里存在，相对链接必须能打开。写文档前先跑 `pwsh -File scripts/verify-docs.ps1`。
6. **前端公共层只放一份**：跨端复用的进 `@fastdotnet/shared`，且公共层内不得出现 `@/`（那是各应用自己的别名）。
7. **不提交** `bin/`、`obj/`、`node_modules/`、运行时 `plugins/`、真实密钥。提交信息用 Conventional Commits，PR 目标 `develop`。

## 命令速查

```bash
# 后端
dotnet build backend/Fastdotnet.sln -c Release
dotnet test  backend/Fastdotnet.sln
cd backend/Fastdotnet.WebApi && dotnet run          # http://localhost:18889/swagger

# 前端（Web/ 是 pnpm workspace）
cd Web && pnpm install
pnpm dev:admin        # 18888
pnpm dev:app          # 18887

# 守门
pwsh -File scripts/verify-docs.ps1
pwsh -File scripts/check-debt.ps1
```

首次启动自动建表并初始化 `superadmin` / `123456`；Development 默认 SQLite，`appsettings.json` 里另有 MySQL/PostgreSQL/达梦样例（注释切换）。

## 按需读取

| 场景 | 文件 |
| --- | --- |
| 搞清分层、请求管线、横切能力、数据库与缓存 | [`references/architecture.md`](references/architecture.md) |
| 写控制器 / Service / 实体 / DTO / 映射 / 分页 / 权限 | [`references/backend.md`](references/backend.md) |
| 改管理端、应用端、公共层、请求封装、qiankun | [`references/frontend.md`](references/frontend.md) |
| 构建、测试、环境、端口、Docker、CI 三道关 | [`references/build-and-test.md`](references/build-and-test.md) |
| 命名、注释、分层、文档守门、技术债基线的具体口径 | [`references/conventions.md`](references/conventions.md) |
| 编译失败、启动失败、401/403/404、前端白屏等按症状排查 | [`references/troubleshooting.md`](references/troubleshooting.md) |
| 做插件（本项目主要交付形态） | 加载 `fastdotnet-plugin-dev` 技能 |

## 给 agent 的工作方式建议

- **先读再改**：涉及某模块时，先读该模块的 `Program.cs`/基类/相邻实现，按现有模式写，不要发明新写法。
- **小步验证**：每轮改动至少跑 `dotnet build`（后端）或 `pnpm --filter <app> build`（前端）；涉及契约/公共层再跑测试。
- **别改噪音**：NuGet 审计警告（NU1900/NU1902）与私有源 `dysoft.vip` 不可达是既有状态，不是你引入的问题。
- **交付要落文件**：方案、排错结论、模板都写进仓库（`docs/` 或对应 README），不要只在对话里说。
