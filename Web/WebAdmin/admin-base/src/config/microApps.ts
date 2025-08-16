import { registerMicroApps, RegistrableApp } from 'qiankun'
import { usePluginStore } from '@/stores/plugin'

// 微应用配置类型
export interface MicroApp extends RegistrableApp<any> {
  id:string
  name: string
  entry: string
  container: string
  activeRule: string
  category: 'Admin' | 'App'
}

// 默认微应用配置
export const defaultMicroApps: MicroApp[] = [
  // 示例配置
  /*
  {
    id: 1,
    name: 'plugin-a-admin',
    entry: '//localhost:8081',
    container: '#subapp-viewport',
    activeRule: '/micro/plugin-a-admin',
    category: 'Admin',
    props: {
      // 可以传递给子应用的数据
    }
  }
  */
]

// 注册微应用
export const registerApps = (apps: MicroApp[]) => {
  registerMicroApps(apps)
}

// 动态加载插件作为微应用
export const loadPluginApps = () => {
  const pluginStore = usePluginStore()
  const enabledPlugins = pluginStore.enabledPlugins
  
  const microApps: MicroApp[] = enabledPlugins.map(plugin => ({
    id: plugin.id,
    name: plugin.code,
    entry: plugin.entryPoint,
    container: '#subapp-viewport',
    activeRule: `/micro/${plugin.code}`,
    category: plugin.category as 'Admin' | 'App'
  }))
  
  registerApps(microApps)
}