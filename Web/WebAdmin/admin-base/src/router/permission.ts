import router from './index'
import { useUserStore } from '@/stores/user'
import type { Menu } from '@/api/menu'
import { constantRoutes } from './index'
import type { RouteRecordRaw } from 'vue-router'

// 白名单路由
const whiteList = ['/login']

// 将菜单转换为路由
function menuToRoute(menu: Menu): RouteRecordRaw | null {
  // 如果是外部链接，不生成路由
  if (menu.isExternal && menu.externalUrl) {
    return null
  }
  
  // 如果没有路径，不生成路由
  if (!menu.path) {
    return null
  }
  
  const route: RouteRecordRaw = {
    path: menu.path,
    name: menu.name,
    meta: {
      title: menu.name,
      icon: menu.icon,
      permission: menu.permissionCode
    }
  }
  
  // 如果是目录类型，创建父级路由
  if (menu.type === 'Directory') {
    route.component = () => import('@/layouts/index.vue')
    route.redirect = menu.children && menu.children.length > 0 ? 
      (menu.children[0].path ? `${menu.path}/${menu.children[0].path}` : menu.path) : 
      menu.path
  } 
  // 如果是菜单类型，创建页面路由
  else {
    // 微前端路由
    if (menu.path.startsWith('/micro/')) {
      route.component = () => import('@/views/micro/index.vue')
    } 
    // 普通路由
    else {
      // 这里可以根据需要动态导入组件
      route.component = () => import('@/views/dashboard/index.vue')
    }
  }
  
  // 处理子路由
  if (menu.children && menu.children.length > 0) {
    const childrenRoutes = menu.children
      .map(child => menuToRoute(child))
      .filter(route => route !== null) as RouteRecordRaw[]
    
    if (childrenRoutes.length > 0) {
      route.children = childrenRoutes
    }
  }
  
  return route
}

// 根据菜单生成路由
function generateRoutesFromMenus(menus: Menu[]): RouteRecordRaw[] {
  return menus
    .map(menu => menuToRoute(menu))
    .filter(route => route !== null) as RouteRecordRaw[]
}

router.beforeEach(async (to: any, from: any, next: any) => {
  const userStore = useUserStore()
  
  // 检查是否已登录
  if (userStore.isLoggedIn) {
    // 已登录访问登录页，重定向到首页
    if (to.path === '/login') {
      next({ path: '/' })
    } else {
      // 访问其他页面，检查是否有菜单数据
      if (userStore.userMenus.length === 0) {
        try {
          // 获取用户菜单
          await userStore.loadUserMenus()
          
          // 根据菜单生成路由
          const accessRoutes = generateRoutesFromMenus(userStore.userMenus)
          
          // 添加路由
          accessRoutes.forEach(route => {
            router.addRoute(route)
          })
          
          // 重新导航到目标路由
          next({ ...to, replace: true })
        } catch (error) {
          // 获取菜单失败，退出登录
          userStore.logout()
          next(`/login?redirect=${to.path}`)
        }
      } else {
        next()
      }
    }
  } else {
    // 未登录
    if (whiteList.includes(to.path)) {
      // 在白名单中，直接访问
      next()
    } else {
      // 不在白名单中，重定向到登录页
      next(`/login?redirect=${to.path}`)
    }
  }
})