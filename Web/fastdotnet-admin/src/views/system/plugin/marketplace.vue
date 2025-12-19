<template>
  <div class="marketplace-container layout-padding">
    <el-card shadow="hover" header="插件市场">
      <!-- 插件商城微应用容器 -->
      <div id="subapp-viewport"></div>
    </el-card>
  </div>
</template>

<script setup lang="ts" name="pluginMarketplace">
import { onMounted, onUnmounted, ref } from 'vue'
import { loadMicroApp } from 'qiankun'
import { useMicroAppsStore } from '@/stores/microApps'
import { getPluginScan } from '@/api/fd-system-api-admin/Plugin'

// 定义微应用实例
let microAppInstance: any = null

// 获取已安装插件列表
const getInstalledPlugins = async () => {
  try {
    const res: any = await getPluginScan()
    return res || []
  } catch (error) {
    console.error('获取已安装插件列表失败:', error)
    return []
  }
}

// 加载插件商城微应用
const loadMarketplaceApp = async () => {
  try {
    // 获取已安装插件列表
    const installedPlugins = await getInstalledPlugins()
    
    // 创建插件商城的微应用配置
    const marketplaceConfig = {
      name: 'PluginMarketplace',
      entry: '//localhost:8081', // 开发环境下指向插件商城的开发服务器
      container: '#subapp-viewport',
      activeRule: '/marketplace',
      props: {
        base: '/marketplace'
      }
    }

    // 加载微应用
    microAppInstance = loadMicroApp({
      ...marketplaceConfig,
      props: {
        ...marketplaceConfig.props,
        // 传递当前已安装的插件列表给插件商城
        installedPlugins: installedPlugins,
        // 提供回调函数，允许插件商城获取最新的插件列表
        getInstalledPlugins: getInstalledPlugins,
        // 提供刷新插件列表的方法
        refreshPluginList: () => {
          // 可以触发事件通知父组件刷新插件列表
          window.dispatchEvent(new CustomEvent('refresh-plugin-list'))
        }
      }
    })

    // 等待微应用加载完成
    await microAppInstance.mountPromise
    console.log('插件商城微应用加载成功')
  } catch (error) {
    console.error('插件商城微应用加载失败:', error)
  }
}

// 卸载微应用
const unloadMarketplaceApp = async () => {
  if (microAppInstance) {
    try {
      await microAppInstance.unmount()
      console.log('插件商城微应用卸载成功')
    } catch (error) {
      console.error('插件商城微应用卸载失败:', error)
    }
    microAppInstance = null
  }
}

// 组件挂载时加载微应用
onMounted(() => {
  loadMarketplaceApp()
})

// 组件卸载时清理微应用
onUnmounted(() => {
  unloadMarketplaceApp()
})
</script>

<style scoped lang="scss">
.marketplace-container {
  :deep(.el-card__body) {
    display: flex;
    flex-direction: column;
    height: calc(100vh - 200px);
  }
  
  #subapp-viewport {
    flex: 1;
    overflow: auto;
  }
  
  // 保证微应用的内容能够正确显示
  :deep(#subapp-viewport > div) {
    height: 100%;
  }
}
</style>