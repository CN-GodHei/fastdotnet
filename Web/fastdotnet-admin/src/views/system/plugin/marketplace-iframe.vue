<template>
  <div class="marketplace-iframe-container" style="height: 100%; position: relative;">
    <!-- iframe - 强制占满整个容器 -->
    <iframe
      ref="marketplaceIframe"
      :src="iframeSrc"
      frameborder="0"
      style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; width: 100%; height: 100%;"
      @load="onIframeLoad"
    ></iframe>
  </div>
</template>

<script setup lang="ts" name="pluginMarketplaceIframe">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { getApiPluginScan, postApiPluginLoad, postApiPluginSetAuthCode } from '@/api/fd-system-api-admin/Plugin'

// 定义事件发射器
const emit = defineEmits<{
  (e: 'plugin-action', event: CustomEvent): void
}>()

// iframe引用
const marketplaceIframe = ref<HTMLIFrameElement | null>(null)

// iframe导航状态
const canGoBack = ref(false)
const canGoForward = ref(false)

// iframe源地址 - 指向我们新创建的插件管理页面
const iframeSrc = ref('http://localhost:3000/plugin-manager/embedded?layout=none') // 指向Nuxt项目的插件管理页面

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
      getApiPluginScan().then((res: any) => {
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

// 监听来自 iframe 的消息
const handleMessage = (event: MessageEvent) => {
  try {
    // 注意：生产环境中应该验证 event.origin
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
        
      case 'INSTALL_PLUGIN':
        // 处理安装插件请求
        handleInstallPlugin(data)
        break
        
      case 'LOGIN_SUCCESS':
        // 处理登录成功消息
        handleLoginSuccess(data)
        break
        
      default:
        console.log('未知消息类型:', type)
    }
  } catch (e) {
    console.error('处理 iframe 消息失败:', e)
  }
}

// 处理安装插件请求
const handleInstallPlugin = async (data: any) => {
  const { pluginId, pluginName,token ,Version} = data
  
  try {
    // TODO: 调用主应用的后端 API 进行安装
    // 这里应该调用实际的安装接口
    // console.log('收到安装插件请求:', pluginId, pluginName,token)
    
    // 模拟安装过程（替换为实际的 API 调用）
    // await new Promise(resolve => setTimeout(resolve, 2000))
    await postApiPluginLoad(data);
    // 发送安装成功结果回 iframe
    if (marketplaceIframe.value?.contentWindow) {
      marketplaceIframe.value.contentWindow.postMessage({
        type: 'INSTALL_PLUGIN_RESULT',
        data: {
          success: true,
          pluginId,
          pluginName
        }
      }, '*')
    }
  } catch (error: any) {
    // 发送安装失败结果回 iframe
    if (marketplaceIframe.value?.contentWindow) {
      marketplaceIframe.value.contentWindow.postMessage({
        type: 'INSTALL_PLUGIN_RESULT',
        data: {
          success: false,
          pluginId,
          error: error.message || '安装失败'
        }
      }, '*')
    }
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

// 处理登录成功消息
const handleLoginSuccess = async (data: any) => {
  const { AuthCode } = data
  
  try {
    // 调用后端接口，将 AuthCode 写入
   var re= await postApiPluginSetAuthCode({
      AuthCode: AuthCode
    })
  } catch (error) {
    console.error('处理登录成功消息失败:', error)
  }
}

// 发送已安装插件列表到 iframe
const sendInstalledPlugins = () => {
  getApiPluginScan().then((res: any) => {
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
  height: 100% !important;
}
</style>