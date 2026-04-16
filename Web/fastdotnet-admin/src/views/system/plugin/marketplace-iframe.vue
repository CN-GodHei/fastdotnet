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

    <!-- 插件安装进度对话框 -->
    <el-dialog 
      v-model="progressDialogVisible" 
      title="插件安装进度" 
      width="500px"
      :close-on-click-modal="false"
      :close-on-press-escape="false"
    >
      <div class="simple-progress">
        <!-- 进度条 -->
        <el-progress 
          :percentage="currentProgress.progress" 
          :status="getProgressStatus(currentProgress.progress)"
          :stroke-width="24"
        />
        
        <!-- 简单信息 -->
        <div class="progress-info mt15">
          <p class="info-text">{{ currentProgress.message || '正在安装...' }}</p>
          <p class="info-percent">{{ currentProgress.progress }}%</p>
        </div>
      </div>

      <template #footer>
        <el-button @click="cancelInstallation" type="warning" plain v-if="currentProgress.progress < 100">
          取消安装
        </el-button>
        <el-button @click="closeProgressDialog" v-if="currentProgress.progress === 100" type="primary">
          完成
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="pluginMarketplaceIframe">
import { ref, computed, onMounted, onBeforeUnmount, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { Loading, Refresh, InfoFilled, Link } from '@element-plus/icons-vue'
import { getApiPluginScan, postApiPluginLoad, postApiPluginSetAuthCode } from '@/api/fd-system-api-admin/Plugin'
import { usePluginStore } from '@/stores/plugin'
import { baseSignalRManager } from '@/utils/signalr'
import * as signalR from '@microsoft/signalr'
import { Session } from '@/utils/storage'

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

// 插件安装进度状态
const progressDialogVisible = ref(false)
const currentProgress = ref<any>({
  pluginId: '',
  pluginName: '',
  stage: '',
  progress: 0,
  message: '',
  startTime: '',
  timestamp: ''
})
const progressHistory = ref<any[]>([])

// 防止重复提示的标志
const hasShownFailedMessage = ref(false)
const hasShownSuccessMessage = ref(false)

// SignalR 监听器函数引用（用于防止重复注册）
let pluginMessageHandler: ((message: any) => void) | null = null

// iframe 源地址 - 指向我们新创建的插件管理页面
// const iframeSrc = ref('https://fastdotnet.top/plugin-manager/embedded?layout=none') // 指向 Nuxt 项目的插件管理页面
const iframeSrc = ref('http://localhost:3000/plugin-manager/embedded?layout=none') // 指向 Nuxt 项目的插件管理页面

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
  // console.error('插件商城 iframe 加载失败:', event)
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
    // console.error('重试加载失败:', error)
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
    // console.warn('无法获取iframe导航状态:', e)
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
    // console.error('发送初始数据到iframe失败:', e)
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
    // console.error('处理 iframe 消息失败:', e)
  }
}

// 处理安装插件请求
const handleInstallPlugin = async (data: any) => {
  const { pluginId, pluginName, token, Version } = data
  
  // 初始化 SignalR 监听器以接收进度通知
  await initializeSignalRListener()
  
  // 初始化进度状态（设置正确的 pluginId）
  currentProgress.value = {
    pluginId,
    pluginName,
    stage: '',
    progress: 0,
    message: '准备开始安装...',
    startTime: new Date().toISOString(),
    timestamp: new Date().toISOString()
  }
  progressDialogVisible.value = true
  
  try {
    // 调用后端 API，立即返回（后台执行）
    const result = await postApiPluginLoad(data)
    
    // 记录安装状态到 localStorage
    localStorage.setItem('plugin_installing', JSON.stringify({
      pluginId,
      pluginName,
      startTime: new Date().toISOString(),
      status: 'installing'
    }))
    
    // 显示提示信息
    // ElMessage.info(result.Msg || '插件安装任务已启动，请等待进度更新...')
    
    // 注意：不在此处发送结果，等待 SignalR 推送最终结果
  } catch (error: any) {
    // console.error('启动安装任务失败:', error)
    ElMessage.error('启动安装任务失败：' + (error.message || '未知错误'))
    // 清除安装状态
    localStorage.removeItem('plugin_installing')
    closeProgressDialog()
  }
}

// 初始化 SignalR 监听器
const initializeSignalRListener = async () => {
  // 检查当前连接状态
  let state = baseSignalRManager.getConnectionState()
  // console.log('SignalR 初始连接状态:', state)
  
  // 如果未连接，尝试重新连接
  if (state !== signalR.HubConnectionState.Connected) {
    // console.warn('SignalR 未连接，尝试重新连接...')
    try {
      await baseSignalRManager.initialize()
      state = baseSignalRManager.getConnectionState()
      // console.log('SignalR 重新连接成功，当前状态:', state)
    } catch (error) {
      // console.error('SignalR 重新连接失败:', error)
      ElMessage.warning('无法建立实时连接，将不会显示安装进度')
      return
    }
  }
  
  // 确保连接成功后再注册监听器
  if (state === signalR.HubConnectionState.Connected) {
    // 如果已有监听器，先移除旧的（防止重复注册）
    if (pluginMessageHandler) {
      baseSignalRManager.off('pluginMessage', pluginMessageHandler)
      // console.log('✅ 已移除旧的 SignalR 监听器')
    }
    
    // 创建新的监听器函数
    pluginMessageHandler = (message: any) => {
      // console.log('✅ 父窗口收到 pluginMessage:', message)
      // console.log('  - MessageType:', message.MessageType)
      // console.log('  - Data:', message.data)
      
      // 处理进度更新消息
      if (message.messageType === 'progress_update') {
        // console.log('📊 收到进度更新，显示进度对话框')
        handleProgressUpdate(message)
      }
      
      // 处理安装完成消息
      if (message.messageType === 'install_completed') {
        // console.log('✅ 收到安装完成消息')
        handleInstallCompleted(message)
      }
      
      // 处理安装失败消息
      if (message.messageType === 'install_failed') {
        // console.log('❌ 收到安装失败消息')
        handleInstallFailed(message)
      }
    }
    
    // 注册新的监听器
    baseSignalRManager.on('pluginMessage', pluginMessageHandler)
    // console.log('✅ SignalR 监听器已注册')
  } else {
    // console.error('SignalR 连接状态异常:', state)
  }
}

// 处理进度更新
const handleProgressUpdate = (message: any) => {
  const data = message.data || {}
  
  // 如果是第一条消息，记录开始时间
  if (!currentProgress.value.startTime) {
    currentProgress.value.startTime = message.timestamp || new Date().toISOString()
  }
  
  // 更新当前进度（保留原有的 pluginId，因为 message.pluginId 是发送者 "System"）
  currentProgress.value = {
    pluginId: currentProgress.value.pluginId || data.pluginId || '',  // 优先使用已有的 pluginId
    pluginName: data.pluginName || message.pluginName || '',
    stage: data.stage || '',
    progress: data.percentage || data.current || 0,
    message: data.description || data.message || '',
    startTime: currentProgress.value.startTime,
    timestamp: message.timestamp || new Date().toISOString()
  }
  
  // 添加到历史记录
  progressHistory.value.unshift({
    ...currentProgress.value,
    timestamp: message.timestamp || new Date().toISOString()
  })
  
  // 显示进度对话框
  progressDialogVisible.value = true
  
  // 注意：不在这里显示成功消息，统一由 handleInstallCompleted 处理
  // 避免重复提示（进度100%和install_completed都会触发）
}

// 关闭进度对话框
const closeProgressDialog = () => {
  progressDialogVisible.value = false
  // 重置状态
  currentProgress.value = {
    pluginId: '',
    pluginName: '',
    stage: '',
    progress: 0,
    message: '',
    startTime: '',
    timestamp: ''
  }
  progressHistory.value = []
}

// 取消安装
const cancelInstallation = async () => {
  try {
    // console.log('[取消安装] currentProgress.value:', currentProgress.value)
    // console.log('[取消安装] pluginId:', currentProgress.value.pluginId)
    
    const result = await baseSignalRManager.invokeHubMethod(
      'UniversalHub',
      'InvokePluginMethod',
      'System',
      'CancelPluginInstallation',
      [currentProgress.value.pluginId]
    )
    
    // console.log('[取消安装] 返回结果:', result)
    
    if (result?.success) {
      ElMessage.success(result.message || '已请求取消安装')
    localStorage.removeItem('plugin_installing')

      // 关闭进度对话框
      closeProgressDialog()
    } else {
      ElMessage.error(result?.message || '取消失败')
    }
  } catch (error) {
    // console.error('取消安装失败:', error)
    ElMessage.error('取消安装失败')
  }
}

// 强制关闭安装
const forceCloseInstall = () => {
  ElMessage.warning('已强制关闭安装进度窗口，但后台安装任务可能仍在运行')
  
  // 清除 localStorage 中的安装状态
  localStorage.removeItem('plugin_installing')
  
  // 关闭对话框
  closeProgressDialog()
}

// 处理安装完成消息
const handleInstallCompleted = (message: any) => {
  const data = message.data || {}
  
  if (data.success) {
    // 防止重复提示
    if (!hasShownSuccessMessage.value) {
      hasShownSuccessMessage.value = true
      ElMessage.success(data.message || '插件安装成功！')
    }
    
    // 更新进度为100%
    currentProgress.value.progress = 100
    currentProgress.value.message = data.message || '安装完成'
    progressDialogVisible.value = true
    
    // 清除安装状态
    localStorage.removeItem('plugin_installing')
    
    // 通知 iframe
    if (marketplaceIframe.value?.contentWindow) {
      marketplaceIframe.value.contentWindow.postMessage({
        type: 'INSTALL_PLUGIN_RESULT',
        data: {
          success: true,
          pluginId: data.pluginId
        }
      }, '*')
    }
    
    // 重置标志（延迟2秒后允许下次提示）
    setTimeout(() => {
      hasShownSuccessMessage.value = false
    }, 2000)
  } else {
    // 安装失败时不显示提示，由 handleInstallFailed 统一处理
    progressDialogVisible.value = false
    localStorage.removeItem('plugin_installing')
  }
}

// 处理安装失败消息
const handleInstallFailed = (message: any) => {
  const data = message.data || {}
  
  // 防止重复提示
  if (!hasShownFailedMessage.value) {
    hasShownFailedMessage.value = true
    ElMessage.error(data.error || '插件安装失败')
  }
  
  progressDialogVisible.value = false
  
  // 清除安装状态
  localStorage.removeItem('plugin_installing')
  
  // 通知 iframe
  if (marketplaceIframe.value?.contentWindow) {
    marketplaceIframe.value.contentWindow.postMessage({
      type: 'INSTALL_PLUGIN_RESULT',
      data: {
        success: false,
        pluginId: data.pluginId,
        error: data.error
      }
    }, '*')
  }
  
  // 重置标志（延迟2秒后允许下次提示）
  setTimeout(() => {
    hasShownFailedMessage.value = false
  }, 2000)
}

// 辅助函数：获取进度条状态
const getProgressStatus = (progress: number) => {
  if (progress === 100) return 'success'
  if (progress < 30) return 'exception'
  if (progress < 70) return 'warning'
  return undefined
}

// 辅助函数：获取阶段标签类型
const getStageTagType = (stage: string) => {
  const stageMap: Record<string, string> = {
    'downloading': 'primary',
    'verifying': 'warning',
    'extracting': 'info',
    'installing': 'success',
    'completed': 'success',
    'failed': 'danger'
  }
  return stageMap[stage] || 'info'
}

// 辅助函数：获取阶段文本
const getStageText = (stage: string) => {
  const stageMap: Record<string, string> = {
    'downloading': '下载中',
    'verifying': '验证中',
    'extracting': '解压中',
    'installing': '安装中',
    'completed': '已完成',
    'failed': '失败'
  }
  return stageMap[stage] || stage
}

// 辅助函数：格式化时间
const formatTime = (timeStr: string) => {
  if (!timeStr) return '-'
  try {
    const date = new Date(timeStr)
    return date.toLocaleTimeString('zh-CN', { 
      hour: '2-digit', 
      minute: '2-digit', 
      second: '2-digit' 
    })
  } catch {
    return timeStr
  }
}

// 辅助函数：获取时间轴项类型
const getTimelineItemType = (progress: number) => {
  if (progress === 100) return 'success'
  if (progress < 50) return 'primary'
  return 'warning'
}

// 组件卸载时清理 SignalR 监听器
onBeforeUnmount(() => {
  // 移除 pluginMessage 监听器
  baseSignalRManager.off('pluginMessage', () => {})
})

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
    // console.error('处理登录成功消息失败:', error)
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
    // console.error('获取已安装插件列表失败:', error)
  })
}

// 组件挂载时添加事件监听
onMounted(async () => {
  window.addEventListener('message', handleMessage)
  loadStartTime.value = Date.now()

  // 设置加载超时检测（10 秒）
  loadTimeoutTimer = setTimeout(() => {
    if (loading.value) {
      handleLoadFailure('加载超时（超过 10 秒）')
    }
  }, 10000)
  
  // 检查是否有正在进行的安装任务
  await checkAndRestoreInstallingState()
})

// 检查并恢复安装状态
const checkAndRestoreInstallingState = async () => {
  try {
    const installingData = localStorage.getItem('plugin_installing')
    if (installingData) {
      const installInfo = JSON.parse(installingData)
      // console.log('检测到正在进行的安装任务:', installInfo)
      
      // 先初始化 SignalR 监听器并等待完成
      await initializeSignalRListener()
      
      // 通过 SignalR 查询任务是否还在运行
      let isStillInstalling = false
      try {
        isStillInstalling = await baseSignalRManager.invokeHubMethod(
          'UniversalHub',
          'InvokePluginMethod',
          'System',  // 插件ID
          'CheckPluginInstalling',  // 方法名
          [installInfo.pluginId]  // 参数数组
        )
        // console.log('任务状态检查结果:', isStillInstalling)
      } catch (error) {
        // console.error('查询任务状态失败:', error)
      }
      
      if (isStillInstalling) {
        // console.log('[恢复安装状态] 任务还在运行，准备显示弹窗')
        // console.log('[恢复安装状态] installInfo:', installInfo)
        
        // 任务还在运行，显示进度对话框
        currentProgress.value = {
          pluginId: installInfo.pluginId || '',
          pluginName: installInfo.pluginName || '',
          stage: '',
          progress: 0,
          message: '页面刷新，等待安装进度更新...',
          startTime: installInfo.startTime || new Date().toISOString(),
          timestamp: new Date().toISOString()
        }
        
        // console.log('[恢复安装状态] currentProgress:', currentProgress.value)
        
        // 使用 nextTick 确保 DOM 更新
        await nextTick()
        progressDialogVisible.value = true
        // console.log('[恢复安装状态] progressDialogVisible 设置为:', progressDialogVisible.value)
        
        ElMessage.info(`检测到插件 ${installInfo.pluginName} 正在安装中...`)
      } else {
        // 任务已结束，清除本地状态
        // console.log('任务已结束，清除本地状态')
        localStorage.removeItem('plugin_installing')
        ElMessage.info('安装任务已结束')
      }
    } else {
      // console.log('未检测到进行中的安装任务')
    }
  } catch (error) {
    // console.error('恢复安装状态失败:', error)
    localStorage.removeItem('plugin_installing')
  }
}

// 组件卸载时移除事件监听和定时器
onBeforeUnmount(() => {
  window.removeEventListener('message', handleMessage)
  if (loadTimeoutTimer) clearTimeout(loadTimeoutTimer)
})

// 暴露方法给父组件
defineExpose({
  sendInstalledPlugins
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

// 简洁进度对话框样式
.simple-progress {
  padding: 20px 10px;
  
  .progress-info {
    text-align: center;
    
    .info-text {
      font-size: 14px;
      color: #606266;
      margin: 0 0 8px 0;
    }
    
    .info-percent {
      font-size: 24px;
      font-weight: bold;
      color: #409eff;
      margin: 0;
    }
  }
}
</style>