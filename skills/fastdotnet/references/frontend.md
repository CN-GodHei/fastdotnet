# 前端开发（宿主侧）

## 1. 工作区与命令

`Web/` 是 **pnpm workspace**（`Web/pnpm-workspace.yaml`）：

```
Web/
├── package.json            根：dev:admin / dev:app / build:admin / build:app / build:all
├── fastdotnet-admin/       管理端（name=fastdotnetadmin，端口 18888，Vite 7）
├── fastdotnet-app/         应用端（name=fastdotnetapp，端口 18887，Vite 7，qiankun 3 rc）
└── packages/shared/        @fastdotnet/shared（源码直连，无构建产物）
```

```bash
cd Web
pnpm install                      # 首次 / 依赖变更后
pnpm dev:admin                    # 18888
pnpm dev:app                      # 18887
pnpm build:admin                  # 产物在 fastdotnet-admin/dist
pnpm --filter fastdotnetadmin <script>   # 只跑管理端脚本
```

- `pnpm-workspace.yaml` 里 `allowBuilds` 放行了 `esbuild` 等安装脚本；**pnpm 11 默认拦截安装脚本**，
  如果出现 `vite build` 报 esbuild 二进制缺失，先检查这里而不是重装依赖。
- 插件前端（`backend/Plugins/*/Frontend/*`）**不在** `Web` 工作区内，各自独立 `pnpm install`。
  插件模板用 Vite 7 + `vite-plugin-qiankun`（已实测可构建 UMD）；历史插件如 PluginA 仍是 Vite 5。
  不要把插件依赖提升到 `Web` 根，版本会打架（Vite 5/7 混用、qiankun 2/3 混用）。

## 2. 公共层 `@fastdotnet/shared`

- 以「源码直连」方式被两个应用消费（`main/types` 都指向 `src/index.ts`），**没有 dist**。改公共层立即影响两个应用。
- 导入用子路径，避免整包引入：

```ts
import request from '@fastdotnet/shared/utils/request';
import { useMicroAppsStore } from '@fastdotnet/shared/stores/microApps';
import { pluginManager } from '@fastdotnet/shared/plugins/PluginManager';
import { configurePluginHost, registerPluginScanApi, registerPluginRequest }
  from '@fastdotnet/shared/runtime/pluginHost';
```

- **公共层里禁止 `@/` 别名**（那是各应用自己的 `src`）。需要应用专属能力时，在 `runtime/pluginHost.ts`
  这类"注入缝"里暴露注册函数，由应用入口注入；依赖方向永远是「应用 → 公共层」。
- 新增跨端复用代码前先问：两个应用都需要吗？只在一个应用用的，留在该应用 `src/utils`。

## 3. 请求封装（`@fastdotnet/shared/utils/request`）

- Axios 实例：
  - 开发：`baseURL = ''`，走 Vite 代理（`/api`、`/api/universalhub`、`/plugins/` → `VITE_API_URL`）。
  - 生产：`baseURL = VITE_API_URL`。
- 请求拦截器：注入 `Authorization: Bearer <token>`、`System-Category`（Admin/App）、防重放头（时间戳 + nonce）、
  必要时加密请求体（`encryptRequest`）。
- 响应拦截器：校验 `Code === 200`，否则弹错并 reject；处理 AES/RSA 加密响应；**成功后返回 `Data`**。
  所以业务代码拿到的是数据本身，不是 `{Code,Msg,Data}`。
- 登录态：`Session.get('token')`；401/4001 自动登出。

## 4. 页面与路由

- 视图放 `src/views/`，按业务分目录；组件 `src/components/`；状态 `src/stores/`（Pinia）；接口 `src/api/`（openapi2ts 生成，勿手改）。
- 路由在 `src/router/`：动态菜单路由由后端菜单树驱动，静态路由只放登录/错误页等。
- 统一用 `<script setup lang="ts">` + Composition API；Element Plus 图标以 `ele-` 前缀注册（`ele-HomeFilled`），
  与后端 `FdMenu.Icon` 存的值一致，新增图标先确认前缀。
- i18n 在 `src/i18n/`；新增文案同时补 `zh-cn`/`en`，不要硬编码中文到模板。
- 主题/暗黑：`src/theme/` 与 `packages/shared/src/theme`，优先用已有 CSS 变量，别写死颜色。

## 5. 微前端（qiankun）宿主侧

管理端 `src/main.ts` 做了三件事：

```ts
configurePluginHost({ frontendSegment: 'admin', strictStyleIsolation: false });
registerPluginScanApi(getPluginScanPlugins);   // @/api/fd-system-api-admin/plugin
registerPluginRequest(request);                // 主应用 axios，会作为 FdRequest 传给子应用
```

插件启动流程（登录后 `startQiankun()`）：

1. `initializePlugins()`：调后端 `GET /api/Plugin/scan`，注册插件元数据，生成
   `microAppConfig = { name, entry: `${VITE_API_URL}/plugins/{id}/admin/index.html`, container: '#subapp-viewport', activeRule: `/micro/{id}` }`。
2. 读用户菜单树，凡 `IsFdMicroApp && PluginId` 的菜单生成子应用配置（同一插件只生成一次）。
3. `microAppsStore` 保存配置，路由进入 `/micro/{PluginId}/...` 时渲染 `<div id="subapp-viewport">` 并由 qiankun 加载。

传给子应用的 props：`base`（`/micro/{id}`）、`FdRequest`（**大小写敏感**，曾因写成 `Fdrequest` 导致插件所有请求失败）、
`FdQueryBuilder`、`signalRManager`、`uploadService`、`menuInfo`。

> 本地调试插件前端：`main.ts` 的 `getDebugEntry(pluginId)` 可返回 `//localhost:8099`，绕过"每次改都要重新 build 到 plugins 目录"。

## 6. 常见坑

| 症状 | 原因 / 处理 |
| --- | --- |
| 控制台 `Failed to resolve import '@/...'`（在 shared 包内） | 公共层写了应用别名；改成注入缝或相对导入 |
| 插件页面白屏、Network 里 `index.html` 404 | 后端没部署插件前端产物，或路径段写错（`admin`/`app`）；确认 `plugins/{id}/wwwroot/admin/index.html` 存在 |
| 插件能打开但接口 404 | 插件控制器缺少 `[ApiUsageScope]` 被宿主移除；或请求没走 `/api/plugins/...` |
| 插件里请求全挂，控制台说 FdRequest 不存在 | 宿主传 props 的名字/时机问题；核对 `PluginManager.loadPlugin` 与子应用 `init(props)` |
| `pnpm dev` 端口不是 18888/18887 | 端口在 `.env` 的 `VITE_PORT`，不要改代码常量 |
| 修改 shared 后某端不生效 | Vite 缓存；`pnpm dev` 带 `--force`（admin 脚本已带）或删 `node_modules/.vite` |

## 7. 构建与验收

```bash
cd Web
pnpm build:admin
pnpm build:app
```

- 构建失败先看**该应用**的 `package.json` 脚本与 `vite.config.ts`（两个应用配置不同，不要互相套用）。
- 前端改动验收：`pnpm dev:*` 打开对应端口 → 登录 → 走一遍改动路径 → 浏览器控制台无报错；
  涉及插件页面时再验证 qiankun 挂载/卸载（切走再切回不重复挂载）。
