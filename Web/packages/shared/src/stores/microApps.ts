import { defineStore } from 'pinia';

export interface MicroAppConfig {
    name: string;
    entry: string;
    /**
     * 已废弃：qiankun 3.0 的 container 只能是 HTMLElement，实际容器由加载方
     * （MicroAppContainer / PluginManager）自行查询后传入，此处不再使用。
     */
    container?: string;
    /** 已废弃：本项目只走 loadMicroApp，未使用 registerMicroApps，不再需要 activeRule。 */
    activeRule?: string;
    props: any;
}

interface MicroAppsState {
    microAppConfigs: Map<string, MicroAppConfig>;
}

export const useMicroAppsStore = defineStore('microApps', {
    state: (): MicroAppsState => ({
        microAppConfigs: new Map(),
    }),
    actions: {
        setMicroApps(configs: Map<string, MicroAppConfig>) {
            this.microAppConfigs = configs;
        },
        getMicroAppConfig(id: string) {
            return this.microAppConfigs.get(id);
        }
    },
});
