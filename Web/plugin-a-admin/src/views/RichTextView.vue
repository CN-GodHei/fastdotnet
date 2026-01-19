<template>
  <div class="rich-text-demo-page">
    <h2>Plugin A - 富文本编辑器演示</h2>
    <p>这个页面演示了Plugin A如何使用富文本编辑器</p>
    
    <el-card class="demo-card">
      <template #header>
        <div class="card-header">
          <span>富文本编辑器集成示例</span>
        </div>
      </template>
      
      <!-- 导航菜单 -->
      <el-tabs type="border-card" class="demo-tabs">
        <el-tab-pane label="富文本编辑器演示">
          <!-- 富文本编辑器调用组件 -->
          <RichTextEditorInvoker 
            :initial-content="content" 
            @content-change="onContentChange"
          />
          
          <div class="content-preview">
            <h3>内容预览：</h3>
            <div v-html="content" class="preview-box"></div>
          </div>
        </el-tab-pane>
        
        <el-tab-pane label="插件化使用演示">
          <div class="demo-section">
            <h3>通过插件化方式使用富文本编辑器</h3>
            <p>点击下面按钮跳转到富文本编辑器插件演示页面：</p>
            <el-button 
              type="primary" 
              @click="goToPluginDemo"
              :icon="Position"
            >
              查看插件化演示
            </el-button>
          </div>
        </el-tab-pane>
        
        <el-tab-pane label="表单集成演示">
          <div class="demo-section">
            <h3>在表单中集成富文本编辑器</h3>
            <p>点击下面按钮跳转到表单集成演示页面：</p>
            <el-button 
              type="success" 
              @click="goToFormDemo"
              :icon="Document"
            >
              查看表单集成
            </el-button>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { Position, Document } from '@element-plus/icons-vue';
import RichTextEditorInvoker from '../components/RichTextEditorInvoker.vue';

// 示例内容
const content = ref('<p>这里是富文本编辑器的初始内容</p>');

// 内容变更处理
const onContentChange = (newContent: string) => {
  content.value = newContent;
  console.log('富文本内容已更新:', newContent);
};

// 路由相关
const router = useRouter();

// 跳转到插件化演示页面
const goToPluginDemo = () => {
  router.push('/rich-text-plugin-demo');
};

// 跳转到表单集成演示页面
const goToFormDemo = () => {
  router.push('/form-with-richtext');
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

.demo-section {
  padding: 20px;
  text-align: center;
}

.demo-section h3 {
  margin-bottom: 15px;
}

.demo-section p {
  margin-bottom: 20px;
  color: #606266;
}

.demo-tabs {
  margin-top: 20px;
}
</style>