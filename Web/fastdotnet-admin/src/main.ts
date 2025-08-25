import { createApp } from 'vue';
import * as Vue from 'vue';
import pinia from '/@/stores/index';
import * as Pinia from 'pinia';
import App from '/@/App.vue';
import router from '/@/router';
import * as VueRouter from 'vue-router';
import { registerMicroApps, start } from 'qiankun';
import { useMenuApi } from '/@/api/menu';

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

// --- Qiankun 启动逻辑 ---
// 定义一个变量，防止 qiankun 被重复启动
let qiankunStarted = false;
export async function startQiankun() {

    try {
        const menuApi = useMenuApi();
        const allMenus = await menuApi.getUserMenuTree();

        const microAppConfigs = new Map();

        // 临时的调试函数，用于根据插件模块名获取本地开发服务器入口
        const getDebugEntry = (moduleName: string) => {
            const lowerCaseModule = moduleName.toLowerCase();
            if (lowerCaseModule.includes('plugin-a')) {
                return '//localhost:8082';
            }
            if (lowerCaseModule.includes('plugin-b')) {
                return '//localhost:8083';
            }
            return null; // 如果没有匹配，则返回 null
        };

        const extractMicroApps = (menus: any[]) => {
            for (const menu of menus) {
                if (menu.IsFdMicroApp && menu.Module) {
                    if (!microAppConfigs.has(menu.Module)) {
                        // 优先使用调试入口，如果不存在，则使用后端提供的 EntryPoint
                        // const entry = menu.EntryPoint || `//localhost:8082`;
                        const entry = getDebugEntry(menu.Module) || menu.EntryPoint;
                        const appName = `${menu.Module}`;
                        const activeRule = `/micro/${menu.Module}`;
                        microAppConfigs.set(menu.Module, {
                            name: appName,
                            entry: entry,
                            container: '#subapp-viewport',
                            activeRule: activeRule,
                            props: {
                                base: activeRule,
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