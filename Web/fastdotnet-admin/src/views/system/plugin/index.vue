<template>
  <div class="system-plugin-container layout-pd">
    <el-card shadow="hover" header="插件管理">
      <div class="server-auth-section">
        <span class="auth-label">用户授权码：</span>
        <el-input 
          v-model="serverAuthCode" 
          placeholder="暂无授权码"
          style="max-width: 400px; margin-right: 10px"
          disabled
        />
        <el-tooltip content="设置授权码" placement="bottom">
          <el-button size="small" type="primary" @click="handleSetAuthCodeDialog" :loading="settingAuthCode">
            <el-icon><ele-Edit /></el-icon>
          </el-button>
        </el-tooltip>
        <span class="code-label ml20">机器码：</span>
        <el-input 
          v-model="machineCode" 
          placeholder="加载中..."
          style="max-width: 400px; margin-right: 10px"
          disabled
        />
        <el-tooltip content="复制机器码" placement="bottom">
          <el-button size="small" type="primary" @click="handleCopyMachineCode">
            <el-icon><CopyDocument /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
      <el-tabs v-model="activeTab" class="system-plugin-tabs" @tab-change="handleTabChange">
        <el-tab-pane label="已安装插件" name="installed">
          <div class="system-plugin-search mb15">
            <el-input size="default" placeholder="请输入插件名称" style="max-width: 180px" v-model="searchName"> </el-input>
            <el-button size="default" type="primary" class="ml10" @click="handleSearch">
              <el-icon>
                <ele-Search />
              </el-icon>
              查询
            </el-button>
            <el-button size="default" type="success" class="ml10" @click="handleScan">
              <el-icon>
                <ele-Refresh />
              </el-icon>
              扫描插件
            </el-button>
            <el-button size="default" type="success" class="ml10" @click="handleBatchLicense">
              <el-icon>
                <ele-Key />
              </el-icon>
              批量授权
            </el-button>
          </div>
          <el-table ref="tableRef" :data="pluginList" style="width: 100%">
            <el-table-column type="selection" width="55" />
            <el-table-column prop="id" label="插件 ID" show-overflow-tooltip></el-table-column>
            <el-table-column prop="name" label="插件名称" show-overflow-tooltip></el-table-column>
            <el-table-column prop="version" label="版本" show-overflow-tooltip></el-table-column>
            <el-table-column prop="description" label="描述" show-overflow-tooltip></el-table-column>
            <el-table-column prop="enabled" label="是否启用" show-overflow-tooltip>
              <template #default="scope">
                <el-tag type="success" v-if="scope.row.enabled">已启用</el-tag>
                <el-tag type="info" v-else>未启用</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" >
              <template #default="scope">
                <el-button size="small" type="primary" @click="handleEnable(scope.row)"
                  v-if="!scope.row.enabled">启用</el-button>
                <el-button size="small" type="danger" @click="handleDisable(scope.row)" v-else>停用</el-button>
                <el-button size="small" type="info" @click="handleConfig(scope.row)">配置</el-button>
                <el-button size="small" type="success" @click="handlePluginLicense(scope.row)">授权</el-button>
                <el-button size="small" type="warning" @click="handleUninstall(scope.row)">卸载</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- <el-tab-pane label="插件市场(qiankun)" name="marketplace">
          <plugin-marketplace ref="marketplaceRef" />
        </el-tab-pane> -->

        <el-tab-pane label="插件市场" name="marketplace-iframe">
          <plugin-marketplace-iframe ref="marketplaceIframeRef" @plugin-action="handlePluginAction" />
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <!-- 插件配置对话框 -->
    <PluginConfigurationDialog v-model="configDialogVisible" :plugin-id="currentConfigPluginId"
      :plugin-name="currentConfigPluginName" @save-success="handleConfigSaveSuccess" />
    
    <!-- 插件授权对话框 -->
    <PluginLicenseDialog 
      v-model="licenseDialogVisible" 
      :plugin-id="currentLicensePluginId"
      :plugin-name="currentLicensePluginName" 
      :is-batch="isBatchLicense"
      :selected-plugins="selectedPlugins"
      @save-success="handleLicenseSaveSuccess" 
    />
    
    <!-- 设置服务器用户授权码对话框 -->
    <el-dialog 
      v-model="setAuthDialogVisible" 
      title="设置服务器用户授权码" 
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form label-position="top">
        <el-alert 
          title="授权说明" 
          type="info" 
          description="请输入服务器用户授权码，确保授权码正确有效后点击保存" 
          show-icon 
          :closable="false"
          class="mb15"
        />
        
        <el-form-item label="服务器用户授权码" required>
          <el-input
            v-model="tempAuthCode"
            type="textarea"
            :rows="6"
            placeholder="请输入服务器用户授权码"
            autocomplete="off"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="setAuthDialogVisible = false">取 消</el-button>
          <el-button type="primary" @click="handleSetAuthCode" :loading="settingAuthCode">
            确 定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="systemPlugin">
import { ref, onMounted, defineAsyncComponent, onUnmounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
import { CopyDocument } from '@element-plus/icons-vue'
// 导入插件相关的 API
import {
  getApiPluginScan,
  postApiPluginEnablePluginId,
  postApiPluginDisablePluginId,
  postApiPluginUninstallPluginId,
  getApiPluginGetAuthCode,
  postApiPluginSetAuthCode
} from '@/api/fd-system-api-admin/Plugin'
import { getApiSystemMachineFingerprint } from '@/api/fd-system-api-admin/System'
import { MicroAppEvents, receiveFromMicroApp, removeMicroAppEventListener } from '@/utils/microAppCommunication'
import PluginConfigurationDialog from './PluginConfigurationDialog.vue'
import PluginLicenseDialog from './PluginLicenseDialog.vue'
import { usePluginStore } from '@/stores/plugin'

// 使用插件 store
const pluginStore = usePluginStore()

// 异步加载插件市场组件
// const PluginMarketplace = defineAsyncComponent(() => import('./marketplace.vue'))
const PluginMarketplaceIframe = defineAsyncComponent(() => import('./marketplace-iframe.vue'))

// 定义插件数据类型
interface Plugin {
  id: string
  name: string
  version: string
  description: string
  enabled: boolean
  author: string
  entryPoint: string
}

// refs
const marketplaceRef = ref<any>(null)
const marketplaceIframeRef = ref<any>(null)
const tableRef = ref<any>(null)

// 配置对话框
const configDialogVisible = ref(false)
const currentConfigPluginId = ref('')
const currentConfigPluginName = ref('')

// 授权对话框
const licenseDialogVisible = ref(false)
const currentLicensePluginId = ref('')
const currentLicensePluginName = ref('')
const isBatchLicense = ref(false)
const selectedPlugins = ref<Plugin[]>([])

// 当前激活的选项卡
const activeTab = ref('installed')

// 服务器用户授权码
const serverAuthCode = ref('')
const settingAuthCode = ref(false)
const setAuthDialogVisible = ref(false)
const tempAuthCode = ref('') // 临时存储输入的授权码

// 机器码
const machineCode = ref('')

// 插件列表
const pluginList = ref<Plugin[]>([])
// 搜索名称
const searchName = ref('')

// 获取插件列表
const getPluginList = () => {
  // 扫描插件
  getApiPluginScan().then((res: any) => {
    pluginList.value = res

    // 如果在插件市场页面，通知它更新插件列表
    if (activeTab.value === 'marketplace' && marketplaceRef.value) {
      // 可以通过适当的方式通知插件市场组件更新数据
    }
  })
}

// 搜索插件
const handleSearch = () => {
  if (searchName.value) {
    pluginList.value = pluginList.value.filter(
      (plugin) => plugin.name.toLowerCase().includes(searchName.value.toLowerCase())
    )
  } else {
    getPluginList()
  }
}

// 扫描插件
const handleScan = () => {
  getPluginList()
}

// 打开设置授权码对话框
const handleSetAuthCodeDialog = () => {
  tempAuthCode.value = serverAuthCode.value // 回显当前值
  setAuthDialogVisible.value = true
}

// 设置服务器用户授权码
const handleSetAuthCode = async () => {
  if (!tempAuthCode.value.trim()) {
    ElMessage.warning('请输入服务器用户授权码')
    return
  }
  
  try {
    settingAuthCode.value = true
    await postApiPluginSetAuthCode({ AuthCode: tempAuthCode.value })
    ElMessage.success('设置授权码成功')
    setAuthDialogVisible.value = false
    // 更新显示的授权码
    serverAuthCode.value = tempAuthCode.value
  } catch (error: any) {
    ElMessage.error('设置授权码失败：' + (error.message || '未知错误'))
  } finally {
    settingAuthCode.value = false
  }
}

// 启用插件
const handleEnable = (row: Plugin) => {
  ElMessageBox.confirm(`确定要启用插件 ${row.name} 吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    postApiPluginEnablePluginId({ pluginId: row.id }).then((res) => {
      if (res.Code == 200) {
        ElMessage.success('启用成功')
        getPluginList() // 重新获取插件列表
        // 通知插件市场插件状态变更
        window.dispatchEvent(new CustomEvent('micro-app-event', {
          detail: {
            type: MicroAppEvents.UPDATE_INSTALLED_PLUGINS,
            data: { action: 'enable', pluginId: row.id }
          }
        }))
      } else {
        ElMessage.error(res.Msg)
      }
    })
  })
}

// 停用插件
const handleDisable = (row: Plugin) => {
  ElMessageBox.confirm(`确定要停用插件 ${row.name} 吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    postApiPluginDisablePluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('停用成功')
      getPluginList() // 重新获取插件列表

      // 通知插件市场插件状态变更
      window.dispatchEvent(new CustomEvent('micro-app-event', {
        detail: {
          type: MicroAppEvents.UPDATE_INSTALLED_PLUGINS,
          data: { action: 'disable', pluginId: row.id }
        }
      }))
    })
  })
}

// 卸载插件
const handleUninstall = (row: Plugin) => {
  ElMessageBox.confirm(`确定要卸载插件 ${row.name} 吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    postApiPluginUninstallPluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('卸载成功')
      getPluginList() // 重新获取插件列表

      // 通知插件市场插件状态变更
      window.dispatchEvent(new CustomEvent('micro-app-event', {
        detail: {
          type: MicroAppEvents.UPDATE_INSTALLED_PLUGINS,
          data: { action: 'uninstall', pluginId: row.id }
        }
      }))
    })
  })
}

// 配置插件
const handleConfig = (row: Plugin) => {
  currentConfigPluginId.value = row.id
  currentConfigPluginName.value = row.name
  configDialogVisible.value = true
}

// 授权插件（单个）
const handlePluginLicense = (row: Plugin) => {
  currentLicensePluginId.value = row.id
  currentLicensePluginName.value = row.name
  isBatchLicense.value = false
  selectedPlugins.value = []
  licenseDialogVisible.value = true
}

// 批量授权
const handleBatchLicense = () => {
  // 这里可以获取表格选中的行
  // 由于使用了 selection 列，需要通过 ref 获取选中的行
  const selection = tableRef.value?.getSelectionRows() || []
  if (selection.length === 0) {
    ElMessage.warning('请先选择要授权的插件')
    return
  }
  selectedPlugins.value = selection
  isBatchLicense.value = true
  currentLicensePluginId.value = ''
  currentLicensePluginName.value = `已选择 ${selection.length} 个插件`
  licenseDialogVisible.value = true
}

// 配置保存成功
const handleConfigSaveSuccess = () => {
  // 可以在这里做一些后续操作，比如刷新列表等
  console.log('配置保存成功')
}

// 授权保存成功
const handleLicenseSaveSuccess = () => {
  // 可以在这里做一些后续操作，比如刷新列表等
  console.log('授权保存成功')
}

// 处理选项卡切换
const handleTabChange = (tabName: string) => {
  if (tabName === 'marketplace') {
    // 切换到插件市场时，可以做一些初始化工作
  } else if (tabName === 'installed') {
    // 切换回已安装插件时，刷新列表
    getPluginList()
  }
}

// 监听来自插件市场的事件
const listenToMicroAppEvents = () => {
  // 监听插件市场请求刷新插件列表
  receiveFromMicroApp(MicroAppEvents.REFRESH_PLUGIN_LIST_REQUEST, () => {
    getPluginList()
  })

  // 监听安装插件请求
  receiveFromMicroApp(MicroAppEvents.INSTALL_PLUGIN_REQUEST, (data) => {
    // 处理安装插件请求
    console.log('收到安装插件请求:', data)
    // 这里应该调用实际的安装插件API
  })

  // 监听卸载插件请求
  receiveFromMicroApp(MicroAppEvents.UNINSTALL_PLUGIN_REQUEST, (data) => {
    // 处理卸载插件请求
    console.log('收到卸载插件请求:', data)
  })

  // 监听启用插件请求
  receiveFromMicroApp(MicroAppEvents.ENABLE_PLUGIN_REQUEST, (data) => {
    // 处理启用插件请求
    console.log('收到启用插件请求:', data)
  })

  // 监听停用插件请求
  receiveFromMicroApp(MicroAppEvents.DISABLE_PLUGIN_REQUEST, (data) => {
    // 处理停用插件请求
    console.log('收到停用插件请求:', data)
  })
}

// 处理来自iframe插件市场的插件操作请求
const handlePluginAction = (event: CustomEvent) => {
  const { action, pluginId } = event.detail

  switch (action) {
    case 'install':
      ElMessage.info(`iframe插件市场请求安装插件: ${pluginId}`)
      // 在这里实现实际的安装逻辑
      break

    case 'uninstall':
      ElMessage.info(`iframe插件市场请求卸载插件: ${pluginId}`)
      // 在这里实现实际的卸载逻辑
      break

    case 'enable':
      ElMessage.info(`iframe插件市场请求启用插件: ${pluginId}`)
      // 在这里实现实际的启用逻辑
      break

    case 'disable':
      ElMessage.info(`iframe插件市场请求停用插件: ${pluginId}`)
      // 在这里实现实际的停用逻辑
      break

    default:
      console.warn('未知插件操作:', action)
  }
}

// 页面加载时获取插件列表
onMounted(() => {
  getPluginList()
  listenToMicroAppEvents()
  
  // 监听自定义事件
  window.addEventListener('refresh-plugin-list', getPluginList)
  
  // 获取服务器用户授权码
  fetchServerAuthCode()
  
  // 获取机器码
  fetchMachineCode()
  
  // 恢复插件商城的授权信息（如果之前有保存）
  pluginStore.restoreMarketplaceAuth()
  
  // 监听授权信息变化
  // console.log('插件商城 Token:', pluginStore.marketplaceToken)
  // console.log('插件商城授权码:', pluginStore.marketplaceAuthCode)
})

// 获取服务器用户授权码
const fetchServerAuthCode = async () => {
  try {
    const res = await getApiPluginGetAuthCode()
    serverAuthCode.value = typeof res === 'string' ? res : ''
  } catch (error: any) {
    console.error('获取授权码失败:', error)
  }
}

// 使用插件商城 Token 的示例方法
// const useMarketplaceToken = () => {
//   // 从 store 中获取 token
//   const token = pluginStore.marketplaceToken
  
//   if (!token) {
//     ElMessage.warning('请先在插件商城登录')
//     return
//   }
  
//   // 这里可以使用 token 调用需要认证的 API
//   console.log('使用插件商城 Token:', token)
  
//   // 示例：调用需要 token 的接口
//   // await someApiThatNeedsToken({ headers: { Authorization: `Bearer ${token}` } })
  
//   ElMessage.success('Token 获取成功')
// }

// 获取机器码
const fetchMachineCode = async () => {
  try {
    const res = await getApiSystemMachineFingerprint()
    machineCode.value = typeof res === 'string' ? res : ''
  } catch (error: any) {
    console.error('获取机器码失败:', error)
  }
}

// 复制机器码
const handleCopyMachineCode = () => {
  if (!machineCode.value) {
    ElMessage.warning('机器码为空')
    return
  }
  
  // 优先使用 Clipboard API，如果不可用则降级使用传统方法
  if (navigator.clipboard && navigator.clipboard.writeText) {
    // 使用现代 Clipboard API
    navigator.clipboard.writeText(machineCode.value).then(() => {
      ElMessage.success('机器码已复制到剪贴板')
    }).catch((err: any) => {
      console.error('复制失败:', err)
      ElMessage.error('复制失败，请手动复制')
    })
  } else {
    // 降级方案：使用 execCommand 方法
    const textArea = document.createElement('textarea')
    textArea.value = machineCode.value
    textArea.style.position = 'fixed'
    textArea.style.left = '-999999px'
    textArea.style.top = '-999999px'
    document.body.appendChild(textArea)
    textArea.focus()
    textArea.select()
    
    try {
      const successful = document.execCommand('copy')
      if (successful) {
        ElMessage.success('机器码已复制到剪贴板')
      } else {
        ElMessage.error('复制失败，请手动复制')
      }
    } catch (err: any) {
      console.error('复制失败:', err)
      ElMessage.error('复制失败，请手动复制')
    }
    
    document.body.removeChild(textArea)
  }
}

// 组件卸载时清理事件监听
onUnmounted(() => {
  window.removeEventListener('refresh-plugin-list', getPluginList)

  // 移除微应用事件监听
  // removeMicroAppEventListener(MicroAppEvents.REFRESH_PLUGIN_LIST_REQUEST, getPluginList)
})
</script>

<style scoped lang="scss">
.system-plugin-container {
  .server-auth-section {
    display: flex;
    align-items: center;
    margin-bottom: 20px;
    padding: 15px;
    background-color: #f5f7fa;
    border-radius: 4px;
    flex-wrap: wrap;
    
    .auth-label {
      font-weight: 600;
      color: #303133;
      margin-right: 12px;
      white-space: nowrap;
    }
    
    .code-label {
      font-weight: 600;
      color: #303133;
      margin-right: 12px;
      white-space: nowrap;
    }
  }
  
  :deep(.el-card__body) {
    display: flex;
    flex-direction: column;
    height: calc(100vh - 200px);
  }

  :deep(.system-plugin-tabs) {
    flex: 1;
    display: flex;
    flex-direction: column;

    .el-tabs__content {
      flex: 1;
      overflow: auto;
      height: 100%;
    }

    .el-tab-pane {
      height: 100%;
      min-height: 400px;
    }
  }
}
</style>