# 后端开发（宿主侧）

## 1. 一次典型改动长什么样

```
新增业务表 MyThing
  → Core/Entities/.../MyThing.cs               实体（[SugarTable]，继承 BaseEntity/AuditableEntity）
  → Core/Dtos/.../MyThingDto.cs                输出 DTO + Create/Update DTO
  → Service/.../MyThingService.cs              IBaseService<T> 够用就直接用，不够再写自定义服务
  → WebApi/Controllers/.../MyThingController.cs 继承 GenericDtoControllerBase 或自写
  → WebApi/Modules/ApplicationModule.cs        自定义服务在这里注册（或用 *Service 命名自动扫描）
  → 权限/菜单初始化器                          需要授权码与导航入口时
```

## 2. 接口返回与错误

- **不要手写** `{ code, data, msg }`。直接返回 DTO、`List<T>`、`PageResult<T>` 或 `ApiResult<T>`；
  `GlobalResultFilter` 会统一包裹成 `ApiResult<T>`（`Code`/`Msg`/`Data`）。前端拦截器只把 `Data` 交给业务代码。
- 分页返回 `PageResult<T>` 会被展开为 `{Data, TotalCount, PageIndex, PageSize, TotalPages}`，再包进 `ApiResult`。
- 业务性失败抛 `BusinessException("原因")`，由全局异常中间件转成统一错误；不要 `return StatusCode(500, ...)`。
- 需要 401/403 时用 `Unauthorized()`/`Forbid()` 或授权策略，别手写 JSON。
- 自动模型验证被关闭（`SuppressModelStateInvalidFilter = true`），**必须显式校验入参**（`ArgumentNullException.ThrowIfNull`、
  业务规则校验并抛 `BusinessException`）。

## 3. 通用 CRUD 控制器（首选）

基类：[`backend/Fastdotnet.Core/Controllers/GenericDtoControllerBase.cs`](../../../backend/Fastdotnet.Core/Controllers/GenericDtoControllerBase.cs)

```csharp
[ApiController]
[Route("api/[controller]")]
public class MyThingController
    : GenericDtoControllerBase<MyThing, string, MyThingCreateDto, MyThingUpdateDto, MyThingDto>
{
    public MyThingController(IBaseService<MyThing, string> service, ICurrentUser currentUser)
        : base(service, currentUser) { }

    // 需要时重写钩子：BeforeGetListByCondition / AfterGetListByCondition / BeforeCreate ...
}
```

继承即得（路由前缀 `api/[controller]`，基类已标 `[ApiUsageScope(AdminOnly)]`）：

| 方法 | 路径 |
| --- | --- |
| GET | `/api/MyThing` |
| POST | `/api/MyThing/list-by-condition`（分页/字段裁剪/动态条件） |
| GET | `/api/MyThing/{id}` |
| GET | `/api/MyThing/page?pageIndex=&pageSize=` |
| POST | `/api/MyThing/page/search`（`PageQueryByConditionDto`） |
| POST | `/api/MyThing`、`/api/MyThing/batch` |
| PUT | `/api/MyThing/{id}`、`/api/MyThing/batch`、`/api/MyThing/batch/updatebycondition` |
| DELETE | `/api/MyThing/{id}`、`/api/MyThing/batch` |
| GET | `/api/MyThing/recyclebin`、`POST /recyclebin/search` |
| PUT/POST | `/api/MyThing/recyclebin/{id}/restore`、`/recyclebin/restore` |
| DELETE/POST | `/api/MyThing/recyclebin/{id}/permanent`、`/recyclebin/permanent` |

分页/条件查询结构：`PageQueryByConditionDto` + `QueryConditionItem`（`PropertyName`、`Operator`=`QueryOperator`、
`Value`/`Value2`、`Logic`、`PageIndex`、`PageSize`）。前端 `FdQueryBuilder.buildMixedQuery` 生成的就是这套结构。

自定义端点直接加 `[HttpGet("...")]`；控制器里只做参数校验与编排，业务写 Service。

## 4. 实体与表

```csharp
[SugarTable("my_thing", "我的业务表")]
public class MyThing : AuditableEntity
{
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; } = string.Empty;
}
```

- 基类在 [`backend/Fastdotnet.Core/Dtos/Base/BaseEntity.cs`](../../../backend/Fastdotnet.Core/Dtos/Base/BaseEntity.cs)：
  `BaseEntity`（Id/时间/软删除）、`AuditableEntity`（再加 CreatedBy/UpdatedBy/DeletedBy）。
- `Id`、`CreatedAt`、`UpdatedAt`、审计人字段由 SqlSugar AOP 自动赋值（雪花 ID 见 `SqlSugarServiceCollectionExtensions`）。
  **不要手动 new Guid / 手写时间**。
- 列名统一 snake_case；显式写 `ColumnName`。`ISoftDelete` 实体由 SqlSugar 全局过滤器自动加 `is_deleted = false`，
  回收站相关端点由基类内部绕过该过滤器实现；**业务查询不要自己再拼 `is_deleted` 条件**（分表/原生 SQL 除外）。
- 建表由 CodeFirst 完成（启动时扫描已加载程序集的 `[SugarTable]`）。改字段后重启即生效；**不要写迁移脚本**。
- 大文本用 `ColumnDataType = StaticConfig.CodeFirst_BigString`（参考 `FdDictData.ExtData`）。

## 5. Service 层

- 通用能力看 `IBaseService<TEntity, TKey>`（`backend/Fastdotnet.Core/IService/IBaseService.cs`）：
  增删改查、分页、条件查询、批量、回收站。泛型实现 `BaseService<,>` 已在 Autofac 注册。
- 自定义服务命名以 `Service` 结尾时，`ApplicationModule` 会扫描注册（`RegisterAssemblyTypes(...).Where(t => t.Name.EndsWith("Service"))`）；
  否则在 `ApplicationModule.Load` 显式 `RegisterType<X>().As<IY>().InstancePerLifetimeScope()`。
- 数据访问只在 Service 里做：注入 `IRepository<T>` 或 `ISqlSugarClient`。
- 需要事务时用 `IUnitOfWork`（`SqlSugarUnitOfWork`，Autofac 注册）或 `ISqlSugarClient.AsTenant().BeginTran()` 成对提交/回滚。

## 6. DTO 与映射（Mapster）

- DTO 放 `Core/Dtos/...` 或模块内 `Dtos/`，命名 `XxxDto` / `XxxCreateDto` / `XxxUpdateDto`。
- 映射用 Mapster 的 `IRegister`：

```csharp
public class MyThingMappingRegistry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MyThing, MyThingDto>().MaskSensitiveData();
        config.NewConfig<MyThingCreateDto, MyThing>();
        config.NewConfig<MyThingUpdateDto, MyThing>().IgnoreNullValues(true);
    }
}
```

- 宿主启动时扫描 `Core`、`Service`、`WebApi` 三个程序集的 `IRegister`（`ApplicationModule`）；插件程序集的 `IRegister`
  由插件加载器在启用时注册。**新增映射不需要改注册表**。
- 出参脱敏用 `.MaskSensitiveData()`（Core 扩展），不要在每个 DTO 上手写打码。

## 7. 权限与菜单

- 权限定义：实现 `IPermissionProvider`（宿主用 `FrameworkPermissionProvider`），启动时
  `PermissionSyncInitializer` 调 `PermissionSyncService` 与数据库同步。
  **代码里删掉的权限码会从数据库删除**，删前先确认没有角色在引用。
- 接口鉴权：`[Authorize(Policy = "system.user.view")]`。`PermissionPolicyProvider` 会把任何未注册的策略名当作权限码，
  `PermissionHandler` 校验用户权限；`SUPER_ADMIN` 直接通过。
- 菜单：`FdMenu`（`Core/Entities/Sys/FdMenu.cs`），关键字段 `Code`（唯一索引）、`Path`、`ParentCode`、`Module`、
  `Belong`（Admin/App）、`Type`（Directory/Menu）、`IsFdMicroApp`、`PluginId`、`Component`、`PermissionCode`。
  新增菜单的写法参考 `Fastdotnet.Service/Initializers/FdMenuConfigInitializer.cs`：构造列表 → 按 `Code` 查已存在 → 只插缺失项（幂等）。

## 8. 日志、当前用户、缓存

- 日志用 `ILogger<T>`；日志文件在运行目录 `Logs/`。异常日志带上业务主键（订单号、用户 ID），别只记 `ex.Message`。
- 当前用户：`ICurrentUser`（`Id`、`IsAuthenticated`、角色/类别）。不要从 `HttpContext` 里手工拆 JWT。
- 缓存：`IHybridCacheService`（本地 + Redis），或特性 `[CacheResult(ExpirationSeconds = 60)]`（`Core/Attributes/CacheResultAttribute.cs`）。
  缓存键必须带租户/用户/查询条件维度，写清失效策略。

## 9. 测试

- 纯逻辑/工具/键生成：加 `backend/Fastdotnet.Core.Tests`（MSTest）。
- 管道/端点契约：加 `backend/Fastdotnet.WebApi.Tests`（`WebApplicationFactory<Program>`，已固化中间件顺序与响应包裹行为）。
- 改宿主公共行为（过滤器、中间件、序列化、鉴权）必须补集成测试，否则容易"本地能跑、线上 500"。
- 运行：`dotnet test backend/Fastdotnet.sln -c Release`。
