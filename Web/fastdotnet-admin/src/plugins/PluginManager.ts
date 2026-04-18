/**
 * 插件管理系统
 * 提供插件的注册、发现、加载和生命周期管理
 */

import { loadMicroApp, MicroApp } from 'qiankun';
import { useMicroAppsStore } from '@/stores/microApps';
import { pluginRegistry } from './PluginRegistry';
import request from '@/utils/request';

export interface PluginMetadata {
  id: string;
  name: string;
  description: string;
  version: string;
  author: string;
  enabled: boolean;
  entryPoint: string;
  dependencies?: string[];
  permissions?: string[];
  microAppConfig?: {
    name: string;
    entry: string;
    container: string;
    activeRule: string;
  };
}

export interface PluginInstance {
  metadata: PluginMetadata;
  instance: MicroApp | null;
  status: 'loaded' | 'unloaded' | 'error';
  createdAt: Date;
}

export interface PluginLifecycleHooks {
  install?: () => Promise<void>;
  uninstall?: () => Promise<void>;
  activate?: () => Promise<void>;
  deactivate?: () => Promise<void>;
  update?: (newMetadata: PluginMetadata) => Promise<void>;
}

class PluginManager {
  private static instance: PluginManager;
  private plugins: Map<string, PluginInstance> = new Map();
  private pluginConfigs: Map<string, PluginMetadata> = new Map();
  private microAppsStore = useMicroAppsStore();

  private constructor() {}

  public static getInstance(): PluginManager {
    if (!PluginManager.instance) {
      PluginManager.instance = new PluginManager();
    }
    return PluginManager.instance;
  }

  /**
   * 注册插件配置
   */
  public registerPlugin(metadata: PluginMetadata): void {
    this.pluginConfigs.set(metadata.id, metadata);
    console.log(`[PluginManager] Plugin registered: ${metadata.name} (${metadata.id})`);
  }

  /**
   * 获取插件配置
   */
  public getPluginConfig(pluginId: string): PluginMetadata | undefined {
    return this.pluginConfigs.get(pluginId);
  }

  /**
   * 获取所有插件配置
   */
  public getAllPluginConfigs(): PluginMetadata[] {
    return Array.from(this.pluginConfigs.values());
  }

  /**
   * 【新增】预加载所有已启用插件的脚本（仅为了触发 UI 组件注册）
   * 这会通过 loadMicroApp 加载插件，但使用一个隐藏的容器
   */
  public async preloadAllPluginsForUIRegistration(): Promise<void> {
    console.log('[PluginManager] 开始预加载所有插件以注册 UI 组件...');
    
    // 【修复】从 pluginRegistry 获取已注册的插件
    const allPlugins = pluginRegistry.getAllPlugins();
    console.log(`[PluginManager] 总共找到 ${allPlugins.length} 个插件`);
    
    // 严格过滤：只处理 enabled === true 的插件
    const enabledPlugins = allPlugins.filter(p => {
      const isEnabled = p.enabled === true;
      console.log(`[PluginManager] 插件 ${p.name} (${p.id}): enabled=${p.enabled}, 是否预加载=${isEnabled}`);
      return isEnabled;
    });
    
    console.log(`[PluginManager] 其中 ${enabledPlugins.length} 个已启用，将预加载`);
    
    for (const plugin of enabledPlugins) {
      if (!plugin.microAppConfig) {
        console.warn(`[PluginManager] 插件 ${plugin.id} 没有 microAppConfig，跳过`);
        continue;
      }
      
      // 【关键修复】将 pluginRegistry 的配置同步到 pluginManager 的 pluginConfigs
      if (!this.pluginConfigs.has(plugin.id)) {
        this.pluginConfigs.set(plugin.id, plugin);
      }
      
      try {
        // 检查是否已经加载过
        if (this.plugins.has(plugin.id)) {
          console.log(`[PluginManager] 插件 ${plugin.id} 已加载，跳过`);
          continue;
        }
        
        console.log(`[PluginManager] 开始预加载插件: ${plugin.name} (${plugin.id})`);
        
        // 使用一个隐藏的容器来加载插件，这样不会显示界面但会执行脚本
        const hiddenContainerId = `hidden-preload-${plugin.id}`;
        let container = document.getElementById(hiddenContainerId);
        if (!container) {
          container = document.createElement('div');
          container.id = hiddenContainerId;
          container.style.display = 'none';
          document.body.appendChild(container);
        }
        
        await this.loadPlugin(plugin.id, `#${hiddenContainerId}`);
        console.log(`[PluginManager] 插件 ${plugin.id} 预加载完成`);
      } catch (error) {
        console.warn(`[PluginManager] 预加载插件 ${plugin.id} 失败:`, error);
      }
    }
    
    console.log('[PluginManager] 所有插件预加载完成');
  }

  /**
   * 动态加载插件
   */
  public async loadPlugin(
    pluginId: string,
    container?: string,
    props?: any
  ): Promise<MicroApp | null> {
    const config = this.getPluginConfig(pluginId);
    if (!config || !config.enabled) {
      console.error(`[PluginManager] Plugin ${pluginId} not found or disabled`);
      return null;
    }

    if (!config.microAppConfig) {
      console.error(`[PluginManager] Plugin ${pluginId} has no micro app configuration`);
      return null;
    }

    try {
      const microAppConfig = config.microAppConfig;
      const appContainer = container || microAppConfig.container || '#subapp-viewport';

      const microApp = loadMicroApp(
        {
          name: microAppConfig.name,
          entry: microAppConfig.entry,
          container: appContainer,
          props: {
            ...props,
            pluginId: pluginId,
            pluginMetadata: config,
            // 【关键修复】传递主应用的 request 实例给子应用
            request: request,
          },
        },
        {
          sandbox: {
            strictStyleIsolation: true,
            experimentalStyleIsolation: true,
          },
          // 设置超时时间
          prefetch: false,
        }
      );

      // 创建插件实例
      const pluginInstance: PluginInstance = {
        metadata: config,
        instance: microApp,
        status: 'loaded',
        createdAt: new Date(),
      };

      this.plugins.set(pluginId, pluginInstance);
      console.log(`[PluginManager] Plugin loaded: ${config.name} (${pluginId})`);

      return microApp;
    } catch (error) {
      console.error(`[PluginManager] Failed to load plugin ${pluginId}:`, error);
      const pluginInstance: PluginInstance = {
        metadata: config,
        instance: null,
        status: 'error',
        createdAt: new Date(),
      };
      this.plugins.set(pluginId, pluginInstance);
      return null;
    }
  }

  /**
   * 卸载插件
   */
  public async unloadPlugin(pluginId: string): Promise<boolean> {
    const pluginInstance = this.plugins.get(pluginId);
    if (!pluginInstance) {
      console.warn(`[PluginManager] Plugin ${pluginId} not found`);
      return false;
    }

    try {
      if (pluginInstance.instance) {
        await pluginInstance.instance.unmount();
      }
      
      this.plugins.delete(pluginId);
      console.log(`[PluginManager] Plugin unloaded: ${pluginInstance.metadata.name} (${pluginId})`);
      return true;
    } catch (error) {
      console.error(`[PluginManager] Failed to unload plugin ${pluginId}:`, error);
      return false;
    }
  }

  /**
   * 获取插件实例
   */
  public getPluginInstance(pluginId: string): PluginInstance | undefined {
    return this.plugins.get(pluginId);
  }

  /**
   * 检查插件是否已加载
   */
  public isPluginLoaded(pluginId: string): boolean {
    const instance = this.getPluginInstance(pluginId);
    return instance?.status === 'loaded' && instance.instance !== null;
  }

  /**
   * 获取所有已加载的插件
   */
  public getLoadedPlugins(): PluginInstance[] {
    return Array.from(this.plugins.values()).filter(
      (instance) => instance.status === 'loaded'
    );
  }

  /**
   * 批量加载插件
   */
  public async loadMultiplePlugins(
    pluginIds: string[],
    container?: string,
    props?: any
  ): Promise<Map<string, MicroApp | null>> {
    const results = new Map<string, MicroApp | null>();
    
    for (const pluginId of pluginIds) {
      const app = await this.loadPlugin(pluginId, container, props);
      results.set(pluginId, app);
    }
    
    return results;
  }

  /**
   * 批量卸载插件
   */
  public async unloadMultiplePlugins(pluginIds: string[]): Promise<boolean[]> {
    const results = [];
    for (const pluginId of pluginIds) {
      const result = await this.unloadPlugin(pluginId);
      results.push(result);
    }
    return results;
  }

  /**
   * 更新插件配置
   */
  public updatePluginConfig(pluginId: string, newConfig: Partial<PluginMetadata>): boolean {
    const existingConfig = this.pluginConfigs.get(pluginId);
    if (!existingConfig) {
      return false;
    }

    const updatedConfig = { ...existingConfig, ...newConfig };
    this.pluginConfigs.set(pluginId, updatedConfig);
    console.log(`[PluginManager] Plugin config updated: ${pluginId}`);
    return true;
  }

  /**
   * 启用插件
   */
  public enablePlugin(pluginId: string): boolean {
    return this.updatePluginConfig(pluginId, { enabled: true });
  }

  /**
   * 禁用插件
   */
  public disablePlugin(pluginId: string): boolean {
    // 如果插件正在运行，先卸载
    if (this.isPluginLoaded(pluginId)) {
      this.unloadPlugin(pluginId);
    }
    return this.updatePluginConfig(pluginId, { enabled: false });
  }

  /**
   * 获取插件状态
   */
  public getPluginStatus(pluginId: string) {
    const instance = this.getPluginInstance(pluginId);
    if (!instance) {
      const config = this.getPluginConfig(pluginId);
      return {
        loaded: false,
        enabled: config ? config.enabled : false,
        status: 'not_registered' as const,
      };
    }
    
    return {
      loaded: instance.status === 'loaded',
      enabled: instance.metadata.enabled,
      status: instance.status,
    };
  }
}

// 创建全局插件管理器实例
export const pluginManager = PluginManager.getInstance();

// 插件通信总线
export class PluginEventBus {
  private static instance: PluginEventBus;
  private eventListeners: Map<string, Array<(data: any) => void>> = new Map();

  private constructor() {}

  public static getInstance(): PluginEventBus {
    if (!PluginEventBus.instance) {
      PluginEventBus.instance = new PluginEventBus();
    }
    return PluginEventBus.instance;
  }

  /**
   * 发布事件
   */
  public publish(eventType: string, data?: any): void {
    const listeners = this.eventListeners.get(eventType);
    if (listeners) {
      listeners.forEach((listener) => {
        try {
          listener(data);
        } catch (error) {
          console.error(`Error in event listener for ${eventType}:`, error);
        }
      });
    }
  }

  /**
   * 订阅事件
   */
  public subscribe(eventType: string, listener: (data: any) => void): () => void {
    if (!this.eventListeners.has(eventType)) {
      this.eventListeners.set(eventType, []);
    }
    const listeners = this.eventListeners.get(eventType)!;
    listeners.push(listener);

    // 返回取消订阅的函数
    return () => {
      this.unsubscribe(eventType, listener);
    };
  }

  /**
   * 取消订阅事件
   */
  public unsubscribe(eventType: string, listener: (data: any) => void): void {
    const listeners = this.eventListeners.get(eventType);
    if (listeners) {
      const index = listeners.indexOf(listener);
      if (index > -1) {
        listeners.splice(index, 1);
      }
    }
  }

  /**
   * 一次性订阅
   */
  public once(eventType: string, listener: (data: any) => void): void {
    const unsubscribe = this.subscribe(eventType, (data) => {
      unsubscribe();
      listener(data);
    });
  }
}

export const pluginEventBus = PluginEventBus.getInstance();