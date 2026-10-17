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
├── utils/     # 与具体应用无关的工具函数
├── types/     # 全局类型声明（EmptyObjectType 等）
├── stores/    # 与端无关的 Pinia store（microApps）
├── plugins/   # 插件运行时（微前端）：PluginAPI / PluginLoader / PluginManager / PluginRegistry
├── runtime/   # 应用级依赖注入缝（见下）
└── index.ts   # 桶文件（仅做汇总，不承载逻辑）
```

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
| 与端无关（格式化、校验、加密、存储封装、水印…） | ✅ 放这里 |
| 只差一个配置项（如 API 前缀、系统类别、插件前端路径段） | ✅ 放这里，用参数注入 |
| 需要各自的 API 客户端（`api/fd-system-api-admin` vs `-app`） | ✅ 通过 `runtime/` 注入，不要让本包直接依赖 `@/api/*` |
| 真分叉（路由表、上传逻辑） | ❌ 留在各自项目 |

> **本包不得出现 `@/` 前缀的导入**——`@` 是各应用自己的 `src` 别名，指向它会破坏"一份代码"的前提。
> 包内部一律使用相对路径。可用 `Get-ChildItem packages/shared/src -Recurse -Filter *.ts | Select-String "from '@/"` 自检。

## 已迁入

| 批次 | 内容 | 文件数 |
| --- | --- | --- |
| Stage 1 | 工具模块 + 全局类型 + `theme/loading.scss` | 21 |
| Stage 2 | 插件运行时（4 个文件）+ `stores/microApps` | 5 |

## 尚未迁入

- `utils/other.ts`、`utils/authFunction.ts`、`utils/plugin-signalr.ts`、`utils/pluginRichTextHandler.ts`
  —— 耦合 `router` / `stores` / `i18n` / 组件，需等这些层也迁入。
- `utils/build.ts` —— 仅被 `vite.config.ts` 在构建期使用，不属于运行期公共工具。
- `utils/request.ts` —— 有 76 处引用，且需合并两端的 498 密钥过期重试逻辑；迁移成本高，暂缓。
- 其余约 185 个完全相同文件（`views` / `layout` / `theme` / `i18n` / `components` / `api`）。

