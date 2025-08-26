import request from '/@/utils/request'

// 插件数据类型定义 (根据后端接口调整)
export interface Plugin {
  id: string            // 插件ID
  name: string          // 插件名称
  version: string       // 插件版本
  description: string   // 插件描述
  enabled: boolean      // 是否启用
  author: string        // 插件作者
  entryPoint: string    // 入口地址
}

// 扫描插件
export const scanPlugins = () => {
  return request({
    url: '/api/plugin/scan',
    method: 'get'
  })
}

// 启用插件
export const enablePlugin = (pluginId: string) => {
  return request({
    url: `/api/plugin/enable/${pluginId}`,
    method: 'post'
  })
}

// 禁用插件
export const disablePlugin = (pluginId: string) => {
  return request({
    url: `/api/plugin/disable/${pluginId}`,
    method: 'post'
  })
}

// 卸载插件
export const uninstallPlugin = (pluginId: string) => {
  return request({
    url: `/api/plugin/uninstall/${pluginId}`,
    method: 'post'
  })
}
