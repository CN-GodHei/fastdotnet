# 构建、测试与运行环境

## 1. 环境要求

| 依赖 | 版本 | 备注 |
| --- | --- | --- |
| .NET SDK | **10.0.1xx** | `backend/global.json` 固定 `10.0.100`、`rollForward: latestFeature`；本仓库在 `10.0.402` 验证。装 11.x 不影响，`dotnet` 会按 global.json 选 10 |
| Node.js | ≥ 20 LTS | 前端与文档站 |
| pnpm | ≥ 8（仓库用 11.x） | `Web/` 与 `docs/` 都是 pnpm 项目 |
| PowerShell | 7+ | 只有守门脚本需要（CI 用 `pwsh`） |
| Docker | 可选 | 镜像构建与 `docker/` 部署 |

## 2. 构建与测试

```bash
# 全量构建（Release，与 CI 一致）
dotnet build backend/Fastdotnet.sln -c Release

# 单元测试 + 集成测试
dotnet test backend/Fastdotnet.sln -c Release

# 只跑某个测试项目
dotnet test backend/Fastdotnet.Core.Tests/Fastdotnet.Core.Tests.csproj
```

- 构建基线：**0 error，约 90 warning**。警告绝大多数是 NuGet 审计（`NU1900` 私有源不可达、`NU1902` 已知漏洞包）
  和第三方包的过时标记，属于既有状态；**不要为了让警告变少而升级核心包**。
- 测试基线：`Fastdotnet.Core.Tests` 74 个 + `Fastdotnet.WebApi.Tests` 6 个（共 80），应当全绿。
- `Directory.Build.props` 把 `CS8604`/`CS8625` 提升为错误；`Directory.Packages.props` 集中管理包版本
  （`ManagePackageVersionsCentrally`），新增包只写 `PackageReference`，版本加到这里。
- 私有源 `dysoft.vip` 在公网不可达时会产生 restore 警告，但仓库已缓存/有回退，构建仍可成功。

## 3. 运行后端

```bash
cd backend/Fastdotnet.WebApi
dotnet run
# Swagger: http://localhost:18889/swagger
# 健康检查: http://localhost:18889/health
```

- 工作目录必须是 `Fastdotnet.WebApi`（`Program.cs` 会把 `Environment.CurrentDirectory` 固定到程序目录）。
  插件目录因此是 `backend/Fastdotnet.WebApi/bin/Debug/net10.0/plugins/`，**不是**仓库里的 `backend/Plugins/`。
- 首次启动：CodeFirst 建表 + 初始化字典/菜单/权限 + 创建 `superadmin` / `123456`。
- 数据库：Development 默认 SQLite（`fd.db`，无需准备）；`appsettings.json` 里 `SqlSugar:Connections` 的 MySQL / PostgreSQL /
  达梦样例是注释状态，切换时同时配置 `main` 与 `log` 两个 `ConfigId`。
- 端口：`Kestrel:Endpoints:Http:Url`（`appsettings*.json`），默认 `http://*:18889`。
- Data Protection 密钥落盘在运行目录 `DataProtection-Keys/`；删掉会导致已有 OIDC 授权码/加密数据失效。
- 本地 Development 已把 `Security:AntiReplay:Enabled` 置为 `false`（原 `#if DEBUG` 隐式行为已移除）。

## 4. 构建与运行前端

```bash
cd Web
pnpm install
pnpm dev:admin          # 18888
pnpm dev:app            # 18887
pnpm build:all          # 两端产物
```

- 开发代理在各自 `vite.config.ts`：`/api`、`/api/universalhub`、`/plugins/` → `VITE_API_URL`（`.env.development` 默认 `http://localhost:18889`）。
- 生产环境改 `.env.production` 的 `VITE_API_URL`；构建产物是纯静态，部署到任意静态服务器/后端 wwwroot。
- `pnpm-workspace.yaml` 的 `allowBuilds` 必须保留 `esbuild`，否则 Vite 构建会因缺二进制失败。

## 5. 文档站

```bash
cd docs
pnpm install
pnpm dev            # VitePress 本地预览
pnpm build          # 产物 docs/.vitepress/dist
```

CI 的 `deploy-docs.yml` 负责发布；本地只验证页面与链接。

## 6. Docker

```bash
./build.ps1                     # 构建镜像 fastdotnetwebapi:latest
cd docker && docker compose up -d
curl http://localhost:18889/health
```

- `docker/` 编排是**后端 + MySQL**，不包含前端容器；前端按第 4 节本地跑或自行打包。
- 容器里插件目录是容器内程序目录下的 `plugins/`，挂载/拷贝插件时注意是 `plugins/{PluginId}/`，不是仓库 `backend/Plugins/`。

## 7. CI 三道关（本地务必自测）

[`.github/workflows/backend-ci.yml`](../../../.github/workflows/backend-ci.yml)：

| 关卡 | 命令 | 失败常见原因 |
| --- | --- | --- |
| Build & Test | `dotnet restore/build/test backend/Fastdotnet.sln -c Release` | 可空警告升级为 error、测试断言过期 |
| Docs Guard | `pwsh -File scripts/verify-docs.ps1` | 相对链接断开、代码块里写了不存在的 `Add*/Use*/Map*/With*` API、围栏不配平 |
| Debt Guard | `pwsh -File scripts/check-debt.ps1` | 新增 `<Compile Remove>` 或 `null!` 超过基线 |

守门脚本的"基线"机制：历史问题登记在 `scripts/doc-baseline.txt` 与 `check-debt.ps1` 顶部常量里，
**只允许下调**；修掉一条要从基线删一行/改小数字。

## 8. 提交前最小自检

```bash
dotnet build backend/Fastdotnet.sln -c Release     # 0 error
dotnet test  backend/Fastdotnet.sln -c Release     # 全绿
pwsh -File scripts/verify-docs.ps1                 # 无新增
pwsh -File scripts/check-debt.ps1                  # 未增长
```

前端改动再加：对应应用 `pnpm build` 通过 + 本地端口手工走查。
