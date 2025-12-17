<template>
  <div class="marketplace-iframe-container layout-padding">
    <el-card shadow="hover" header="插件市场" class="marketplace-card">
      <div class="toolbar mb15">
        <el-button 
          size="default" 
          type="primary" 
          @click="refreshIframe"
        >
          <el-icon><ele-Refresh /></el-icon>
          刷新
        </el-button>
        <el-button 
          size="default" 
          @click="goBack"
          :disabled="!canGoBack"
        >
          <el-icon><ele-ArrowLeft /></el-icon>
          后退
        </el-button>
        <el-button 
          size="default" 
          @click="goForward"
          :disabled="!canGoForward"
        >
          <el-icon><ele-ArrowRight /></el-icon>
          前进
        </el-button>
      </div>
      
      <div class="iframe-container">
        <iframe
          ref="marketplaceIframe"
          :src="iframeSrc"
          frameborder="0"
          width="100%"
          height="100%"
          @load="onIframeLoad"
        ></iframe>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts" name="pluginMarketplaceIframe">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { getPluginScan } from '@/api/fd-system-api/Plugin'

// 定义事件发射器
const emit = defineEmits<{
  (e: 'plugin-action', event: CustomEvent): void
}>()

// iframe引用
const marketplaceIframe = ref<HTMLIFrameElement | null>(null)

// iframe导航状态
const canGoBack = ref(false)
const canGoForward = ref(false)

// iframe源地址 - 指向插件商城的实际地址
const iframeSrc = ref('http://localhost:8099/app/plugins') // 更新为插件市场的正确地址

// 刷新iframe
const refreshIframe = () => {
  if (marketplaceIframe.value) {
    marketplaceIframe.value.src = iframeSrc.value
  }
}

// 后退
const goBack = () => {
  if (marketplaceIframe.value?.contentWindow) {
    marketplaceIframe.value.contentWindow.history.back()
  }
}

// 前进
const goForward = () => {
  if (marketplaceIframe.value?.contentWindow) {
    marketplaceIframe.value.contentWindow.history.forward()
  }
}

// iframe加载完成事件
const onIframeLoad = () => {
  console.log('插件商城iframe加载完成')
  updateNavigationState()
  
  // 发送初始数据到iframe
  sendInitialData()
}

// 更新导航状态
const updateNavigationState = () => {
  // 注意：由于同源策略限制，这部分功能可能受限
  try {
    if (marketplaceIframe.value?.contentWindow) {
      // 这些属性可能因同源策略无法访问
      // canGoBack.value = marketplaceIframe.value.contentWindow.history.length > 1
      // canGoForward.value = true // 简化处理
    }
  } catch (e) {
    // 忽略跨域错误
    console.warn('无法获取iframe导航状态:', e)
  }
}

// 发送初始数据到iframe
const sendInitialData = () => {
  try {
    // 使用postMessage发送数据到iframe
    if (marketplaceIframe.value?.contentWindow) {
      // 先获取已安装插件列表
      getPluginScan().then((res: any) => {
        const installedPlugins = res || []
        
        // 发送到iframe
        marketplaceIframe.value!.contentWindow!.postMessage({
          type: 'INITIAL_DATA',
          data: {
            installedPlugins: installedPlugins,
            timestamp: Date.now()
          }
        }, '*') // 生产环境中应指定确切的origin
      })
    }
  } catch (e) {
    console.error('发送初始数据到iframe失败:', e)
  }
}

// 监听来自iframe的消息
const handleMessage = (event: MessageEvent) => {
  try {
    // 注意：生产环境中应该验证event.origin
    const { type, data } = event.data
    
    switch (type) {
      case 'PLUGIN_ACTION':
        // 处理来自插件商城的操作请求
        handlePluginAction(data)
        break
        
      case 'GET_INSTALLED_PLUGINS':
        // 插件商城请求获取已安装插件列表
        sendInstalledPlugins()
        break
        
      case 'REFRESH_PLUGIN_LIST':
        // 插件商城请求刷新插件列表
        window.dispatchEvent(new CustomEvent('refresh-plugin-list'))
        break
        
      default:
        console.log('未知消息类型:', type)
    }
  } catch (e) {
    console.error('处理iframe消息失败:', e)
  }
}

// 处理来自插件商城的插件操作请求
const handlePluginAction = (data: any) => {
  const { action, pluginId } = data
  
  // 通过emit将事件传递给父组件
  emit('plugin-action', new CustomEvent('plugin-action', {
    detail: { action, pluginId }
  }))
}

// 发送已安装插件列表到iframe
const sendInstalledPlugins = () => {
  getPluginScan().then((res: any) => {
    const installedPlugins = res || []
    
    if (marketplaceIframe.value?.contentWindow) {
      marketplaceIframe.value.contentWindow.postMessage({
        type: 'INSTALLED_PLUGINS',
        data: {
          plugins: installedPlugins
        }
      }, '*') // 生产环境中应指定确切的origin
    }
  }).catch(error => {
    console.error('获取已安装插件列表失败:', error)
  })
}

// 组件挂载时添加事件监听
onMounted(() => {
  window.addEventListener('message', handleMessage)
})

// 组件卸载时移除事件监听
onBeforeUnmount(() => {
  window.removeEventListener('message', handleMessage)
})
</script>

<style scoped lang="scss">
.marketplace-iframe-container {
  height: 100%; // 确保容器高度为100%

  :deep(.el-card__body) {
    display: flex;
    flex-direction: column;
    height: 100%; // 确保卡片内容区域高度为100%
  }

  .toolbar {
    display: flex;
    gap: 10px;
  }

  .iframe-container {
    flex: 1; // 使用Flexbox填充剩余空间
    overflow: hidden;
    margin-top: 15px;
    height: calc(100% - 50px); // 调整高度以适应工具栏

    iframe {
      border-radius: 4px;
      box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
      height: 100%; // 确保iframe高度为100%
      width: 100%;
    }
  }
}
</style>