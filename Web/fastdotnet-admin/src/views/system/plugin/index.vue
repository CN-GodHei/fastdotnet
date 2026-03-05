<template>
  <div class="system-plugin-container layout-pd">
    <el-card shadow="hover" header="插件管理">
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
          </div>
          <el-table :data="pluginList" style="width: 100%">
            <el-table-column prop="id" label="插件ID" show-overflow-tooltip></el-table-column>
            <el-table-column prop="name" label="插件名称" show-overflow-tooltip></el-table-column>
            <el-table-column prop="version" label="版本" show-overflow-tooltip></el-table-column>
            <el-table-column prop="description" label="描述" show-overflow-tooltip></el-table-column>
            <el-table-column prop="enabled" label="是否启用" show-overflow-tooltip>
              <template #default="scope">
                <el-tag type="success" v-if="scope.row.enabled">已启用</el-tag>
                <el-tag type="info" v-else>未启用</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200">
              <template #default="scope">
                <el-button size="small" type="primary" @click="handleEnable(scope.row)" v-if="!scope.row.enabled"
                  >启用</el-button
                >
                <el-button size="small" type="danger" @click="handleDisable(scope.row)" v-else>停用</el-button>
                <el-button size="small" type="info" @click="handleConfig(scope.row)">配置</el-button>
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
    <PluginConfigurationDialog 
      v-model="configDialogVisible" 
      :plugin-id="currentConfigPluginId"
      @save-success="handleConfigSaveSuccess"
    />
  </div>
</template>

<script setup lang="ts" name="systemPlugin">
import { ref, onMounted, defineAsyncComponent, onUnmounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
// 导入插件相关的API
import {
  getApiPluginScan,
  postApiPluginEnablePluginId,
  postApiPluginDisablePluginId,
  postApiPluginUninstallPluginId
} from '@/api/fd-system-api-admin/Plugin'
import { MicroAppEvents, receiveFromMicroApp, removeMicroAppEventListener } from '@/utils/microAppCommunication'
import PluginConfigurationDialog from './PluginConfigurationDialog.vue'

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

// 配置对话框
const configDialogVisible = ref(false)
const currentConfigPluginId = ref('')

// 当前激活的选项卡
const activeTab = ref('installed')

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

// 启用插件
const handleEnable = (row: Plugin) => {
  ElMessageBox.confirm(`确定要启用插件 ${row.name} 吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    postApiPluginEnablePluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('启用成功')
      getPluginList() // 重新获取插件列表
      
      // 通知插件市场插件状态变更
      window.dispatchEvent(new CustomEvent('micro-app-event', {
        detail: {
          type: MicroAppEvents.UPDATE_INSTALLED_PLUGINS,
          data: { action: 'enable', pluginId: row.id }
        }
      }))
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
  configDialogVisible.value = true
}

// 配置保存成功
const handleConfigSaveSuccess = () => {
  // 可以在这里做一些后续操作，比如刷新列表等
  console.log('配置保存成功')
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
})

// 组件卸载时清理事件监听
onUnmounted(() => {
  window.removeEventListener('refresh-plugin-list', getPluginList)
  
  // 移除微应用事件监听
  // removeMicroAppEventListener(MicroAppEvents.REFRESH_PLUGIN_LIST_REQUEST, getPluginList)
})
</script>

<style scoped lang="scss">
.system-plugin-container {
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