import type { LifeCycles } from 'qiankun'
import init from './main'

let appInstance: ReturnType<typeof init> | null = null;

export async function bootstrap() {
  console.log('[PluginA] bootstraped');
}

export async function mount(props: any) {
  console.log('[PluginA] mounting with props:', props);
  appInstance = init(props);
}

export async function unmount() {
  console.log('[PluginA] unmounting');
  if (appInstance && typeof appInstance.unmount === 'function') {
    appInstance.unmount();
  }
  appInstance = null;
}

export async function update(props: any) {
  console.log('[PluginA] updated with props:', props);
}
