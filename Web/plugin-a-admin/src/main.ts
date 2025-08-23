import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'
import App from './App.vue'
import routes from './router'

let app: ReturnType<typeof createApp> | null = null;

const init = (props: any = {}) => {
  const { container } = props
  
  app = createApp(App)
  
  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? props.base : '/'
  const router = createRouter({
    history: createWebHistory(historyBase),
    routes,
  })
  const pinia = createPinia()

  app.use(router)
  app.use(pinia)
  
  const mountPoint = container ? container.querySelector('#app') : document.getElementById('app')
  if (mountPoint) {
    app.mount(mountPoint)
  } else {
    console.error('Failed to find mount point for standalone app')
  }
  
  return { 
    app, 
    router, 
    pinia,
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
    }
  }
}

if (!(window as any).__POWERED_BY_QIANKUN__) {
  init()
}

export default init
