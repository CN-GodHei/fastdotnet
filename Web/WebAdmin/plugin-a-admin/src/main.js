import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'

let router = null
let instance = null

// qiankun 生命周期钩子
export async function bootstrap() {
  console.log('PluginA bootstraped')
}

export async function mount(props) {
  console.log('PluginA mount', props)
  render(props)
}

export async function unmount() {
  console.log('PluginA unmount')
  instance.unmount()
  instance = null
  router = null
}

function render(props = {}) {
  const { container } = props
  router = createRouter({
    history: createWebHistory('/plugin-a'),
    routes: [
      {
        path: '/',
        component: () => import('./views/Index.vue')
      }
    ]
  })

  instance = createApp(App)
  instance.use(router)
  instance.use(ElementPlus)
  instance.mount(container ? container.querySelector('#app') : '#app')
}

// 独立运行时
if (!window.__POWERED_BY_QIANKUN__) {
  render()
}
