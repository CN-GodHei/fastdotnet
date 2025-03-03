import { registerMicroApps, start } from 'qiankun'

// 插件配置列表
const plugins = []

// 注册单个插件
export function registerPlugin(config) {
  const { name, entry } = config
  
  // 检查插件是否已注册
  if (plugins.find(p => p.name === name)) {
    console.warn(`Plugin ${name} already registered`)
    return
  }

  // 构建微应用配置
  const microApp = {
    name,
    entry,
    container: '#subapp-viewport',
    activeRule: `/plugin-${name.toLowerCase()}`,
    props: {
      // 可以传递给子应用的数据
      mainApp: window.__MAIN_APP__
    }
  }

  // 注册微应用
  registerMicroApps([microApp])
  
  // 将插件添加到列表
  plugins.push(config)
}

// 初始化qiankun
export function initQiankun() {
  // 全局配置
  window.__POWERED_BY_QIANKUN__ = true
  window.__MAIN_APP__ = {
    name: 'admin-base'
  }

  // 启动qiankun
  start({
    prefetch: true,
    sandbox: {
      strictStyleIsolation: true
    }
  })
}

// 获取已注册的插件列表
export function getRegisteredPlugins() {
  return [...plugins]
}