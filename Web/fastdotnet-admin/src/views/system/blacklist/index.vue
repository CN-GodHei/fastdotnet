<template>
  <div class="blacklist-management">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>黑名单管理</span>
          <div class="card-header-actions">
            <el-button type="primary" @click="handleCreate">新增黑名单</el-button>
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
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="值">
              <el-input v-model="searchForm.value" placeholder="请输入值" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="原因">
              <el-input v-model="searchForm.reason" placeholder="请输入原因" clearable></el-input>
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
        <el-table-column prop="value" label="值" show-overflow-tooltip></el-table-column>
        <el-table-column prop="reason" label="原因" show-overflow-tooltip></el-table-column>
        <el-table-column prop="expiredAt" label="过期时间" width="180">
          <template #default="scope">
            {{ scope.row.expiredAt ? formatDate(scope.row.expiredAt) : '-' }}
          </template>
        </el-table-column>
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
      width="500px"
      @close="handleDialogClose"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="100px">
        <el-form-item label="类型" prop="type">
          <el-select v-model="formData.type" placeholder="请选择类型" :disabled="!!formData.id">
            <el-option label="IP" value="IP"></el-option>
            <el-option label="User" value="User"></el-option>
            <el-option label="ApiKey" value="ApiKey"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="值" prop="value">
          <el-input v-model="formData.value" placeholder="请输入值"></el-input>
        </el-form-item>
        <el-form-item label="原因" prop="reason">
          <el-input v-model="formData.reason" type="textarea" placeholder="请输入原因"></el-input>
        </el-form-item>
        <el-form-item label="过期时间" prop="expiredAt">
          <el-date-picker
            v-model="formData.expiredAt"
            type="datetime"
            placeholder="请选择过期时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DDTHH:mm:ss"
          />
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
  getAdminBlacklistsPage, 
  postAdminBlacklists, 
  putAdminBlacklistsId, 
  deleteAdminBlacklistsId,
  getAdminBlacklists
} from '/@/api/fd-system-api/Blacklists'
import type { FdBlacklistDto, CreateFdBlacklistDto, UpdateFdBlacklistDto } from '/@/api/fd-system-api/typings'

// 表格数据
const tableData = ref<FdBlacklistDto[]>([])
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
  value: '',
  reason: ''
})

// 对话框
const dialogVisible = ref(false)
const dialogTitle = ref('')
const submitLoading = ref(false)
const formRef = ref()

// 表单数据
const formData = reactive<CreateFdBlacklistDto & UpdateFdBlacklistDto & { id?: string }>({
  type: '',
  value: '',
  reason: '',
  expiredAt: undefined
})

// 表单验证规则
const formRules = {
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  value: [{ required: true, message: '请输入值', trigger: 'blur' }]
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
    
    const response = await getAdminBlacklistsPage({ params })
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
  searchForm.value = ''
  searchForm.reason = ''
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
  dialogTitle.value = '新增黑名单'
  dialogVisible.value = true
  Object.assign(formData, {
    type: '',
    value: '',
    reason: '',
    expiredAt: undefined
  })
}

// 处理编辑
const handleEdit = (row: FdBlacklistDto) => {
  dialogTitle.value = '编辑黑名单'
  dialogVisible.value = true
  Object.assign(formData, {
    id: row.id,
    type: row.type,
    value: row.value,
    reason: row.reason,
    expiredAt: row.expiredAt
  })
}

// 处理删除
const handleDelete = (row: FdBlacklistDto) => {
  ElMessageBox.confirm('确定要删除这条黑名单记录吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteAdminBlacklistsId({ params: { id: row.id! } })
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
        const updateData: UpdateFdBlacklistDto = {
          type: formData.type,
          value: formData.value,
          reason: formData.reason,
          expiredAt: formData.expiredAt
        }
        await putAdminBlacklistsId({ 
          params: { id: formData.id }, 
          body: updateData 
        })
        ElMessage.success('更新成功')
      } else {
        // 新增
        const createData: CreateFdBlacklistDto = {
          type: formData.type,
          value: formData.value,
          reason: formData.reason,
          expiredAt: formData.expiredAt
        }
        await postAdminBlacklists(createData)
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
.blacklist-management {
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