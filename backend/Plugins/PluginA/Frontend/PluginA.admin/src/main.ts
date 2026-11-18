import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import routes from './router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import type { AxiosInstance } from 'axios'
// BaseSignalRManager 类型在主应用中定义，这里使用 any 类型

let app: ReturnType<typeof createApp> | null = null;

// --- 添加一个变量来存储接收到的 FdRequest 实例 ---
let sharedFdRequest: AxiosInstance | null = null;
// --- 添加查询构建器变量 ---
let sharedFdQueryBuilder: any | null = null;
// --- 添加SignalR管理器变量 ---
let sharedSignalRManager: any | null = null; // 使用 any 类型，因为 BaseSignalRManager 在主应用中定义
// --- 添加上传服务变量 ---
let sharedUploadService: any | null = null;
// --- 添加菜单信息变量 ---
let menuInfo: any | null = null;
// --- 添加结束 ---

// --- 导出一个函数，让插件内部可以获取到共享的 FdRequest 实例 ---
export const getSharedFdRequest = () => sharedFdRequest;
// --- 导出查询构建器 ---
export const getSharedFdQueryBuilder = () => sharedFdQueryBuilder;
// --- 导出SignalR管理器 ---
export const getSharedSignalRManager = () => sharedSignalRManager;
// --- 导出上传服务 ---
export const getSharedUploadService = () => sharedUploadService;
// --- 导出菜单信息 ---
export const getMenuInfo = () => menuInfo;
// --- 导出结束 ---

const init = (props: any = {}) => {
  // --- 从 props 中解构出 FdRequest、FdQueryBuilder、SignalRManager 和 uploadService ---
  const { container, FdRequest, FdQueryBuilder, signalRManager, menuInfo: propsMenuInfo } = props
  // --- 解构结束 ---

  // --- 保存接收到的 FdRequest 实例、查询构建器、SignalR管理器和上传服务 ---
  sharedFdRequest = FdRequest;
  sharedFdQueryBuilder = FdQueryBuilder;
  sharedSignalRManager = signalRManager;
  // --- 保存上传服务 ---
  sharedUploadService = props.uploadService;
  menuInfo = propsMenuInfo;
  // --- 保存结束 ---

  app = createApp(App)

  // 注册 Element Plus（配置中文语言包）
  app.use(ElementPlus, {
    locale: zhCn,
    size: 'small'
  })

  // 注册 Element Plus 图标（使用 ele- 前缀，与主应用保持一致）
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(`ele-${key}`, component)
  }

  // qiankun 3.0：base 仍由宿主通过 props 传入（宿主只走 loadMicroApp，不用 activeRule），
  // 独立运行（vite dev / 直接刷新）时没有 props.base，退回 '/'。
  const historyBase = props.base || '/'
  const router = createRouter({
    history: createWebHistory(historyBase),
    routes,
  })
  const pinia = createPinia()

  app.use(router)
  app.use(pinia)

  // --- 可选：将 FdRequest 实例和查询构建器挂载到 app.config.globalProperties 上 ---
  // app.config.globalProperties.$FdRequest = FdRequest;
  // app.config.globalProperties.$FdQueryBuilder = FdQueryBuilder;
  // --- 可选结束 ---

  // qiankun 3.0：官方推荐渲染到 props.container 的子树（本应用 HTML 里的 #app），
  // 而不是全局 document。宿主容器里已注入 index.html 的 body 内容，故优先取 #app。
  const mountPoint =
    (container && typeof container.querySelector === 'function'
      ? container.querySelector('#app')
      : null) ||
    container ||
    '#app'

  app.mount(mountPoint)

  return {
    app,
    router,
    pinia,
    // --- 将 FdRequest 实例、查询构建器和SignalR管理器也返回 ---
    FdRequest,
    FdQueryBuilder,
    signalRManager,
    menuInfo,
    // --- 返回结束 ---
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
      // --- 清理共享的 FdRequest 实例、查询构建器、SignalR管理器和上传服务 ---
      sharedFdRequest = null;
      sharedFdQueryBuilder = null;
      sharedSignalRManager = null;
      // --- 清理上传服务 ---
      sharedUploadService = null;
      menuInfo = null;
      // --- 清理结束 ---
    }
  }
}

// ---------------------------------------------------------------------------
// qiankun 生命周期
//
// qiankun 3.0 的原生 ESM 路线：入口模块（index.html 里那个 <script type="module">）
// 的具名导出 == 微应用生命周期，不再需要 2.x 时代的 micro-main.ts 转发薄壳 +
// vite-plugin-qiankun 把生命周期挂到 window 上。
//
// 注意 bootstrap 的参数在 3.0 已收紧为只读的 { name, entry }，不含业务 props；
// 业务 props 只在 mount 里拿（本应用从来如此，不受影响）。
// ---------------------------------------------------------------------------
let appInstance: ReturnType<typeof init> | null = null;

export async function bootstrap() {
  // 无需预热；保留空实现以满足生命周期约定（可省略，但显式声明更清晰）
}

export async function mount(props: any) {
  appInstance = init(props);
}

export async function unmount() {
  if (appInstance && typeof appInstance.unmount === 'function') {
    appInstance.unmount();
  }
  appInstance = null;
}

export async function update(_props: any) {
  // 宿主目前不使用 update 通道；留空以免 qiankun 判定生命周期契约不完整
}

// 独立运行（不经 qiankun，例如直接访问 vite dev server）时自行挂载
if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init
