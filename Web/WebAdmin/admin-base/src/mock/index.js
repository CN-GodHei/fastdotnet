import Mock from 'mockjs'
import plugins from './plugins'

// 模拟插件列表数据
const pluginList = [
  {
    name: 'plugin-a',
    version: '1.0.0',
    description: '示例插件A',
    status: 'stopped'
  },
  {
    name: 'plugin-b',
    version: '1.0.0',
    description: '示例插件B',
    status: 'running'
  }
]

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
      // {
      //   path: '/dashboard',
      //   name: 'Dashboard',
      //   meta: {
      //     title: '仪表盘',
      //     icon: 'Odometer'
      //   }
      // },
      {
        path: '/system',
        name: 'System',
        meta: {
          title: '系统管理',
          icon: 'Setting'
        },
        children: [
          {
            path: '/system/test',
            name: 'Test',
            component: 'sys/test/Index.vue',
            meta: {
              title: '测试页面',
              icon: 'Monitor'
            }
          }
        ]
      },
      {
        path: '/plugin',
        name: 'Plugin',
        component: 'sys/plugin/Index',
        meta: {
          title: '插件管理',
          icon: 'Connection'
        }
      }
    ],
    message: '获取成功'
  }
})

// 模拟获取插件列表接口
Mock.mock('/api/plugins', 'get', () => {
  return {
    code: 200,
    data: pluginList,
    message: '获取成功'
  }
})

// 模拟安装插件接口
Mock.mock('/api/plugins/install', 'post', (options) => {
  const { url } = JSON.parse(options.body)
  // 模拟安装成功
  const newPlugin = {
    name: 'new-plugin-' + Mock.Random.guid().substring(0, 8),
    version: '1.0.0',
    description: '新安装的插件',
    status: 'stopped'
  }
  pluginList.push(newPlugin)
  return {
    code: 200,
    data: newPlugin,
    message: '安装成功'
  }
})

// 模拟启动插件接口
Mock.mock(new RegExp('/api/plugins/.*/start'), 'post', (options) => {
  const pluginName = options.url.split('/')[3]
  const plugin = pluginList.find(p => p.name === pluginName)
  if (plugin) {
    plugin.status = 'running'
    return {
      code: 200,
      message: '启动成功'
    }
  }
  return {
    code: 404,
    message: '插件不存在'
  }
})

// 模拟停止插件接口
Mock.mock(new RegExp('/api/plugins/.*/stop'), 'post', (options) => {
  const pluginName = options.url.split('/')[3]
  const plugin = pluginList.find(p => p.name === pluginName)
  if (plugin) {
    plugin.status = 'stopped'
    return {
      code: 200,
      message: '停止成功'
    }
  }
  return {
    code: 404,
    message: '插件不存在'
  }
})

// 模拟卸载插件接口
Mock.mock(new RegExp('/api/plugins/.*/uninstall'), 'delete', (options) => {
  const pluginName = options.url.split('/')[3]
  const index = pluginList.findIndex(p => p.name === pluginName)
  if (index > -1) {
    pluginList.splice(index, 1)
    return {
      code: 200,
      message: '卸载成功'
    }
  }
  return {
    code: 404,
    message: '插件不存在'
  }
})

export default [...plugins]