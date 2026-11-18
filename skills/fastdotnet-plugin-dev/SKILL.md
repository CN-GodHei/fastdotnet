---
name: fastdotnet-plugin-dev
description: Use when creating, changing, packaging, or debugging a Fastdotnet plugin (backend under backend/Plugins/<Name>/Backend, frontend under .../Frontend, qiankun micro app, plugin.json, permissions, menus, events, SignalR, MCP tools, storage, workflow integration). Covers the plugin contract and lifecycle, route rewriting and ApiUsageScope, DI scope, packaging/licensing, and the delivery checklist. Load the fastdotnet skill too when the work touches the host or shared frontend code.
---

# Fastdotnet 插件开发

插件是 Fastdotnet 的**主要交付形态**：一个独立程序集 + 一份 `plugin.json` + 可选前端资源，放进宿主运行目录的
`plugins/{PluginId}/`，由宿主在运行时加载。插件拥有独立 `AssemblyLoadContext`、独立 Autofac 生命周期作用域、
独立依赖上下文，可以启用/停用/卸载而不重启宿主。

> 先读框架技能 `fastdotnet` 的硬规则与命令；本文只讲插件特有的东西。

> **契约分布提醒**：`IPlugin`/`PluginBase`、事件总线、`IPluginToolProvider`、`IPluginSignalREndpoint`、`IWorkflowManager`
> 在 `Fastdotnet.Plugin.Contracts`；而 `IPermissionProvider`、`IStorageContext` 在 `Fastdotnet.Core.IService.Sys`，
> `PluginContext`/`PluginInfo` 在 `Fastdotnet.Core.Plugin`，`IFdAppUserExtensionHandler<T>` 在
> `Fastdotnet.Core.Extensibility.Users`（Contracts 里这几个文件被 `<Compile Remove>` 排除了）。
> 模板 csproj 同时引用 Contracts 与 Core，插件代码按上述命名空间 using 即可。

## 铁律（每条都对应真实故障）

1. **一个插件程序集只能有一个 `PluginBase`/`IPlugin` 实现**。加载器用
   `assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t))` 取"第一个"，
   两个实现的先后顺序没有保证 → 实际运行的是谁不可预期。（`PluginA` 当前就残留两个：`PluginAImpl` 与 `PluginaPlugin`，别照抄。）
2. **`plugin.json` 用小写 camelCase 字段**：`id/name/description/version/enabled/author/dependencies/tags/entryPoint`。
   文档里 PascalCase 的旧示例是错的；`PluginInfo` 的属性就是小写。宿主启用/停用时会**重写**该文件（补 `ManualStop`、改 `enabled`），
   所以不要放注释，也不要指望手写格式被保留。
3. **入口 DLL 必须在插件目录根**，文件名等于 `plugin.json.entryPoint`；第三方依赖放 `dependencies/`。
4. **插件控制器必须标 `[ApiUsageScope]`**（控制器或方法级），否则宿主会把该控制器的所有 action 移除 → 运行时 404。
5. **路由会被重写**：`[Route("api/[controller]")]` → `/api/plugins/{admin|app|shared}/p{pluginId}/{controller}`。
   前端/调用方不要按原路径请求。
6. **不要打包宿主程序集**（尤其 `Fastdotnet.Plugin.Contracts.dll`、`Fastdotnet.Core.dll`、`Fastdotnet.Orm.dll`）。
   插件上下文里出现第二份 `IPlugin` 类型身份，宿主永远识别不了该插件。
7. **启用需要许可**：`plugins/{id}/plugin.lic` 与 PluginId、机器指纹、授权码匹配；否则报"插件授权验证失败"。
   开发前先从商城开发者中心拿到 PluginId 与许可，别在代码里绕。
8. **热更新 = 停用 → 替换文件 → 启用**。宿主用影子复制加载，停用后原 DLL 不被占用，但没有文件监听自动重载。
9. **菜单必须带 `PluginId` + `IsFdMicroApp=true`**，`Path` 用 `/micro/{PluginId}/...`；否则前端不会加载你的微应用。
10. **版本号三处同步**：`plugin.json.version`、程序集版本、前端 `package.json.version`；升级时一起改。

## 七步跑通一个插件

### 1. 拿 PluginId 与许可

从 [插件商城开发者中心](https://fastdotnet.top/marketplace) 申请 PluginId（数字串，如 `11375910391972869`）并获取许可。
本地没有许可时插件能构建、能部署，但**启用会被拒**。

### 2. 生成骨架

```powershell
# 方式 A：仓库内模板（推荐，离线可用；只需后端就去掉 -IncludeFrontend）
pwsh -File skills/fastdotnet-plugin-dev/assets/new-plugin.ps1 `
  -Name MyPlugin -PluginId 12345678901234567 -DisplayName "我的插件" -IncludeFrontend

# 方式 B：官方 CLI（需联网安装）
dotnet tool update -g Fastdotnet.Plugin.CLI
fd-plugin create --name MyPlugin --plugin-id 12345678901234567 --display-name "我的插件"
```

生成结构：

```
backend/Plugins/MyPlugin/
├── Backend/
│   ├── MyPlugin.csproj            # <PluginId> + 编译期校验 + 产物复制到 publish/{id}
│   ├── plugin.json
│   ├── MyPluginPlugin.cs          # 唯一的 PluginBase 实现
│   ├── Controllers/               # 必须标 [ApiUsageScope]
│   ├── Services/  Entities/  Dto/ Initializers/  EventHandlers/
└── Frontend/
    ├── MyPlugin.admin/            # 管理端微应用（端口 8099）
    └── MyPlugin.app/              # 应用端微应用（端口 8100）
```

### 3. 后端最小闭环

```csharp
public class MyPluginPlugin : PluginBase
{
    public override string PluginId => "12345678901234567";
    public override string Name => "MyPlugin";
    public override string Version => "1.0.0";

    protected override Task OnInitializeAsync(IServiceProvider sp) => Task.CompletedTask;

    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 插件自己的服务；控制器由宿主自动注册，不用写在这里
        builder.RegisterType<MyThingService>().AsSelf().InstancePerLifetimeScope();
    }
}

[ApiController]
[Route("api/[controller]")]
[ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]   // 缺了它 = 404
public class MyThingController : ControllerBase
{
    private readonly MyThingService _service;
    public MyThingController(MyThingService service) => _service = service;

    [HttpGet]
    public async Task<List<MyThingDto>> GetAll() => await _service.GetAllAsync();

    [HttpPost]
    [Authorize(Policy = "myplugin.thing.create")]   // 权限码，不是角色
    public async Task<MyThingDto> Create(MyThingCreateDto dto) => await _service.CreateAsync(dto);
}
```

完整模板与可编译代码见 `assets/templates/`，逐条解释见 [`references/backend-guide.md`](references/backend-guide.md)。

### 4. 构建并部署到宿主运行目录

```powershell
dotnet build backend/Plugins/MyPlugin/Backend/MyPlugin.csproj -c Debug
# csproj 的 AfterBuild 目标会把入口 DLL + plugin.json 复制到
# backend/Plugins/MyPlugin/publish/{PluginId}/

# 宿主的插件目录是「运行目录」下的 plugins/：
$dst = "backend/Fastdotnet.WebApi/bin/Debug/net10.0/plugins/{PluginId}"
New-Item -ItemType Directory -Force $dst | Out-Null
Copy-Item "backend/Plugins/MyPlugin/publish/{PluginId}/*" $dst -Recurse -Force
# 许可文件一起放进去
Copy-Item "<你的 plugin.lic>" $dst
```

### 5. 启用并验证后端

```powershell
# 宿主已在运行的话
Invoke-RestMethod -Method Post "http://localhost:18889/api/Plugin/enable/{PluginId}" `
  -Headers @{ Authorization = "Bearer <admin token>" }

# 验证：Swagger 出现 plugin-{PluginId}-admin 分组
Start-Process "http://localhost:18889/swagger"
# 验证：路由按重写后的形式可访问
Invoke-RestMethod "http://localhost:18889/api/plugins/admin/p{PluginId}/MyThing"
```

管理端「系统 → 插件管理」也能扫描/启用/停用/卸载。

### 6. 前端微应用（可选但常见）

- `vite.config.ts` 用 `vite-plugin-qiankun`（name = PluginId），构建成 UMD，产物输出到
  `publish/{PluginId}/wwwroot/{admin|app}`，入口 `micro-main.ts` 导出 `bootstrap/mount/unmount/update`。
- 菜单初始化器插入 `FdMenu`：`PluginId=PluginId`、`IsFdMicroApp=true`、`Path=/micro/{PluginId}/home`。
- 构建后把 `publish/{PluginId}/wwwroot` 一并复制到运行目录插件目录，重新启用。

细节见 [`references/frontend-guide.md`](references/frontend-guide.md)。

### 7. 按清单验收

逐项过 [`references/checklist.md`](references/checklist.md)：加载、启用、菜单、权限、前端挂载、停用、卸载、日志无异常。

## 加载流程（决定了你会遇到什么）

宿主 `POST /api/Plugin/enable/{id}` 内部（`Fastdotnet.Plugin.Core` 包）：

1. 校验 `plugin.lic`（PluginId / 机器指纹 / 授权码 / 签名）。
2. 读 `plugin.json`，按 `entryPoint` 找入口 DLL。
3. 把整个插件目录**影子复制**到 `%TEMP%/Fastdotnet_Plugins_ShadowCopy/{id}/{guid}`（所以运行中文件不被锁）。
4. 新建可回收 `AssemblyLoadContext`，先加载 `dependencies/`、`refs/` 下的 DLL，再加载入口程序集。
5. 取**第一个** `IPlugin` 实现，创建插件子 `LifetimeScope`：
   - 调你的 `ConfigureServices(builder)`；
   - 自动注册：`IPlugin`、所有 `ControllerBase`、所有 `IPermissionProvider`、所有 `IPluginSignalRProvider`、
     所有 `IApplicationInitializer`、`IRepository<>`/`IBaseService<>` 泛型。
6. 对插件程序集执行 CodeFirst（`[SugarTable]` 建表）→ 执行插件内 `IApplicationInitializer`（菜单/配置等）→
   同步 `IPermissionProvider` 权限（module = PluginId）→ `InitializeAsync` → 自动订阅事件处理器 → `StartAsync` →
   注册 SignalR 方法。
7. 把 `wwwroot/{admin,app}` 映射到 `/plugins/{id}/admin|app`，把 `plugin.json.enabled` 写回 `true`。

停用按相反顺序：静态资源注销 → `StopAsync` → 退订事件 → `UnloadAsync` → 释放子容器 → 卸载 ALC（多轮强制 GC 等待回收）→
清理影子副本 → 移除 CodeFirst 缓存。**任何静态事件、定时器、后台线程、长生命周期对象持有插件类型，都会让 ALC 卸载不掉。**

## 按需读取

| 场景 | 文件 |
| --- | --- |
| 契约、生命周期、DI 作用域、程序集/依赖边界、许可与 plugin.json | [`references/contract-lifecycle.md`](references/contract-lifecycle.md) |
| csproj、控制器与路由重写、实体/建表、服务、映射、配置、菜单、权限 | [`references/backend-guide.md`](references/backend-guide.md) |
| qiankun 微应用、Vite 配置、API 生成、菜单路径、样式与调试 | [`references/frontend-guide.md`](references/frontend-guide.md) |
| 事件总线、SignalR、指标、MCP 工具、存储、工作流、用户扩展、中间件 | [`references/extension-points.md`](references/extension-points.md) |
| 打包、安装包结构、离线安装、版本、热更新、排错 | [`references/packaging-and-debug.md`](references/packaging-and-debug.md) |
| 交付前逐项验收 | [`references/checklist.md`](references/checklist.md) |
| 可复制的骨架代码 | `assets/templates/`、`assets/new-plugin.ps1` |
