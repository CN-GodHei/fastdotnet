<template>
  <div class="rich-text-demo-container">
    <h2>Plugin A - 富文本编辑器插件演示</h2>
    <p>这个页面演示了Plugin A如何通过插件API使用富文本编辑器插件</p>
    
    <el-card class="demo-card">
      <template #header>
        <div class="card-header">
          <span>通过插件API使用富文本编辑器</span>
        </div>
      </template>
      
      <!-- 使用iframe加载富文本编辑器插件 -->
      <div class="editor-section">
        <h3>富文本编辑器:</h3>
        <div class="editor-container">
          <iframe 
            :src="richTextEditorUrl" 
            width="100%" 
            height="400"
            frameborder="0"
            ref="editorFrame"
          ></iframe>
        </div>
      </div>
      
      <div class="content-preview">
        <h3>内容预览：</h3>
        <div v-html="editorContent" class="preview-box"></div>
      </div>
      
      <div class="content-actions">
        <el-button @click="clearContent" type="warning">清空内容</el-button>
        <el-button @click="loadSampleContent" type="info">加载示例内容</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

// 富文本编辑器内容
const editorContent = ref('<p>欢迎使用富文本编辑器插件！</p><p>您可以在此处编辑内容。</p>');

// 富文本编辑器URL
const richTextEditorUrl = ref(''); // 富文本编辑器插件的URL

// 从插件系统获取富文本编辑器插件的URL
const getRichTextPluginUrl = () => {
  // 从全局插件API获取富文本编辑器插件的入口地址
  if ((window as any).__PLUGIN_API__) {
    console.log("进1")
    const pluginInfo = (window as any).__PLUGIN_API__.getPluginInfo('11365281228129299');
    console.log(pluginInfo)
    if (pluginInfo && pluginInfo.microAppConfig && pluginInfo.microAppConfig.entry) {
      console.log(pluginInfo.microAppConfig.entry)
      richTextEditorUrl.value = pluginInfo.microAppConfig.entry;
      return;
    }
  }
    console.log("进2")
  
  // 如果全局插件API不可用，尝试其他方式获取插件URL
  // 这里可以根据实际情况获取插件URL
  richTextEditorUrl.value = `${window.location.protocol}//${window.location.host}/plugins/11365281228129299/index.html`;
};

// 清空内容
const clearContent = () => {
  editorContent.value = '';
};

// 加载示例内容
const loadSampleContent = () => {
  editorContent.value = `
    <h2>示例内容</h2>
    <p>这是通过<em>插件API</em>加载的富文本编辑器插件。</p>
    <ul>
      <li>支持<strong>粗体</strong>、<em>斜体</em>、<u>下划线</u>等格式</li>
      <li>支持标题、列表等结构化内容</li>
      <li>支持撤销/重做操作</li>
    </ul>
    <p>这个插件可以被主程序和其他插件复用。</p>
  `;
};

// 监听来自富文本编辑器的消息
onMounted(() => {
  // 调用函数获取富文本编辑器插件URL
  getRichTextPluginUrl();
  
  window.addEventListener('message', (event) => {
    // 处理来自富文本编辑器插件的消息
    // 从URL中提取origin部分进行验证
    try {
      if (richTextEditorUrl.value) {
        const pluginOrigin = new URL(richTextEditorUrl.value).origin;
        if (event.origin === pluginOrigin) {
          if (event.data.type === 'richTextUpdate') {
            editorContent.value = event.data.content;
            console.log('从富文本编辑器接收到内容更新:', event.data.content);
          }
        }
      }
    } catch (e) {
      // 如果URL格式无效，跳过origin验证
      if (event.data.type === 'richTextUpdate') {
        editorContent.value = event.data.content;
        console.log('从富文本编辑器接收到内容更新:', event.data.content);
      }
    }
  });
});
</script>

<style scoped>
.rich-text-demo-container {
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

.editor-section {
  margin-bottom: 20px;
}

.editor-container {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
}

.content-preview {
  margin: 20px 0;
}

.preview-box {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 15px;
  min-height: 100px;
  background-color: #fafafa;
}

.content-actions {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}
</style>