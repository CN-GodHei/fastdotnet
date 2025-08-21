// src/micro-main.ts
import type { Lifecycle } from 'qiankun'
import init from './main'

// qiankun 要求导出的生命周期函数
let app: ReturnType<typeof init> | null = null

// Bootstrap 生命周期 - 只会在微应用初始化时调用一次
export async function bootstrap() {
  console.log('[PluginA] bootstraped')
}

// Mount 生命周期 - 每次微应用接入时调用
export async function mount(props: any) {
  console.log('[PluginA] mounting with props:', props)
  app = init(props) // 初始化并挂载 Vue 应用
}

// Unmount 生命周期 - 每次微应用卸载时调用
export async function unmount() {
  console.log('[PluginA] unmounting')
  // app 实例在 init 中创建，这里直接访问可能有问题
  // 通常由 main.ts 中的逻辑处理卸载
  // 如果需要在这里做清理，可以考虑将 app 实例存储在全局或通过 props 传递
  // 为简化，我们假设 main.ts 中的逻辑足够
}

// 可选的 Update 生命周期
export async function update(props: any) {
  console.log('[PluginA] updated with props:', props)
}