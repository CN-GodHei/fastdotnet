# 按症状排错

先拿证据再改配置：`dotnet build` 输出、宿主控制台日志、`Logs/`、浏览器 Network/Console、`/health`、`/swagger`。
下面按症状给出**最可能的原因**与验证动作。

## 1. 构建期

| 症状 | 原因与处理 |
| --- | --- |
| `error NETSDK1045 / 找不到 SDK 10.0.100` | 机器缺 .NET 10 SDK，或 `global.json` 被改。`dotnet --list-sdks` 确认有 10.0.1xx；不要删 `backend/global.json` |
| `CS8604/CS8625` 编译错误 | 项目把可空警告当错误。按语义补可空标注或抛 `ArgumentNullException`，**不要 `null!`** |
| `NU1101/NU1301` 找不到包 | 私有源 `dysoft.vip` 不可达时先看是否只是警告；确实缺包再检查 `Directory.Packages.props` 与网络 |
| 改了代码但行为没变 | 插件/前端产物没重新部署；`dotnet build` 只更新 `bin/`，运行中的宿主读的是 `bin/.../plugins/` |
| `CS0433 类型存在于多个程序集` | 插件打包时把宿主程序集（尤其 `Fastdotnet.Plugin.Contracts.dll`）也打进去了；见插件技能"依赖边界" |

## 2. 启动期

| 症状 | 原因与处理 |
| --- | --- |
| 启动直接抛 `启动自检失败：…无法从容器解析` | `DependencySelfCheck` 拦住了漏注册/生命周期不匹配。按清单补注册（`ApplicationModule` 或 `Program.cs`），不要关自检 |
| `Address already in use`（18889） | 上一个 `dotnet run` 没退；`Get-NetTCPConnection -LocalPort 18889` 找 PID 后处理 |
| 连不上数据库 | Development 默认 SQLite；若切了 MySQL/PostgreSQL，确认 `SqlSugar:Connections` 的 `main` 与 `log` 都改了，且连接串可用 |
| OIDC 授权码跨重启失效 | `DataProtection-Keys/` 被删或工作目录变了；恢复目录，别改 `SetApplicationName` |
| 启动日志里插件加载失败 | 看下一节；插件异常通常只打印 `Console`，注意红色输出 |

## 3. 插件加载 / 启用

| 症状 | 原因与处理 |
| --- | --- |
| `找不到插件目录` | 插件没放到 `AppDomain.BaseDirectory/plugins/{PluginId}/`（`backend/Fastdotnet.WebApi/bin/Debug/net10.0/plugins/`） |
| `找不到入口文件 X.dll` | `plugin.json.entryPoint` 与实际 DLL 文件名不一致；入口 DLL 必须在插件目录根（不是 `dependencies/`） |
| `插件授权验证失败!` | 缺 `plugin.lic`、lic 与 PluginId/机器指纹/授权码不匹配，或目录名≠PluginId。开发插件需从商城开发者中心获取许可 |
| `无法解析 plugin.json` | JSON 语法错误（注释、尾逗号）；宿主会用 Newtonsoft 反序列化并**重写**该文件（格式化、补 `ManualStop`） |
| 启用成功但接口 404 | 插件控制器没有 `[ApiUsageScope]`（宿主会移除）；或路径不是 `/api/plugins/{admin\|app\|shared}/p{id}/...` |
| 启用后 Swagger 没有插件文档 | 插件 DLL 无 XML 注释/未复制 `.xml`；或插件未真正启用。看 `/swagger` 的 `plugin-{id}-admin` 分组 |
| 停用后 DLL 仍被占用 | 影子复制在 `%TEMP%/Fastdotnet_Plugins_ShadowCopy/{id}`；停用流程会清理，若失败检查是否有后台线程持有类型引用 |
| 热更新后行为还是旧的 | 只替换文件不会自动重载；走「停用 → 启用」，或重启宿主。确认替换的是运行目录 `plugins/` 下的文件 |

## 4. HTTP 行为

| 症状 | 原因与处理 |
| --- | --- |
| 所有接口 401 | 默认全局策略要求「已认证 + ApiScope」；带 `Authorization: Bearer`，或对匿名端点显式 `[AllowAnonymous]` |
| 403 且日志无异常 | 权限码不匹配：`[Authorize(Policy="x")]` 的 `x` 必须是数据库里存在的权限码；`SUPER_ADMIN` 才会无条件通过 |
| 404 但控制器明明存在 | ① 插件路由前缀被重写；② 控制器被 `[ApiUsageScope]` 缺失移除；③ 宿主控制器可能被 `<Compile Remove>` 排除 |
| 405 / 路由参数对不上 | `[Route]` 模板与实际请求不一致；插件控制器的 `api/` 前缀会被替换，不要把完整前缀写死 |
| 响应被包了两层 `{Code,Data}` | 你手动返回了 `ApiResult`，又经 `GlobalResultFilter` 包装；直接返回 DTO 即可 |
| 400 不是统一格式 | 自动模型验证被关闭，参数错误需要自己抛 `BusinessException` 或返回 `BadRequest` |
| 防重放 408/409 | 本地 Development 应已关闭；若打开，检查请求头时间戳/nonce 与服务器时间偏差 |
| CORS 失败 | Development 才 `UseCors()`；生产跨域要在网关/宿主显式配置，不要加 `AllowAnyOrigin + AllowCredentials` 组合 |

## 5. 前端

| 症状 | 原因与处理 |
| --- | --- |
| `pnpm install` 后 Vite 起不来 / esbuild 报错 | `pnpm-workspace.yaml` 缺 `allowBuilds: esbuild: true`，或 Node 版本过低 |
| `@fastdotnet/shared` 解析失败 | 公共层被按子路径导入但文件不存在；或应用没加 `"@fastdotnet/shared": "workspace:*"` |
| 页面白屏、Console 报 `Cannot read properties of undefined` | 多为登录态/菜单/路由初始化顺序问题；先确认 token、`initializePlugins()`、路由守卫 |
| 插件页 404 / 资源 404 | 插件前端没构建到 `plugins/{id}/wwwroot/admin`，或 `micro-index.html` 里脚本路径大小写不匹配（Linux 区分大小写） |
| 插件接口全部失败 | 传给子应用的 props 名必须是 `FdRequest`（大小写敏感）；子应用 `init(props)` 要保存并使用它 |
| 刷新 `/micro/{id}/xxx` 404 | 插件静态资源未启用 SPA 回退（`wwwroot/publish` 才有回退）；插件 admin/app 目录默认 `false`，路由应在前端内部处理 |
| 样式互相污染 | qiankun `strictStyleIsolation` 默认关闭、`experimentalStyleIsolation` 开启；插件样式用 scoped + 命名空间前缀 |
| 改 shared 不生效 | Vite 预构建缓存；`--force` 或清 `node_modules/.vite` |

## 6. 守门脚本

| 症状 | 处理 |
| --- | --- |
| `verify-docs.ps1` 报断链 | 按提示补文件或改链接（VitePress 链接不带 `.md`） |
| 报"文档中使用的 API 在代码中不存在" | 要么改文档，要么先实现该扩展方法；外部库 API 加到 `scripts/doc-api-allowlist.txt` |
| `check-debt.ps1` 报 `null!` 增长 | 把 `null!` 改成真实可空性处理；不要调高基线 |
| 报 `<Compile Remove>` 增长 | 删掉无用文件，或恢复编译并修好；同样不要调高基线 |

## 7. 快速取证命令

```powershell
# 端口占用
Get-NetTCPConnection -LocalPort 18889 -ErrorAction SilentlyContinue | Select-Object OwningProcess

# 健康检查 / Swagger
Invoke-RestMethod http://localhost:18889/health
Start-Process http://localhost:18889/swagger

# 运行目录里的插件清单
Get-ChildItem backend\Fastdotnet.WebApi\bin\Debug\net10.0\plugins -Directory |
  ForEach-Object { $j = Join-Path $_.FullName 'plugin.json'; if (Test-Path $j) { Get-Content $j -Raw } }

# 影子复制残留
Get-ChildItem $env:TEMP\Fastdotnet_Plugins_ShadowCopy -Directory -ErrorAction SilentlyContinue
```

排错结论请写回仓库（`docs/` 对应章节或技能的 troubleshooting），下次别人不用再踩一遍。
