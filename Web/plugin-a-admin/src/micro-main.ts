import type { LifeCycles } from 'qiankun'
import init from './main'

let appInstance: ReturnType<typeof init> | null = null;

// 【关键优化】在脚本加载时立即尝试注册 UI 组件，不依赖 mount 调用
const registerUI = async () => {
  const pluginAPI = (window as any).__PLUGIN_API__;
  if (pluginAPI) {
    // 动态导入组件，避免模块加载时执行 API 相关代码
    const { default: UserExtensionPanel } = await import('./UserExtensionPanel.vue');
    pluginAPI.registerUIComponent({
      pluginId: '11375910391972869',
      componentName: 'UserExtensionPanel',
      component: UserExtensionPanel,
      description: '演示插件用户扩展管理面板'
    });
  }
};

// 延迟执行注册，等待主应用初始化完成
setTimeout(() => registerUI(), 100);

export async function bootstrap() {
  //console.log('[PluginA] bootstraped');
}

export async function mount(props: any) {
  //console.log('[PluginA] mounting with props:', props);
  
  // 确保注册（如果之前因为 window.__PLUGIN_API__ 未就绪而失败）
  registerUI();

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
