import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import routes from './router'

// 独立运行时的入口
let app: ReturnType<typeof createApp> | null = null

const init = (props: any = {}) => {
  const { container, base } = props
  
  // 如果应用已经存在，先卸载
  if (app) {
    app.unmount()
  }

  app = createApp(App)
  
  // 如果是 qiankun 环境，使用传入的 base；否则使用默认的 '/'
  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? base : '/'
  const router = createRouter({
    history: createWebHistory(historyBase),
    routes,
  })
  const pinia = createPinia()

  app.use(router)
  app.use(pinia)
  
  // 挂载应用
  const mountPoint = container ? container.querySelector('#app') : document.getElementById('app')
  if (mountPoint) {
    app.mount(mountPoint)
  } else {
    console.error('Failed to find mount point for standalone app')
  }
  
  return { app, router, pinia }
}

// 如果不是在 qiankun 环境中（即独立运行），则直接初始化
if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init