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
  // Check if current route belongs to this micro-app
  // Assuming route path starts with /micro/{pluginId}
  return route.path.startsWith(`/micro/${id}`);
};

const handleRouteChange = async () => {
  if (!route.path.startsWith('/micro/')) return;

  const pluginId = route.path.split('/')[2];
  if (!pluginId) return;

  await loadApp(pluginId);
};

// Watch for route changes
watch(() => route.path, handleRouteChange, { immediate: true });

// Cleanup non-keep-alive apps or on destroy
// Note: For now we keep all "loaded" apps in the map if they are keep-alive.
// If we need to destroy non-keep-alive apps when leaving them, we can add logic here.
watch(() => route.path, (newPath, oldPath) => {
    if (oldPath && oldPath.startsWith('/micro/')) {
        const oldPluginId = oldPath.split('/')[2];
        const app = loadedApps.value[oldPluginId];
        if (app && !app.isKeepAlive) {
             console.log(`[MicroAppContainer] Unmounting non-keep-alive app: ${oldPluginId}`);
             app.instance.unmount();
             delete loadedApps.value[oldPluginId];
        }
    }
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
