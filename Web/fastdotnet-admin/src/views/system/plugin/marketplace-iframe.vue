<template>
  <div class="marketplace-iframe-container" style="height: 100%; position: relative;">
    <!-- iframe - 强制占满整个容器 -->
    <iframe ref="marketplaceIframe" :src="iframeSrc" frameborder="0"
      style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; width: 100%; height: 100%;" @load="onIframeLoad"
      @error="onIframeError"></iframe>

    <!-- 加载失败提示 -->
    <div v-if="loadError" class="error-overlay">
      <el-result icon="error" title="插件商城加载失败" sub-title="无法连接到插件商城服务，请检查网络是否正常或前往官网使用离线版本">
        <template #extra>
          <div class="error-actions">
            <el-button type="primary" @click="retryLoad" :loading="retrying">
              <el-icon>
                <Refresh />
              </el-icon>
              重试
            </el-button>
            <el-button @click="showDetails">
              <el-icon>
                <InfoFilled />
              </el-icon>
              查看详情
            </el-button>
            <el-button type="success" @click="goToOfficialWebsite">
              <el-icon>
                <Link />
              </el-icon>
              前往官网
            </el-button>
          </div>
        </template>
      </el-result>

      <!-- 错误详情对话框 -->
      <el-dialog v-model="errorDetailVisible" title="错误详情" width="600px">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="错误时间">
            {{ errorTime }}
          </el-descriptions-item>
          <el-descriptions-item label="请求地址">
            {{ getDomainFromUrl(iframeSrc) }}
          </el-descriptions-item>
          <el-descriptions-item label="错误信息">
            {{ errorMessage }}
          </el-descriptions-item>
        </el-descriptions>
        <el-alert class="mt15" title="可能的原因" type="warning" :closable="false">
          <ul style="margin: 10px 0; padding-left: 20px;">
            <li>网络连接问题</li>
            <li>防火墙或安全软件阻止访问</li>
          </ul>
        </el-alert>
      </el-dialog>
    </div>

    <!-- 加载中提示 -->
    <div v-if="loading && !loadError" class="loading-overlay">
      <el-card shadow="hover" class="loading-card">
        <div class="loading-content">
          <el-icon class="is-loading" :size="48">
            <Loading />
          </el-icon>
          <p class="loading-text">正在加载插件商城...</p>
        </div>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts" name="pluginMarketplaceIframe">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { ElMessage } from 'element-plus'
import { Loading, Refresh, InfoFilled, Link } from '@element-plus/icons-vue'
import { getApiPluginScan, postApiPluginLoad, postApiPluginSetAuthCode } from '@/api/fd-system-api-admin/Plugin'
import { usePluginStore } from '@/stores/plugin'

// 定义事件发射器
const emit = defineEmits<{
  (e: 'plugin-action', event: CustomEvent): void
}>()

// 使用插件 store
const pluginStore = usePluginStore()

// iframe 引用
const marketplaceIframe = ref<HTMLIFrameElement | null>(null)

// 加载状态
const loading = ref(true)
const loadError = ref(false)
const retrying = ref(false)
const errorDetailVisible = ref(false)
const errorMessage = ref('')
const errorTime = ref('')

// iframe 导航状态
const canGoBack = ref(false)
const canGoForward = ref(false)

// iframe 源地址 - 指向我们新创建的插件管理页面
const iframeSrc = ref('https://fastdotnet.top/plugin-manager/embedded?layout=none') // 指向 Nuxt 项目的插件管理页面

// 记录开始加载时间
const loadStartTime = ref<number>(0)

// 加载超时定时器
let loadTimeoutTimer: ReturnType<typeof setTimeout> | null = null

// 计算域名（从 iframeSrc 中提取）
const getDomainFromUrl = (url: string) => {
  try {
    const urlObj = new URL(url)
    return urlObj.protocol + '//' + urlObj.host
  } catch {
    return url
  }
}

// 官网地址（从 iframeSrc 动态提取）
const officialWebsite = computed(() => getDomainFromUrl(iframeSrc.value))

// iframe 加载完成事件
const onIframeLoad = () => {
  // console.log('插件商城 iframe 加载完成')
  loadStartTime.value = Date.now()

  updateNavigationState()

  // 发送初始数据到 iframe
  sendInitialData()
}

// 处理加载失败
const handleLoadFailure = (errorMsg: string) => {
  loading.value = false
  loadError.value = true
  errorTime.value = new Date().toLocaleString('zh-CN')
  errorMessage.value = errorMsg || '无法加载插件商城页面，请检查网络连接和服务状态'

  // 计算加载耗时
  const loadDuration = Date.now() - loadStartTime.value
  if (loadDuration < 3000) {
    errorMessage.value = '连接被拒绝或服务不可用（加载耗时：' + loadDuration + 'ms）'
  } else if (loadDuration > 30000) {
    errorMessage.value = '请求超时（加载耗时：' + Math.round(loadDuration / 1000) + '秒）'
  }
}

// iframe 加载失败事件（这个事件在连接被拒绝时不会触发，需要靠超时检测）
const onIframeError = (event: Event) => {
  console.error('插件商城 iframe 加载失败:', event)
  handleLoadFailure('iframe 加载错误')
}

// 重试加载
const retryLoad = async () => {
  retrying.value = true
  loadError.value = false
  loading.value = true
  loadStartTime.value = Date.now()

  try {
    // 清除之前的定时器
    if (loadTimeoutTimer) clearTimeout(loadTimeoutTimer)

    // 重新设置 iframe 的 src 来触发重新加载
    const currentSrc = iframeSrc.value
    iframeSrc.value = ''

    // 等待一小段时间后重新加载
    await new Promise(resolve => setTimeout(resolve, 500))
    iframeSrc.value = currentSrc
    // 注意：不需要重新设置定时器，因为 iframe 重新加载后会再次触发 onIframeLoad 和发送 IFRAME_LOADED 消息
  } catch (error: any) {
    console.error('重试加载失败:', error)
    ElMessage.error('重试失败：' + (error.message || '未知错误'))
    handleLoadFailure(error.message || '重试失败')
  } finally {
    retrying.value = false
  }
}

// 显示错误详情
const showDetails = () => {
  errorDetailVisible.value = true
}

// 前往官网
const goToOfficialWebsite = () => {
  window.open(officialWebsite.value, '_blank')
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
      case 'IFRAME_LOADED':
        // console.log('收到 iframe 加载成功信号')
        if (loadTimeoutTimer) {
          clearTimeout(loadTimeoutTimer)
          loadTimeoutTimer = null
        }
        loading.value = false
        loadError.value = false
        break
      default:
      // console.log('未知消息类型:', type)
    }
  } catch (e) {
    console.error('处理 iframe 消息失败:', e)
  }
}

// 处理安装插件请求
const handleInstallPlugin = async (data: any) => {
  const { pluginId, pluginName, token, Version } = data
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
  const { AuthCode, Token } = data
  try {
    // 调用后端接口，将 AuthCode 写入
    await postApiPluginSetAuthCode({
      AuthCode: AuthCode
    })

    // 将 Token 和 AuthCode 存储到 Pinia Store，供 index.vue 使用
    pluginStore.setMarketplaceAuth({
      Token,
      AuthCode
    })

    // console.log('插件商城授权信息已保存')
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
  loadStartTime.value = Date.now()

  // 设置加载超时检测（10 秒）
  loadTimeoutTimer = setTimeout(() => {
    if (loading.value) {
      handleLoadFailure('加载超时（超过 10 秒）')
    }
  }, 10000)
})

// 组件卸载时移除事件监听和定时器
onBeforeUnmount(() => {
  window.removeEventListener('message', handleMessage)
  if (loadTimeoutTimer) clearTimeout(loadTimeoutTimer)
})
</script>

<style scoped lang="scss">
.marketplace-iframe-container {
  height: 100% !important;

  .error-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #f5f7fa;
    z-index: 10;

    .error-actions {
      display: flex;
      gap: 10px;
      margin-top: 20px;
    }
  }

  .loading-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: rgba(255, 255, 255, 0.9);
    z-index: 9;

    .loading-card {
      width: 100%;
      text-align: center;

      .loading-content {
        padding: 20px;

        .loading-text {
          margin-top: 15px;
          color: #606266;
          font-size: 14px;
        }
      }
    }
  }
}
</style>