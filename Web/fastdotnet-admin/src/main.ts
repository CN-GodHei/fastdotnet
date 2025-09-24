import { createApp } from 'vue';
import * as Vue from 'vue';
import pinia from '/@/stores/index';
import * as Pinia from 'pinia';
import App from '/@/App.vue';
import router from '/@/router';
import * as VueRouter from 'vue-router';
import { registerMicroApps, start } from 'qiankun';
import { useMenuApi } from '/@/api/menu';
import { Session } from '/@/utils/storage';
import request from '/@/utils/request'; // 引入主应用的 Axios 实例
import { buildMixedQuery } from '/@/utils/queryBuilder'; // 引入查询构建工具
import { baseSignalRManager } from '/@/utils/signalr'; // 引入主应用的 SignalR 管理器

// --- 共享依赖 ---
(window as any).Vue = Vue;
(window as any).VueRouter = VueRouter;
(window as any).Pinia = Pinia;
(window as any).ElementPlus = ElementPlus;
// --- 共享依赖结束 ---

import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';
import * as ElementPlus from 'element-plus';
import '/@/theme/index.scss';
import VueGridLayout from 'vue-grid-layout';

const app = createApp(App);

directive(app);
other.elSvg(app);

app.use(pinia).use(router).use(ElementPlus).use(i18n).use(VueGridLayout).mount('#app');

// 初始化主题配置（获取后端配置）
import { initializeThemeConfig } from '/@/stores/themeConfig';
initializeThemeConfig().then(() => {
    // console.log('[MainApp] Theme configuration initialized from backend');
}).catch(error => {
    console.error('[MainApp] Failed to initialize theme configuration:', error);
});

// --- Qiankun 启动逻辑 ---
// 定义一个变量，防止 qiankun 被重复启动
let qiankunStarted = false;
export async function startQiankun() {
    // 检查是否有 token，如果没有则不启动 qiankun
    const token = Session.get('token');
    if (!token) {
        console.log('[MainApp] No token found, skipping Qiankun initialization');
        return;
    }

    try {
        const menuApi = useMenuApi();
        const allMenus = await menuApi.getUserMenuTree();
        const microAppConfigs = new Map();

        // 临时的调试函数，用于根据插件ID获取本地开发服务器入口
        const getDebugEntry = (pluginId: string) => {
            // 检查当前是否为开发环境
            const isDevelopment = import.meta.env.MODE === 'development';

            if (isDevelopment) {
                // 在开发环境中，可以为特定插件ID指定本地开发服务器入口
                if (pluginId === '这是你开发时的插件Id，为了避免重复编译才能测试，你可以在这里指定本地开发服务器入口') {
                    return '//localhost:8082';
                }
                // 可以为更多插件添加调试入口
            }
            return null; // 如果没有匹配或者不是开发环境，则返回 null
        };

        const extractMicroApps = (menus: any[]) => {
            for (const menu of menus) {
                if (menu.IsFdMicroApp && menu.PluginId) {
                    if (!microAppConfigs.has(menu.PluginId)) {
                            let entry = getDebugEntry(menu.PluginId);                                       
                        // 如果还是没有entry，则使用默认的插件地址                                                       
                        if (!entry) {                                                                                    
                                entry = import.meta.env.VITE_API_URL+"/plugins/"+menu.PluginId+"/index.html";                
                        }
                        const appName = `${menu.PluginId}`;
                        const activeRule = `/micro/${menu.PluginId}`;
                        microAppConfigs.set(menu.PluginId, {
                            name: appName,
                            entry: entry,
                            container: '#subapp-viewport',
                            activeRule: activeRule,
                            props: {
                                base: activeRule,
                                // --- 添加这行 ---
                                FdRequest: request, // 将主应用的 Axios 实例通过 props 传递，名称为 FdRequest
                                // --- 添加查询构建工具 ---
                                FdQueryBuilder: { buildMixedQuery }, // 将查询构建工具通过 props 传递
                                // --- 添加SignalR管理器 ---
                                signalRManager: baseSignalRManager, // 将主应用的 SignalR 管理器通过 props 传递
                                // --- 添加结束 ---
                            },
                        });
                    }
                }
                const children = menu.children || menu.Children;
                if (children && children.length > 0) {
                    extractMicroApps(children);
                }
            }
        };
        extractMicroApps(allMenus);

        const apps = Array.from(microAppConfigs.values());
        if (apps.length > 0) {
            console.log('[MainApp] Registering micro-apps with this configuration:', apps);
            registerMicroApps(apps, {
                beforeLoad: app => {
                    console.log('[MainApp] before load', app.name);
                    return Promise.resolve();
                },
                afterMount: app => {
                    console.log('[MainApp] after mount', app.name);
                    return Promise.resolve();
                }
            });

            start({
                prefetch: 'all',
                sandbox: { experimentalStyleIsolation: true },
            });
            console.log('[MainApp] Qiankun started with apps:', apps);
        }
    } catch (error) {
        console.error('[MainApp] Failed to start Qiankun:', error);
    }
}