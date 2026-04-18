import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import routes from './router'
// --- 添加类型导入 ---
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
  // --- 从 props 中解构出 request、FdQueryBuilder 和 SignalRManager ---
  const { container, request, FdQueryBuilder, signalRManager, menuInfo: propsMenuInfo } = props
  // --- 解构结束 ---

  // --- 保存接收到的 request 实例、查询构建器和SignalR管理器 ---
  sharedFdRequest = request;
  sharedFdQueryBuilder = FdQueryBuilder;
  sharedSignalRManager = signalRManager;
  // --- 保存上传服务 ---
  sharedUploadService = props.uploadService;
  // --- 保存菜单信息 ---
  menuInfo = propsMenuInfo;
  // --- 保存结束 ---

  app = createApp(App)

  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? props.base : '/'
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

  const mountPoint = container ? container : '#app'
  app.mount(mountPoint)

  return {
    app,
    router,
    pinia,
    // --- 将 request 实例、查询构建器和SignalR管理器也返回 ---
    request,
    FdQueryBuilder,
    signalRManager,
    menuInfo,
    // --- 返回结束 ---
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
      // --- 清理共享的 FdRequest 实例、查询构建器和SignalR管理器 ---
      sharedFdRequest = null;
      sharedFdQueryBuilder = null;
      sharedSignalRManager = null;
      // --- 清理上传服务 ---
      sharedUploadService = null;
      // --- 清理菜单信息 ---
      menuInfo = null;
      // --- 清理结束 ---
    }
  }
}

if (!(window as any).__POWERED_BY_QIANKUN__) {
  //console.log('[PluginA] not 我进来了');
  init()
}

export default init