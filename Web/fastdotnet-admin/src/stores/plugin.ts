import { defineStore } from 'pinia'
import { getPluginList, type Plugin } from '/@/api/plugin' // 假设你有一个 API 文件来获取插件列表

export const usePluginStore = defineStore('plugin', {
  state: () => ({
    plugins: [] as Plugin[], // 插件列表
    loading: false
  }),

  getters: {
    // 获取已启用的插件
    enabledPlugins: (state) => state.plugins.filter(plugin => plugin.enabled),
    // 获取 Admin 类型的插件
    adminPlugins: (state) => state.plugins.filter(plugin => plugin.category === 'Admin'),
    // 获取 App 类型的插件
    appPlugins: (state) => state.plugins.filter(plugin => plugin.category === 'App')
  },

  actions: {
    // 加载插件列表
    async loadPlugins() {
      // 避免重复加载
      if (this.plugins.length > 0) {
        return
      }
      
      this.loading = true
      try {
        // 调用 API 获取插件列表
        const response = await getPluginList()
        // 假设 response.data 是插件数组
        this.plugins = response.data || []
      } catch (error) {
        console.error('加载插件列表失败:', error)
        this.plugins = []
      } finally {
        this.loading = false
      }
    },

    // 启用插件 (示例方法，具体实现需调用后端 API)
    async enablePlugin(id: string) {
      // 实现启用插件的逻辑
      // 例如调用 API: await enablePluginApi(id)
      // 更新本地状态
      const plugin = this.plugins.find(p => p.id === id)
      if (plugin) {
        plugin.enabled = true
      }
    },

    // 禁用插件 (示例方法，具体实现需调用后端 API)
    async disablePlugin(id: string) {
      // 实现禁用插件的逻辑
      // 例如调用 API: await disablePluginApi(id)
      // 更新本地状态
      const plugin = this.plugins.find(p => p.id === id)
      if (plugin) {
        plugin.enabled = false
      }
    }
  }
})

// 插件数据类型定义 (如果还没有定义的话)
export interface Plugin {
  id: string
  name: string           // 插件名称
  code: string           // 插件代码 (用于路由匹配)
  version: string        // 插件版本
  description: string    // 插件描述
  author: string         // 插件作者
  enabled: boolean       // 是否启用
  entryPoint: string     // 入口地址 (例如: //localhost:8081)
  category: 'Admin' | 'App' // 插件分类
  createdAt: string      // 创建时间
  updatedAt: string      // 更新时间
  // 可以根据需要添加更多字段
}