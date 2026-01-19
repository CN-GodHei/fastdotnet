import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

let app: ReturnType<typeof createApp> | null = null;

// 共享的请求实例和查询构建器变量
let sharedFdRequest: any = null;
let sharedFdQueryBuilder: any = null;
let sharedSignalRManager: any = null;
let sharedUploadService: any = null;
let menuInfo: any = null;

// 导出获取共享实例的函数
export const getSharedFdRequest = () => sharedFdRequest;
export const getSharedFdQueryBuilder = () => sharedFdQueryBuilder;
export const getSharedSignalRManager = () => sharedSignalRManager;
export const getSharedUploadService = () => sharedUploadService;
export const getMenuInfo = () => menuInfo;

const init = (props: any = {}) => {
  const { container, FdRequest, FdQueryBuilder, signalRManager, menuInfo: propsMenuInfo } = props

  // 保存接收到的共享实例
  sharedFdRequest = FdRequest;
  sharedFdQueryBuilder = FdQueryBuilder;
  sharedSignalRManager = signalRManager;
  sharedUploadService = props.uploadService;
  menuInfo = propsMenuInfo;

  app = createApp(App)

  // 设置路由基础路径
  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? props.base : '/'
  const router = createRouter({
    history: createWebHistory(historyBase),
    routes: [],
  })
  const pinia = createPinia()

  app.use(router)
  app.use(pinia)
  app.use(ElementPlus)

  // 挂载应用
  const mountPoint = container ? container.querySelector('#app') || container : '#app'
  app.mount(mountPoint)

  return {
    app,
    router,
    pinia,
    FdRequest,
    FdQueryBuilder,
    signalRManager,
    menuInfo,
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
      // 清理共享实例
      sharedFdRequest = null;
      sharedFdQueryBuilder = null;
      sharedSignalRManager = null;
      sharedUploadService = null;
      menuInfo = null;
    }
  }
}

// 非微前端环境下的直接启动
if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init