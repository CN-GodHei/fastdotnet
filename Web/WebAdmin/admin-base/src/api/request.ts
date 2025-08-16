import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import type { PageResult } from './types'

// 扩展ImportMeta接口
declare global {
  interface ImportMeta {
    env: {
      VITE_API_BASE_URL?: string
    }
  }
}

// 创建axios实例
const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api', // url = base url + request url
  timeout: 15000, // 请求超时时间
  // 只要响应状态码在200-499范围内，都视为成功，进入then方法
  validateStatus: (status) => {
    return true; // 任何状态码都视为成功，进入then方法，统一在响应拦截器中处理
  }
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

    // 以HTTP状态码为主要判断依据
    if (response.status >= 200 && response.status < 300) {
      // HTTP状态码表示成功
      
      // Code是我后端和前端的约定，0表示业务成功
      if (res.Code === 0) {
        // 成功响应，将Data字段作为返回数据
        return {
          ...res,
          data: res.Data
        }
      } else {
        // 业务错误处理
        // 401: 未登录
        if (res.Code === 401) {
          ElMessage.error('登录已过期，请重新登录')
          // 清除token并跳转到登录页
          const userStore = useUserStore()
          userStore.logout()
          window.location.href = '/login'
          return Promise.reject(new Error('登录已过期，请重新登录'))
        }

        // 422: 验证错误
        if (res.Code === 422) {
          return Promise.reject(new Error(res.Msg))
        }

        // 其他业务错误
        ElMessage.error(res.Msg || '请求失败')
        return Promise.reject(new Error(res.Msg || 'Error'))
      }
    } else {
      // HTTP错误状态码处理
      if (response.status === 401) {
        ElMessage.error('登录已过期，请重新登录')
        // 清除token并跳转到登录页
        const userStore = useUserStore()
        userStore.logout()
        window.location.href = '/login'
        return Promise.reject(new Error('登录已过期，请重新登录'))
      }

      // 422: 验证错误
      if (response.status === 422) {
        return Promise.reject(new Error(res.Msg || res.message || '验证错误'));
      }

      // 其他错误状态
      ElMessage.error(res.Msg || res.message || '请求失败')
      return Promise.reject(new Error(res.Msg || res.message || 'Error'))
    }
  },
  (error: any) => {
    // 对响应错误做点什么
    console.log('err' + error);
    // 只有当请求完全失败（例如网络断开、DNS 解析失败、服务器无响应等）时才显示网络错误
    if (error.response) {
        // 请求已发出，但服务器响应的状态码不在 2xx 范围内
        ElMessage.error(error.response.data.Msg || error.response.data.message || error.message || '请求失败')
        return Promise.reject(error); 
    } else if (error.request) {
        // 请求已发出，但没有收到响应
        ElMessage.error('网络错误，请稍后重试');
        return Promise.reject(error);
    } else {
        // 其他未知错误
        ElMessage.error('未知错误，请稍后重试');
        return Promise.reject(error);
    }
  }
)

export default service