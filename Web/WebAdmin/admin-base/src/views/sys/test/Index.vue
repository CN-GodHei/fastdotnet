<template>
  <div class="test-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>测试表单</span>
        </div>
      </template>
      <el-form :model="form" label-width="120px">
        <el-form-item label="名称">
          <el-input v-model="form.name" placeholder="请输入名称" />
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="form.type" placeholder="请选择类型">
            <el-option label="类型A" value="A" />
            <el-option label="类型B" value="B" />
            <el-option label="类型C" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="form.status" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="form.remark" type="textarea" rows="3" placeholder="请输入备注" />
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
          <span>测试列表</span>
          <el-button type="primary" @click="refreshList">刷新</el-button>
        </div>
      </template>
      <el-table :data="tableData" style="width: 100%">
        <el-table-column prop="name" label="名称" />
        <el-table-column prop="type" label="类型" />
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag :type="row.status ? 'success' : 'info'">
              {{ row.status ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="remark" label="备注" />
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

// 表单数据
const form = ref({
  name: '',
  type: '',
  status: false,
  remark: ''
})

// 表格数据
const tableData = ref([
  {
    name: '测试数据1',
    type: 'A',
    status: true,
    remark: '这是测试数据1的备注'
  },
  {
    name: '测试数据2',
    type: 'B',
    status: false,
    remark: '这是测试数据2的备注'
  }
])

// 提交表单
const handleSubmit = () => {
  ElMessage.success('提交成功')
  resetForm()
}

// 重置表单
const resetForm = () => {
  form.value = {
    name: '',
    type: '',
    status: false,
    remark: ''
  }
}

// 刷新列表
const refreshList = () => {
  ElMessage.success('数据已刷新')
}

// 编辑数据
const handleEdit = (row) => {
  form.value = { ...row }
  ElMessage.info(`正在编辑: ${row.name}`)
}

// 删除数据
const handleDelete = (row) => {
  ElMessage.warning(`删除数据: ${row.name}`)
  tableData.value = tableData.value.filter(item => item.name !== row.name)
}
</script>

<style scoped>
.test-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.box-card {
  margin-bottom: 20px;
}
</style>