import { baseSignalRManager } from '@/utils/signalr';

// 插件SignalR工具类（完全由插件控制）
class PluginSignalRToolkit {
    private pluginId: string;
    private baseSignalRManager: any; // 这里应该使用BaseSignalRManager类型，但为了避免循环依赖，使用any
    private eventHandlers = new Map<string, Set<Function>>(); // 插件自己的事件处理器管理

    constructor(pluginId: string, baseSignalRManager: any) {
        this.pluginId = pluginId;
        this.baseSignalRManager = baseSignalRManager;
    }

    // 获取插件Hub名称（插件可以自定义规则）
    getPluginHubName(): string {
        // 由于无法动态注册Hub，我们使用主框架的通用Hub
        return "UniversalHub";
    }

    // 注册插件特定的全局事件（如果需要）
    registerGlobalEventHandler(eventName: string, handler: Function): void {
        let handlers = this.eventHandlers.get(eventName);
        if (!handlers) {
            handlers = new Set<Function>();
            this.eventHandlers.set(eventName, handlers);
        }
        handlers.add(handler);

        // 注册到基础管理器
        this.baseSignalRManager.on(eventName, handler);
    }

    // 移除插件特定的全局事件
    unregisterGlobalEventHandler(eventName: string, handler: Function): void {
        const handlers = this.eventHandlers.get(eventName);
        if (handlers) {
            handlers.delete(handler);
            this.baseSignalRManager.off(eventName, handler);
        }
    }

    // 清理所有资源
    cleanup(): void {
        // 清理所有事件处理器
        this.eventHandlers.forEach((handlers, eventName) => {
            handlers.forEach(handler => {
                this.baseSignalRManager.off(eventName, handler);
            });
        });
        this.eventHandlers.clear();
    }
}

// 插件Hub客户端（通过主框架的通用Hub进行通信）
class PluginHubClient {
    private pluginId: string;
    private baseSignalRManager: any; // 这里应该使用BaseSignalRManager类型，但为了避免循环依赖，使用any
    private eventHandlers = new Map<string, Set<Function>>(); // 该Hub的事件处理器

    constructor(pluginId: string, baseSignalRManager: any) {
        this.pluginId = pluginId;
        this.baseSignalRManager = baseSignalRManager;
    }

    // 调用Hub方法（通过主框架的通用Hub）
    async invoke(methodName: string, ...args: any[]): Promise<any> {
        try {
            // 通过主框架的通用Hub调用方法
            // 我们将插件ID作为第一个参数传递，以便主框架知道是哪个插件在调用
            const result = await this.baseSignalRManager.invokeHubMethod("UniversalHub", methodName, this.pluginId, ...args);
            //console.log(`[PluginHub] 调用${this.pluginId}.${methodName}成功`);
            return result;
        } catch (error) {
            console.error(`[PluginHub] 调用${this.pluginId}.${methodName}失败:`, error);
            throw error;
        }
    }

    // 注册事件处理器（监听主框架通用Hub的事件）
    on(eventName: string, handler: Function): void {
        // 为插件特定的事件添加前缀
        const pluginEventName = `plugin-${this.pluginId}-${eventName}`;
        
        // 记录事件处理器
        let handlers = this.eventHandlers.get(pluginEventName);
        if (!handlers) {
            handlers = new Set<Function>();
            this.eventHandlers.set(pluginEventName, handlers);
        }
        handlers.add(handler);

        // 注册到基础管理器
        this.baseSignalRManager.on(pluginEventName, handler);
        //console.log(`[PluginHub] 注册${this.pluginId}事件处理器: ${pluginEventName}`);
    }

    // 移除事件处理器
    off(eventName: string, handler: Function): void {
        const pluginEventName = `plugin-${this.pluginId}-${eventName}`;
        const handlers = this.eventHandlers.get(pluginEventName);
        if (handlers) {
            handlers.delete(handler);
            this.baseSignalRManager.off(pluginEventName, handler);
            //console.log(`[PluginHub] 移除${this.pluginId}事件处理器: ${pluginEventName}`);
        }
    }

    // 移除所有事件处理器
    removeAllListeners(): void {
        this.eventHandlers.forEach((handlers, eventName) => {
            handlers.forEach(handler => {
                this.baseSignalRManager.off(eventName, handler);
            });
        });
        this.eventHandlers.clear();
        //console.log(`[PluginHub] 移除${this.pluginId}所有事件处理器`);
    }

    // 清理资源
    cleanup(): void {
        this.removeAllListeners();
    }
}

export { PluginSignalRToolkit, PluginHubClient };