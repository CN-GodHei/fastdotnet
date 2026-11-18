<template>
  <div class="__PLUGIN_SLUG__-home">
    <el-card>
      <template #header>
        <span>__DISPLAY_NAME__</span>
      </template>
      <p>插件前端已成功挂载。</p>
      <el-button type="primary" @click="ping">调用后端 ping</el-button>
      <p v-if="result">{{ result }}</p>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getSharedFdRequest } from '../main'

const result = ref('')

// 路由被宿主重写为 /api/plugins/{admin|app|shared}/p{PluginId}/...
// 这里演示用宿主注入的 FdRequest 访问插件自己的接口。
const ping = async () => {
  const request = getSharedFdRequest()
  if (!request) {
    ElMessage.warning('未获取到宿主 FdRequest（独立运行模式）')
    return
  }
  try {
    const data = await request({ url: '/api/plugins/admin/p__PLUGIN_ID__/MyThing/ping', method: 'GET' })
    result.value = JSON.stringify(data)
  } catch (e: any) {
    ElMessage.error(e?.message ?? '请求失败')
  }
}
</script>

<style scoped>
.__PLUGIN_SLUG__-home {
  padding: 16px;
}
</style>
