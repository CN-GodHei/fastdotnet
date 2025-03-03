// 插件管理相关的mock数据

const plugins = [
  {
    name: 'plugin-a',
    version: '1.0.0',
    description: '示例插件A',
    status: 'running'
  },
  {
    name: 'plugin-b',
    version: '1.0.0',
    description: '示例插件B',
    status: 'stopped'
  },
  {
    name: 'plugin-c',
    version: '2.0.0',
    description: '示例插件C',
    status: 'running'
  }
]

export default [
  {
    url: '/api/plugins',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '获取成功',
        data: plugins
      }
    }
  },
  {
    url: '/api/plugins/install',
    method: 'post',
    response: () => {
      return {
        code: 200,
        message: '安装成功'
      }
    }
  },
  {
    url: '/api/plugins/:name/start',
    method: 'post',
    response: () => {
      return {
        code: 200,
        message: '启动成功'
      }
    }
  },
  {
    url: '/api/plugins/:name/stop',
    method: 'post',
    response: () => {
      return {
        code: 200,
        message: '停止成功'
      }
    }
  },
  {
    url: '/api/plugins/:name/uninstall',
    method: 'delete',
    response: () => {
      return {
        code: 200,
        message: '卸载成功'
      }
    }
  }
]