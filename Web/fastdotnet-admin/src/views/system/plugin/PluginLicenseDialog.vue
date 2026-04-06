<template>
  <el-dialog 
    v-model="dialogVisible" 
    title="插件授权" 
    width="600px" 
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <div class="license-content">
      <el-alert 
        title="授权说明" 
        type="info" 
        :description="getAlertDescription()" 
        show-icon 
        :closable="false"
        class="mb15"
      />
      
      <el-form 
        ref="licenseFormRef"
        :model="licenseForm"
        label-position="top"
        :rules="formRules"
      >
        <el-form-item label="插件信息" class="mb15" v-if="!isBatch">
          <div class="plugin-info">
            <span class="plugin-label">插件名称：</span>
            <span class="plugin-value">{{ pluginName }}</span>
          </div>
          <div class="plugin-info">
            <span class="plugin-label">插件 ID：</span>
            <span class="plugin-value">{{ pluginId }}</span>
          </div>
        </el-form-item>
        
        <el-form-item label="已选插件" class="mb15" v-else>
          <div class="selected-plugins-info">
            <div class="plugin-count">
              <el-tag type="primary" size="large">{{ selectedPlugins.length }} 个插件</el-tag>
            </div>
            <div class="plugin-list">
              <el-tag 
                v-for="plugin in selectedPlugins" 
                :key="plugin.id" 
                size="small" 
                style="margin-right: 8px; margin-bottom: 8px"
              >
                {{ plugin.name }}
              </el-tag>
            </div>
          </div>
        </el-form-item>
        
        <!-- 授权类型：根据是否批量自动判断，用户不可修改 -->
        <!-- 单个插件使用类型 '0'，批量授权使用类型 '1' -->
        <el-form-item label="授权类型" prop="Type" required>
          <el-select 
            v-model="licenseForm.Type" 
            placeholder="请选择授权类型" 
            style="width: 100%"
            disabled
          >
            <el-option label="单个授权" value="0" />
            <el-option label="多个授权" value="1" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="授权信息" prop="LicenseStr" required>
          <el-input
            v-model="licenseForm.LicenseStr"
            type="textarea"
            :rows="8"
            :placeholder="getPlaceholderText()"
            autocomplete="off"
          />
          <div v-if="jsonError" class="json-error-tip">
            <el-tag type="danger" size="small">
              <el-icon><ele-Close /></el-icon>
              {{ jsonError }}
            </el-tag>
          </div>
          <div v-else-if="jsonValid && licenseForm.LicenseStr.trim()" class="json-valid-tip">
            <el-tag type="success" size="small">
              <el-icon><ele-Check /></el-icon>
              {{ props.isBatch ? 'JSON 数组格式正确' : 'JSON 对象格式正确' }}
            </el-tag>
          </div>
          
          <!-- 在线授权按钮 -->
          <div class="online-license-btn" v-if="!isBatch">
            <el-button 
              type="primary" 
              plain 
              @click="handleOnlineLicense"
              :loading="onlineSubmitting"
            >
              <el-icon><ele-Connection /></el-icon>
              在线授权
            </el-button>
          </div>
        </el-form-item>
      </el-form>
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button icon="ele-Close" @click="handleCancel">关 闭</el-button>
        <el-button type="primary" @click="handleConfirm" :loading="submitting">
          保存授权
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="PluginLicenseDialog">
import { ref, computed, watch, nextTick } from 'vue'
import { ElMessage, FormInstance } from 'element-plus'
import { postApiPluginSetPluginLicense, postApiPluginUpdatePluginLicenseOnline } from '@/api/fd-system-api-admin/Plugin'
import { usePluginStore } from '@/stores/plugin'

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

// 定义 props
interface Props {
  modelValue: boolean
  pluginId?: string
  pluginName?: string
  isBatch?: boolean // 是否为批量授权
  selectedPlugins?: Plugin[] // 批量授权时选中的插件列表
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  pluginId: '',
  pluginName: '',
  isBatch: false,
  selectedPlugins: () => []
})

// 定义 emits
const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'save-success': []
}>()

// 对话框可见性
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 使用插件 store 获取商城 token
const pluginStore = usePluginStore()

// 表单引用
const licenseFormRef = ref<FormInstance>()

// 提交状态
const submitting = ref(false)
const onlineSubmitting = ref(false) // 在线授权提交状态

// 表单数据
const licenseForm = ref({
  Type: '0', // '0'-单个，'1'-多个（自动赋值）
  LicenseStr: ''
})

// 授权类型
const licenseType = computed(() => licenseForm.value.Type)

// JSON 验证状态
const jsonValid = ref(false)
const jsonError = ref('')

// 表单验证规则
const formRules = {
  Type: [
    { required: true, message: '请选择授权类型', trigger: 'change' }
  ],
  LicenseStr: [
    { required: true, message: '请输入授权信息', trigger: 'blur' }
  ]
}

// 监听授权信息输入变化，实时验证 JSON
watch(
  () => licenseForm.value.LicenseStr,
  () => {
    if (licenseForm.value.Type) {
      validateJson()
    }
  }
)

// 关闭对话框

// 获取占位符文本
const getPlaceholderText = () => {
  if (props.isBatch) {
    return '请输入批量授权信息（JSON 数组格式，包含所有插件的授权数据）'
  }
  // 单个授权默认使用 JSON 对象格式
  return '请输入授权信息（JSON 对象格式，例如：{"key": "value"}）'
}

// 获取提示信息
const getAlertDescription = () => {
  if (props.isBatch) {
    return `正在为 ${props.selectedPlugins.length} 个插件进行批量授权，请输入授权信息`
  }
  return '请输入插件授权信息，确保格式正确后点击保存'
}

// 验证 JSON 格式
const validateJson = () => {
  if (!licenseForm.value.LicenseStr.trim()) {
    jsonValid.value = false
    jsonError.value = ''
    return
  }
  
  try {
    const parsed = JSON.parse(licenseForm.value.LicenseStr)
    
    // 根据授权类型验证格式
    if (licenseType.value === '0' && !Array.isArray(parsed)) {
      // 单个授权应该是对象
      jsonValid.value = true
      jsonError.value = ''
    } else if (licenseType.value === '1' && Array.isArray(parsed)) {
      // 多个授权应该是数组
      jsonValid.value = true
      jsonError.value = ''
    } else if (licenseType.value === '0' && Array.isArray(parsed)) {
      jsonValid.value = false
      jsonError.value = '单个授权应输入 JSON 对象格式'
    } else if (licenseType.value === '1' && !Array.isArray(parsed)) {
      jsonValid.value = false
      jsonError.value = '批量授权应输入 JSON 数组格式'
    } else {
      jsonValid.value = false
      jsonError.value = 'JSON 格式不正确'
    }
  } catch (e: any) {
    jsonValid.value = false
    jsonError.value = e.message
  }
}

// 处理授权类型变化
const handleTypeChange = () => {
  // 类型变化时重新验证 JSON
  if (licenseForm.value.LicenseStr.trim()) {
    validateJson()
  }
}

// 关闭对话框
const handleClose = () => {
  licenseFormRef.value?.resetFields()
  licenseForm.value = {
    Type: '0',
    LicenseStr: ''
  }
  jsonValid.value = false
  jsonError.value = ''
}

// 取消
const handleCancel = () => {
  dialogVisible.value = false
}

// 在线授权
const handleOnlineLicense = async () => {
  if (!props.pluginId) {
    ElMessage.warning('插件 ID 不能为空')
    return
  }
  
  // 检查是否有商城 token
  const token = pluginStore.marketplaceToken
  if (!token) {
    ElMessage.warning('请先在插件商城登录')
    return
  }
  
  try {
    onlineSubmitting.value = true
    
    // 调用在线授权 API，使用商城 Token 进行授权
    const response = await postApiPluginUpdatePluginLicenseOnline({
      Token: token,      // 使用商城的 Token
      PluginId: props.pluginId
    })
    
    // 检查返回值，true 表示授权成功
    if (response === true) {
      ElMessage.success('在线授权成功')
      dialogVisible.value = false
      emit('save-success')
    } else {
      ElMessage.error('在线授权失败')
    }
  } catch (error: any) {
    ElMessage.error('在线授权失败：' + (error.message || '未知错误'))
  } finally {
    onlineSubmitting.value = false
  }
}

// 保存授权
const handleConfirm = async () => {
  if (!licenseFormRef.value) return
  
  await licenseFormRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.warning('请填写完整的授权信息')
      return
    }
    
    // 额外验证 JSON 格式
    if (!jsonValid.value && licenseForm.value.LicenseStr.trim()) {
      ElMessage.error('授权信息格式不正确，请检查')
      return
    }
    
    try {
      submitting.value = true
      
      // 调用设置插件许可 API
      await postApiPluginSetPluginLicense({
        Type: licenseForm.value.Type,
        LicenseStr: licenseForm.value.LicenseStr
      })
      
      ElMessage.success('授权成功')
      dialogVisible.value = false
      emit('save-success')
    } catch (error: any) {
      ElMessage.error('授权失败：' + (error.message || '未知错误'))
    } finally {
      submitting.value = false
    }
  })
}

// 监听对话框打开
watch(
  () => props.modelValue,
  async (val) => {
    if (val && (props.pluginId || props.selectedPlugins.length > 0)) {
      // 等待 DOM 更新
      await nextTick()
      
      // 根据是否批量授权自动设置类型
      licenseForm.value = {
        Type: props.isBatch ? '1' : '0', // 批量授权为'1'，单个授权为'0'
        LicenseStr: ''
      }
      jsonValid.value = false
      jsonError.value = ''
      
      // 清除验证状态
      licenseFormRef.value?.clearValidate()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="scss">
.license-content {
  .mb15 {
    margin-bottom: 15px;
  }
  
  .plugin-info {
    padding: 8px 12px;
    background-color: #f5f7fa;
    border-radius: 4px;
    margin-bottom: 8px;
    
    .plugin-label {
      color: #909399;
      font-weight: 500;
      margin-right: 8px;
    }
    
    .plugin-value {
      color: #303133;
      font-weight: 600;
    }
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.json-error-tip,
.json-valid-tip {
  margin-top: 8px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.online-license-btn {
  margin-top: 12px;
  text-align: right;
}

.selected-plugins-info {
  .plugin-count {
    margin-bottom: 12px;
  }
  
  .plugin-list {
    max-height: 200px;
    overflow-y: auto;
    padding: 8px;
    background-color: #f5f7fa;
    border-radius: 4px;
  }
}
</style>
