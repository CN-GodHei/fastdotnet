import type { LifeCycles } from 'qiankun'
import init from './main'
import UserExtensionPanel from './UserExtensionPanel.vue'

let appInstance: ReturnType<typeof init> | null = null;

export async function bootstrap() {
  //console.log('[PluginA] bootstraped');
}

export async function mount(props: any) {
  //console.log('[PluginA] mounting with props:', props);
  
  // 注册用户扩展面板组件到主应用
  const pluginAPI = props.pluginAPI || (window as any).__PLUGIN_API__;
  if (pluginAPI) {
    pluginAPI.registerUIComponent({
      pluginId: '11375910391972869',
      componentName: 'UserExtensionPanel',
      component: UserExtensionPanel,
      description: '演示插件用户扩展管理面板'
    });
    console.log('[PluginA] UserExtensionPanel registered.');
  }

  appInstance = init(props);
}

export async function unmount() {
  //console.log('[PluginA] unmounting');
  if (appInstance && typeof appInstance.unmount === 'function') {
    appInstance.unmount();
  }
  appInstance = null;
}

export async function update(props: any) {
  //console.log('[PluginA] updated with props:', props);
}
