# 架构与运行时

## 1. 工程分层

```
backend/
├── Fastdotnet.WebApi             宿主：HTTP 入口、中间件、DI 装配、插件加载引擎接入
├── Fastdotnet.Service            业务实现：用户/角色/菜单/权限/字典/通知…
├── Fastdotnet.Orm                SqlSugar 封装：仓储、工作单元、CodeFirst、多库连接
├── Fastdotnet.Core               零外部依赖的基座：实体/DTO/枚举/异常/特性/中间件/通用控制器
├── Fastdotnet.Core.Notification  事件推送基础设施：Outbox、SignalR、Webhook、AsyncAPI
├── Fastdotnet.Mcp                 MCP 暴露层：把宿主与插件能力变成 AI Agent 可调工具
├── Fastdotnet.Plugin.Contracts   插件契约层（生命周期/事件/工具/工作流；权限与存储契约在 Core）
├── Fastdotnet.Plugin.Shared      插件 MVC 适配：控制器发现、激活、路由重写、ALC 管理
├── Plugins/PluginA               官方演示插件（后端 + 双前端）
├── Plugins/AiToolDemo            MCP 工具演示插件
├── Fastdotnet.Core.Tests         单元测试
└── Fastdotnet.WebApi.Tests       宿主集成测试（WebApplicationFactory）
```

依赖方向严格单向：`WebApi → Service → Orm → Core`，`Plugin.Contracts`/`Plugin.Shared` 只向下依赖 `Core`。
**`Fastdotnet.Core` 不引用任何项目**；改它等于改所有人的地基，先确认插件层解决不了。

宿主的插件加载实现**不在仓库里**：`PluginLoadService`、`PluginManager`（包内版）、许可校验等在 NuGet 包
`Fastdotnet.Plugin.Core`（版本见 `backend/Directory.Packages.props`，当前 `1.1.4`）。
仓库内 `Fastdotnet.Plugin.Shared/AdapterAOT/` 只保留 MVC 适配器（`PluginActionDescriptorProvider`、
`PluginControllerActivator`）和供包使用的 `PluginManager` 类型。**排查加载/许可行为时要意识到一部分逻辑在包内。**

## 2. 宿主启动管线

入口：[`backend/Fastdotnet.WebApi/Program.cs`](../../../backend/Fastdotnet.WebApi/Program.cs)（顶级语句）。顺序很关键：

1. `WebApplication.CreateBuilder`，把 `Environment.CurrentDirectory` 固定到程序目录。
2. Data Protection 密钥持久化到 `DataProtection-Keys/`（OpenIddict 授权码跨重启可解）。
3. 响应压缩（Brotli/Gzip）、CORS（允许任意来源 + 凭证）。
4. 全局授权策略：默认要求「已认证 + `ApiScope`」；`AddControllers` 时注册 `AuthorizeFilter`（**因此接口默认需要登录**）。
5. 全局结果过滤器 `GlobalResultFilter`：把返回值统一包成 `ApiResult<T>`（`Code`/`Msg`/`Data`），`PageResult<T>` 会展开分页字段。
   模型验证的自动 400 被显式关闭（`SuppressModelStateInvalidFilter = true`），校验要么用 Fluent 风格手动做，要么看全局异常中间件。
6. Swagger 自定义、事件总线 `AddEventBus()`、SqlSugar、Notification、HybridCache、JWT/OIDC、SignalR、存储、MCP。
7. Autofac 接管容器：`builder.Host.UseAutofacWithCustomModules()` → `Fastdotnet.WebApi/Modules/ApplicationModule.cs`。
8. `app.Build()` 后 **`DependencySelfCheck.Run(app)`**：Development 下解析关键服务，失败直接阻止启动（自检清单在 `Startup/DependencySelfCheck.cs`）。
9. `await app.UseApplicationInitializers()`：按 `IApplicationInitializer.Order` 顺序执行（CodeFirst 最先，权限/字典/菜单随后）。
10. 管道顺序（**不要随意调换**）：
    `UseRouting` → `RequestId` → 事件目录端点 → **`PluginReverseProxyMiddleware`（必须早于防重放/加密）** →
    `AntiReplay` → `Encryption` → `GlobalException` → `OperationLog` → `PluginStaticFileMiddleware` →
    `Blacklist` → `RateLimit` → `DynamicMiddlewareDispatcher` → `UsePluginBranchGate` → 认证 → 授权 →
    响应压缩 → 优雅停机 → `MapControllers` / `MapHub<UniversalHub>("/universalhub")` / MCP 端点。
11. `RunStartupTasks()`：其中 `StartupTask` 调 `IPluginLoadService.StartInstalledPlugins()` 加载 `plugins/` 下已启用插件。
12. Kestrel 地址取 `AppSettings.Kestrel.Endpoints.Http.Url`，默认 `http://*:18889`。

## 3. 横切能力（写业务前先知道它们存在）

| 能力 | 位置 | 说明 |
| --- | --- | --- |
| 统一响应 / 分页包装 | `WebApi/Filters/GlobalResultFilter.cs` | 直接返回 DTO 或 `PageResult<T>`，不要自己拼 `{code,data}` |
| 全局异常 | `WebApi/Middleware/GlobalExceptionMiddleware.cs` + `BusinessException` | 业务失败抛 `BusinessException`，不要返回 500 手写 JSON |
| 操作日志 | `WebApi/Middleware/OperationLogMiddleware.cs` | 自动记录请求；敏感接口注意脱敏 |
| 防重放 / 传输加密 | `AntiReplayMiddleware`、`EncryptionMiddleware`、`[EncryptResponse]` | 请求头带时间戳/nonce；本地 Development 已显式关闭防重放 |
| 认证 | JWT Bearer + OpenIddict（OIDC/SSO） | `ICurrentUser` 取当前用户；`System-Category` 头区分 Admin/App |
| 授权 | `PermissionPolicyProvider` + `PermissionHandler` | `[Authorize(Policy = "xxx.yyy")]` 中不存在的策略名会被当作权限码；`SUPER_ADMIN` 直接通过 |
| 缓存 | `AddHybridCacheService`（本地 + Redis）、`[CacheResult]` | 缓存键要带租户/用户维度 |
| 事件总线 | `AddEventBus()` → `IEventBus`（内存实现） | 强类型 + 弱类型（`{pluginId}.{event}`）两种；插件间通信用弱类型 |
| 对外推送 | `Fastdotnet.Core.Notification` + `/_events/spec` | Outbox 可靠投递（SignalR / Webhook） |
| 实时通信 | `UniversalHub`（`/universalhub`） | 插件通过 `IPluginSignalRProvider` 注册方法 |
| MCP | `AddFastdotnetMcp` + `MapFastdotnetMcp` | 把工具暴露给 AI Agent，端点默认 `/mcp`，要求 JWT |
| 插件静态资源 | `PluginStaticFileMiddleware` | 把 `plugins/{id}/wwwroot/{admin,app,publish}` 映射到 `/plugins/{id}/...` |

## 4. 数据访问

- SqlSugar 由 `AddSqlSugar(configuration)` 注册，**多连接**：`SqlSugar:Connections` 里的 `ConfigId` 为 `main`（业务库）与 `log`（日志库）。
- 实体基类：`Fastdotnet.Core/Dtos/Base/BaseEntity.cs`
  - `BaseEntity`：`Id`(string, 主键)、`CreatedAt`、`UpdatedAt`、`IsDeleted`、`DeletedAt`（软删除）。
  - `AuditableEntity`：再加 `CreatedBy`/`UpdatedBy`/`DeletedBy`（`FdMenu` 等系统表用它）。
- 表映射用 SqlSugar 特性：`[SugarTable("fd_menu", "菜单")]`、`[SugarColumn(ColumnName = "xxx")]`、`[SplitField]`（分表字段）。
- 列名统一 **snake_case**；主键 `id`；时间列 `created_at/updated_at/deleted_at`。
- CodeFirst：宿主启动扫描**当前已加载程序集**中的 `[SugarTable]` 建表（`Fastdotnet.Service/Initializers/OrmCodeFirstInitializer.cs`）；
  插件在其自身启用时由宿主调用 `UsePluginCodeFirst(pluginAssembly)` 建表。**不需要手写迁移脚本**。
- 仓储/服务：`IRepository<T>` / `IBaseService<T>`，泛型注册见 `ApplicationModule`（`Repository<>`、`BaseService<>`）。
  Controller 层经过 Service 访问数据（见 `CONTRIBUTING.md` 的分层约定）。

## 5. 前端架构

```
Web/                         pnpm workspace 根（package.json + pnpm-workspace.yaml）
├── fastdotnet-admin/        管理端（Vue 3 + Element Plus + Pinia，端口 18888）
├── fastdotnet-app/          应用端（端口 18887）
└── packages/shared/         @fastdotnet/shared 公共层（源码直连，无构建产物）
```

- 公共层导入用**子路径**：`import request from '@fastdotnet/shared/utils/request'`；不要 `@/`（那是各应用别名）。
- 公共层里的插件运行时（`src/plugins/`）通过 `src/runtime/pluginHost.ts` 注入宿主差异：
  插件列表扫描接口、主应用 axios 实例、前端资源段（`admin`/`app`）、qiankun 样式隔离开关。
  两个应用在 `main.ts` 里调用 `configurePluginHost` / `registerPluginScanApi` / `registerPluginRequest`。
- 管理端登录后由菜单数据（`IsFdMicroApp` + `PluginId`）生成 qiankun 子应用配置：
  `entry = {VITE_API_URL}/plugins/{PluginId}/admin/index.html`、`activeRule = /micro/{PluginId}`、容器 `#subapp-viewport`。
- 请求封装：`@fastdotnet/shared/utils/request`，开发环境走 Vite 代理（`/api`、`/api/universalhub`、`/plugins/` → `VITE_API_URL`），
  生产直连 `VITE_API_URL`；响应拦截器统一处理 `{Code, Msg, Data}` 并**直接返回 Data**。

## 6. 插件体系接入点（框架侧）

- 契约：以 `Fastdotnet.Plugin.Contracts` 为主（`IPlugin`、`PluginBase`、`IEventBus`、`IPluginToolProvider`、
  `IPluginSignalREndpoint`、`IWorkflowManager`）；**注意**当前代码把 `IPermissionProvider`、`IStorageContext` 放在
  `Fastdotnet.Core.IService.Sys`，`PluginContext`/`PluginInfo` 放在 `Fastdotnet.Core.Plugin`，
  `IFdAppUserExtensionHandler<T>` 放在 `Fastdotnet.Core.Extensibility.Users`（Contracts 中对应文件被 Compile Remove）。
  插件项目按需同时引用 Contracts 与 Core。
- 加载：宿主启动后扫描 `AppDomain.BaseDirectory/plugins/{pluginId}/plugin.json`，只启动 `enabled=true` 的插件；
  启用动作经 `POST /api/Plugin/enable/{pluginId}`，内部做许可校验、影子复制、ALC 加载、独立 Autofac scope。
- 路由：插件控制器被重写为 `api/plugins/{admin|app|shared}/p{pluginId}/...`，**没有 `[ApiUsageScope]` 的控制器会被移除（404）**。
- 前端：插件构建产物放 `plugins/{id}/wwwroot/{admin,app}`，由 `PluginStaticFileMiddleware` 提供，qiankun 按菜单加载。

插件的完整开发流程、坑与模板见技能 `fastdotnet-plugin-dev`。
