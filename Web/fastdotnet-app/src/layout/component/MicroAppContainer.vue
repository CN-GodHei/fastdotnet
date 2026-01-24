<template>
  <div class="micro-app-container">
    <div
      v-for="(app, id) in loadedApps"
      :key="id"
      :id="app.containerId"
      v-show="isAppVisible(id)"
      class="micro-app-wrapper h100"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { loadMicroApp, MicroApp } from 'qiankun';
import { useMicroAppsStore } from '@/stores/microApps';

const route = useRoute();
const microAppsStore = useMicroAppsStore();

// Track loaded apps: pluginId -> { appInstance, containerId, isKeepAlive }
interface LoadedApp {
  instance: MicroApp;
  containerId: string;
  isKeepAlive: boolean;
}
const loadedApps = ref<Record<string, LoadedApp>>({});

// Generate a unique container ID for each app
const getContainerId = (name: string) => `micro-app-${name}`;

const loadApp = async (pluginId: string) => {
  if (loadedApps.value[pluginId]) return;

  const config = microAppsStore.getMicroAppConfig(pluginId);
  if (!config) {
    console.warn(`[MicroAppContainer] No config found for plugin: ${pluginId}`);
    return;
  }

  const containerId = getContainerId(pluginId);
  
  // Ensure DOM element exists before loading
  await nextTick();

  console.log(`[MicroAppContainer] Loading app: ${config.name}`);
  const instance = loadMicroApp({
    name: config.name,
    entry: config.entry,
    container: `#${containerId}`,
    props: config.props,
  }, {
      sandbox: { experimentalStyleIsolation: true },
      // globalContext: window // Optional, depending on needs
  });

  loadedApps.value[pluginId] = {
    instance,
    containerId,
    isKeepAlive: config.props.menuInfo?.isKeepAlive !== false // Default to true
  };
};

const isAppVisible = (id: string) => {
  // Check if current route is a micro-app AND matches this pluginId
  return route.meta.isFdMicroApp && route.meta.pluginId === id;
};

const handleRouteChange = async () => {
  if (!route.meta.isFdMicroApp) return;

  const pluginId = route.meta.pluginId as string;
  if (!pluginId) return;

  await loadApp(pluginId);
};

// Watch for route changes
watch(() => route.path, handleRouteChange, { immediate: true });

// Watch for store changes to handle initial load race condition (when refreshing page)
watch(() => microAppsStore.microAppConfigs, (newConfigs) => {
  if (newConfigs.size > 0 && route.meta.isFdMicroApp) {
    handleRouteChange();
  }
});

// Cleanup non-keep-alive apps or on destroy
// Note: For now we keep all "loaded" apps in the map if they are keep-alive.
// If we need to destroy non-keep-alive apps when leaving them, we can add logic here.
// Cleanup non-keep-alive apps or on destroy
// Note: For now we keep all "loaded" apps in the map if they are keep-alive.
// If we need to destroy non-keep-alive apps when leaving them, we can add logic here.
watch(() => route.path, (newPath, oldPath) => {
    // We can't easily get the old route's meta here directly without storing it or using router.afterEach
    // But we can check if any loaded app is NOT the current one and needs to be unmounted
    
    // Simple approach: Iterate loaded apps, if not current and not keep-alive, unmount
    const currentPluginId = route.meta.pluginId;
    
    Object.keys(loadedApps.value).forEach(pluginId => {
        if (pluginId !== currentPluginId) {
            const app = loadedApps.value[pluginId];
            if (app && !app.isKeepAlive) {
                 console.log(`[MicroAppContainer] Unmounting non-keep-alive app: ${pluginId}`);
                 app.instance.unmount();
                 delete loadedApps.value[pluginId];
            }
        }
    });
});

onUnmounted(() => {
  Object.values(loadedApps.value).forEach(app => app.instance.unmount());
});
</script>

<style scoped>
.micro-app-container {
  width: 100%;
  height: 100%;
}
.micro-app-wrapper {
    width: 100%;
    height: 100%;
}
</style>
