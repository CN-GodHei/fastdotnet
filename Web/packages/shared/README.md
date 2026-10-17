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
└── index.ts   # 桶文件（仅做汇总，不承载逻辑）
```

## 什么该放进来

判断标准只有一条：**这段代码是否需要知道"自己是管理端还是应用端"？**

| 情况 | 处置 |
| --- | --- |
| 与端无关（格式化、校验、加密、存储封装、水印…） | ✅ 放这里 |
| 只差一个配置项（如 API 前缀、系统类别） | ✅ 放这里，用参数注入 |
| 需要各自的 API 客户端（`api/fd-system-api-admin` vs `-app`） | ⚠️ 通过适配层注入，不要让本包直接依赖 `@/api/*` |
| 真分叉（路由表、插件状态管理、上传逻辑） | ❌ 留在各自项目 |

> **本包不得出现 `@/` 前缀的导入**——`@` 是各应用自己的 `src` 别名，指向它会破坏"一份代码"的前提。
> 包内部一律使用相对路径。

## 当前状态（Stage 1）

已迁入：14 个自包含的工具模块（见 `src/utils/`）。

尚未迁入（后续阶段）：

- `utils/other.ts`、`utils/authFunction.ts`、`utils/plugin-signalr.ts`、`utils/pluginRichTextHandler.ts`
  —— 它们耦合了 `router` / `stores` / `i18n` / 组件，需等这些层也迁入后再处理。
- `utils/build.ts` —— 仅被 `vite.config.ts` 在构建期使用，不属于运行期公共工具。
- 其余 190+ 个完全相同文件（`components` / `stores` / `theme` / `i18n` / `views` / `layouts`）。
