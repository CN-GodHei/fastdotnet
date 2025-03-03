import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import { registerMicroApps, start } from 'qiankun'
import App from './App.vue'
import router from './router'
import './mock'
import axios from 'axios'

const app = createApp(App)
const pinia = createPinia()

app.use(router)
app.use(pinia)
app.use(ElementPlus)

app.mount('#app')

// 启动 qiankun
start()

// 动态注册微应用
async function registerPlugin(pluginConfig) {
  registerMicroApps([
    {
      name: pluginConfig.name,
      entry: pluginConfig.entry,
      container: '#subapp-viewport',
      activeRule: `/plugin-${pluginConfig.name.toLowerCase()}`,
    },
  ])
}

// 导出注册插件方法，供其他模块使用
window.registerPlugin = registerPlugin
