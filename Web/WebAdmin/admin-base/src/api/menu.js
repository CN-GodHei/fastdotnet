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
  if (!component) {
    console.warn('组件路径为空')
    return null
  }

  // 处理布局组件
  if (component === 'Layout') {
    return () => import('@/views/sys/Layout.vue')
  }

  // 处理其他组件
  try {
    // 确保组件路径以.vue结尾
    console.log(1111,component)
    const componentPath = component.endsWith('.vue') ? component : `${component}.vue`
    return () => import(`@/views/${componentPath}`)
  } catch (error) {
    console.error(`加载组件失败: ${component}\n错误详情:`, error)
    console.warn(`请检查组件路径是否正确，完整路径: @/views/${component}`)
    return null
  }
}