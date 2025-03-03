import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import router from './router'
import './mock'
import axios from 'axios'
import { initQiankun, registerPlugin } from './config/qiankun'

const app = createApp(App)
const pinia = createPinia()

app.use(router)
app.use(pinia)
app.use(ElementPlus)

app.mount('#app')

// 初始化qiankun
initQiankun()

// 导出注册插件方法，供其他模块使用
window.registerPlugin = registerPlugin
