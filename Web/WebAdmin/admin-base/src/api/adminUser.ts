import request from '@/api/request'
import type { PageQuery, PageResult } from './types'

// 管理员用户信息DTO
export interface AdminUserDto {
  id:string
  username: string
  fullName: string | null
  email: string | null
  phone: string | null
  isActive: boolean
  lastLoginTime: string | null
  lastLoginIp: string | null
  createTime: string
}

// 创建管理员用户DTO
export interface CreateAdminUserDto {
  username: string
  password: string
  fullName: string | null
  email: string | null
  phone: string | null
  isActive: boolean
}

// 更新管理员用户DTO
export interface UpdateAdminUserDto {
  fullName: string | null
  email: string | null
  phone: string | null
  isActive: boolean | null
}

// 重置密码DTO
export interface ResetPasswordDto {
  newPassword: string
}

// 获取所有管理员用户
export const getAdminUsers = () => {
  return request({
    url: '/api/admin/users',
    method: 'get'
  })
}

// 根据ID获取管理员用户
export const getAdminUserById = (id:string) => {
  return request({
    url: `/api/admin/users/${id}`,
    method: 'get'
  })
}

// 分页获取管理员用户
export const getAdminUsersPage = (params: PageQuery) => {
  return request({
    url: '/api/admin/users',
    method: 'get',
    params
  })
}

// 创建管理员用户
export const createAdminUser = (data: CreateAdminUserDto) => {
  return request({
    url: '/api/admin/users',
    method: 'post',
    data
  })
}

// 更新管理员用户
export const updateAdminUser = (id:string, data: UpdateAdminUserDto) => {
  return request({
    url: `/api/admin/users/${id}`,
    method: 'put',
    data
  })
}

// 删除管理员用户
export const deleteAdminUser = (id:string) => {
  return request({
    url: `/api/admin/users/${id}`,
    method: 'delete'
  })
}

// 重置管理员用户密码
export const resetAdminUserPassword = (id:string, data: ResetPasswordDto) => {
  return request({
    url: `/api/admin/users/${id}/reset-password`,
    method: 'post',
    data
  })
}