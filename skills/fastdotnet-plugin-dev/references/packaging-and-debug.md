# 打包、发布与排错

## 1. 后端产物

目标目录：`backend/Plugins/<Name>/publish/{PluginId}/`

```powershell
cd backend/Plugins/MyPlugin/Backend
dotnet build -c Release
# 构建后 csproj 的 CopyToPublishDir 已把以下文件放进 publish/{PluginId}：
#   MyPlugin.dll / .pdb / .xml / plugin.json
```

第三方依赖（宿主没有的库）：

```powershell
dotnet publish -c Release -o ..\publish\{PluginId}\dependencies
```

然后**必须清掉宿主已有的程序集**，否则插件上下文出现重复类型：

```powershell
$deps = "..\publish\{PluginId}\dependencies"
$hostAssemblies = @(
  "Fastdotnet.Plugin.Contracts.dll","Fastdotnet.Plugin.Shared.dll","Fastdotnet.Core.dll",
  "Fastdotnet.Core.Notification.dll","Fastdotnet.Orm.dll","Fastdotnet.Service.dll",
  "Fastdotnet.Mcp.dll","Fastdotnet.WebApi.dll",
  "Microsoft.AspNetCore*.dll","System.*.dll"   # 框架程序集也不要带
)
Get-ChildItem $deps -Filter *.dll |
  Where-Object { $n = $_.Name; $hostAssemblies | Where-Object { $n -like $_ } } |
  Remove-Item -Force

# 入口 DLL 只保留在插件根目录，dependencies 里的同名副本要删掉
Remove-Item "$deps\MyPlugin.dll" -ErrorAction SilentlyContinue
```

保留 `plugin.lic`：把许可文件放在 `publish/{PluginId}/plugin.lic`。

## 2. 前端产物

```powershell
cd backend/Plugins/MyPlugin/Frontend/MyPlugin.admin
pnpm install
pnpm build          # → ../../publish/{PluginId}/wwwroot/admin

cd ../MyPlugin.app
pnpm install
pnpm build          # → ../../publish/{PluginId}/wwwroot/app
```

确认 `wwwroot/admin/index.html` 存在且里面引用的 JS 文件名与构建产物一致（大小写敏感）。

## 3. 最终目录（必须长这样）

```
publish/{PluginId}/
├── plugin.json
├── plugin.lic
├── MyPlugin.dll
├── MyPlugin.pdb            # 可选
├── MyPlugin.xml            # 可选（Swagger 注释）
├── dependencies/           # 只放宿主没有的第三方库
└── wwwroot/
    ├── admin/index.html + *.js + assets/*.css
    └── app/index.html + *.js + assets/*.css
```

自检：

```powershell
$id = "{PluginId}"
$root = "backend/Plugins/MyPlugin/publish/$id"
Get-Content "$root/plugin.json"                     # id/entryPoint 与实际一致
Test-Path "$root/$((Get-Content "$root/plugin.json" -Raw | ConvertFrom-Json).entryPoint)"   # 应为 True
Get-ChildItem "$root/dependencies" -Filter "Fastdotnet.*.dll"   # 应为空
```

## 4. 打包与安装

```powershell
cd backend/Plugins/MyPlugin/publish
Compress-Archive -Path "{PluginId}" -DestinationPath "MyPlugin-v1.0.0.zip" -Force
# 或进目录后压缩内容（宿主能处理"单层嵌套目录"）
```

安装方式：

| 方式 | 操作 | 注意 |
| --- | --- | --- |
| 手动部署 | 解压/复制到 `plugins/{PluginId}/` | 目录名必须 = PluginId |
| 离线包上传 | 管理端插件页 / `POST /api/Plugin/upload-offline`（multipart，≤20MB） | 仅 `.zip` |
| 商城安装 | `POST /api/Plugin/load`（PluginId + Version + Token） | 需登录与授权码 |

安装后到管理端「插件管理」→ 扫描 → 启用；或 `POST /api/Plugin/enable/{PluginId}`。

## 5. 版本与升级

三处同步：

1. `plugin.json` 的 `version`
2. `Backend/*.csproj` 的 `<Version>`（若设置）
3. 前端 `package.json` 的 `version`

升级流程（不重启宿主）：

1. 停用插件（`POST /api/Plugin/disable/{PluginId}`）。
2. 覆盖 `plugins/{PluginId}/` 下的文件（影子复制保证原 DLL 未被锁）。
3. 启用插件；检查 Swagger 分组、菜单、接口。
4. 前端有变更时让用户强刷浏览器（qiankun 会缓存 entry）。

> 数据库表不会随卸载删除（CodeFirst 只建不删）。升级如果改了列语义，要在插件初始化器里做一次性数据修复并幂等。

## 6. 启用失败排查顺序

1. **目录**：`plugins/{PluginId}/plugin.json` 是否存在，目录名是否等于 id。
2. **清单**：JSON 是否合法；`entryPoint` 指向的 DLL 是否在根目录。
3. **许可**：`plugin.lic` 是否存在、是否与 PluginId/授权码/机器匹配（错误信息"插件授权验证失败!"）。
4. **程序集**：是否混入宿主 DLL（`Fastdotnet.Plugin.Contracts.dll` 等）→ 类型身份冲突。
5. **类型**：是否只有一个 `PluginBase` 实现；构造函数是否无参（`Activator.CreateInstance(pluginType)`）。
6. **依赖**：第三方库是否在 `dependencies/`，版本是否与宿主冲突。
7. **初始化器**：异常只打日志、不阻断启用；看宿主控制台红色输出。
8. **日志**：宿主控制台 + 运行目录 `Logs/`。

## 7. 症状 → 原因对照

| 症状 | 排查点 |
| --- | --- |
| 启用了但接口 404 | 控制器缺 `[ApiUsageScope]`；或请求路径不是 `/api/plugins/...`；或控制器被 `<Compile Remove>` |
| Swagger 没有 `plugin-{id}-admin` | 插件未真正启用 / DLL 没有 XML 注释 / `entryPoint` 与实际不符 |
| 菜单不出现 | 初始化器没执行（类没实现 `IApplicationInitializer` 或不在插件程序集）；`Code` 与已有菜单冲突；`IsEnabled=false` |
| 菜单出现但页面空白 | `IsFdMicroApp`/`PluginId` 缺失；`wwwroot/admin/index.html` 不存在；入口 JS 路径大小写错误 |
| 刷新插件页 404 | admin/app 目录无 SPA 回退；用前端路由内部跳转，或在网关加回退 |
| 插件事件不触发 | 处理器没在 `ConfigureServices` 注册为 `IEventHandler<T>`；订阅的 key 与实际发布不一致 |
| MCP 工具看不到 | `IPluginToolProvider` 没注册；`RequiredPermission` 用户没有；插件未激活 |
| 指标为空 | `IMetricProvider` 没注册；指标 Id 与 `CalculateAsync` 分支不一致 |
| SignalR 方法调用报未注册 | Provider 未实现 `IPluginSignalRProvider` / 方法没标 `[SignalRMethod]`；PluginId 不一致 |
| 停用后内存不降 | 静态引用/定时器/后台线程/未退订事件持有插件类型（见 `contract-lifecycle.md` 第 7 节） |

## 8. 逆向排查（确认宿主到底加载了什么）

```powershell
# 运行目录的插件与清单
Get-ChildItem backend\Fastdotnet.WebApi\bin\Debug\net10.0\plugins -Directory |
  ForEach-Object { $j = Join-Path $_.FullName 'plugin.json'; if (Test-Path $j) { "$($_.Name) -> " + ((Get-Content $j -Raw | ConvertFrom-Json).enabled) } }

# 已加载的插件（需登录 token）
Invoke-RestMethod http://localhost:18889/api/Plugin/loaded -Headers @{ Authorization = "Bearer <token>" }

# 影子复制残留（异常退出后可能堆积）
Get-ChildItem $env:TEMP\Fastdotnet_Plugins_ShadowCopy -Directory -ErrorAction SilentlyContinue
```

## 9. 发布前

逐项过 `checklist.md`。特别确认：没有宿主 DLL 混入、没有密钥/连接串、`plugin.lic` 有效、前端产物齐全、版本号一致。
