import { defineStore } from 'pinia'
import axios from 'axios'

export const useUserStore = defineStore('user', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    realName: '',
    avatar: '',
    roles: []
  }),

  actions: {
    // 设置token
    setToken(token) {
      this.token = token
      localStorage.setItem('token', token)
    },

    // 设置用户信息
    setUserInfo(userInfo) {
      this.realName = userInfo.realName
      this.avatar = userInfo.avatar || ''
      this.roles = userInfo.roles || []
    },

    // 获取用户信息
    async getUserInfo() {
      try {
        const res = await axios.get('/api/user/info')
        if (res.data.code === 200) {
          this.setUserInfo(res.data.data)
          return res.data.data
        }
      } catch (error) {
        console.error('获取用户信息失败', error)
        return null
      }
    },

    // 登录
    async login(loginForm) {
      try {
        const res = await axios.post('/api/user/login', loginForm)
        if (res.data.code === 200) {
          this.setToken(res.data.data.token)
          return true
        }
        return false
      } catch (error) {
        console.error('登录失败', error)
        return false
      }
    },

    // 退出登录
    logout() {
      this.token = ''
      this.realName = ''
      this.avatar = ''
      this.roles = []
      localStorage.removeItem('token')
    }
  }
})