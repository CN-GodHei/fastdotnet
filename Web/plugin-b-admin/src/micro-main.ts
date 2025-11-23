import type { LifeCycles } from 'qiankun'
import init from './main'

let appInstance: ReturnType<typeof init> | null = null;

export async function bootstrap() {
  //console.log('[PluginB] bootstraped');
}

export async function mount(props: any) {
  //console.log('[PluginB] mounting with props:', props);
  appInstance = init(props);
}

export async function unmount() {
  //console.log('[PluginB] unmounting');
  if (appInstance && typeof appInstance.unmount === 'function') {
    appInstance.unmount();
  }
  appInstance = null;
}

export async function update(props: any) {
  //console.log('[PluginB] updated with props:', props);
}
