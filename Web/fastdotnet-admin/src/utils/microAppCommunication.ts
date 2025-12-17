/**
 * 主应用与微应用通信工具类
 */

// 定义通信事件类型
export enum MicroAppEvents {
  // 主应用向微应用发送事件
  UPDATE_INSTALLED_PLUGINS = 'update-installed-plugins',
  REFRESH_PLUGIN_LIST = 'refresh-plugin-list',
  
  // 微应用向主应用发送事件
  INSTALL_PLUGIN_REQUEST = 'install-plugin-request',
  UNINSTALL_PLUGIN_REQUEST = 'uninstall-plugin-request',
  ENABLE_PLUGIN_REQUEST = 'enable-plugin-request',
  DISABLE_PLUGIN_REQUEST = 'disable-plugin-request',
  REFRESH_PLUGIN_LIST_REQUEST = 'refresh-plugin-list-request'
}

// 事件回调类型
type EventCallback = (data?: any) => void;

// 事件监听器映射
const eventListeners = new Map<MicroAppEvents, EventCallback[]>();

/**
 * 发送事件到微应用
 * @param eventType 事件类型
 * @param data 数据
 */
export function sendToMicroApp(eventType: MicroAppEvents, data?: any) {
  // 通过CustomEvent发送事件
  window.dispatchEvent(new CustomEvent('micro-app-event', {
    detail: {
      type: eventType,
      data: data
    }
  }));
}

/**
 * 从微应用接收事件
 * @param eventType 事件类型
 * @param callback 回调函数
 */
export function receiveFromMicroApp(eventType: MicroAppEvents, callback: EventCallback) {
  if (!eventListeners.has(eventType)) {
    eventListeners.set(eventType, []);
  }
  
  const listeners = eventListeners.get(eventType)!;
  listeners.push(callback);
  
  // 如果是第一个监听器，添加事件监听
  if (listeners.length === 1) {
    const handler = (event: CustomEvent) => {
      const { type, data } = event.detail;
      if (type === eventType) {
        listeners.forEach(listener => listener(data));
      }
    };
    
    window.addEventListener('micro-app-event', handler as EventListener);
    // 保存handler以便后续移除监听
    (callback as any)._handler = handler;
  }
}

/**
 * 移除事件监听
 * @param eventType 事件类型
 * @param callback 回调函数
 */
export function removeMicroAppEventListener(eventType: MicroAppEvents, callback: EventCallback) {
  const listeners = eventListeners.get(eventType);
  if (listeners) {
    const index = listeners.indexOf(callback);
    if (index > -1) {
      listeners.splice(index, 1);
      
      // 如果没有监听器了，移除事件监听
      if (listeners.length === 0) {
        window.removeEventListener('micro-app-event', (callback as any)._handler);
        eventListeners.delete(eventType);
      }
    }
  }
}

/**
 * 获取主应用的插件管理API
 */
export function getPluginManagementAPI() {
  return {
    /**
     * 请求安装插件
     * @param pluginId 插件ID
     */
    installPlugin(pluginId: string) {
      sendToMicroApp(MicroAppEvents.INSTALL_PLUGIN_REQUEST, { pluginId });
    },
    
    /**
     * 请求卸载插件
     * @param pluginId 插件ID
     */
    uninstallPlugin(pluginId: string) {
      sendToMicroApp(MicroAppEvents.UNINSTALL_PLUGIN_REQUEST, { pluginId });
    },
    
    /**
     * 请求启用插件
     * @param pluginId 插件ID
     */
    enablePlugin(pluginId: string) {
      sendToMicroApp(MicroAppEvents.ENABLE_PLUGIN_REQUEST, { pluginId });
    },
    
    /**
     * 请求停用插件
     * @param pluginId 插件ID
     */
    disablePlugin(pluginId: string) {
      sendToMicroApp(MicroAppEvents.DISABLE_PLUGIN_REQUEST, { pluginId });
    },
    
    /**
     * 请求刷新插件列表
     */
    refreshPluginList() {
      sendToMicroApp(MicroAppEvents.REFRESH_PLUGIN_LIST_REQUEST);
    }
  };
}

export default {
  MicroAppEvents,
  sendToMicroApp,
  receiveFromMicroApp,
  removeMicroAppEventListener,
  getPluginManagementAPI
};