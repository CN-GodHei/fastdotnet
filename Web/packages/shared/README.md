# @fastdotnet/shared

Fastdotnet 前端的**公共层**：管理端（`fastdotnet-admin`）与应用端（`fastdotnet-app`）共享的工具与类型。

## 为什么存在

两个前端原本是复制分叉的：231 个同路径文件里 **205 个字节级完全相同（约 25,237 行）**，
导致任何改动都要在两个项目里各改一遍、且极易漏改。

本包的目标是：**公共部分只有一份。**

## 如何使用

两个前端通过 `workspace:*` 依赖本包，并在 `vite.config.ts` / `tsconfig.json` 中把
`@fastdotnet/shared` 指向本包源码（源码直连，无构建产物，改完即生效）。

```ts
// 迁移前（两份，各改一次）
import { formatTime } from '@/utils/formatTime';

// 迁移后（一份）
import { formatTime } from '@fastdotnet/shared/utils/formatTime';
```

## 目录约定

```
src/
├── utils/       # 与具体应用无关的工具函数
├── types/       # 全局类型声明（EmptyObjectType 等）
├── theme/       # 全局样式（含 element-plus 覆盖、媒体查询、mixins）
├── components/  # 与端无关的通用组件（cropper / editor / iconSelector / noticeBar / svgIcon）
├── stores/      # 与端无关的 Pinia store（microApps）
├── plugins/     # 插件运行时（微前端）：PluginAPI / PluginLoader / PluginManager / PluginRegistry
├── runtime/     # 应用级依赖注入缝（见下）
└── index.ts     # 桶文件（仅做汇总，不承载逻辑）
```

## ⚠️ 两条硬约束

### 1. 第三方依赖必须在本包 `package.json` 中声明

pnpm 使用隔离式 `node_modules`，**本包未声明的包一律解析不到**，构建会直接失败：

```
[vite]: Rollup failed to resolve import "pinia" from ".../packages/shared/src/stores/microApps.ts"
```

本包当前声明了 `pinia`、`qiankun`、`axios`、`element-plus`、`@wangeditor/*`、`cropperjs` 等。
**搬入新文件后，务必检查其 import 的第三方包是否已声明。**

> 反面教材：`axios` 漏声明时，`types/axios.d.ts` 的模块增强会**静默失效**——
> 不报错，但会让 `AxiosResponse` 上的属性访问全部变成类型错误（实测引发 15 个错误）。

### 2. 本包内不得出现 `@/` 别名，也不得自引用 `@fastdotnet/shared`

`@` 是各应用自己的 `src` 别名，指向它会破坏"一份代码"的前提；自引用别名则让本包依赖消费者的配置。

```powershell
# 自检：应当只剩注释与报错文案中的出现
Get-ChildItem packages/shared/src -Recurse -File | Select-String -Pattern "@/" 
```

> 注意：只检查 `from '@/...'` 是不够的——`<style>` 块里的 `@import '@/theme/x.scss'`
> 和动态 `import('@/components/x')` 同样会漏网（实测踩过）。

## 依赖注入缝（runtime/）

公共层不得依赖 `@/api/*`（那是各应用自己生成的 openapi2ts 客户端）。
凡是「公共层需要、但只有应用才知道」的能力，一律走注册点：

| 文件 | 注入内容 | 由谁在何时注册 |
| --- | --- | --- |
| `runtime/publicKey.ts` | 获取加密公钥 | 各应用 `main.ts` |
| `runtime/pluginHost.ts` | 插件扫描接口、主应用 axios 实例、插件前端路径段、qiankun 样式隔离 | 各应用 `main.ts` |

```ts
// 应用入口（main.ts）示例
import { configurePluginHost, registerPluginRequest, registerPluginScanApi } from '@fastdotnet/shared/runtime/pluginHost';
import { getPluginScanPlugins } from '@/api/fd-system-api-admin/plugin';
import request from '@/utils/request';

configurePluginHost({ frontendSegment: 'admin', strictStyleIsolation: false });
registerPluginScanApi(getPluginScanPlugins);
registerPluginRequest(request);
```

未注册时公共层会**抛出带指引的错误**，而不是静默返回 undefined。

## 什么该放进来

判断标准只有一条：**这段代码是否需要知道"自己是管理端还是应用端"？**

| 情况 | 处置 |
| --- | --- |
| 与端无关（格式化、校验、加密、存储封装、水印、全局样式、通用组件…） | ✅ 放这里 |
| 只差一个配置项（如 API 前缀、系统类别、插件前端路径段） | ✅ 放这里，用参数注入 |
| 需要各自的 API 客户端（`api/fd-system-api-admin` vs `-app`） | ✅ 通过 `runtime/` 注入，不要让本包直接依赖 `@/api/*` |
| 真分叉（路由表、上传逻辑） | ❌ 留在各自项目 |

## 已迁入

| 批次 | 内容 | 文件数 |
| --- | --- | --- |
| Stage 1 | 工具模块 + 全局类型 + `theme/loading.scss` | 21 |
| Stage 2a | 插件运行时（4 个文件）+ `stores/microApps` | 5 |
| Stage 2b | `theme/`（25）+ 通用组件（6） | 31 |
| Stage 3a | 框架页（401/404/personal/tools/login 的 mobile+scan） | 6 |
| Stage 3b | `utils/request.ts` + `api/fd-system-api-app/`（17） | 18 |

## 尚未迁入

- **`layout/`（27 个文件）** —— 会牵出整层基础设施，需先解决：
  - `stores/`（`themeConfig` 两端差 2 行、`routesList`、`tagsViewRoutes`、`keepAliveNames`、`index`）
  - `assets/`（3 个 svg，两端相同）
  - `utils/other.ts` —— 耦合 `@/router/index` 与 `@/i18n/index`，**需要新增 router / i18n 两个注入缝**
  - `views/home`、`views/login/index` —— 同样依赖上述 stores
- **`i18n/`（10 个文件）** —— ⚠️ **不是简单搬文件**。`i18n/index.ts` 用
  `import.meta.glob('./**/*.ts', { eager: true })` 按目录自动装载语言包，
  文件搬走后 glob 扫不到，**翻译会静默丢失且构建不报错**。
- `components/auth/*`、`components/table`、`components/upload/*` —— 耦合 `stores/userInfo` / `stores/themeConfig`。
- `utils/authFunction.ts`、`utils/plugin-signalr.ts`、`utils/pluginRichTextHandler.ts`、`utils/build.ts`。
- `directive/`（3）、`App.vue`、`router/` 中的部分文件。



