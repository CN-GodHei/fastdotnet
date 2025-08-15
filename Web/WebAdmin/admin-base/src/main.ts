import router from './'
import { useUserStore } from '@/stores/user'

router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  
  // 页面标题修改
  if (to.meta.title) {
    document.title = to.meta.title as string
  }

  // 路由鉴权
  if (to.meta.requiresAuth) {
    if (userStore.isAuthenticated) {
      next()
    } else {
      next('/login')
    }
  } else {
    next()
  }
})

export default router
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import router from './router'
import './styles/index.scss'

// 导入路由守卫
import './router/permission'

// 导入全局指令
import { permission } from '@/directives/permission'

// 初始化qiankun
import { initQiankun } from './config/qiankun'

const app = createApp(App)
const pinia = createPinia()

// 注册全局指令
app.directive('permission', permission)

app.use(router)
app.use(pinia)
app.use(ElementPlus)

app.mount('#app')

// 初始化qiankun微前端
initQiankun()