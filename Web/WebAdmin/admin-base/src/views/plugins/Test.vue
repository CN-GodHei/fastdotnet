<template>
  <div class="plugin-test">
    <h2>插件测试页面</h2>
    <el-card class="plugin-card">
      <template #header>
        <div class="card-header">
          <span>插件加载测试</span>
        </div>
      </template>
      <el-form :model="pluginForm" label-width="120px">
        <el-form-item label="插件名称">
          <el-input v-model="pluginForm.name" placeholder="请输入插件名称"></el-input>
        </el-form-item>
        <el-form-item label="插件入口地址">
          <el-input v-model="pluginForm.entry" placeholder="请输入插件入口地址"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadPlugin">加载插件</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const pluginForm = ref({
  name: '',
  entry: ''
})

const loadPlugin = () => {
  if (!pluginForm.value.name || !pluginForm.value.entry) {
    ElMessage.warning('请填写完整的插件信息')
    return
  }
  
  // 调用全局注册插件方法
  window.registerPlugin({
    name: pluginForm.value.name,
    entry: pluginForm.value.entry
  })
  
  ElMessage.success('插件加载成功')
}
</script>

<style scoped>
.plugin-test {
  padding: 20px;
}

.plugin-card {
  max-width: 800px;
  margin: 0 auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>