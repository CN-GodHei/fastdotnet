import type { LifeCycles } from 'qiankun'
import init from './main'

let appInstance: ReturnType<typeof init> | null = null

export async function bootstrap(): Promise<void> {
  // 只执行一次的初始化
}

export async function mount(props: any): Promise<void> {
  appInstance = init(props)
}

export async function unmount(): Promise<void> {
  appInstance?.unmount()
  appInstance = null
}

export async function update(_props: any): Promise<void> {
  // 宿主 props 变化时（主题/菜单等）
}

export type { LifeCycles }
