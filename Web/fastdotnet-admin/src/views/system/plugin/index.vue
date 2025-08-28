<template>
  <div class="system-plugin-container layout-padding">
    <el-card shadow="hover" header="插件管理">
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
            <el-button size="small" type="warning" @click="handleUninstall(scope.row)">卸载</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts" name="systemPlugin">
import { ref, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus'
// 导入插件相关的API
import {
  getPluginScan,
  postPluginEnablePluginId,
  postPluginDisablePluginId,
  postPluginUninstallPluginId
} from '/@/api/fd-system-api/Plugin'

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

// 插件列表
const pluginList = ref<Plugin[]>([])
// 搜索名称
const searchName = ref('')

// 获取插件列表
const getPluginList = () => {
  // 扫描插件
  getPluginScan().then((res: any) => {
    pluginList.value = res
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
    postPluginEnablePluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('启用成功')
      getPluginList() // 重新获取插件列表
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
    postPluginDisablePluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('停用成功')
      getPluginList() // 重新获取插件列表
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
    postPluginUninstallPluginId({ pluginId: row.id }).then(() => {
      ElMessage.success('卸载成功')
      getPluginList() // 重新获取插件列表
    })
  })
}

// 页面加载时获取插件列表
onMounted(() => {
  getPluginList()
})
</script>

<style scoped lang="scss">
.system-plugin-container {
  :deep(.el-card__body) {
    display: flex;
    flex-direction: column;
    height: calc(100vh - 200px);
  }
}
</style>