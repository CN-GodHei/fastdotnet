import * as signalR from '@microsoft/signalr';
import { Session } from '@/utils/storage';

// 主框架SignalR基础管理器（完全通用，不预设任何Hub）
class BaseSignalRManager {
    private connection: signalR.HubConnection | null = null;
    private isInitialized = false;
    private hubProxies = new Map<string, signalR.HubConnection>(); // 动态管理Hub代理

    // 初始化基础SignalR连接
    async initialize(accessToken: string | null = null): Promise<void> {
        if (this.isInitialized) return;

        // 创建基础SignalR连接（不指定具体Hub）
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/api/signalr", {
                accessTokenFactory: () => accessToken || Session.get('token') || ''
            })
            .build();

        // 启动连接
        await this.connection.start();
        this.isInitialized = true;

        //console.log('[BaseSignalR] 基础SignalR连接已建立');
    }

    // 动态创建Hub代理（插件可以创建任意Hub的代理）
    createHubProxy(hubName: string): signalR.HubConnection {
        if (!this.hubProxies.has(hubName)) {
            if (!this.connection) {
                throw new Error('[BaseSignalR] SignalR未初始化');
            }
            
            const proxy = this.connection; // 在这个实现中，我们复用主连接
            this.hubProxies.set(hubName, proxy);
            //console.log(`[BaseSignalR] 创建Hub代理: ${hubName}`);
        }
        return this.hubProxies.get(hubName)!;
    }

    // 获取已创建的Hub代理
    getHubProxy(hubName: string): signalR.HubConnection | null {
        return this.hubProxies.get(hubName) || null;
    }

    // 调用任意Hub的方法
    async invokeHubMethod(hubName: string, methodName: string, ...args: any[]): Promise<any> {
        if (!this.isInitialized) {
            throw new Error('[BaseSignalR] SignalR未初始化');
        }

        const hubProxy = this.createHubProxy(hubName);
        return await hubProxy.invoke(methodName, ...args);
    }

    // 更新访问令牌
    updateAccessToken(token: string): void {
        if (this.connection) {
            // 重新创建连接以更新令牌
            this.connection.stop().then(() => {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl("/api/signalr", {
                        accessTokenFactory: () => token
                    })
                    .build();
                
                this.connection.start().then(() => {
                    //console.log('[BaseSignalR] 访问令牌已更新');
                });
            });
        }
    }

    // 获取连接状态
    getConnectionState(): signalR.HubConnectionState {
        return this.connection ? this.connection.state : signalR.HubConnectionState.Disconnected;
    }

    // 动态添加事件监听器（用于插件注册通用事件）
    on(eventName: string, handler: (...args: any[]) => void): void {
        if (this.connection) {
            this.connection.on(eventName, handler);
        }
    }

    // 移除事件监听器
    off(eventName: string, handler: (...args: any[]) => void): void {
        if (this.connection) {
            this.connection.off(eventName, handler);
        }
    }
}

// 导出单例实例
export const baseSignalRManager = new BaseSignalRManager();