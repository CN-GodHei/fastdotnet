<template>
  <div class="rich-text-editor-container">
    <div class="editor-header" v-if="showHeader">
      <h3>{{ title }}</h3>
      <el-button @click="saveContent" type="primary" size="small">保存</el-button>
      <el-button @click="closeEditor" size="small">关闭</el-button>
    </div>
    <div class="editor-toolbar" v-if="showToolbar">
      <el-button-group>
        <el-button size="small" @click="formatText('bold')"><strong>B</strong></el-button>
        <el-button size="small" @click="formatText('italic')"><em>I</em></el-button>
        <el-button size="small" @click="formatText('underline')"><u>U</u></el-button>
        <el-button size="small" @click="insertHTML('<p>&nbsp;</p>')">段落</el-button>
        <el-button size="small" @click="insertHTML('<br>')">换行</el-button>
      </el-button-group>
    </div>
    <div class="editor-content">
      <div 
        ref="editorRef" 
        class="editor-box" 
        contenteditable="true" 
        :placeholder="placeholder" 
        @input="onInput"
        @blur="onBlur"
        style="width: 100%; height: 400px; padding: 10px; border: 1px solid #dcdfe6; border-radius: 4px; overflow-y: auto;"
      ></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue';

// 接收父组件传递的属性
const props = withDefaults(defineProps<{ 
  initialContent?: string;
  title?: string;
  showHeader?: boolean;
  showToolbar?: boolean;
  placeholder?: string;
}>(), {
  initialContent: '',
  title: '富文本编辑器',
  showHeader: true,
  showToolbar: true,
  placeholder: '请输入内容...'
})

// 定义事件
const emit = defineEmits(['save', 'close']);

// 内部状态
const content = ref(props.initialContent);
const editorRef = ref<HTMLElement>();

// 格式化文本
const formatText = (command: string) => {
  document.execCommand(command, false);
  editorRef.value?.focus();
};

// 插入HTML
const insertHTML = (html: string) => {
  document.execCommand('insertHTML', false, html);
  editorRef.value?.focus();
};

// 输入事件处理
const onInput = () => {
  if (editorRef.value) {
    content.value = editorRef.value.innerHTML;
  }
};

// 失焦事件处理
const onBlur = () => {
  if (editorRef.value) {
    content.value = editorRef.value.innerHTML;
  }
};

// 监听外部内容变化
watch(() => props.initialContent, (newContent) => {
  if (newContent !== content.value && editorRef.value) {
    editorRef.value.innerHTML = newContent;
    content.value = newContent;
  }
}, { immediate: true });

// 保存内容
const saveContent = () => {
  emit('save', content.value);
};

// 关闭编辑器
const closeEditor = () => {
  emit('close');
};

// 富文本编辑器通信事件类型
enum RichTextEventType {
  GET_CONTENT = 'richText:getContent',
  SET_CONTENT = 'richText:setContent',
  SAVE_CONTENT = 'richText:saveContent',
  INSERT_IMAGE = 'richText:insertImage',
  INSERT_LINK = 'richText:insertLink',
}

// 监听来自主应用的通信事件
const handleRichTextEvent = (event: any) => {
  if (event.detail && event.detail.type) {
    const { type, data } = event.detail;
    
    switch (type) {
      case RichTextEventType.GET_CONTENT:
        // 返回当前内容
        const responseEvent = new CustomEvent('richTextEvent', {
          detail: { 
            type: RichTextEventType.GET_CONTENT, 
            data: { 
              eventId: data.eventId, 
              content: content.value 
            } 
          }
        });
        window.dispatchEvent(responseEvent);
        break;
      
      case RichTextEventType.SET_CONTENT:
        // 设置内容
        if (editorRef.value) {
          editorRef.value.innerHTML = data.content;
          content.value = data.content;
        }
        break;
        
      case RichTextEventType.SAVE_CONTENT:
        // 触发保存
        emit('save', data.content);
        break;
        
      case RichTextEventType.INSERT_IMAGE:
        // 插入图片
        insertHTML(`<img src="${data.imageUrl}" alt="插入的图片" style="max-width: 100%;" />`);
        break;
        
      case RichTextEventType.INSERT_LINK:
        // 插入链接
        insertHTML(`<a href="${data.url}" target="_blank">${data.text}</a>`);
        break;
    }
  }
};

// 组件挂载时注册事件监听器
onMounted(() => {
  window.addEventListener('richTextEvent', handleRichTextEvent as EventListener);
});

// 组件卸载时移除事件监听器
onUnmounted(() => {
  window.removeEventListener('richTextEvent', handleRichTextEvent as EventListener);
});

// 监听外部内容变化
watch(() => props.initialContent, (newContent) => {
  if (newContent !== content.value) {
    content.value = newContent;
  }
});

defineExpose({
  getContent: () => content.value,
  setContent: (html: string) => {
    content.value = html;
    if (editorRef.value) {
      editorRef.value.innerHTML = html;
    }
  }
});
</script>

<style scoped>
.rich-text-editor-container {
  padding: 20px;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.editor-content {
  flex: 1;
  overflow: hidden;
}
</style>