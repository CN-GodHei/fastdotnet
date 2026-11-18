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

  // 路由 base 由宿主 props.base 提供（形如 /micro/{PluginId}）；独立运行时退回 '/'
  app.use(createRouter({ history: createWebHistory(base || '/'), routes }))
  app.use(createPinia())

  // 渲染到 props.container 的子树（本应用 index.html 里的 #app），不要用页面级全局选择器
  const mountPoint =
    (container && typeof container.querySelector === 'function'
      ? container.querySelector('#app')
      : null) ||
    container ||
    '#app'
  app.mount(mountPoint)

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

// ---------------------------------------------------------------------------
// qiankun 生命周期（qiankun 3.0 原生 ESM 路线）
//
// 入口模块的具名导出 == 微应用生命周期。不需要 micro-main.ts 转发薄壳，也不需要
// vite-plugin-qiankun 把生命周期挂到 window 上。
// 注意 bootstrap 的参数在 3.0 已收紧为只读的 { name, entry }，不含业务 props；
// 业务 props 只在 mount 里拿。
// ---------------------------------------------------------------------------
let appInstance: ReturnType<typeof init> | null = null

export async function bootstrap(): Promise<void> {
  // 只执行一次的初始化
}

export async function mount(props: any): Promise<void> {
  appInstance = init(props)
}

export async function unmount(): Promise<void> {
  appInstance?.unmount()
  appInstance = null
}

export async function update(_props: any): Promise<void> {
  // 宿主 props 变化时（主题/菜单等）
}

// 独立运行（pnpm dev，不经 qiankun）时自行挂载
if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init
