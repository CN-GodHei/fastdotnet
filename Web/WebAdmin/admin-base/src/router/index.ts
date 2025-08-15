import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useUserStore } from '@/stores/user'

// 静态路由
export const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: {
      hidden: true
    }
  },
  {
    path: '/',
    component: () => import('@/layouts/index.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        name: 'Dashboard',
        meta: {
          title: '控制台',
          icon: 'House'
        }
      }
    ]
  },
  // 微前端容器路由
  {
    path: '/micro',
    component: () => import('@/layouts/index.vue'),
    children: [
      {
        path: ':microApp(.*)',
        component: () => import('@/views/micro/index.vue'),
        meta: {
          title: '微应用',
          icon: 'Grid'
        }
      }
    ]
  }
]

// 异步路由（基于菜单动态生成）
export const asyncRoutes: RouteRecordRaw[] = [
  // 动态路由将在这里添加
]

// 创建路由实例
const router = createRouter({
  history: createWebHistory('/'),
  routes: constantRoutes,
  scrollBehavior: () => ({ left: 0, top: 0 })
})

// 动态添加路由
export function addRoutes(routes: RouteRecordRaw[]) {
  routes.forEach(route => {
    router.addRoute(route)
  })
}

export default router