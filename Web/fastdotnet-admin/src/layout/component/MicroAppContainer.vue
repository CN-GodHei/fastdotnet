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
import { useMicroAppsStore } from '@fastdotnet/shared/stores/microApps';
import { getPluginHostConfig } from '@fastdotnet/shared/runtime/pluginHost';
import { disposeMicroApp } from '@fastdotnet/shared/runtime/microApp';

const route = useRoute();
const microAppsStore = useMicroAppsStore();

// Track loaded apps: pluginId -> { appInstance, containerId, isKeepAlive }
interface LoadedApp {
  instance: MicroApp | null;
  containerId: string;
  isKeepAlive: boolean;
}
const loadedApps = ref<Record<string, LoadedApp>>({});
// 正在加载中的 pluginId，避免同一微应用被并发加载
const loadingApps = new Set<string>();

// Generate a unique container ID for each app
const getContainerId = (name: string) => `micro-app-${name}`;

const loadApp = async (pluginId: string) => {
  if (loadedApps.value[pluginId] || loadingApps.has(pluginId)) return;

  const config = microAppsStore.getMicroAppConfig(pluginId);
  if (!config) {
    console.warn(`[MicroAppContainer] No config found for plugin: ${pluginId}`);
    return;
  }

  const containerId = getContainerId(pluginId);

  loadingApps.add(pluginId);
  try {
    // 【关键时序】容器 div 是模板里 v-for="(app, id) in loadedApps" 渲染出来的，
    // 必须先把占位项放进 loadedApps、等 DOM 真正渲染出来，才能把 HTMLElement 交给 qiankun。
    //
    // qiankun 2.x 时代这里传的是选择器字符串（'#id'），由 qiankun 内部延迟解析，
    // 等微任务跑完 div 刚好已经存在，所以「碰巧」能工作。
    // 3.0 的 container 只接受 HTMLElement，若仍按老顺序在调用前同步取元素，必然拿到 null，
    // 表现就是「页面空白、控制台只有一条 warning」。
    loadedApps.value[pluginId] = {
      instance: null,
      containerId,
      isKeepAlive: config.props.menuInfo?.isKeepAlive !== false, // Default to true
    };
    await nextTick();

    const el = document.querySelector<HTMLElement>(`#${containerId}`);
    if (!el) {
      console.error(`[MicroAppContainer] 容器未渲染出来: #${containerId}`);
      delete loadedApps.value[pluginId];
      return;
    }

    const instance = loadMicroApp({
      name: config.name,
      entry: config.entry,
      container: el,
      props: config.props,
    }, {
        // qiankun 3.0：样式隔离统一为 sandbox.styleIsolation（原生 CSS @scope），
        // 取代 2.x 的 experimentalStyleIsolation / strictStyleIsolation。
        sandbox: { styleIsolation: getPluginHostConfig().styleIsolation },
    });

    const loaded = loadedApps.value[pluginId];
    if (loaded) {
      loaded.instance = instance;
    } else {
      // 加载期间容器已被清理（例如快速切走路由），立即释放这次实例
      await disposeMicroApp(instance);
    }
  } finally {
    loadingApps.delete(pluginId);
  }
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
// Note: 释放走 disposeMicroApp —— rc.22 没有文档所称的 unload()，会退回到 unmount()。
watch(() => route.path, () => {
    // Simple approach: Iterate loaded apps, if not current and not keep-alive, dispose
    const currentPluginId = route.meta.pluginId;
    
    Object.keys(loadedApps.value).forEach(pluginId => {
        if (pluginId !== currentPluginId) {
            const app = loadedApps.value[pluginId];
            if (app && !app.isKeepAlive) {
                 console.log(`[MicroAppContainer] Disposing non-keep-alive app: ${pluginId}`);
                 disposeMicroApp(app.instance);
                 delete loadedApps.value[pluginId];
            }
        }
    });
});

onUnmounted(() => {
  Object.values(loadedApps.value).forEach(app => disposeMicroApp(app.instance));
  loadedApps.value = {};
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
