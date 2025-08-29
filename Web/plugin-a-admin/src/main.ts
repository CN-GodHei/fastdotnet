import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import routes from './router'
// --- 添加类型导入 ---
import type { AxiosInstance } from 'axios';

let app: ReturnType<typeof createApp> | null = null;

// --- 添加一个变量来存储接收到的 FdRequest 实例 ---
let sharedFdRequest: AxiosInstance | null = null;
// --- 添加结束 ---

// --- 导出一个函数，让插件内部可以获取到共享的 FdRequest 实例 ---
export const getSharedFdRequest = () => sharedFdRequest;
// --- 导出结束 ---

const init = (props: any = {}) => {
  // --- 从 props 中解构出 FdRequest ---
  const { container, FdRequest } = props
  // --- 解构结束 ---

  // --- 保存接收到的 FdRequest 实例 ---
  sharedFdRequest = FdRequest;
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

  // --- 可选：将 FdRequest 实例挂载到 app.config.globalProperties 上 ---
  // app.config.globalProperties.$FdRequest = FdRequest;
  // --- 可选结束 ---

  const mountPoint = container ? container : '#app'
  app.mount(mountPoint)

  return {
    app,
    router,
    pinia,
    // --- 将 FdRequest 实例也返回 ---
    FdRequest,
    // --- 返回结束 ---
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
      // --- 清理共享的 FdRequest 实例 ---
      sharedFdRequest = null;
      // --- 清理结束 ---
    }
  }
}

if (!(window as any).__POWERED_BY_QIANKUN__) {
  console.log('[PluginA] not 我进来了');
  init()
}

export default init
