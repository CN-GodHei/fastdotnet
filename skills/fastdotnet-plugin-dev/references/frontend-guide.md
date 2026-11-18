# 插件前端（qiankun 微应用）

插件前端是**独立项目**（不在 `Web/` workspace 内），构建成 UMD 由 qiankun 加载。管理端与宿主共享 Vue/Element Plus 运行时，
但插件运行时由 `vite-plugin-qiankun` 包装，路由前缀是 `/micro/{PluginId}`。

> 模板使用 **Vite 7 + @vitejs/plugin-vue 6 + vite-plugin-qiankun 1.0.15**（本仓库验证过可产出 UMD）。
> 仓库里的 PluginA 仍是 Vite 5，两者都可用，但不要在同一个项目里混用。

## 1. 目录

```
Frontend/
├── MyPlugin.admin/            # 管理端微应用，开发端口 8099
│   ├── index.html             # 独立开发时用
│   ├── micro-index.html       # 微前端入口（构建时复制为 index.html）
│   ├── plugin.json            # 前端元数据（id/name/entryPoint=index.html）
│   ├── vite.config.ts
│   ├── openapi2ts.config.ts
│   ├── package.json
│   └── src/
│       ├── main.ts            # createApp + init(props)，独立运行时自动挂载
│       ├── micro-main.ts      # 导出 bootstrap/mount/unmount/update
│       ├── router/index.ts    # 插件内部路由（/home、/about…）
│       ├── views/
│       └── api/               # openapi2ts 生成 + 适配器
└── MyPlugin.app/              # 应用端微应用，开发端口 8100（结构同上）
```

两个前端可以只做其中一个；没有前端资源的插件只提供 API（此时不需要菜单，也不会有微应用）。

## 2. Vite 配置要点

```ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import qiankun from 'vite-plugin-qiankun'
import path from 'path'
import fs from 'fs'

export default defineConfig({
  plugins: [
    vue(),
    // name 必须是 PluginId：宿主 loadMicroApp 的 name 就是菜单的 PluginId
    qiankun('12345678901234567', { useDevMode: true }),
    {
      name: 'copy-micro-index',
      closeBundle() {
        const src = path.resolve(__dirname, 'micro-index.html')
        const out = path.resolve(__dirname, '../../publish/12345678901234567/wwwroot/admin/index.html')
        if (fs.existsSync(src)) {
          fs.mkdirSync(path.dirname(out), { recursive: true })
          fs.copyFileSync(src, out)
        }
      }
    }
  ],
  server: { port: 8099, headers: { 'Access-Control-Allow-Origin': '*' } },
  base: './',
  build: {
    outDir: '../../publish/12345678901234567/wwwroot/admin',
    emptyOutDir: true,
    lib: {
      name: '12345678901234567',              // 必须是 PluginId（UMD 全局名）
      entry: path.resolve(__dirname, 'src/micro-main.ts'),
      formats: ['umd'],
      fileName: () => `12345678901234567-admin.js`,
      cssFileName: '12345678901234567-admin'  // 产物：assets/12345678901234567-admin.css
    },
    rollupOptions: {
      // 默认不 external，让产物自包含；确认宿主提供 window.Vue 等全局变量后再打开：
      // external: ['vue', 'vue-router', 'pinia', 'element-plus'],
      output: {
        globals: {},
        assetFileNames: 'assets/[name].[ext]'
      }
    }
  }
})
```

`micro-index.html` 必须同时引用 JS 和 CSS（lib 模式不会自动注入 CSS）：

```html
<script src="/plugins/12345678901234567/admin/12345678901234567-admin.js"></script>
<link rel="stylesheet" href="/plugins/12345678901234567/admin/assets/12345678901234567-admin.css" />
```

> ⚠️ **externals 的真相**：external 的库必须由宿主在 `window` 上提供同名全局变量。
> 本仓库宿主默认不注入 `window.Vue`/`window.ElementPlus`（`VITE_OPEN_CDN=false` 时没有 CDN 脚本）。
> 因此**新插件建议先不要 external**（让 Vite 把 vue/element-plus 打进 UMD，产物大但自包含），
> 确认宿主确实提供了全局变量后再 external。PluginA 的配置是历史写法，复用前务必实测。

## 3. 入口与生命周期

`micro-main.ts`：

```ts
import init from './main'

let appInstance: ReturnType<typeof init> | null = null

export async function bootstrap() {}
export async function mount(props: any) { appInstance = init(props) }
export async function unmount() { appInstance?.unmount(); appInstance = null }
export async function update(_props: any) {}
```

`main.ts` 的关键点：

```ts
const init = (props: any = {}) => {
  const { container, FdRequest, FdQueryBuilder, signalRManager, uploadService, menuInfo } = props

  // 保存宿主注入的实例，供 api/*.ts 的适配器使用
  sharedFdRequest = FdRequest

  const app = createApp(App)
  app.use(ElementPlus, { locale: zhCn, size: 'small' })
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(`ele-${key}`, component)     // 与菜单 Icon 的 ele- 前缀一致
  }

  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? props.base : '/'
  app.use(createRouter({ history: createWebHistory(historyBase), routes }))
  app.use(createPinia())
  app.mount(container ? container : '#app')

  return { app, unmount() { app.unmount() } }
}

// 独立运行（pnpm dev 直接访问 8099）
if (!(window as any).__POWERED_BY_QIANKUN__) init()
export default init
```

- **props 名称大小写敏感**：`FdRequest`（不是 `Fdrequest`）。宿主曾因拼写错误导致插件所有请求失败。
- `unmount` 里要清理：卸载 Vue 实例、断开 SignalR、清定时器/事件监听、清空共享实例。
- 路由 `history` 在 qiankun 下必须用 `props.base`（`/micro/{PluginId}`），否则刷新/深链会 404。
- 插件内部路由用相对路径（`/home`、`/about`），不要重复写 `/micro/{id}`。

## 4. 页面如何对应菜单

前端菜单（`FdMenu`）与插件路由是一一对应的：

| 菜单字段 | 前端含义 |
| --- | --- |
| `Path = /micro/{id}/home` | 浏览器地址；qiankun activeRule 命中 `/micro/{id}` |
| 插件路由 `/home` | 微应用内路由（去掉 base 后的部分） |
| `IsFdMicroApp = true` + `PluginId` | 告诉宿主"这是插件页面，需要加载微应用" |
| `IsKeepAlive` | 通过 props `menuInfo.isKeepAlive` 传给子应用，决定是否缓存页面 |

菜单由**后端插件初始化器**插入（见 `backend-guide.md`），不要在前端硬编码。

## 5. 调用后端 API

插件接口被重写成 `/api/plugins/{admin|app|shared}/p{pluginId}/...`。用 `openapi2ts` 从插件 Swagger 生成客户端：

```ts
// openapi2ts.config.ts
export default [{
  schemaPath: 'http://localhost:18889/swagger/plugin-12345678901234567-admin/swagger.json',
  serversPath: './src/api',
  projectName: 'MyPlugin',
  namespace: 'APIModel',
  enumStyle: 'enum',
  requestLibPath: '../../utils/fdRequestAdapter',   // 指向适配器（默认导出）
  isCamelCase: true
}]
```

适配器把生成代码的 `request(url, options)` 转成宿主的 Axios 实例：

```ts
// src/utils/fdRequestAdapter.ts
import { getSharedFdRequest } from '../main'

export default async function fdRequestAdapter(url: string, options: any = {}) {
  const FdRequest: any = getSharedFdRequest()
  if (!FdRequest) throw new Error('[FdRequestAdapter] 宿主未注入 FdRequest')
  const { method = 'GET', params, data, headers, ...rest } = options
  return FdRequest({
    url, method, params, headers, ...rest,
    ...(['POST', 'PUT', 'PATCH', 'DELETE'].includes(String(method).toUpperCase()) ? { data } : {})
  })
}
```

- 生成命令：`pnpm openapi2ts`（在 admin/app 项目内）。
- **不要手改 `src/api/` 生成代码**；接口变更就重新生成。
- 注意 Swagger 分组名是 `plugin-{PluginId 小写}-admin` / `-app`，插件启用后才会出现。

## 6. 本地开发与联调

1. 后端 `dotnet run`（18889），插件已在运行目录且已启用。
2. 插件前端 `pnpm dev`（8099）。
3. 让宿主加载本地 dev server：管理端 `src/main.ts` 的 `getDebugEntry(pluginId)` 返回 `//localhost:8099`，
   或用浏览器直接访问 `http://localhost:8099` 独立运行（此时 `init()` 自动执行，但没有宿主 props，
   `FdRequest` 为空 —— 需要独立登录/请求时自己在 `main.ts` 里做降级）。
4. 改完前端验证生产形态：`pnpm build` → 产物进 `publish/{id}/wwwroot/admin` → 复制到运行目录插件目录 → 停用/启用插件 →
   浏览器强刷（qiankun 会缓存 entry）。

## 7. 常见问题

| 症状 | 处理 |
| --- | --- |
| `[qiankun] ... not found` / 白屏 | `vite-plugin-qiankun` 的 name 必须等于 PluginId；`micro-index.html` 里的脚本路径必须指向构建产物 |
| 脚本 404 | `micro-index.html` 用了 `/Plugins/`（大写 P），宿主映射的是 `/plugins/`。**Linux 区分大小写**，统一小写 |
| 刷新 404 | 插件 admin/app 目录没有 SPA 回退；用 hash 路由或在宿主/网关配置回退（`wwwroot/publish` 才有回退） |
| 样式污染宿主 | 加 `<style scoped>`，根节点加插件前缀类名；不要覆盖 `:root`/Element Plus 全局变量 |
| 图标不显示 | 图标名必须是 `ele-` + Element Plus 图标名，且前端已全量注册 |
| 接口 401 | `FdRequest` 未注入（props 名/顺序）或宿主 token 过期；确认请求走的是宿主 axios |
| 接口 404 | 后端控制器缺 `[ApiUsageScope]`，或用了旧路径 `/api/{controller}` |
| 停用后再次启用白屏 | qiankun 缓存了旧 entry；强刷浏览器，重建插件产物后重新启用 |

## 8. 构建与部署

```bash
cd Frontend/MyPlugin.admin
pnpm install
pnpm build                 # 输出到 ../../publish/{PluginId}/wwwroot/admin
cd ../MyPlugin.app
pnpm install
pnpm build                 # 输出到 ../../publish/{PluginId}/wwwroot/app
```

把 `publish/{PluginId}/wwwroot` 复制到运行目录 `plugins/{PluginId}/wwwroot`，然后停用/启用插件（或重启宿主）。
详见 `packaging-and-debug.md`。

> **pnpm 11 注意**：插件前端是独立项目（不在 `Web` workspace 内），默认会拦截依赖安装脚本，导致 `esbuild` 缺二进制、
> `vite build` 失败。模板已在项目根放 `pnpm-workspace.yaml`：
>
> ```yaml
> allowBuilds:
>   es5-ext: true
>   esbuild: true
>   vue-demi: true
> ```
>
> 若仍提示 `ERR_PNPM_IGNORED_BUILDS`，在项目目录执行 `pnpm approve-builds` 并勾选 esbuild。
