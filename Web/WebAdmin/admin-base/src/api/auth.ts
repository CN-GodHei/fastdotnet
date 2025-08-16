import request from '@/api/request'

// 登录数据传输对象
export interface LoginDto {
  username: string
  password: string
}

// 登录响应数据
export interface LoginResponse {
  Data: {
    Token: string
  }
  Code: number
  Msg: string | null
}

// 管理端登录
export const adminLogin = (data: LoginDto) => {
  return request({
    url: '/api/auth/admin/login',
    method: 'post',
    data
  })
}

// 客户端登录
export const appLogin = (data: LoginDto) => {
  return request({
    url: '/api/auth/app/login',
    method: 'post',
    data
  })
}

// 获取用户信息
export const getUserInfo = () => {
  return request({
    url: '/auth/info',
    method: 'get'
  })
}

// 退出登录
export const logout = () => {
  return request({
    url: '/auth/logout',
    method: 'post'
  })
}