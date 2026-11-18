# 插件交付验收清单

> 每一项都能用命令或界面验证；不能验证的写清原因，不要打勾。

## A. 标识与结构

- [ ] PluginId 与商城申请一致；`plugin.json.id`、csproj `<PluginId>`、部署目录名、`plugin.lic` 四者相同
- [ ] 目录结构与模板一致（`Backend/`、`Frontend/`、`publish/{PluginId}/`）
- [ ] `plugin.json` 是合法 JSON（无注释），`entryPoint` 指向根目录真实存在的 DLL
- [ ] 程序集内只有**一个** `PluginBase`/`IPlugin` 实现
- [ ] 没有把 `Fastdotnet.Plugin.Contracts.dll` 等宿主程序集打进包
- [ ] `plugin.lic` 已放入 `publish/{PluginId}/`

## B. 后端

- [ ] `dotnet build backend/Plugins/<Name>/Backend/<Name>.csproj -c Release` 通过，0 warning（或与基线一致）
- [ ] 所有控制器都标了 `[ApiUsageScope]`（或继承自带该特性的基类）
- [ ] 需要鉴权的接口标了 `[Authorize(Policy="...")]`，权限码已在 `IPermissionProvider` 声明
- [ ] 匿名接口显式 `[AllowAnonymous]`，且没有泄露数据
- [ ] 入参显式校验；业务失败抛 `BusinessException`
- [ ] 返回 DTO/`PageResult`，没有手写 `{code,data,msg}`
- [ ] 实体标 `[SugarTable]`，继承 `BaseEntity`/`AuditableEntity`；未手写 Id/时间
- [ ] 服务注册在 `ConfigureServices`；没有跨插件直接引用类型
- [ ] 事件处理器/工具提供者/指标提供者/信号 R Provider 均已注册
- [ ] 卸载路径无静态引用/未停定时器/未退订事件

## C. 前端（如有）

- [ ] `pnpm build` 成功，产物落在 `publish/{PluginId}/wwwroot/{admin|app}`
- [ ] `micro-index.html`（构建后 index.html）引用的 JS **和 CSS** 路径**小写** `/plugins/{id}/...` 且文件名正确
- [ ] vite-plugin-qiankun 的 name = PluginId；UMD 全局名 = PluginId
- [ ] `micro-main.ts` 导出 `bootstrap/mount/unmount/update`
- [ ] `init(props)` 正确接收 `FdRequest`（大小写）并清理资源
- [ ] 路由 history base 使用 `props.base`；插件内部路径不带 `/micro/{id}`
- [ ] `openapi2ts` 从 `plugin-{id}-admin/-app` 生成，未手改生成代码
- [ ] 样式 scoped / 命名空间前缀，未全局覆盖 Element Plus

## D. 菜单与权限

- [ ] 菜单初始化器幂等（按 `Code` 查重）
- [ ] 插件页面菜单带 `PluginId` + `IsFdMicroApp=true`，`Path=/micro/{id}/...`
- [ ] `Code` 全局唯一，`ParentCode` 正确
- [ ] 权限码小写点分、带插件前缀，`Category`/`Type` 正确
- [ ] 删除权限码前确认没有角色引用（同步会删库里的旧码）

## E. 运行验证（必须实测）

- [ ] 部署到运行目录 `plugins/{PluginId}/`，管理端「插件管理」能扫描到
- [ ] 启用成功；宿主控制台无红色异常
- [ ] Swagger 出现 `plugin-{id}-admin` / `-app` 分组
- [ ] 每个接口按 `/api/plugins/...` 实际调用通过（含 401/403 用例）
- [ ] 菜单出现，点击能打开插件页面；刷新/深链行为符合预期
- [ ] 有权限/无权限用户的表现都验证
- [ ] 停用成功：菜单不可用、接口 404、DLL 可删除
- [ ] 再次启用成功（证明卸载干净）
- [ ] 升级覆盖文件 → 停用/启用 → 功能正常

## F. 安全与合规

- [ ] 无密钥/连接串/证书进入包或仓库
- [ ] 敏感数据出参脱敏（`.MaskSensitiveData()`）
- [ ] 文件上传限制类型/大小，路径不穿越
- [ ] SQL 全部参数化（`IStorageContext` 传参，不拼字符串）
- [ ] 破坏性 MCP 工具 `IsDestructive=true`，且权限码正确
- [ ] 第三方库许可证允许分发

## G. 交付物

- [ ] 插件 README：用途、依赖、安装步骤、配置项、已知限制
- [ ] 版本号三处一致
- [ ] 变更日志（CHANGELOG 或发布说明）
- [ ] 安装包 `{Name}-v{Version}.zip` 可在干净环境安装启用

## 命令速查

```powershell
# 构建 + 部署 + 启用（把 {id} 换成真实 PluginId）
dotnet build backend/Plugins/MyPlugin/Backend/MyPlugin.csproj -c Release
$dst = "backend/Fastdotnet.WebApi/bin/Debug/net10.0/plugins/{id}"
New-Item -ItemType Directory -Force $dst | Out-Null
Copy-Item "backend/Plugins/MyPlugin/publish/{id}/*" $dst -Recurse -Force
Invoke-RestMethod -Method Post "http://localhost:18889/api/Plugin/enable/{id}" -Headers @{ Authorization = "Bearer <token>" }

# 检查包里是否混入宿主程序集（应无输出）
Get-ChildItem "backend/Plugins/MyPlugin/publish/{id}/dependencies" -Filter "Fastdotnet*.dll"
```
