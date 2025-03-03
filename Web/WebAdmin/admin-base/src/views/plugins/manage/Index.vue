<template>
  <div class="plugin-manage">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>插件管理</span>
          <el-button type="primary" @click="handleInstall">安装插件</el-button>
        </div>
      </template>
      <el-table :data="plugins" style="width: 100%">
        <el-table-column prop="name" label="插件名称" />
        <el-table-column prop="version" label="版本" width="100" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'running' ? 'success' : 'info'">
              {{ row.status === 'running' ? '运行中' : '已停止' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button
              v-if="row.status === 'stopped'"
              type="success"
              size="small"
              @click="handleStart(row)"
            >
              启动
            </el-button>
            <el-button
              v-else
              type="warning"
              size="small"
              @click="handleStop(row)"
            >
              停止
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleUninstall(row)"
            >
              卸载
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 安装插件对话框 -->
    <el-dialog
      v-model="installDialogVisible"
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
          <el-button @click="installDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="confirmInstall">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getPlugins, installPlugin, startPlugin, stopPlugin, uninstallPlugin } from '@/api/plugin'

// 插件列表数据
const plugins = ref([])

// 获取插件列表
const fetchPlugins = async () => {
  try {
    const res = await getPlugins()
    plugins.value = res.data
  } catch (error) {
    console.error('获取插件列表失败:', error)
    ElMessage.error('获取插件列表失败')
  }
}

// 安装插件相关
const installDialogVisible = ref(false)
const installForm = ref({
  url: ''
})

const handleInstall = () => {
  installDialogVisible.value = true
}

const confirmInstall = async () => {
  if (!installForm.value.url) {
    ElMessage.warning('请输入插件下载地址')
    return
  }

  try {
    await installPlugin(installForm.value.url)
    ElMessage.success('插件安装成功')
    installDialogVisible.value = false
    installForm.value.url = ''
    fetchPlugins()
  } catch (error) {
    console.error('安装插件失败:', error)
    ElMessage.error('安装插件失败')
  }
}

// 启动插件
const handleStart = async (plugin) => {
  try {
    await startPlugin(plugin.name)
    ElMessage.success('插件启动成功')
    fetchPlugins()
  } catch (error) {
    console.error('启动插件失败:', error)
    ElMessage.error('启动插件失败')
  }
}

// 停止插件
const handleStop = async (plugin) => {
  try {
    await stopPlugin(plugin.name)
    ElMessage.success('插件停止成功')
    fetchPlugins()
  } catch (error) {
    console.error('停止插件失败:', error)
    ElMessage.error('停止插件失败')
  }
}

// 卸载插件
const handleUninstall = (plugin) => {
  ElMessageBox.confirm(
    '确定要卸载该插件吗？',
    '警告',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning',
    }
  )
    .then(async () => {
      try {
        await uninstallPlugin(plugin.name)
        ElMessage.success('插件卸载成功')
        fetchPlugins()
      } catch (error) {
        console.error('卸载插件失败:', error)
        ElMessage.error('卸载插件失败')
      }
    })
    .catch(() => {
      // 取消卸载
    })
}

// 初始化
fetchPlugins()
</script>

<style scoped>
.plugin-manage {
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