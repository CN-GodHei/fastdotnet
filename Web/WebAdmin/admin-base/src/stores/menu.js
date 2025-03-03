import { defineStore } from 'pinia'
import axios from 'axios'

export const useMenuStore = defineStore('menu', {
  state: () => ({
    menus: []
  }),

  actions: {
    // 设置菜单数据
    setMenus(menus) {
      this.menus = menus
    },

    // 获取菜单数据
    async getMenus() {
      try {
        const res = await axios.get('/api/user/menus')
        if (res.data.code === 200) {
          this.setMenus(res.data.data)
          return res.data.data
        }
      } catch (error) {
        console.error('获取菜单失败', error)
        return []
      }
    },

    // 清空菜单数据
    clearMenus() {
      this.menus = []
    }
  }
})