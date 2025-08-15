import { registerMicroApps, start, setDefaultMountApp } from 'qiankun'
import type { MicroApp } from './microApps'
import { defaultMicroApps } from './microApps'

// 注册微应用
export function registerApps(apps: MicroApp[] = defaultMicroApps) {
  const allApps = [...defaultMicroApps, ...apps]
  registerMicroApps(allApps)
}

// 设置默认 mount 应用
export function setDefaultApp(appName: string) {
  setDefaultMountApp(appName)
}

// 初始化qiankun
export function initQiankun() {
  // 注册默认微应用
  registerApps()
  
  // 启动qiankun
  start({
    prefetch: true,
    sandbox: {
      strictStyleIsolation: false
    }
  })
}