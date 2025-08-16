import router from './index'
import { useUserStore } from '@/stores/user'
import type { Menu } from '@/api/menu'
import type { RouteRecordRaw } from 'vue-router'
import Layout from '@/layouts/index.vue'

// 白名单路由
const whiteList = ['/login']

// 使用 Vite 的 import.meta.glob 实现高效的动态组件加载
const modules = import.meta.glob('/src/views/**/*.vue')

// 将后端菜单项转换为 Vue Router 路由记录
function menuToRoute(menu: Menu): RouteRecordRaw | null {
  // 不为外部链接生成路由
  if (menu.IsExternal && menu.ExternalUrl) {
    return null
  }
  
  // 不为没有路径的菜单项生成路由
  if (!menu.Path) {
    return null
  }
  
  const route: RouteRecordRaw = {
    path: menu.Path,
    name: menu.Name,
    meta: {
      title: menu.Name,
      icon: menu.Icon,
      permission: menu.PermissionCode
    }
  }
  
  // 处理目录/布局路由 (菜单类型 MenuType=0)
  if (menu.Type === 0) {
    route.component = Layout
    // 设置重定向到第一个子路由，以避免显示空白布局
    if (menu.Children && menu.Children.length > 0 && menu.Children[0].Path) {
      const firstChildPath = menu.Children[0].Path
      if (firstChildPath.startsWith('/')) {
        route.redirect = firstChildPath
      } else {
        // 确保重定向路径格式正确
        route.redirect = menu.Path === '/' ? `/${firstChildPath}` : `${menu.Path}/${firstChildPath}`
      }
    }
  } 
  // 处理页面/视图路由 (菜单类型 MenuType=1)
  else {
    // 对微前端路由进行特殊处理
    if (menu.Path.startsWith('/micro/')) {
      route.component = () => import('@/views/micro/index.vue')
    } else {
      // 根据菜单路径构建可能的组件路径
      const componentPath = `/src/views${menu.Path}.vue`
      const componentIndexPath = `/src/views${menu.Path}/index.vue`

      if (modules[componentPath]) {
        route.component = modules[componentPath]
      } else if (modules[componentIndexPath]) {
        route.component = modules[componentIndexPath]
      } else {
        console.warn(`[Router] 菜单路径 "${menu.Path}" 的组件未找到。查找路径: ${componentPath} 和 ${componentIndexPath}。`)
        return null // 如果组件不存在，则跳过创建路由
      }
    }
  }
  
  // 递归处理子菜单项
  if (menu.Children && menu.Children.length > 0) {
    const childrenRoutes = menu.Children
      .map(child => menuToRoute(child))
      .filter(route => route !== null) as RouteRecordRaw[]
    
    if (childrenRoutes.length > 0) {
      route.children = childrenRoutes
    }
  }
  
  console.log('生成路由:', route)
  return route
}

// 从菜单树生成所有路由
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
            console.log('添加路由:', route)
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