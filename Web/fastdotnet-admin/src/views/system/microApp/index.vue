<template>
  <div class="micro-app-container">
    <!-- 微应用加载状态 -->
    <div v-if="loading" class="loading">Loading micro app...</div>
    <div v-if="error" class="error">Failed to load micro app: {{ error }}</div>
    
    <!-- 微应用容器 -->
    <div id="subapp-viewport" class="micro-app-wrapper"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onBeforeUnmount, watch } from 'vue'
import { useRoute } from 'vue-router'
import { loadMicroApp, type MicroApp, type FrameworkConfiguration } from 'qiankun'
import request from '/@/utils/request'

const route = useRoute()
console.log('MicroApp component loaded. Current route:', route); // Log when component is loaded

const loading = ref(false)
const error = ref<string | null>(null)
const microAppInstance = ref<MicroApp | null>(null)

const getMicroAppModule = () => {
  const module = route.meta?.module ? String(route.meta.module) : null
  console.log('MicroApp module from route.meta:', module); // Log module extraction
  return module
}

const getMicroAppBasePath = () => {
  const path = route.path
  console.log('Calculating base path for route path:', path); // Log path used for base path calculation
  const parts = path.split('/')
  const microIndex = parts.indexOf('micro')
  if (microIndex !== -1 && parts.length > microIndex + 1) {
    const basePath = '/' + parts.slice(1, microIndex + 2).join('/')
    console.log('Calculated base path:', basePath); // Log calculated base path
    return basePath
  }
  const fallbackPath = path
  console.log('Fallback base path:', fallbackPath); // Log fallback path
  return fallbackPath
}

const fetchMicroAppEntry = async (pluginId: string) => {
  console.log('Fetching entry for plugin:', pluginId); // Log plugin ID for entry fetch
  try {
    const response = await request({
      url: `/api/plugin/getpluginqiankunentry/${pluginId}`,
      method: 'get'
    })
    console.log('Fetched entry for plugin:', pluginId, 'Entry:', response.Entry); // Log fetched entry
    return response.Entry
  } catch (err) {
    console.error(`Failed to fetch entry for plugin ${pluginId}:`, err)
    throw new Error(`无法获取插件 ${pluginId} 的入口地址`)
  }
}

const loadApp = async (appModule: string) => {
  console.log('Attempting to load micro app module:', appModule); // Log module being loaded
  loading.value = true
  error.value = null

  try {
    if (microAppInstance.value) {
      await microAppInstance.value.unmount!()
      microAppInstance.value = null
    }
    
    const entryPoint = await fetchMicroAppEntry(appModule)
    
    const basePath = getMicroAppBasePath()
    const appName = `app-${appModule.toLowerCase()}`
    
    const appConfig: MicroApp = {
      name: appName,
      entry: entryPoint,
      container: '#subapp-viewport',
      activeRule: basePath
    }
    console.log('qiankun appConfig:', appConfig); // Log qiankun config
    
    const frameworkConfig: FrameworkConfiguration = {
      sandbox: {
        strictStyleIsolation: false,
        experimentalStyleIsolation: true
      },
      prefetch: false // 暂时关闭预加载，简化调试
    }
    
    microAppInstance.value = loadMicroApp(appConfig, frameworkConfig)
    await microAppInstance.value.mountPromise
    console.log(`Micro app ${appName} (${appModule}) loaded successfully from ${entryPoint} with activeRule ${basePath}`)
  } catch (err) {
    console.error('Failed to load micro app:', err)
    // 提供更详细的错误信息
    if (err instanceof Error) {
        error.value = `${err.name}: ${err.message}`;
        if (err.stack) {
            console.error('Stack trace:', err.stack);
        }
    } else {
        error.value = `Unknown error: ${String(err)}`;
    }
  } finally {
    loading.value = false
  }
}

const unloadApp = async () => {
  console.log('Attempting to unload micro app'); // Log unload attempt
  if (microAppInstance.value) {
    try {
      await microAppInstance.value.unmount!()
      console.log('Micro app unloaded successfully')
    } catch (err) {
      console.error('Failed to unload micro app:', err)
    }
    microAppInstance.value = null
  }
}

watch(
  () => route.path,
  async () => {
    console.log('Route path changed to:', route.path); // Log route path change
    const appModule = getMicroAppModule()
    console.log('App module for route:', appModule); // Log determined module
    if (appModule) {
      await loadApp(appModule)
    } else {
      await unloadApp()
    }
  },
  { immediate: true }
)

onBeforeUnmount(async () => {
  console.log('MicroApp component before unmount'); // Log before unmount
  await unloadApp()
})
</script>

<style scoped>
.micro-app-container {
  width: 100%;
  height: 100%;
  position: relative;
}

.loading, .error {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 16px;
}

.loading {
  color: #909399;
}

.error {
  color: #F56C6C;
}

.micro-app-wrapper {
  width: 100%;
  height: 100%;
}
</style>