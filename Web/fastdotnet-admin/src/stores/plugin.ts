import { defineStore } from 'pinia'
import { getApiPluginScan } from '@/api/fd-system-api-admin/Plugin' // 导入插件 API

export const usePluginStore = defineStore('plugin', {
  state: () => ({
    plugins: [] as any[], // 插件列表
    loading: false,
    // 插件商城相关
    marketplaceToken: '' as string, // 插件商城 Token
    marketplaceAuthCode: '' as string // 插件商城授权码
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
    // 设置插件商城 Token 和授权码
    setMarketplaceAuth(data: { Token?: string; AuthCode?: string }) {
      if (data.Token) {
        this.marketplaceToken = data.Token
        // 可以选择持久化到 localStorage
        localStorage.setItem('marketplace_token', data.Token)
      }
      if (data.AuthCode) {
        this.marketplaceAuthCode = data.AuthCode
        // 可以选择持久化到 localStorage
        localStorage.setItem('marketplace_auth_code', data.AuthCode)
      }
    },
    
    // 清除插件商城 Token 和授权码
    clearMarketplaceAuth() {
      this.marketplaceToken = ''
      this.marketplaceAuthCode = ''
      localStorage.removeItem('marketplace_token')
      localStorage.removeItem('marketplace_auth_code')
    },
    
    // 从 localStorage 恢复 Token(可选)
    restoreMarketplaceAuth() {
      const token = localStorage.getItem('marketplace_token')
      const authCode = localStorage.getItem('marketplace_auth_code')
      if (token) {
        this.marketplaceToken = token
      }
      if (authCode) {
        this.marketplaceAuthCode = authCode
      }
    },
    
    // 加载插件列表
    async loadPlugins() {
      // 避免重复加载
      if (this.plugins.length > 0) {
        return
      }
      
      this.loading = true
      try {
        // 调用 API 获取插件列表
        const response = await getApiPluginScan()
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
