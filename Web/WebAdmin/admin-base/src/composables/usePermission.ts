import { useUserStore } from '@/stores/user'

/**
 * 权限相关的可组合函数
 */
export const usePermission = () => {
  const userStore = useUserStore()

  /**
   * 检查是否有某个权限
   * @param permission 权限代码
   * @returns boolean
   */
  const hasPermission = (permission: string): boolean => {
    return userStore.userPermissions.includes(permission)
  }

  /**
   * 检查是否有任意一个权限
   * @param permissions 权限代码数组
   * @returns boolean
   */
  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => userStore.userPermissions.includes(permission))
  }

  /**
   * 检查是否有所有权限
   * @param permissions 权限代码数组
   * @returns boolean
   */
  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => userStore.userPermissions.includes(permission))
  }

  return {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions
  }
}