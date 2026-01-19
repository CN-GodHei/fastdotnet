<template>
  <div class="rich-text-invoker">
    <el-button @click="openRichTextEditor" type="primary" :icon="Edit">
      打开富文本编辑器
    </el-button>
    <el-input
      v-model="displayContent"
      type="textarea"
      :rows="4"
      placeholder="富文本内容将显示在这里..."
      readonly
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Edit } from '@element-plus/icons-vue';

// 富文本编辑器通信事件类型
enum RichTextEventType {
  GET_CONTENT = 'richText:getContent',
  SET_CONTENT = 'richText:setContent',
  SAVE_CONTENT = 'richText:saveContent',
  INSERT_IMAGE = 'richText:insertImage',
  INSERT_LINK = 'richText:insertLink',
}

// 定义组件属性
const props = defineProps({
  initialContent: {
    type: String,
    default: ''
  }
});

// 定义事件
const emit = defineEmits(['contentChange']);

// 内容显示
const displayContent = ref(props.initialContent);

// 打开富文本编辑器
const openRichTextEditor = () => {
  // 发送事件到主应用，请求打开富文本编辑器
  const event = new CustomEvent('openRichTextEditor', {
    detail: {
      initialContent: props.initialContent,
      callback: (content: string) => {
        displayContent.value = content;
        emit('contentChange', content);
      }
    }
  });
  window.parent.dispatchEvent(event);
};

// 监听来自主应用的富文本编辑器事件
const handleRichTextEvent = (event: any) => {
  if (event.detail && event.detail.type) {
    const { type, data } = event.detail;
    if (type === RichTextEventType.SAVE_CONTENT) {
      displayContent.value = data.content;
      emit('contentChange', data.content);
    }
  }
};

// 组件挂载时注册事件监听器
// 注意：在微应用中，可能需要通过主应用提供的方法来注册事件
</script>

<style scoped>
.rich-text-invoker {
  margin: 20px 0;
}
</style>