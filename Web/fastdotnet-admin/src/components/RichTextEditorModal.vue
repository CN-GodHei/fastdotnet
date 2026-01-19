<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    :width="richTextWidth"
    :fullscreen="isFullscreen"
    @close="handleClose"
  >
    <div class="rich-text-container">
      <!-- 工具栏 -->
      <div class="editor-toolbar">
        <el-button size="small" @click="toggleFullscreen">
          {{ isFullscreen ? '退出全屏' : '全屏编辑' }}
        </el-button>
        <el-button size="small" @click="insertImage">
          插入图片
        </el-button>
      </div>
      
      <!-- 富文本编辑器微应用容器 -->
      <div id="rich-text-editor-container" class="editor-area"></div>
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick, watch } from 'vue';
import { loadMicroApp, MicroApp } from 'qiankun';
import { useMicroAppsStore } from '@/stores/microApps';

// 定义 props
const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  initialContent: {
    type: String,
    default: ''
  },
  title: {
    type: String,
    default: '富文本编辑器'
  },
  width: {
    type: String,
    default: '80%'
  }
});

// 定义 emits
const emit = defineEmits(['update:modelValue', 'save', 'cancel']);

// 响应式数据
const dialogVisible = ref(props.modelValue);
const isFullscreen = ref(false);
const richTextWidth = ref(props.width);
let microAppInstance: MicroApp | null = null;
let editorInstance: any = null;

// 从 Pinia 获取微应用配置
const microAppsStore = useMicroAppsStore();

// 监听 dialog 显示状态
watch(() => props.modelValue, async (newVal) => {
  dialogVisible.value = newVal;
  if (newVal) {
    await nextTick();
    loadRichTextEditor();
  }
});

// 加载富文本编辑器微应用
const loadRichTextEditor = async () => {
  // 等待 DOM 更新完成
  await nextTick();
  
  // 获取富文本编辑器的配置
  const config = microAppsStore.getMicroAppConfig('RichTextEditor');
  
  if (!config) {
    console.error('[RichTextEditorModal] 未找到富文本编辑器配置');
    return;
  }

  // 加载微应用
  microAppInstance = loadMicroApp(
    {
      name: 'RichTextEditor',
      entry: config.entry,
      container: '#rich-text-editor-container',
      props: {
        initialContent: props.initialContent,
        title: props.title,
        showHeader: false, // 在模态框中不显示头部
      }
    },
    {
      sandbox: {
        strictStyleIsolation: true,
      }
    }
  );
};

// 切换全屏
const toggleFullscreen = () => {
  isFullscreen.value = !isFullscreen.value;
  richTextWidth.value = isFullscreen.value ? '100%' : props.width;
};

// 插入图片
const insertImage = () => {
  // 通知编辑器插入图片
  if (editorInstance) {
    // 如果有编辑器实例，调用其方法
  } else {
    // 通过事件或postMessage方式通知编辑器
    console.log('Insert image clicked');
  }
};

// 保存内容
const handleSave = () => {
  // 获取编辑器内容并触发保存事件
  // 通过微应用通信获取内容
  emit('save', props.initialContent); // 临时实现
  handleClose();
};

// 取消编辑
const handleCancel = () => {
  emit('cancel');
  handleClose();
};

// 关闭对话框
const handleClose = () => {
  emit('update:modelValue', false);
  
  // 卸载微应用
  if (microAppInstance) {
    microAppInstance.unmount();
    microAppInstance = null;
  }
};

// 组件卸载时清理
onUnmounted(() => {
  if (microAppInstance) {
    microAppInstance.unmount();
    microAppInstance = null;
  }
});
</script>

<style scoped>
.rich-text-container {
  min-height: 500px;
}

.editor-toolbar {
  margin-bottom: 10px;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
  display: flex;
  gap: 10px;
}

.editor-area {
  min-height: 400px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
}
</style>