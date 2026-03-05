<template>
  <el-dialog v-model="dialogVisible" title="插件配置" width="800px" :close-on-click-modal="false">
    <div class="config-content">
      <el-alert 
        title="配置说明" 
        type="info" 
        description="请配置插件的 JSON 格式参数，确保格式正确后点击保存" 
        show-icon 
        :closable="false"
        class="mb15"
      />
      
      <el-form label-position="top">
        <el-form-item label="插件 ID">
          <el-input v-model="pluginId" disabled placeholder="插件 ID" />
        </el-form-item>
        
        <el-form-item label="配置内容 (JSON 格式)" required>
          <el-input
            v-model="configJson"
            type="textarea"
            :rows="15"
            placeholder='请输入 JSON 格式的配置，例如：{"key": "value"}'
            @input="validateJson"
          />
          <div v-if="jsonError" class="json-error-tip">
            <el-tag type="danger" size="small">JSON 格式错误：{{ jsonError }}</el-tag>
          </div>
          <div v-else-if="jsonValid" class="json-valid-tip">
            <el-tag type="success" size="small">✓ JSON 格式正确</el-tag>
          </div>
        </el-form-item>
      </el-form>
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleConfirm" :disabled="!jsonValid && !!jsonError">
          保存配置
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="PluginConfigurationDialog">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import {
  getApiPluginConfigurationGetPluginConfigurationByPluginId,
  putApiPluginConfigurationPluginId
} from '@/api/fd-system-api-admin/PluginConfiguration'

// 定义 props
interface Props {
  modelValue: boolean
  pluginId?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  pluginId: ''
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

// 插件 ID
const pluginId = ref('')

// 配置 JSON 字符串
const configJson = ref('')

// JSON 验证状态
const jsonValid = ref(false)
const jsonError = ref('')

// 验证 JSON 格式
const validateJson = () => {
  if (!configJson.value.trim()) {
    jsonValid.value = false
    jsonError.value = ''
    return
  }
  
  try {
    JSON.parse(configJson.value)
    jsonValid.value = true
    jsonError.value = ''
  } catch (e: any) {
    jsonValid.value = false
    jsonError.value = e.message
  }
}

// 获取插件配置
const fetchConfig = async (id: string) => {
  try {
    const response = await getApiPluginConfigurationGetPluginConfigurationByPluginId({ PluginId: id })
    console.log(response)
    // 后端返回的是 JSON 字符串，直接赋值
    configJson.value = response || '{}'
    validateJson()
  } catch (error: any) {
    ElMessage.error('获取配置失败：' + (error.message || '未知错误'))
    configJson.value = '{}'
    jsonValid.value = false
    jsonError.value = ''
  }
}

// 保存配置
const handleConfirm = async () => {
  if (!jsonValid.value) {
    ElMessage.warning('请先修正 JSON 格式')
    return
  }
  
  try {
    // 调用 PUT 接口，传入 JSON 字符串
    await putApiPluginConfigurationPluginId(
      { PluginId: pluginId.value },
      JSON.stringify(configJson.value)
    )
    
    ElMessage.success('保存成功')
    dialogVisible.value = false
    emit('save-success')
  } catch (error: any) {
    ElMessage.error('保存失败：' + (error.message || '未知错误'))
  }
}

// 取消
const handleCancel = () => {
  dialogVisible.value = false
}

// 监听对话框打开
watch(
  () => props.modelValue,
  (val) => {
    if (val && props.pluginId) {
      pluginId.value = props.pluginId
      fetchConfig(props.pluginId)
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="scss">
.config-content {
  .mb15 {
    margin-bottom: 15px;
  }
  
  .json-error-tip,
  .json-valid-tip {
    margin-top: 8px;
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>
