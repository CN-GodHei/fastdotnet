import request from '@/api/request'

// 菜单数据类型定义
export interface Menu {
  Id: number                // 菜单ID
  Name: string              // 菜单名称
  Code: string              // 菜单代码
  Path: string              // 菜单路径
  Icon: string              // 菜单图标
  ParentId: number | null   // 父级菜单ID
  Sort: number              // 排序
  Type: number              // 菜单类型 (1: 目录, 2: 菜单)
  Module: string            // 所属模块
  Category: string          // 菜单分类
  IsExternal: boolean       // 是否外链
  ExternalUrl: string       // 外链地址
  IsEnabled: boolean        // 是否启用
  PermissionCode: string    // 关联的权限代码
  Children: Menu[]          // 子菜单
  CreateTime: string        // 创建时间
  UpdateTime: string | null // 更新时间
  IsDeleted: boolean        // 是否删除
  DeleteTime: string | null // 删除时间
}

// 获取用户菜单树
export const getUserMenuTree = () => {
  return request({
    url: '/api/admin/menus/tree',
    method: 'get'
  })
}

// 获取所有菜单
export const getAllMenus = () => {
  return request({
    url: '/api/admin/menus',
    method: 'get'
  })
}

// 获取菜单详情
export const getMenuById = (id: number) => {
  return request({
    url: `/api/admin/menus/${id}`,
    method: 'get'
  })
}

// 创建菜单
export const createMenu = (data: Partial<Menu>) => {
  return request({
    url: '/api/admin/menus',
    method: 'post',
    data
  })
}

// 更新菜单
export const updateMenu = (id: number, data: Partial<Menu>) => {
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

