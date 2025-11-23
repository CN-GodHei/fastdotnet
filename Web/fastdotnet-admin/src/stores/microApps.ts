import { defineStore } from 'pinia';

export interface MicroAppConfig {
    name: string;
    entry: string;
    container: string;
    activeRule: string;
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
