import request from '@/api/request'

// 插件数据类型定义
export interface Plugin {
  id: number
  name: string           // 插件名称
  code: string           // 插件代码
  version: string        // 插件版本
  description: string    // 插件描述
  author: string         // 插件作者
  enabled: boolean       // 是否启用
  entryPoint: string     // 入口地址
  category: 'Admin' | 'App' // 插件分类
  createdAt: string      // 创建时间
  updatedAt: string      // 更新时间
}

// 获取插件列表
export const getPluginList = () => {
  return request({
    url: '/admin/plugins',
    method: 'get'
  })
}

// 获取插件详情
export const getPluginById = (id: number) => {
  return request({
    url: `/admin/plugins/${id}`,
    method: 'get'
  })
}

// 创建插件
export const createPlugin = (data: Partial<Plugin>) => {
  return request({
    url: '/admin/plugins',
    method: 'post',
    data
  })
}

// 更新插件
export const updatePlugin = (id: number, data: Partial<Plugin>) => {
  return request({
    url: `/admin/plugins/${id}`,
    method: 'put',
    data
  })
}

// 删除插件
export const deletePlugin = (id: number) => {
  return request({
    url: `/admin/plugins/${id}`,
    method: 'delete'
  })
}

// 启用插件
export const enablePlugin = (id: number) => {
  return request({
    url: `/admin/plugins/${id}/enable`,
    method: 'post'
  })
}

// 禁用插件
export const disablePlugin = (id: number) => {
  return request({
    url: `/admin/plugins/${id}/disable`,
    method: 'post'
  })
}