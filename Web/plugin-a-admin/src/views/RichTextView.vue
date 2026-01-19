<template>
  <div class="rich-text-demo-page">
    <h2>Plugin A - 富文本编辑器演示</h2>
    <p>这个页面演示了Plugin A如何使用主应用提供的富文本编辑器</p>
    
    <el-card class="demo-card">
      <template #header>
        <div class="card-header">
          <span>富文本编辑器集成示例</span>
        </div>
      </template>
      
      <!-- 富文本编辑器调用组件 -->
      <RichTextEditorInvoker 
        :initial-content="content" 
        @content-change="onContentChange"
      />
      
      <div class="content-preview">
        <h3>内容预览：</h3>
        <div v-html="content" class="preview-box"></div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import RichTextEditorInvoker from '../components/RichTextEditorInvoker.vue';

// 示例内容
const content = ref('<p>这里是富文本编辑器的初始内容</p>');

// 内容变更处理
const onContentChange = (newContent: string) => {
  content.value = newContent;
  console.log('富文本内容已更新:', newContent);
};

// 页面加载完成后的一些初始化工作
onMounted(() => {
  console.log('RichText demo page mounted');
});
</script>

<style scoped>
.rich-text-demo-page {
  padding: 20px;
}

.demo-card {
  max-width: 1000px;
  margin: 20px auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.content-preview {
  margin-top: 20px;
}

.preview-box {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 15px;
  min-height: 100px;
  background-color: #fafafa;
}
</style>