import { getSharedSignalRManager } from '@/main'; // 从主应用获取共享的SignalR管理器

// 插件SignalR管理器
class PluginASignalRManager {
    baseSignalRManager: any; // 从主应用传递的SignalR管理器实例

    constructor() {
        this.baseSignalRManager = getSharedSignalRManager();
    }

    // 初始化插件SignalR
    async initialize(): Promise<void> {
        try {
            // 插件可以在这里进行特定的初始化逻辑
            console.log('[PluginA SignalR] 插件SignalR管理器已初始化');
        } catch (error) {
            console.error('[PluginA SignalR] 插件SignalR管理器初始化失败:', error);
            throw error;
        }
    }

    // 发送消息
    async sendMessage(message: any): Promise<void> {
        try {
            // 通过主应用的SignalR管理器发送消息
            await this.baseSignalRManager.invokeHubMethod("UniversalHub", "SendPluginNotification", "PluginA", message);
            console.log('[PluginA SignalR] 消息发送成功');
        } catch (error) {
            console.error('[PluginA SignalR] 消息发送失败:', error);
            throw error;
        }
    }

    // 清理资源
    cleanup(): void {
        console.log('[PluginA SignalR] 插件SignalR管理器已清理');
    }
}

export { PluginASignalRManager };