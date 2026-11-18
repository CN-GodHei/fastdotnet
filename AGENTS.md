# Fastdotnet 仓库 Agent 入口

Fastdotnet 是一个基于 **.NET 10 + Vue 3** 的企业级插件化底座：宿主（`backend/`）提供认证、权限、菜单、缓存、事件总线、MCP 等基础能力，业务功能以**插件**（`backend/Plugins/<Name>/`）形式热插拔，前端用 **qiankun 微前端** 把插件页面挂到管理端 / 应用端。

## 先加载技能包

| 你要做的事 | 技能 |
| --- | --- |
| 改宿主、写后端接口/服务/实体、改管理端或应用端页面、构建测试、排错 | [`skills/fastdotnet/SKILL.md`](skills/fastdotnet/SKILL.md) |
| 新建/修改插件（后端 + 前端 + 打包发布） | [`skills/fastdotnet-plugin-dev/SKILL.md`](skills/fastdotnet-plugin-dev/SKILL.md) |

> 插件是主要交付形态。做插件时两个技能都读：框架技能给宿主规则，插件技能给完整流程与模板。

## 硬规则（违反会破坏架构或 CI）

1. **非必要不改框架基座**。新业务优先做成插件；改基座要能说清为什么插件做不到。
2. **分层依赖单向**：`WebApi → Service → Orm → Core`；插件契约以 `Plugin.Contracts` 为主，权限/存储/插件上下文契约在 `Core`。Controller 不跨层直接调 `IRepository`，走 Service。
3. **可空性警告是错误**：`CS8604`/`CS8625` 已在 `backend/Directory.Build.props` 升级为 error；不要用 `null!` 掩盖（`scripts/check-debt.ps1` 有基线）。
4. **不要用 `<Compile Remove>` 掩盖死代码**，它会让人读代码时得到错误心智模型（同样有基线）。
5. **文档不许说谎**：`scripts/verify-docs.ps1` 会校验相对链接、代码块里 `Add*/Use*/Map*/With*` API 是否真实存在；新增 API 要先写进代码。
6. **前端公共代码只放一份**：进 `Web/packages/shared`（`@fastdotnet/shared`），依赖方向永远是「应用 → 公共层」，公共层里禁止 `@/` 别名。
7. **不提交产物与密钥**：`bin/`、`obj/`、`node_modules/`、运行时 `plugins/` 目录、`appsettings` 里的真实密钥一律不提交。
8. **提交规范**：Conventional Commits（`feat:`/`fix:`/`docs:`/`refactor:`…），PR 目标分支 `develop`。

## 命令速查

```bash
# 后端：构建 / 测试 / 运行
dotnet build backend/Fastdotnet.sln -c Release
dotnet test  backend/Fastdotnet.sln
cd backend/Fastdotnet.WebApi && dotnet run     # http://localhost:18889/swagger

# 前端（Web 是 pnpm workspace）
cd Web && pnpm install
pnpm dev:admin      # http://localhost:18888
pnpm dev:app        # http://localhost:18887

# 三道守门（CI 同款）
pwsh -File scripts/verify-docs.ps1
pwsh -File scripts/check-debt.ps1
dotnet test backend/Fastdotnet.sln -c Release
```

默认账号（首次启动自动初始化，生产必须改）：`superadmin` / `123456`。

## 环境与端口

- .NET SDK **10.0.1xx**（`backend/global.json` 要求 `10.0.100`，`rollForward: latestFeature`；本仓库在 `10.0.402` 验证）。机器上同时装了 11.x 也没关系，`global.json` 会选 10。
- Node.js ≥ 20、pnpm ≥ 8（仓库用 pnpm workspace，根目录 `Web/pnpm-workspace.yaml`）。
- 后端 **18889**、管理端 **18888**、应用端 **18887**；插件前端本地开发端口惯例 **8099 / 8100**。

## 出问题时

先看 `skills/fastdotnet/references/troubleshooting.md` 与 `docs/` 对应章节，不要盲改配置。NuGet 审计警告（NU1900/NU1902）和 `dysoft.vip` 私有源不可达是**既有噪音**，不是你的改动引起的。
