import { useUserStore } from '@/stores/user'
import type { Directive, DirectiveBinding } from 'vue'

/**
 * 权限指令
 * 使用方法: v-permission="'permission.code'"
 * 或者: v-permission="['permission.code1', 'permission.code2']"
 */
export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    const userStore = useUserStore()
    const permissions = userStore.userPermissions

    if (value && typeof value === 'string') {
      // 单个权限检查
      if (!permissions.includes(value)) {
        el.parentNode?.removeChild(el)
      }
    } else if (value && Array.isArray(value)) {
      // 多个权限检查，满足其中一个即可
      const hasPermission = value.some((permission) => permissions.includes(permission))
      if (!hasPermission) {
        el.parentNode?.removeChild(el)
      }
    } else {
      throw new Error('权限指令值必须是字符串或字符串数组')
    }
  }
}