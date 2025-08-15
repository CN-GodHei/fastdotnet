import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import type { PageResult } from './types'

// 创建axios实例
const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api', // url = base url + request url
  timeout: 15000 // 请求超时时间
})

// 请求拦截器
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // 在发送请求之前做些什么
    // 可以添加token等信息
    const token = localStorage.getItem('access_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error: any) => {
    // 对请求错误做些什么
    console.log(error)
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse) => {
    // 对响应数据做点什么
    const res = response.data

    // 根据后端返回的状态码进行处理
    if (res.code !== 200) {
      // 401: 未登录
      if (res.code === 401) {
        ElMessage.error('登录已过期，请重新登录')
        // 清除token并跳转到登录页
        const userStore = useUserStore()
        userStore.logout()
        window.location.href = '/login'
        return Promise.reject(new Error('登录已过期，请重新登录'))
      }

      // 422: 验证错误
      if (res.code === 422) {
        // 只显示错误信息，不返回reject，让调用方决定是否处理
        if (res.data) {
          ElMessage.error(res.data)
        } 
        // else if (res.message) {
        //   ElMessage.error(res.message)
        // }
        // return Promise.resolve(res)
      }

      // 其他错误状态
      ElMessage.error(res.message || res.data || '请求失败')
      return Promise.reject(new Error(res.message || res.data || 'Error'))
    } else {
      return res
    }
  },
  (error: any) => {
    // 对响应错误做点什么
    console.log('err' + error)
    ElMessage.error('网络错误，请稍后重试')
    return Promise.reject(error)
  }
)

export default service