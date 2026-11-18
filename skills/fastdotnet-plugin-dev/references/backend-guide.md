# 插件后端开发

## 1. 项目文件（csproj）

关键不是"能编译"，而是 **PluginId 注入 + 产物落到 publish/{PluginId}**：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AssemblyName>MyPlugin</AssemblyName>
    <RootNamespace>MyPlugin</RootNamespace>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);CS1591</NoWarn>

    <!-- ① 必填：与 plugin.json / plugin.lic / 目录名一致 -->
    <PluginId>12345678901234567</PluginId>
  </PropertyGroup>

  <!-- ② 没配 PluginId 直接编译失败，避免产出"装不上的插件" -->
  <Target Name="ValidatePluginConfiguration" BeforeTargets="BeforeBuild">
    <Error Condition="'$(PluginId)' == ''"
           Text="编译失败：必须在 .csproj 中配置 &lt;PluginId&gt;。" />
  </Target>

  <!-- ③ 把 PluginId 写进程序集元数据；宿主/PluginContext 靠它识别 -->
  <ItemGroup>
    <AssemblyAttribute Include="System.Reflection.AssemblyMetadataAttribute"
                       Condition="'$(PluginId)' != ''">
      <_Parameter1>PluginId</_Parameter1>
      <_Parameter2>$(PluginId)</_Parameter2>
    </AssemblyAttribute>
  </ItemGroup>

  <ItemGroup>
    <!-- Private=false：宿主程序集不复制进插件包，避免类型身份冲突 -->
    <ProjectReference Include="..\..\..\Fastdotnet.Plugin.Contracts\Fastdotnet.Plugin.Contracts.csproj" Private="false" />
    <ProjectReference Include="..\..\..\Fastdotnet.Core\Fastdotnet.Core.csproj" Private="false" />
    <ProjectReference Include="..\..\..\Fastdotnet.Orm\Fastdotnet.Orm.csproj" Private="false" />
  </ItemGroup>

  <!-- ④ 构建后把入口 DLL/符号/注释/清单复制到 publish/{PluginId} -->
  <Target Name="CopyToPublishDir" AfterTargets="Build">
    <PropertyGroup>
      <PublishBaseDir>$(MSBuildThisFileDirectory)..\publish\$(PluginId)</PublishBaseDir>
      <SourceBinDir>$(OutputPath)</SourceBinDir>
      <TargetFileName>$(TargetName)$(TargetExt)</TargetFileName>
    </PropertyGroup>
    <MakeDir Directories="$(PublishBaseDir)" />
    <Copy SourceFiles="$(SourceBinDir)$(TargetFileName)" DestinationFolder="$(PublishBaseDir)" SkipUnchangedFiles="true" />
    <Copy SourceFiles="$(SourceBinDir)$(TargetName).pdb" DestinationFolder="$(PublishBaseDir)" SkipUnchangedFiles="true" Condition="Exists('$(SourceBinDir)$(TargetName).pdb')" />
    <Copy SourceFiles="$(SourceBinDir)$(TargetName).xml" DestinationFolder="$(PublishBaseDir)" SkipUnchangedFiles="true" Condition="Exists('$(SourceBinDir)$(TargetName).xml')" />
    <Copy SourceFiles="$(MSBuildThisFileDirectory)plugin.json" DestinationFolder="$(PublishBaseDir)" SkipUnchangedFiles="true" />
  </Target>
</Project>
```

- `SqlSugarCore`、`Mapster`、`Autofac` 通过 `Fastdotnet.Core` 传递引用即可；不要再写版本（`Directory.Packages.props` 集中管理）。
- 需要 Notification/工作流等额外能力时再加 ProjectReference，但**同样 `Private="false"`**（这些程序集宿主已有）。
- 第三方库（宿主没有的）正常 `PackageReference`，它们会进输出目录，发布时复制到 `dependencies/`。

## 2. 控制器与路由（最容易踩的坑）

### 路由重写规则

宿主 `PluginActionDescriptorProvider` 对**插件程序集**里的控制器重写路由：

```
api/plugins/{scope}/p{pluginId 小写}/{原路由去掉 "api/" 前缀}
```

`scope` 由 `[ApiUsageScope]` 决定：`AdminOnly → admin`、`AppOnly → app`、`Both → shared`。

| 你的写法 | 运行时实际路径 |
| --- | --- |
| `[Route("api/[controller]")]` + `[ApiUsageScope(AdminOnly)]` | `/api/plugins/admin/p{id}/{Controller}` |
| `[Route("api/orders")]` + `[ApiUsageScope(Both)]` | `/api/plugins/shared/p{id}/orders` |
| 方法级 `[ApiUsageScope(AppOnly)]` | 该方法走 `app/` 前缀，其余按控制器级 |
| **没有 `[ApiUsageScope]`** | **整个控制器被移除 → 404** |

特性可继承：继承 `GenericDtoControllerBase` 自带 `[ApiUsageScope(AdminOnly)]`。

```csharp
[ApiController]
[Route("api/[controller]")]
[ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
public class MyThingController : ControllerBase
{
    private readonly MyThingService _service;
    private readonly ICurrentUser _currentUser;

    public MyThingController(MyThingService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public Task<List<MyThingDto>> GetAll(CancellationToken ct) => _service.GetAllAsync(ct);

    [HttpGet("{id}")]
    public Task<MyThingDto> Get(string id, CancellationToken ct) => _service.GetAsync(id, ct);

    [HttpPost]
    [Authorize(Policy = "myplugin.thing.create")]
    public Task<MyThingDto> Create(MyThingCreateDto dto, CancellationToken ct) => _service.CreateAsync(dto, ct);

    // 需要匿名访问时显式声明（全局默认要求登录 + ApiScope）
    [HttpGet("public/ping")]
    [AllowAnonymous]
    public object Ping() => new { ok = true, at = DateTime.Now };
}
```

### 返回与错误

- 直接返回 DTO / `List<T>` / `PageResult<T>`；`GlobalResultFilter` 会包成 `{Code, Msg, Data}`，**不要自己拼**。
- 业务失败抛 `BusinessException("原因")`；不要 `return BadRequest(new { ... })` 造第二套格式。
- 入参校验自己写（自动模型验证已关闭）：`ArgumentNullException.ThrowIfNull(dto)`、业务规则抛 `BusinessException`。
- 大量 CRUD 直接继承 `GenericDtoControllerBase<TEntity, TKey, TCreateDto, TUpdateDto, TDto>`（构造需要
  `IBaseService<TEntity,TKey>`，可选 `ICurrentUser`），自带列表/分页/条件查询/批量/回收站端点。

### 授权

- `[Authorize(Policy = "权限码")]`：策略名就是权限码（`PermissionPolicyProvider` 兜底），权限码必须在
  `IPermissionProvider` 里声明并已同步到库。`SUPER_ADMIN` 直接通过。
- 默认策略要求「已认证 + ApiScope」，所以未标 `[AllowAnonymous]` 的接口天然要登录。
- 前端请求要带 `Authorization: Bearer` 与 `System-Category`（Admin/App）；插件 Swagger 分组也按 scope 拆分。

## 3. 实体与建表

```csharp
[SugarTable("my_thing", "我的业务表")]
public class MyThing : AuditableEntity
{
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true)]
    public string? Remark { get; set; }
}
```

- 继承 `BaseEntity`（Id/时间/软删除）或 `AuditableEntity`（再加审计人）。
- 主键/时间/审计人由 SqlSugar AOP 填充；**不要手动赋值 Id**。
- 表名/列名用 snake_case；显式 `[SugarColumn(ColumnName = "...")]`。
- 建表：插件启用时宿主对插件程序集执行 CodeFirst，`[SugarTable]` 类自动建表/补列。
  因此**不要写迁移脚本**；但改字段名不会自动改旧列（等于新列），需要自己处理数据迁移（插件首个初始化器里做一次性 UPDATE）。
- 分表字段用 `[SplitField]`（参考 `BaseEntity.CreatedAt`）；一般业务表不需要。
- 多租户/数据隔离：插件表自己带 `tenant_id`/`owner_id` 列并在查询里过滤。

## 4. Service 层

```csharp
public class MyThingService
{
    private readonly IRepository<MyThing> _repo;
    private readonly ILogger<MyThingService> _logger;

    public MyThingService(IRepository<MyThing> repo, ILogger<MyThingService> logger)
    {
        _repo = repo; _logger = logger;
    }

    public async Task<List<MyThingDto>> GetAllAsync(CancellationToken ct)
        => (await _repo.GetListAsync(null, ct)).Adapt<List<MyThingDto>>();

    public async Task<MyThingDto> GetAsync(string id, CancellationToken ct)
        => (await _repo.GetByIdAsync(id, ct)).Adapt<MyThingDto>();

    public async Task<MyThingDto> CreateAsync(MyThingCreateDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BusinessException("名称不能为空");

        var entity = dto.Adapt<MyThing>();
        await _repo.InsertAsync(entity);            // 注意：InsertAsync 没有 CancellationToken 重载
        _logger.LogInformation("创建 MyThing {Id}", entity.Id);
        return entity.Adapt<MyThingDto>();
    }
}
```

- 在 `ConfigureServices` 里注册：`builder.RegisterType<MyThingService>().AsSelf().InstancePerLifetimeScope();`。
- 需要事务用 `IUnitOfWork`（宿主已注册 `SqlSugarUnitOfWork`）或 `ISqlSugarClient.AsTenant()`；跨仓储写操作必须同一事务。
- **不要**注入另一个插件的类型；跨插件协作走事件总线（见 `extension-points.md`）。
- 宿主已有能力优先复用：`IStorageContext`（原生 SQL）、`IStorageService`（文件存储）、`ICurrentUser`、`IEventBus`、
  `IPluginConfigurationService`、`IHybridCacheService`。

## 5. DTO 与映射

- DTO 放 `Dto/`，命名 `XxxDto` / `XxxCreateDto` / `XxxUpdateDto`。
- Mapster 映射：插件内所有 `IRegister` 实现会在**启用时被宿主扫描并注册**（无需改宿主）：

```csharp
public class MyMappingRegistry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MyThing, MyThingDto>().MaskSensitiveData();
        config.NewConfig<MyThingCreateDto, MyThing>();
        config.NewConfig<MyThingUpdateDto, MyThing>().IgnoreNullValues(true);
    }
}
```

- 出参脱敏用 `.MaskSensitiveData()`；部分更新用 `.IgnoreNullValues(true)`。

## 6. 菜单初始化

> `PluginContext` 在 `Fastdotnet.Core.Plugin`；`FdMenu` 在 `Fastdotnet.Core.Entities.Sys`。

插件启用时宿主会执行为数 `IApplicationInitializer` 的实现，这是注册菜单的官方时机：

```csharp
public class MyMenuInitializer : IApplicationInitializer
{
    private readonly IRepository<FdMenu> _menus;
    public MyMenuInitializer(IRepository<FdMenu> menus) => _menus = menus;

    // 2001–3000 为插件级（见 IApplicationInitializer.Order 约定）
    public int Order => 2100;

    public async Task InitializeAsync()
    {
        var pluginId = PluginContext.GetCurrentPluginInfo()!.id;
        const string parentCode = "MENU_CODE_MYPLUGIN_ROOT";

        var rows = new List<FdMenu>
        {
            new()
            {
                Title = "我的插件", Code = parentCode, Path = $"/micro/{pluginId}",
                Icon = "ele-Monitor", Sort = 100, Type = MenuType.Directory,
                Module = pluginId, Belong = SystemCategory.Admin,
                Component = "layout/routerView/parent.vue", IsEnabled = true
            },
            new()
            {
                Title = "首页", Code = "MENU_CODE_MYPLUGIN_HOME",
                Path = $"/micro/{pluginId}/home", ParentCode = parentCode,
                Icon = "ele-HomeFilled", Sort = 1, Type = MenuType.Menu,
                Module = pluginId, Belong = SystemCategory.Admin,
                IsFdMicroApp = true, PluginId = pluginId, IsKeepAlive = true
            }
        };

        var codes = rows.Select(r => r.Code).ToList();
        var existing = (await _menus.GetListAsync(m => codes.Contains(m.Code))).Select(m => m.Code).ToHashSet();
        var toInsert = rows.Where(r => !existing.Contains(r.Code)).ToList();
        if (toInsert.Count > 0) await _menus.InsertRangeAsync(toInsert);
    }
}
```

要点：

- `Code` 必须全库唯一（`fd_menu` 有唯一索引），用 `MENU_CODE_{PLUGIN}_{PAGE}` 这类带前缀的稳定值。
- `PluginId` 与 `IsFdMicroApp=true` 缺一不可，否则前端不会加载微应用。
- `Path` 必须以 `/micro/{pluginId}` 开头；`ParentCode` 指向父菜单（目录本身不需要 PluginId）。
- 幂等：先按 `Code` 查再插；不要每次启用都全量 `InsertRange`。
- `Icon` 用 `ele-` 前缀（Element Plus 图标名），与前端注册方式一致。
- 删除菜单不要硬删被角色引用的行；先清理 `fd_role_menu`。

## 7. 权限声明

> 命名空间：`IPermissionProvider` / `PermissionDefinition` 在 `Fastdotnet.Core.IService.Sys`（不在 Contracts）。
```csharp
public static class MyPluginPermissions
{
    public const string ThingView   = "myplugin.thing.view";
    public const string ThingCreate = "myplugin.thing.create";
    public const string ThingDelete = "myplugin.thing.delete";
}

public class MyPermissionProvider : IPermissionProvider
{
    public IEnumerable<PermissionDefinition> GetPermissions() => new List<PermissionDefinition>
    {
        new() { Module = "MyPlugin", Code = MyPluginPermissions.ThingView,   Name = "查看物品", Category = "Admin", Type = "Api" },
        new() { Module = "MyPlugin", Code = MyPluginPermissions.ThingCreate, Name = "新建物品", Category = "Admin", Type = "Api" },
        new() { Module = "MyPlugin", Code = MyPluginPermissions.ThingDelete, Name = "删除物品", Category = "Admin", Type = "Api" }
    };
}
```

- `Module` 建议 = PluginId 或插件名（宿主同步时实际用 PluginId 作为 module 落库）。
- `Code` 小写点分、带插件前缀，避免和宿主/其他插件撞车。
- `Category`：`Admin` / `User`；`Type`：`Api` / `Menu` / `Data` / `Button`。
- 同步是**双向**的：代码里移除的权限码会从数据库删除。删权限前先确认没有角色引用。
- 权限提供者要在 `ConfigureServices` 注册（虽然宿主会自动注册所有 `IPermissionProvider` 实现，显式注册更清晰）。
- MCP 工具的 `RequiredPermission` 也用同一套权限码。

## 8. 配置

```csharp
public sealed class MyPluginOptions
{
    public string Endpoint { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;

    public static MyPluginOptions Default => new();
}

public class MyConfigInitializer : IApplicationInitializer
{
    private readonly IPluginConfigurationService _config;
    public MyConfigInitializer(IPluginConfigurationService config) => _config = config;

    public async Task InitializeAsync()
    {
        var id = PluginContext.GetCurrentPluginInfo()!.id;
        var saved = await _config.GetSettingsAsync<MyPluginOptions>(id);
        var merged = ObjectMerger.ApplyOverrides(MyPluginOptions.Default, saved!);
        await _config.SaveSettingsAsync(id, merged!);
    }
}
```

- 读取时每次都从 `IPluginConfigurationService` 取（有缓存），不要自己读文件。
- 默认值 + 存量覆盖用 `ObjectMerger.ApplyOverrides`，保证升级后新增字段有默认值。
- 管理端配置页可复用宿主插件配置接口（`PluginConfigurationController`）。

## 9. 日志与诊断

- `ILogger<T>` 注入即可；宿主日志在运行目录 `Logs/`。
- 关键路径打印 PluginId + 业务主键，便于在宿主日志里过滤。
- 插件 Swagger：`/swagger/plugin-{PluginId}-admin/swagger.json`（前端 `openapi2ts` 用它生成客户端）。
- 调试期优先看宿主控制台：启用/停用/初始化器失败都会打印（初始化器异常只打日志、不阻断）。
