<template>
  <div class="plugin-a-container">
    <h1>PluginA 测试页面</h1>
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>表单测试</span>
        </div>
      </template>
      <el-form :model="form" label-width="120px">
        <el-form-item label="名称">
          <el-input v-model="form.name" placeholder="请输入名称"/>
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="form.type" placeholder="请选择类型">
            <el-option label="类型A" value="A" />
            <el-option label="类型B" value="B" />
            <el-option label="类型C" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="是否启用">
          <el-switch v-model="form.enabled" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSubmit">提交</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="box-card" style="margin-top: 20px;">
      <template #header>
        <div class="card-header">
          <span>数据列表</span>
          <el-button type="primary" @click="refreshList">刷新</el-button>
        </div>
      </template>
      <el-table :data="tableData" style="width: 100%">
        <el-table-column prop="name" label="名称" />
        <el-table-column prop="type" label="类型" />
        <el-table-column prop="enabled" label="状态">
          <template #default="scope">
            <el-tag :type="scope.row.enabled ? 'success' : 'info'">
              {{ scope.row.enabled ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="scope">
            <el-button size="small" @click="handleEdit(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

const form = ref({
  name: '',
  type: '',
  enabled: false
})

const tableData = ref([
  { name: '测试数据1', type: 'A', enabled: true },
  { name: '测试数据2', type: 'B', enabled: false },
  { name: '测试数据3', type: 'C', enabled: true }
])

const handleSubmit = () => {
  ElMessage.success('提交成功')
}

const resetForm = () => {
  form.value = {
    name: '',
    type: '',
    enabled: false
  }
}

const refreshList = () => {
  ElMessage.success('数据已刷新')
}

const handleEdit = (row) => {
  ElMessage.info(`编辑: ${row.name}`)
}

const handleDelete = (row) => {
  ElMessage.warning(`删除: ${row.name}`)
}
</script>

<style scoped>
.plugin-a-container {
  padding: 20px;
}

.box-card {
  margin-top: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>