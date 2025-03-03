import Mock from 'mockjs'

// 模拟登录接口
Mock.mock('/api/login', 'post', (options) => {
  const { username, password } = JSON.parse(options.body)
  if (username === 'admin' && password === '123456') {
    return {
      code: 200,
      data: {
        token: 'mock-token-' + Mock.Random.guid(),
        username: 'admin',
        realName: '管理员'
      },
      message: '登录成功'
    }
  } else {
    return {
      code: 401,
      message: '用户名或密码错误'
    }
  }
})

// 模拟获取用户信息接口
Mock.mock('/api/user/info', 'get', () => {
  return {
    code: 200,
    data: {
      username: 'admin',
      realName: '管理员',
      avatar: Mock.Random.image('100x100'),
      roles: ['admin'],
      permissions: ['*']
    },
    message: '获取成功'
  }
})

// 模拟获取菜单接口
Mock.mock('/api/user/menus', 'get', () => {
  return {
    code: 200,
    data: [
      {
        path: '/dashboard',
        name: 'Dashboard',
        meta: {
          title: '仪表盘',
          icon: 'Odometer'
        }
      },
      {
        path: '/system',
        name: 'System',
        meta: {
          title: '系统管理',
          icon: 'Setting'
        },
        children: [
          {
            path: '/system/user',
            name: 'User',
            meta: {
              title: '用户管理',
              icon: 'User'
            }
          },
          {
            path: '/system/role',
            name: 'Role',
            meta: {
              title: '角色管理',
              icon: 'UserFilled'
            }
          }
        ]
      },
      {
        path: '/plugin',
        name: 'Plugin',
        meta: {
          title: '插件管理',
          icon: 'Connection'
        }
      }
    ],
    message: '获取成功'
  }
})