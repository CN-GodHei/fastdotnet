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
// --- 共享依赖结束 ---

import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';
import ElementPlus from 'element-plus';
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
    if (qiankunStarted) {
        return;
    }
    qiankunStarted = true;

    try {
        const menuApi = useMenuApi();
        const allMenus = await menuApi.getUserMenuTree();

        const microAppConfigs = new Map();
        const extractMicroApps = (menus: any[]) => {
            for (const menu of menus) {
                if (menu.IsFdMicroApp && menu.Module) {
                    if (!microAppConfigs.has(menu.Module)) {
                        const entry = menu.EntryPoint || `//localhost:8083`;
                        const appName = `app-${menu.Module}`;
                        microAppConfigs.set(menu.Module, {
                            name: appName,
                            entry: entry,
                            container: '#subapp-viewport',
                            activeRule: `/micro/${menu.Module}`,
                        });
                    }
                }
                if (menu.children && menu.children.length > 0) {
                    extractMicroApps(menu.children);
                }
            }
        };
        extractMicroApps(allMenus);

        const apps = Array.from(microAppConfigs.values());
        if (apps.length > 0) {
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