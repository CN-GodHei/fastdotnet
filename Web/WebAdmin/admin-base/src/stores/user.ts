import { defineStore } from 'pinia'
import { getUserMenuTree } from '@/api/menu'
import { adminLogin } from '@/api/auth'
import type { Menu } from '@/api/menu'
import type { LoginDto } from '@/api/auth'

export const useUserStore = defineStore('user', {
  state: () => ({
    token: localStorage.getItem('access_token') || '',
    userInfo: null,
    permissions: [] as string[],
    menus: [] as Menu[],
    roles: [] as string[]
  }),

  getters: {
    isLoggedIn: (state) => !!state.token,
    userPermissions: (state) => state.permissions,
    userMenus: (state) => state.menus
  },

  actions: {
    // 设置token
    setToken(token: string) {
      this.token = token
      localStorage.setItem('access_token', token)
    },

    // 清除token
    clearToken() {
      this.token = ''
      localStorage.removeItem('access_token')
    },

    // 用户登录
    async login(username: string, password: string) {
      try {
        const loginData: LoginDto = {
          username,
          password
        }
        
        const response = await adminLogin(loginData)
        const token = response.data.token
        this.setToken(token)
        return token
      } catch (error) {
        console.error('登录失败:', error)
        throw error
      }
    },

    // 用户退出
    logout() {
      this.clearToken()
      this.userInfo = null
      this.permissions = []
      this.menus = []
      this.roles = []
    },

    // 加载用户菜单
    async loadUserMenus() {
      try {
        const response = await getUserMenuTree()
        this.menus = response.data || []
        // 提取所有权限代码
        this.permissions = this.extractPermissions(this.menus)
        return response.data
      } catch (error) {
        console.error('加载用户菜单失败:', error)
        this.menus = []
        this.permissions = []
        return []
      }
    },

    // 从菜单中提取权限代码
    extractPermissions(menus: Menu[]): string[] {
      const permissions: string[] = []
      
      const traverse = (menuList: Menu[]) => {
        menuList.forEach(menu => {
          if (menu.permissionCode) {
            permissions.push(menu.permissionCode)
          }
          if (menu.children && menu.children.length > 0) {
            traverse(menu.children)
          }
        })
      }
      
      traverse(menus)
      // 去重
      return [...new Set(permissions)]
    }
  }
})