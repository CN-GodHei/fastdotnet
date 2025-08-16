import { defineStore } from 'pinia'

interface LoginState {
  // 登录加载状态
  loading: boolean
  // 登录错误信息
  errorMessage: string
}

export const useLoginStore = defineStore('login', {
  state: (): LoginState => ({
    loading: false,
    errorMessage: ''
  }),

  actions: {
    /**
     * 设置加载状态
     * @param loading 是否正在加载
     */
    setLoading(loading: boolean) {
      this.loading = loading
    },

    /**
     * 设置错误信息
     * @param message 错误信息
     */
    setErrorMessage(message: string) {
      this.errorMessage = message
    },

    /**
     * 清除错误信息
     */
    clearErrorMessage() {
      this.errorMessage = ''
    },

    /**
     * 重置登录状态
     */
    reset() {
      this.loading = false
      this.errorMessage = ''
    }
  }
})