/**
 * 插件API
 * 提供插件间通信、服务调用等功能
 */

import { pluginEventBus, PluginEventBus } from './PluginManager';
import { pluginRegistry } from './PluginRegistry';

export interface PluginAPIMessage {
  fromPlugin: string;
  toPlugin?: string; // 如果为空，则广播给所有插件
  action: string;
  payload: any;
  timestamp: number;
  messageId?: string;
}

export interface PluginService {
  name: string;
  provider: string; // 提供服务的插件ID
  execute: (...args: any[]) => Promise<any>;
  description?: string;
}

export interface PluginUIComponent {
  pluginId: string;
  componentName: string;
  component: any; // Vue Component
  description?: string;
}

export class PluginAPI {
  private static instance: PluginAPI;
  private services: Map<string, PluginService> = new Map(); // serviceName -> service
  private serviceProviders: Map<string, string[]> = new Map(); // pluginId -> serviceNames[]
  private uiComponents: Map<string, PluginUIComponent> = new Map(); // key -> component
  private eventBus: PluginEventBus;

  private constructor() {
    this.eventBus = pluginEventBus;
  }

  public static getInstance(): PluginAPI {
    if (!PluginAPI.instance) {
      PluginAPI.instance = new PluginAPI();
    }
    return PluginAPI.instance;
  }

  /**
   * 发送消息给其他插件
   */
  public sendMessage(message: Omit<PluginAPIMessage, 'timestamp'>): void {
    const fullMessage: PluginAPIMessage = {
      ...message,
      timestamp: Date.now(),
      messageId: `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
    };

    if (message.toPlugin) {
      // 发送给特定插件
      this.eventBus.publish(`plugin-message:${message.toPlugin}`, fullMessage);
    } else {
      // 广播给所有插件
      this.eventBus.publish('plugin-message:broadcast', fullMessage);
    }
  }

  /**
   * 订阅插件消息
   */
  public subscribeToMessages(
    pluginId: string, 
    handler: (message: PluginAPIMessage) => void
  ): () => void {
    const unsubscribeBroadcast = this.eventBus.subscribe('plugin-message:broadcast', (message: PluginAPIMessage) => {
      handler(message);
    });

    const unsubscribeSpecific = this.eventBus.subscribe(`plugin-message:${pluginId}`, (message: PluginAPIMessage) => {
      handler(message);
    });

    // 返回取消订阅函数
    return () => {
      unsubscribeBroadcast();
      unsubscribeSpecific();
    };
  }

  /**
   * 注册服务
   */
  public registerService(service: PluginService): boolean {
    if (this.services.has(service.name)) {
      console.warn(`[PluginAPI] Service ${service.name} already registered`);
      return false;
    }

    this.services.set(service.name, service);

    // 记录服务提供者
    if (!this.serviceProviders.has(service.provider)) {
      this.serviceProviders.set(service.provider, []);
    }
    this.serviceProviders.get(service.provider)!.push(service.name);

    console.log(`[PluginAPI] Service registered: ${service.name} by ${service.provider}`);
    return true;
  }

  /**
   * 取消注册服务
   */
  public unregisterService(serviceName: string, providerId: string): boolean {
    const service = this.services.get(serviceName);
    if (!service || service.provider !== providerId) {
      return false;
    }

    this.services.delete(serviceName);

    // 从服务提供者列表中移除
    const providers = this.serviceProviders.get(providerId);
    if (providers) {
      const index = providers.indexOf(serviceName);
      if (index > -1) {
        providers.splice(index, 1);
      }
    }

    return true;
  }

  /**
   * 注册UI组件（用于用户扩展面板等场景）
   */
  public registerUIComponent(component: PluginUIComponent): boolean {
    const key = `${component.pluginId}_${component.componentName}`;
    if (this.uiComponents.has(key)) {
      console.warn(`[PluginAPI] UI Component ${key} already registered`);
      return false;
    }

    this.uiComponents.set(key, component);
    console.log(`[PluginAPI] UI Component registered: ${key}`);
    return true;
  }

  /**
   * 获取注册的UI组件
   */
  public getUIComponent(pluginId: string, componentName: string): any {
    const key = `${pluginId}_${componentName}`;
    return this.uiComponents.get(key)?.component;
  }

  /**
   * 获取特定插件注册的所有UI组件
   */
  public getUIComponentsByPlugin(pluginId: string): PluginUIComponent[] {
    return Array.from(this.uiComponents.values())
      .filter(c => c.pluginId === pluginId);
  }

  /**
   * 注销UI组件
   */
  public unregisterUIComponent(pluginId: string, componentName: string): boolean {
    const key = `${pluginId}_${componentName}`;
    return this.uiComponents.delete(key);
  }

  /**
   * 调用服务
   */
  public async callService(serviceName: string, ...args: any[]): Promise<any> {
    const service = this.services.get(serviceName);
    if (!service) {
      throw new Error(`Service ${serviceName} not found`);
    }

    try {
      return await service.execute(...args);
    } catch (error) {
      console.error(`[PluginAPI] Error calling service ${serviceName}:`, error);
      throw error;
    }
  }

  /**
   * 获取可用的服务列表
   */
  public getAvailableServices(): string[] {
    return Array.from(this.services.keys());
  }

  /**
   * 获取特定插件提供的服务
   */
  public getServicesByProvider(pluginId: string): PluginService[] {
    const serviceNames = this.serviceProviders.get(pluginId) || [];
    return serviceNames
      .map(name => this.services.get(name))
      .filter(Boolean) as PluginService[];
  }

  /**
   * 检查插件是否可用
   */
  public isPluginAvailable(pluginId: string): boolean {
    // 检查插件是否在注册表中且已启用
    return pluginRegistry.isPluginEnabled(pluginId);
  }

  /**
   * 获取插件信息
   */
  public getPluginInfo(pluginId: string) {
    return pluginRegistry.getPluginMetadata(pluginId);
  }

  /**
   * 广播系统事件
   */
  public broadcastSystemEvent(eventType: string, data: any): void {
    const eventData = {
      type: eventType,
      data,
      timestamp: Date.now(),
      source: 'system'
    };

    this.eventBus.publish(`system-event:${eventType}`, eventData);
    this.eventBus.publish('system-event:all', eventData);
  }

  /**
   * 监听系统事件
   */
  public subscribeToSystemEvent(
    eventType: string, 
    handler: (data: any) => void
  ): () => void {
    const unsubscribeType = this.eventBus.subscribe(`system-event:${eventType}`, (data: any) => {
      handler(data);
    });

    const unsubscribeAll = this.eventBus.subscribe('system-event:all', (data: any) => {
      if (data.type === eventType) {
        handler(data);
      }
    });

    return () => {
      unsubscribeType();
      unsubscribeAll();
    };
  }

  /**
   * 插件间共享数据
   */
  private sharedData: Map<string, any> = new Map(); // key -> data

  /**
   * 设置共享数据
   */
  public setSharedData(key: string, data: any, pluginId: string): void {
    const sharedKey = `${pluginId}:${key}`;
    this.sharedData.set(sharedKey, data);
    
    // 广播数据变更事件
    this.broadcastSystemEvent('shared-data-updated', {
      key: sharedKey,
      data,
      pluginId
    });
  }

  /**
   * 获取共享数据
   */
  public getSharedData(key: string, pluginId: string): any {
    const sharedKey = `${pluginId}:${key}`;
    return this.sharedData.get(sharedKey);
  }

  /**
   * 获取其他插件的共享数据
   */
  public getOtherPluginSharedData(key: string, otherPluginId: string): any {
    const sharedKey = `${otherPluginId}:${key}`;
    return this.sharedData.get(sharedKey);
  }

  /**
   * 移除共享数据
   */
  public removeSharedData(key: string, pluginId: string): boolean {
    const sharedKey = `${pluginId}:${key}`;
    return this.sharedData.delete(sharedKey);
  }

  /**
   * 获取插件列表
   */
  public getPluginList(): { id: string; name: string; enabled: boolean }[] {
    return pluginRegistry.getAllPlugins().map(plugin => ({
      id: plugin.id,
      name: plugin.name,
      enabled: plugin.enabled
    }));
  }

  /**
   * 获取激活的插件信息
   */
  public getActivePlugins(): { id: string; name: string; description: string; version: string; entry: string }[] {
    return pluginRegistry.getAllPlugins()
      .filter(plugin => plugin.enabled)
      .map(plugin => ({
        id: plugin.id,
        name: plugin.name,
        description: plugin.description,
        version: plugin.version,
        entry: plugin.microAppConfig?.entry || ''
      }));
  }
}

// 创建全局插件API实例
export const pluginAPI = PluginAPI.getInstance();

// 便捷的插件通信函数
export const usePluginCommunication = (pluginId: string) => {
  const api = PluginAPI.getInstance();
  
  return {
    // 发送消息给其他插件
    sendMessage: (toPlugin: string, action: string, payload: any) => {
      api.sendMessage({
        fromPlugin: pluginId,
        toPlugin,
        action,
        payload
      });
    },

    // 广播消息
    broadcastMessage: (action: string, payload: any) => {
      api.sendMessage({
        fromPlugin: pluginId,
        action,
        payload
      });
    },

    // 订阅消息
    subscribe: (handler: (message: PluginAPIMessage) => void) => {
      return api.subscribeToMessages(pluginId, handler);
    },

    // 注册服务
    registerService: (name: string, execute: (...args: any[]) => Promise<any>, description?: string) => {
      return api.registerService({
        name,
        provider: pluginId,
        execute,
        description
      });
    },

    // 调用服务
    callService: async (serviceName: string, ...args: any[]) => {
      return await api.callService(serviceName, ...args);
    },

    // 设置共享数据
    setSharedData: (key: string, data: any) => {
      api.setSharedData(key, data, pluginId);
    },

    // 获取共享数据
    getSharedData: (key: string) => {
      return api.getSharedData(key, pluginId);
    },

    // 获取其他插件的共享数据
    getOtherPluginSharedData: (key: string, otherPluginId: string) => {
      return api.getOtherPluginSharedData(key, otherPluginId);
    }
  };
};