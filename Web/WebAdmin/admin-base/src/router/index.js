import { createRouter, createWebHistory } from 'vue-router'
import { ElMessage } from 'element-plus'

// 公共路由，不需要登录就可以访问
const publicRoutes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/sys/Login.vue')
  },
  {
    path: '/404',
    name: 'NotFound',
    component: () => import('../views/sys/404.vue')
  },
  {
    path: '/401',
    name: 'Unauthorized',
    component: () => import('../views/sys/401.vue')
  },
  {
    path: '/500',
    name: 'ServerError',
    component: () => import('../views/sys/500.vue')
  }
]

// 基础布局路由
const baseRoutes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/home',
    name: 'Home',
    component: () => import('../views/sys/Layout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: 'plugins',
        name: 'Plugins',
        meta: { 
          title: '插件管理',
          icon: 'Menu'
        },
        children: [
          {
            path: 'test',
            name: 'PluginTest',
            component: () => import('../views/plugins/Test.vue'),
            meta: {
              title: '插件测试页面',
              icon: 'Document'
            }
          },
          {
            path: 'manage',
            name: 'PluginManage',
            component: () => import('../views/plugins/manage/Index.vue'),
            meta: {
              title: '插件管理',
              icon: 'Setting'
            }
          }
        ]
      }
    ]
  }
]

const routes = [...publicRoutes, ...baseRoutes, {
  path: '/:pathMatch(.*)*',
  redirect: '/404'
}]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 全局路由守卫
import { getUserMenus, transformMenuToRoutes } from '@/api/menu'

// 是否已添加动态路由
let hasAddedRoutes = false

router.beforeEach(async (to, from, next) => {
  const token = localStorage.getItem('token')
  
  // 如果访问登录页
  if (to.path === '/login') {
    if (token) {
      // 已登录时访问登录页，重定向到首页
      next('/home')
    } else {
      // 未登录时允许访问登录页
      next()
    }
    return
  }

  // 检查是否需要登录权限
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!token) {
      // 需要登录但未登录，重定向到登录页
      next({
        path: '/login',
        query: { redirect: to.fullPath }
      })
      ElMessage.warning('请先登录')
      return
    }
  }

  // 如果已登录且未加载动态路由
  if (token && !hasAddedRoutes) {
    try {
      const res = await getUserMenus()
      const asyncRoutes = transformMenuToRoutes(res)
      // 添加动态路由
      asyncRoutes.forEach(route => {
        router.addRoute('Home', route)
      })
      hasAddedRoutes = true
      // 重新进入当前路由
      next({ ...to, replace: true })
    } catch (error) {
      console.error('获取菜单数据失败', error)
      // 获取菜单失败时，如果是访问需要权限的页面，跳转到500错误页
      if (to.matched.some(record => record.meta.requiresAuth)) {
        next('/500')
      } else {
        next()
      }
    }
    return
  }

  // 其他情况正常通过
  next()
})

export default router