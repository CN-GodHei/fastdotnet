import Mock from 'mockjs'

const menuList = [
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: 'dashboard/Index',
    meta: {
      title: '仪表盘',
      icon: 'Odometer'
    }
  },
  {
    path: '/system',
    name: 'System',
    component: 'Layout',
    meta: {
      title: '系统管理',
      icon: 'Setting'
    },
    children: [
      {
        path: 'user',
        name: 'User',
        component: 'system/user/Index',
        meta: {
          title: '用户管理',
          icon: 'User'
        }
      },
      {
        path: 'role',
        name: 'Role',
        component: 'system/role/Index',
        meta: {
          title: '角色管理',
          icon: 'UserFilled'
        }
      }
    ]
  }
]

Mock.mock('/api/user/menus', 'get', {
  code: 200,
  message: '获取成功',
  data: menuList
})