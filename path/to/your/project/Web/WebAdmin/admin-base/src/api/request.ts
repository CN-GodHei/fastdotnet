import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'
import type { PageResult } from './types'

// 创建axios实例
const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api', // url = base url + request url
  timeout: 15000, // 请求超时时间
  // 只有2xx状态码视为成功，进入then方法
  validateStatus: (status) => {
    return status >= 200 && status < 300;
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
    // 忽略OPTIONS请求的响应
    if (response.config.method?.toUpperCase() === 'OPTIONS') {
      return response;
    }
    
    // 对响应数据做点什么
    const res = response.data

    // 根据后端返回的状态码或HTTP状态码进行处理
    if (res.code === 200 && response.status === 200) {
      return res
    } else {
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
      if (res.code === 422 || response.status === 422) {
        ElMessage.error(res.Msg); // 统一使用res.Msg进行错误提示，与后端保持一致
        return Promise.reject(new Error(res.Msg)); // 确保Promise.reject也使用正确的Msg字段
      }

      // 其他错误状态
      ElMessage.error(res.message || res.data || '请求失败')
      return Promise.reject(new Error(res.message || res.data || 'Error'))
    }
  },
  (error: any) => {
    // 忽略OPTIONS请求的错误
    if (error.config?.method?.toUpperCase() === 'OPTIONS') {
      return Promise.reject(error);
    }
    
    // 对响应错误做点什么
    console.log('err' + error);
    if (error.response) {
      const { status, data } = error.response;
      // 401: 未登录
      if (status === 401) {
        ElMessage.error('登录已过期，请重新登录')
        // 清除token并跳转到登录页
        const userStore = useUserStore()
        userStore.logout()
        window.location.href = '/login'
        return Promise.reject(new Error('登录已过期，请重新登录'))
      }

      // 500: 服务器内部错误
      if (status === 500) {
        ElMessage.error(data.message || data.data || '服务器内部错误');
        return Promise.reject(new Error(data.message || data.data || '服务器内部错误'));
      }

      // 其他错误状态
      ElMessage.error(data.message || data.data || `请求失败，状态码: ${status}`);
      return Promise.reject(new Error(data.message || data.data || `请求失败，状态码: ${status}`));
    } else if (error.request) {
      // 请求已发出，但没有收到响应
      ElMessage.error('网络错误，请稍后重试');
      return Promise.reject(new Error('网络错误，请稍后重试'));
    } else {
      // 其他未知错误
      ElMessage.error('未知错误，请稍后重试');
      return Promise.reject(new Error('未知错误，请稍后重试'));
    }
  }
)

export default service