# 契约、生命周期与边界

> 本文中"宿主行为"均以仓库内可核实代码与 NuGet 包 `Fastdotnet.Plugin.Core 1.1.4` 反编译结果为准；
> 包升级后请重新核对。

## 1. 契约

插件实际引用两个宿主程序集：`Fastdotnet.Plugin.Contracts`（生命周期/事件/工具/工作流）与
`Fastdotnet.Core`（`IPermissionProvider`、`IStorageContext`、`PluginContext`、用户扩展）。

> 为什么拆在两边：`Fastdotnet.Plugin.Contracts.csproj` 把 `IPermissionProvider.cs`、`IStorageContext.cs`、
> `PluginContext.cs`、`PluginInfo.cs`、`PluginInfoCache.cs`、`Extensibility/**` 都 `<Compile Remove>` 了，
> 这些类型的有效定义在 `Fastdotnet.Core`。写 using 时以本表为准，别照着文件路径猜。

| 类型 | 命名空间 | 所在程序集 |
| --- | --- | --- |
| `IPlugin`、`PluginBase`、`PluginLifecycleState` | `Fastdotnet.Plugin.Contracts` | Contracts |
| `IEventBus`、`IEventHandler<T>`、`[EventContract]`、`DomainEvent` | `Fastdotnet.Plugin.Contracts.Events` | Contracts |
| `IPluginToolProvider`、`PluginToolDescriptor`、`ToolInvocationResult` | `Fastdotnet.Plugin.Contracts` | Contracts |
| `IPluginSignalREndpoint`、`IWorkflowManager` | `Fastdotnet.Plugin.Contracts` | Contracts |
| `IPermissionProvider`、`PermissionDefinition`、`IStorageContext` | `Fastdotnet.Core.IService.Sys` | Core |
| `PluginContext`、`PluginInfo`、`PluginInfoCache` | `Fastdotnet.Core.Plugin` | Core |
| `IFdAppUserExtensionHandler<T>` | `Fastdotnet.Core.Extensibility.Users` | Core |
| `IPluginSignalRProvider`、`[SignalRMethod]`、`DynamicMiddlewareRegistry`、`PluginPipelineRegistry<T>` | `Fastdotnet.Core.Plugin` / `Fastdotnet.Core.Middleware` | Core |

```csharp
public interface IPlugin
{
    string PluginId { get; }
    string Name { get; }
    string Version { get; }
    PluginLifecycleState LifecycleState { get; }

    Task InitializeAsync(IServiceProvider serviceProvider);
    Task StartAsync();
    Task StopAsync();
    Task UnloadAsync(IServiceProvider serviceProvider);
    void ConfigureServices(ContainerBuilder builder);
}
```

推荐继承 `PluginBase`（实现了状态机，禁止你重写状态方法）：

| 你想做的事 | 重写 |
| --- | --- |
| 初始化（可解析宿主服务、注册中间件、读配置） | `protected override Task OnInitializeAsync(IServiceProvider sp)` |
| 启动（订阅、预热） | `protected override Task OnStartAsync()` |
| 停止（停定时器、断连接） | `protected override Task OnStopAsync()` |
| 卸载清理（退订、释放静态引用） | `protected override Task OnUnloadAsync(IServiceProvider sp)` |
| 注册插件自己的服务 | `public override void ConfigureServices(ContainerBuilder builder)` |

`PluginBase` 提供的状态：`Uninitialized → Initialized → Started → Stopped → Unloaded`。
**不要重写 `InitializeAsync/StartAsync/StopAsync/UnloadAsync`**（它们是状态机入口）。

```csharp
public class MyPluginPlugin : PluginBase
{
    public override string PluginId => "12345678901234567";
    public override string Name => "MyPlugin";
    public override string Version => "1.0.0";

    private ILogger? _logger;

    protected override Task OnInitializeAsync(IServiceProvider sp)
    {
        _logger = sp.GetService<ILoggerFactory>()?.CreateLogger("MyPlugin");
        // 例：注册插件 HTTP 中间件（类型也要在 ConfigureServices 注册）
        sp.GetService<DynamicMiddlewareRegistry>()?.Register(typeof(MyMiddleware));
        return Task.CompletedTask;
    }

    public override void ConfigureServices(ContainerBuilder builder)
    {
        builder.RegisterType<MyService>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterType<MyPermissionProvider>().As<IPermissionProvider>().InstancePerLifetimeScope();
        builder.RegisterType<MyToolProvider>().As<IPluginToolProvider>().InstancePerLifetimeScope();
    }
}
```

> **一个程序集只能有一个** `PluginBase` 实现。加载器取 `assembly.GetTypes().FirstOrDefault(...)`，多实现时没有稳定顺序。

## 2. 插件自己的 DI 作用域

启用时宿主在根容器下为插件开一个子 `ILifetimeScope`（`BeginLoadContextLifetimeScope`），并在里面**自动注册**：

| 自动注册 | 说明 |
| --- | --- |
| `IPlugin` | 你的插件类型，生命周期内单例（scope 级） |
| 所有 `ControllerBase` 子类 | 每个请求从插件 scope 解析（`PluginControllerActivator`） |
| 所有 `IPermissionProvider` 实现 | 启用时逐个同步权限（类型来自 `Fastdotnet.Core.IService.Sys`） |
| 所有 `IPluginSignalRProvider` 实现 | 启用时注册到 `UniversalHub` 方法表 |
| 所有 `IApplicationInitializer` 实现 | 启用时立即执行（按 `Order`） |
| `IRepository<>` / `IRepository<,>` / `IBaseService<>` / `IBaseService<,>` | 可直接注入 |
| 宿主已注册的所有服务 | 子 scope 继承父容器：`IEventBus`、`ISqlSugarClient`、`ICurrentUser`、`IStorageContext`、`IPluginConfigurationService` 等 |
| 你的 `ConfigureServices` 注册项 | 只属于本插件，不会污染宿主 |

因此：

- **控制器不需要手写注册**；服务、工具提供者、事件处理器、Hub 辅助类需要你在 `ConfigureServices` 里注册。
- 插件服务之间可以互相注入；**不要**去 resolve 另一个插件的类型（程序集隔离，类型身份不同）。
- 需要宿主能力时注入接口（`IStorageContext`/`IEventBus`/`IPluginConfigurationService`…），不要 `new` 宿主内部类。

## 3. 生命周期真实顺序

启用（`POST /api/Plugin/enable/{id}` / 启动时 `StartInstalledPlugins`）：

1. 校验 `plugin.lic`（见第 5 节）。
2. 读 `plugins/{id}/plugin.json`；`entryPoint` 为空时默认 `{id}.dll`。
3. 影子复制整个插件目录 → `%TEMP%/Fastdotnet_Plugins_ShadowCopy/{id}/{guid}`。
4. 创建可回收 `AssemblyLoadContext`；把 `dependencies/`、`refs/` 里的 DLL 先加载进该上下文，再加载入口程序集。
5. 找第一个 `IPlugin` 实现；注册 Mapster `IRegister`（插件内映射）。
6. 创建子 `LifetimeScope`：先 `ConfigureServices(builder)`，再自动注册（见第 2 节）。
7. 插件程序集的 CodeFirst：所有 `[SugarTable]` 实体建表/补列。
8. 执行插件程序集内的 `IApplicationInitializer`（按 `Order`；异常只打日志、不阻断启用）。
9. 同步 `IPermissionProvider` 的权限（module = PluginId；**代码中不存在的权限码会从库里删除**）。
10. `InitializeAsync(pluginScope)` → 自动订阅 `IEventHandler<T>` → `StartAsync()` → 注册 `IPluginSignalRProvider` 方法。
11. 注册静态文件：`wwwroot/admin` → `/plugins/{id}/admin`，`wwwroot/app` → `/plugins/{id}/app`，
    `wwwroot/publish` → `/plugins/{id}/publish`（仅这个启用 SPA 回退）。
12. 把 `plugin.json.enabled` 写回 `true`。

停用：

1. 注销静态文件映射；检查是否有**活动插件依赖**它（`dependencies` 包含本 id 会拒绝停用）。
2. `plugin.json.enabled=false`（手动停用还会写 `ManualStop=true`）。
3. `StopAsync()` → 退订事件处理器 → `UnloadAsync(pluginScope)` → 释放子 scope。
4. 卸载 ALC，循环强制 GC 最多 10 次等待回收；移除 CodeFirst 缓存；清理影子副本目录。

**推论**：`OnStopAsync` 只负责"停"，真正释放托管/非托管资源放 `OnUnloadAsync`；两者都要幂等（可能被重复调用或启用失败回滚）。

## 4. plugin.json

```json
{
  "id": "12345678901234567",
  "name": "我的插件",
  "description": "插件用途",
  "version": "1.0.0",
  "enabled": true,
  "author": "Your Name",
  "dependencies": [],
  "tags": ["demo"],
  "entryPoint": "MyPlugin.dll"
}
```

| 字段 | 必填 | 说明 |
| --- | --- | --- |
| `id` | ✅ | PluginId；必须与目录名、`plugin.lic`、csproj `<PluginId>` 一致 |
| `name` | ✅ | 显示名（中文可） |
| `description` | | 插件描述，插件管理页展示 |
| `version` | ✅ | 语义化版本，前端/程序集保持一致 |
| `enabled` | | 宿主管理；扫描时只自动启动 `true` 的插件 |
| `ManualStop` | | 宿主写入：用户手动停用后不再自动启动（不要在模板里手写） |
| `author` | | 作者 |
| `dependencies` | | 依赖的其他**插件 Id**；停用被依赖插件会被拒绝 |
| `tags` | | 分类标签 |
| `entryPoint` | ✅ | 入口 DLL 文件名（相对插件根目录） |

注意：

- 字段名是 **camelCase**（`PluginInfo` 的属性定义）。
- 宿主用 Newtonsoft 反序列化并**回写**该文件（缩进、补 `ManualStop`、改 `enabled`），所以：不要加注释；不要把它当只读配置。
- 扫描逻辑：遍历 `plugins/` 下**一级子目录**，存在 `plugin.json` 即视为插件；因此 `plugins/AuthUser.json` 这类文件不会误判。
- 插件自己的业务配置不要塞进 `plugin.json`，用 `IPluginConfigurationService`（见后端指南）。

## 5. 许可（License）

- 启用前必须通过 `plugin.lic` 校验：RSA 签名 + PluginId + 授权码（`plugins/AuthUser.json`）+ 机器指纹
  （`SingleServer` 类型校验指纹）。
- 缺失/不匹配 → 抛"插件授权验证失败!"并把 `enabled` 置回 `false`。
- 包内对 PluginId `11365281228129286` 有硬编码跳过（内置调度插件），**不要**指望用自己的 ID 复现这个特例。
- 开发流程：先在商城开发者中心申请 PluginId 与许可；把 `plugin.lic` 与插件一起部署。
- 授权码管理：管理端插件页的 `SetAuthCode`/`GetAuthCode`（`/api/Plugin/SetAuthCode`、`/api/Plugin/GetAuthCode`）；
  在线更新许可 `/api/Plugin/UpdatePluginLicenseOnline`。

## 6. 目录结构与依赖边界

部署形态（`plugins/{PluginId}/`）：

```
{PluginId}/
├── plugin.json
├── plugin.lic
├── MyPlugin.dll            # 入口程序集（entryPoint）
├── MyPlugin.pdb / .xml     # 可选：调试符号、XML 注释（Swagger 用）
├── dependencies/           # 第三方依赖 DLL（Quartz、驱动等），会被加载进插件 ALC
├── refs/                   # 需要单独引用的程序集（可选）
└── wwwroot/
    ├── admin/              # 管理端微应用（index.html + assets）
    ├── app/                # 应用端微应用
    └── publish/            # 其他静态资源（启用 SPA 回退）
```

**绝对不要**放进包里的：

- `Fastdotnet.Plugin.Contracts.dll`、`Fastdotnet.Core.dll`、`Fastdotnet.Orm.dll`、`Fastdotnet.Plugin.Shared.dll`
  等宿主程序集。它们由宿主默认上下文提供；包里再带一份会造成类型身份冲突（`IPlugin` 认不出来、
  `InvalidCastException`、`CS0433`）。
- `appsettings*.json`（插件读宿主配置或自己的配置服务）、密钥、证书。

允许放进 `dependencies/` 的：宿主**没有**、且插件私有的第三方库（如示例调度插件带 `Quartz*.dll`）。
判断标准：宿主已引用的包不要重复带；带版本冲突的库放 `dependencies/` 由 ALC 隔离。

> 仓库内 `PluginA.csproj` 用 `ProjectReference` 引用了 Core/Orm/Contracts，是为了**在仓库内编译**；
> 打发布包时必须过滤掉这些程序集（模板 csproj 用 `Private="false"` 让宿主程序集不进入输出目录）。

## 7. 卸载与内存

以下东西会让 ALC 无法回收（表现为"停用后内存不降、再次启用出现两份静态状态"）：

- `static` 事件订阅、`static` 缓存持有插件类型/实例；
- 未停止的 `Timer`、`Thread`、`Task.Run` 长循环；
- 注册到宿主全局注册表但停用时没反注册（中间件、SignalR 方法、EventRouter 订阅等）；
- `IEventBus` 强类型订阅未释放（宿主会自动退订它代管的 `IEventHandler<T>`，自己手动 `Subscribe` 的要 `IDisposable.Dispose()`）。

清理写在 `OnStopAsync`（停）与 `OnUnloadAsync`（放）里，并在 `OnUnloadAsync` 里对可能为 null 的对象做判空。

## 8. 插件上下文

```csharp
var info = PluginContext.GetCurrentPluginInfo();   // id/name/version
var pluginId = info.id;
```

用途：初始化器里取自己的 id（写入菜单 `PluginId`、权限模块名、配置键），避免到处硬编码。
`PluginContext` 通过调用程序集 + 缓存/目录推断，别在别的程序集里调用它来"猜"当前插件。

## 9. 配置服务

不要往 `plugin.json` 写业务配置。用宿主提供的：

```csharp
public class MyConfigInitializer : IApplicationInitializer
{
    private readonly IPluginConfigurationService _config;
    public MyConfigInitializer(IPluginConfigurationService config) => _config = config;

    public async Task InitializeAsync()
    {
        var id = PluginContext.GetCurrentPluginInfo()!.id;
        var current = await _config.GetSettingsAsync<MyPluginOptions>(id);
        var merged = ObjectMerger.ApplyOverrides(MyPluginOptions.Default, current!);
        await _config.SaveSettingsAsync(id, merged!);
    }
}
```

`MyPluginOptions` 只放可序列化的普通属性；敏感字段依赖宿主的存储加密策略，不要自建明文文件。
