import request from '@/api/request'

// 菜单数据类型定义
export interface Menu {
  id: number
  name: string              // 菜单名称
  code: string              // 菜单代码
  path: string              // 菜单路径
  icon: string              // 菜单图标
  parentId: number | null   // 父级菜单ID
  sort: number              // 排序
  type: 'Directory' | 'Menu' // 菜单类型
  module: string            // 所属模块
  category: 'Admin' | 'App' // 菜单分类
  isExternal: boolean       // 是否外链
  externalUrl: string       // 外链地址
  isEnabled: boolean        // 是否启用
  permissionCode: string    // 关联的权限代码
  isHidden?: boolean        // 是否隐藏(前端使用)
  children: Menu[]          // 子菜单
}

// 获取用户菜单树
export const getUserMenuTree = () => {
  return request({
    url: '/api/admin/menus/tree',
    method: 'get'
  })
}

// 获取所有菜单
export const getMenuList = () => {
  return request({
    url: '/api/admin/menus',
    method: 'get'
  })
}

// 根据ID获取菜单
export const getMenuById = (id: number) => {
  return request({
    url: `/api/admin/menus/${id}`,
    method: 'get'
  })
}

// 创建菜单
export const createMenu = (data: any) => {
  return request({
    url: '/api/admin/menus',
    method: 'post',
    data
  })
}

// 更新菜单
export const updateMenu = (id: number, data: any) => {
  return request({
    url: `/api/admin/menus/${id}`,
    method: 'put',
    data
  })
}

// 删除菜单
export const deleteMenu = (id: number) => {
  return request({
    url: `/api/admin/menus/${id}`,
    method: 'delete'
  })
}