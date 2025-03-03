<template>
  <div class="plugin-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>插件管理</span>
          <el-button type="primary" @click="handleInstallPlugin">安装插件</el-button>
        </div>
      </template>
      
      <el-table :data="pluginList" style="width: 100%">
        <el-table-column prop="name" label="插件名称" />
        <el-table-column prop="version" label="版本" width="120" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="120">
          <template #default="{ row }">
            <el-tag :type="row.status === 'running' ? 'success' : 'info'">
              {{ row.status === 'running' ? '运行中' : '已停止' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button 
              v-if="row.status !== 'running'"
              type="primary"
              link
              @click="handleStartPlugin(row)"
            >
              启动
            </el-button>
            <el-button 
              v-else
              type="warning"
              link
              @click="handleStopPlugin(row)"
            >
              停止
            </el-button>
            <el-button 
              type="danger"
              link
              @click="handleUninstallPlugin(row)"
            >
              卸载
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 安装插件对话框 -->
    <el-dialog
      v-model="dialogVisible"
      title="安装插件"
      width="30%"
    >
      <el-form :model="installForm" label-width="80px">
        <el-form-item label="插件地址">
          <el-input v-model="installForm.url" placeholder="请输入插件下载地址" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="confirmInstall">确认</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

// 插件列表数据
const pluginList = ref([])

// 安装插件表单
const dialogVisible = ref(false)
const installForm = ref({
  url: ''
})

// 获取插件列表
const getPluginList = async () => {
  try {
    const response = await fetch('/api/plugins')
    const result = await response.json()
    if (result.code === 200) {
      pluginList.value = result.data
    } else {
      ElMessage.error(result.message)
    }
  } catch (error) {
    ElMessage.error('获取插件列表失败')
  }
}

// 安装插件
const handleInstallPlugin = () => {
  dialogVisible.value = true
}

// 确认安装
const confirmInstall = async () => {
  try {
    const response = await fetch('/api/plugins/install', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(installForm.value)
    })
    const result = await response.json()
    if (result.code === 200) {
      ElMessage.success('插件安装成功')
      getPluginList()
      dialogVisible.value = false
      installForm.value.url = ''
    } else {
      ElMessage.error(result.message)
    }
  } catch (error) {
    ElMessage.error('插件安装失败')
  }
}

// 启动插件
const handleStartPlugin = async (plugin) => {
  try {
    const response = await fetch(`/api/plugins/${plugin.name}/start`, {
      method: 'POST'
    })
    const result = await response.json()
    if (result.code === 200) {
      ElMessage.success('插件启动成功')
      getPluginList()
    } else {
      ElMessage.error(result.message)
    }
  } catch (error) {
    ElMessage.error('插件启动失败')
  }
}

// 停止插件
const handleStopPlugin = async (plugin) => {
  try {
    const response = await fetch(`/api/plugins/${plugin.name}/stop`, {
      method: 'POST'
    })
    const result = await response.json()
    if (result.code === 200) {
      ElMessage.success('插件停止成功')
      getPluginList()
    } else {
      ElMessage.error(result.message)
    }
  } catch (error) {
    ElMessage.error('插件停止失败')
  }
}

// 卸载插件
const handleUninstallPlugin = async (plugin) => {
  try {
    const response = await fetch(`/api/plugins/${plugin.name}/uninstall`, {
      method: 'DELETE'
    })
    const result = await response.json()
    if (result.code === 200) {
      ElMessage.success('插件卸载成功')
      getPluginList()
    } else {
      ElMessage.error(result.message)
    }
  } catch (error) {
    ElMessage.error('插件卸载失败')
  }
}

// 初始化加载插件列表
getPluginList()
</script>

<style scoped>
.plugin-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>