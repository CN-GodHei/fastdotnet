import request from '@/utils/request'

// 获取用户菜单和权限数据
export function getUserMenus() {
  return request({
    url: '/api/user/menus',
    method: 'get'
  })
}

// 将后端返回的菜单数据转换为路由配置
export function transformMenuToRoutes(menus) {
  return menus.map(menu => {
    const route = {
      path: menu.path,
      name: menu.name,
      meta: {
        title: menu.title,
        icon: menu.icon
      },
      component: loadComponent(menu.component)
    }

    if (menu.children && menu.children.length > 0) {
      route.children = transformMenuToRoutes(menu.children)
    }

    return route
  })
}

// 动态加载组件
function loadComponent(component) {
  if (!component) return null
  // 这里假设所有组件都在views目录下
  return () => import(`@/views/${component}.vue`)
}