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
        <el-form-item :label="`${pluginName} -  配置内容`" required>
          <div ref="monacoEditorRef" style="width: 100%; height: 500px; border: 1px solid #dcdfe6; border-radius: 4px;"></div>
          <div v-if="jsonError" class="json-error-tip">
            <el-tag type="danger" size="small">
              <el-icon><ele-Close /></el-icon>
              {{ jsonError }}
            </el-tag>
          </div>
          <div v-else-if="jsonValid && configJson.trim()" class="json-valid-tip">
            <el-tag type="success" size="small">
              <el-icon><ele-Check /></el-icon>
              JSON 格式正确
            </el-tag>
          </div>
        </el-form-item>
      </el-form>
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button icon="ele-Close" @click="handleCancel">关 闭</el-button>
        <el-button icon="ele-CopyDocument" type="primary" @click="handleCopy">复 制</el-button>
        <el-button type="primary" @click="handleConfirm" :disabled="!jsonValid && !!jsonError">
          保存配置
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="PluginConfigurationDialog">
import { ref, computed, watch, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import * as monaco from 'monaco-editor'
import EditorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker'
import {
  getApiPluginConfigurationGetPluginConfigurationByPluginId,
  putApiPluginConfigurationPluginId
} from '@/api/fd-system-api-admin/PluginConfiguration'

// 定义 props
interface Props {
  modelValue: boolean
  pluginId?: string
  pluginName?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  pluginId: '',
  pluginName: ''
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

// 防止 monaco 报黄
self.MonacoEnvironment = {
  getWorker: (_: string, label: string) => new EditorWorker(),
};

// Monaco Editor 实例
let monacoEditor: any = null;
const monacoEditorRef = ref();

// 初始化 Monaco Editor
const initMonacoEditor = () => {
  if (!monacoEditorRef.value) {
    console.error('monacoEditorRef is not available');
    return;
  }
  
  monacoEditor = monaco.editor.create(monacoEditorRef.value, {
    theme: 'vs-dark', // 主题 vs vs-dark hc-black
    value: '', // 默认显示的值
    language: 'json',
    formatOnPaste: true,
    wordWrap: 'on', // 自动换行
    wrappingIndent: 'indent',
    folding: true, // 是否折叠
    foldingHighlight: true, // 折叠等高线
    foldingStrategy: 'indentation', // 折叠方式 auto | indentation
    showFoldingControls: 'always', // 是否一直显示折叠 always | mouseOver
    disableLayerHinting: true, // 等宽优化
    emptySelectionClipboard: false, // 空选择剪切板
    selectionClipboard: false, // 选择剪切板
    automaticLayout: true, // 自动布局
    codeLens: false, // 代码镜头
    scrollBeyondLastLine: false, // 滚动完最后一行后再滚动一屏幕
    colorDecorators: true, // 颜色装饰器
    accessibilitySupport: 'auto', // 辅助功能支持 "auto" | "off" | "on"
    lineNumbers: 'on', // 行号
    lineNumbersMinChars: 5, // 行号最小字符
    readOnly: false, // 是否只读
    minimap: {
      enabled: false // 不显示小地图
    },
    suggest: {
      showWords: false // 禁用单词建议
    },
    padding: {
      top: 10, // 顶部内边距
      bottom: 10 // 底部内边距
    }
  });
  
  // 监听编辑器内容变化
  monacoEditor.onDidChangeModelContent(() => {
    const newValue = monacoEditor.getValue();
    configJson.value = newValue;
    validateJson();
  });
};

// 验证 JSON 格式
const validateJson = () => {
  if (!configJson.value.trim()) {
    jsonValid.value = false
    jsonError.value = ''
    return
  }
  
  try {
    const parsed = JSON.parse(configJson.value)
    // 自动格式化为带缩进的 JSON
    configJson.value = JSON.stringify(parsed, null, 2)
    jsonValid.value = true
    jsonError.value = ''
  } catch (e: any) {
    jsonValid.value = false
    jsonError.value = e.message
  }
}

// 复制 JSON
const handleCopy = async () => {
  if (!monacoEditor || !configJson.value.trim()) return;
  
  try {
    await navigator.clipboard.writeText(configJson.value);
    ElMessage.success('复制成功');
  } catch (e: any) {
    ElMessage.error('复制失败');
  }
};

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
  async (val) => {
    if (val && props.pluginId) {
      pluginId.value = props.pluginId
      
      // 等待 DOM 更新后初始化编辑器
      await nextTick()
      if (!monacoEditor) {
        initMonacoEditor()
      }
      
      // 获取配置数据
      await fetchConfig(props.pluginId)
      
      // 数据加载完成后设置到编辑器
      if (monacoEditor) {
        monacoEditor.setValue(configJson.value)
      }
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
  
  .monaco-editor-wrapper {
    border-radius: 4px;
    overflow: hidden;
  }
  
  .json-error-tip,
  .json-valid-tip {
    margin-top: 8px;
    display: flex;
    align-items: center;
    gap: 6px;
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>
