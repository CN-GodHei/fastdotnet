/**
 * 插件管理系统
 * 提供插件的注册、发现、加载和生命周期管理
 */

import { loadMicroApp, MicroApp } from 'qiankun';
import { useMicroAppsStore } from '@/stores/microApps';

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