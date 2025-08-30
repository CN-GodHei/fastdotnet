<template>
  <div class="rate-limit-rule-management">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>限流规则管理</span>
          <div class="card-header-actions">
            <el-button type="primary" @click="handleCreate">新增限流规则</el-button>
            <el-button @click="handleRefresh">刷新</el-button>
          </div>
        </div>
      </template>

      <!-- 搜索表单 -->
      <el-form :model="searchForm" label-width="80px" class="search-form">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-form-item label="类型">
              <el-select v-model="searchForm.type" placeholder="请选择类型" clearable>
                <el-option label="IP" value="IP"></el-option>
                <el-option label="User" value="User"></el-option>
                <el-option label="ApiKey" value="ApiKey"></el-option>
                <el-option label="Route" value="Route"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="键">
              <el-input v-model="searchForm.key" placeholder="请输入键" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="描述">
              <el-input v-model="searchForm.description" placeholder="请输入描述" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item>
              <el-button type="primary" @click="handleSearch">搜索</el-button>
              <el-button @click="handleReset">重置</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <!-- 数据表格 -->
      <el-table :data="tableData" border style="width: 100%" v-loading="loading">
        <el-table-column prop="type" label="类型" width="120"></el-table-column>
        <el-table-column prop="key" label="键" show-overflow-tooltip></el-table-column>
        <el-table-column prop="permitLimit" label="允许最大请求数" width="150"></el-table-column>
        <el-table-column prop="windowSeconds" label="时间窗口(秒)" width="150"></el-table-column>
        <el-table-column prop="description" label="描述" show-overflow-tooltip></el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="isSystem" label="系统内置" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isSystem ? 'success' : 'info'">
              {{ scope.row.isSystem ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button size="small" @click="handleEdit(scope.row)" :disabled="scope.row.isSystem">编辑</el-button>
            <el-button size="small" type="danger" @click="handleDelete(scope.row)" :disabled="scope.row.isSystem">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.currentPage"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="类型" prop="type">
          <el-select v-model="formData.type" placeholder="请选择类型" :disabled="!!formData.id">
            <el-option label="IP" value="IP"></el-option>
            <el-option label="User" value="User"></el-option>
            <el-option label="ApiKey" value="ApiKey"></el-option>
            <el-option label="Route" value="Route"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="键" prop="key">
          <el-input v-model="formData.key" placeholder="请输入键"></el-input>
        </el-form-item>
        <el-form-item label="允许最大请求数" prop="permitLimit">
          <el-input-number v-model="formData.permitLimit" :min="1" :max="1000000" placeholder="请输入允许最大请求数"></el-input-number>
        </el-form-item>
        <el-form-item label="时间窗口(秒)" prop="windowSeconds">
          <el-input-number v-model="formData.windowSeconds" :min="1" :max="86400" placeholder="请输入时间窗口(秒)"></el-input-number>
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="formData.description" type="textarea" placeholder="请输入描述"></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitLoading">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  getAdminRateLimitRulesPage, 
  postAdminRateLimitRules, 
  putAdminRateLimitRulesId, 
  deleteAdminRateLimitRulesId
} from '/@/api/fd-system-api/RateLimitRules'
import type { FdRateLimitRuleDto, CreateFdRateLimitRuleDto, UpdateFdRateLimitRuleDto } from '/@/api/fd-system-api/typings'

// 表格数据
const tableData = ref<FdRateLimitRuleDto[]>([])
const loading = ref(false)

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 0
})

// 搜索表单
const searchForm = reactive({
  type: '',
  key: '',
  description: ''
})

// 对话框
const dialogVisible = ref(false)
const dialogTitle = ref('')
const submitLoading = ref(false)
const formRef = ref()

// 表单数据
const formData = reactive<CreateFdRateLimitRuleDto & UpdateFdRateLimitRuleDto & { id?: string }>({
  type: '',
  key: '',
  permitLimit: 100,
  windowSeconds: 60,
  description: ''
})

// 表单验证规则
const formRules = {
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  key: [{ required: true, message: '请输入键', trigger: 'blur' }],
  permitLimit: [{ required: true, message: '请输入允许最大请求数', trigger: 'blur' }],
  windowSeconds: [{ required: true, message: '请输入时间窗口(秒)', trigger: 'blur' }]
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-CN')
}

// 获取表格数据
const fetchData = async () => {
  loading.value = true
  try {
    const params = {
      pageIndex: pagination.currentPage,
      pageSize: pagination.pageSize,
      ...searchForm
    }
    
    const response = await getAdminRateLimitRulesPage({ params })
    tableData.value = response.items || []
    pagination.total = response.pageInfo?.total || 0
  } catch (error) {
    ElMessage.error('获取数据失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

// 处理搜索
const handleSearch = () => {
  pagination.currentPage = 1
  fetchData()
}

// 重置搜索
const handleReset = () => {
  searchForm.type = ''
  searchForm.key = ''
  searchForm.description = ''
  pagination.currentPage = 1
  fetchData()
}

// 刷新数据
const handleRefresh = () => {
  fetchData()
}

// 处理分页大小变化
const handleSizeChange = (val: number) => {
  pagination.pageSize = val
  pagination.currentPage = 1
  fetchData()
}

// 处理当前页变化
const handleCurrentChange = (val: number) => {
  pagination.currentPage = val
  fetchData()
}

// 处理新增
const handleCreate = () => {
  dialogTitle.value = '新增限流规则'
  dialogVisible.value = true
  Object.assign(formData, {
    type: '',
    key: '',
    permitLimit: 100,
    windowSeconds: 60,
    description: ''
  })
}

// 处理编辑
const handleEdit = (row: FdRateLimitRuleDto) => {
  dialogTitle.value = '编辑限流规则'
  dialogVisible.value = true
  Object.assign(formData, {
    id: row.id,
    type: row.type,
    key: row.key,
    permitLimit: row.permitLimit,
    windowSeconds: row.windowSeconds,
    description: row.description
  })
}

// 处理删除
const handleDelete = (row: FdRateLimitRuleDto) => {
  ElMessageBox.confirm('确定要删除这条限流规则吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteAdminRateLimitRulesId({ params: { id: row.id! } })
      ElMessage.success('删除成功')
      fetchData()
    } catch (error) {
      ElMessage.error('删除失败')
      console.error(error)
    }
  }).catch(() => {
    // 用户取消删除
  })
}

// 处理提交
const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid: boolean) => {
    if (!valid) return
    
    submitLoading.value = true
    try {
      if (formData.id) {
        // 编辑
        const updateData: UpdateFdRateLimitRuleDto = {
          type: formData.type,
          key: formData.key,
          permitLimit: formData.permitLimit,
          windowSeconds: formData.windowSeconds,
          description: formData.description
        }
        await putAdminRateLimitRulesId({ 
          params: { id: formData.id }, 
          body: updateData 
        })
        ElMessage.success('更新成功')
      } else {
        // 新增
        const createData: CreateFdRateLimitRuleDto = {
          type: formData.type,
          key: formData.key,
          permitLimit: formData.permitLimit,
          windowSeconds: formData.windowSeconds,
          description: formData.description
        }
        await postAdminRateLimitRules(createData)
        ElMessage.success('新增成功')
      }
      
      dialogVisible.value = false
      fetchData()
    } catch (error) {
      ElMessage.error(formData.id ? '更新失败' : '新增失败')
      console.error(error)
    } finally {
      submitLoading.value = false
    }
  })
}

// 处理对话框关闭
const handleDialogClose = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
}

// 组件挂载时获取数据
onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.rate-limit-rule-management {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>