import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import App from './App.vue'
import routes from './router'

let app: ReturnType<typeof createApp> | null = null
let sharedFdRequest: any = null

/** 供 api 适配器取宿主注入的 axios 实例 */
export const getSharedFdRequest = () => sharedFdRequest

const init = (props: any = {}) => {
  // 宿主通过 props 注入；注意大小写：FdRequest
  const { container, base, FdRequest, FdQueryBuilder, signalRManager, uploadService, menuInfo } = props
  sharedFdRequest = FdRequest ?? null

  app = createApp(App)
  app.use(ElementPlus, { locale: zhCn, size: 'small' })
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(`ele-${key}`, component)
  }

  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? (base ?? '/') : '/'
  app.use(createRouter({ history: createWebHistory(historyBase), routes }))
  app.use(createPinia())

  app.mount(container ?? '#app')

  // 需要时把宿主能力挂到全局（按需使用）
  ;(window as any).__FD_PLUGIN_PROPS__ = {
    FdRequest, FdQueryBuilder, signalRManager, uploadService, menuInfo
  }

  return {
    unmount() {
      app?.unmount()
      app = null
      sharedFdRequest = null
      delete (window as any).__FD_PLUGIN_PROPS__
    }
  }
}

// 独立运行（pnpm dev）
if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init
