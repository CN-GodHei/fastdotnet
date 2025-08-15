import { defineStore } from 'pinia'
import { 
  getPluginList, 
  enablePlugin, 
  disablePlugin,
  type Plugin 
} from '@/api/plugin'

export const usePluginStore = defineStore('plugin', {
  state: () => ({
    plugins: [] as Plugin[],
    loading: false
  }),

  getters: {
    enabledPlugins: (state) => state.plugins.filter(plugin => plugin.enabled),
    adminPlugins: (state) => state.plugins.filter(plugin => plugin.category === 'Admin'),
    appPlugins: (state) => state.plugins.filter(plugin => plugin.category === 'App')
  },

  actions: {
    // 加载插件列表
    async loadPlugins() {
      this.loading = true
      try {
        const response = await getPluginList()
        this.plugins = response.data || []
      } catch (error) {
        console.error('加载插件列表失败:', error)
        this.plugins = []
      } finally {
        this.loading = false
      }
    },

    // 启用插件
    async enablePlugin(id: number) {
      try {
        await enablePlugin(id)
        const plugin = this.plugins.find(p => p.id === id)
        if (plugin) {
          plugin.enabled = true
        }
        return true
      } catch (error) {
        console.error('启用插件失败:', error)
        return false
      }
    },

    // 禁用插件
    async disablePlugin(id: number) {
      try {
        await disablePlugin(id)
        const plugin = this.plugins.find(p => p.id === id)
        if (plugin) {
          plugin.enabled = false
        }
        return true
      } catch (error) {
        console.error('禁用插件失败:', error)
        return false
      }
    }
  }
})