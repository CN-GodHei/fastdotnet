<template>
  <div class="rich-text-demo" style="padding: 20px;">
    <h1 style="font-size: 32px; color: #1890ff; margin-bottom: 20px;">富文本编辑器演示</h1>
    <p style="font-size: 16px; margin-bottom: 30px;">演示如何在页面中嵌入富文本编辑器</p>

    <!-- 显示富文本内容 -->
    <div v-if="richTextContent" style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>富文本内容预览：</h3>
      <div v-html="richTextContent" style="border-top: 1px solid #eee; padding-top: 15px;"></div>
    </div>

    <!-- 操作按钮 -->
    <div style="margin-top: 30px;">
      <el-button type="primary" @click="openRichTextEditorDialog">打开富文本编辑器</el-button>
    </div>

    <!-- 富文本编辑器对话框 -->
    <el-dialog
      v-model="richTextDialogVisible"
      title="富文本编辑器"
      width="80%"
      top="5vh"
      destroy-on-close
    >
      <div id="richtext-container" style="height: 500px; border: 1px solid #ccc;"></div>
      <template #footer>
        <el-button @click="closeRichTextEditorDialog">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue';
import { ElButton, ElMessage, ElDialog } from 'element-plus';
import { loadMicroApp } from 'qiankun';

// 获取主应用提供的插件API
const pluginAPI = (window as any).__PLUGIN_API__;

// 检查插件API是否可用
if (!pluginAPI) {
  console.error('主应用未提供插件API，请确保在微前端环境中运行');
}

// 富文本内容
const richTextContent = ref('');

// 对话框显示控制
const richTextDialogVisible = ref(false);

// 确保初始化时对话框是隐藏的
if (richTextDialogVisible.value) {
  richTextDialogVisible.value = false;
}

// 用于管理微应用实例
let microAppInstance: any = null;

// 从主应用获取富文本插件URL
let richTextPluginUrl = ''; // 默认URL

if (pluginAPI) {
  const plugins = pluginAPI.getActivePlugins();
  // 使用实际的插件ID
  let richTextPlugin = plugins.find((p: any) => p.id === '11365281228129299');
  if (richTextPlugin) {
    richTextPluginUrl = richTextPlugin.entry;
  }
}

// 打开富文本编辑器对话框
const openRichTextEditorDialog = async () => {
  richTextDialogVisible.value = true;  
  // 等待对话框DOM完全渲染
  await nextTick();
  loadRichTextMicroApp();
};

// 关闭富文本编辑器对话框
const closeRichTextEditorDialog = () => {
  richTextDialogVisible.value = false;
  
  // 卸载富文本微应用
  if (microAppInstance) {
    microAppInstance.unmount();
    microAppInstance = null;
  }
};

// 加载富文本微应用
const loadRichTextMicroApp = async () => {
  const container = document.getElementById('richtext-container');
  if (!container) {
    ElMessage.error('找不到富文本编辑器容器');
    return;
  }
  
  try {
    // 从当前插件获取上传服务
    const { getSharedUploadService } = await import('@/main');
    const uploadService = getSharedUploadService();
    console.log("演示插件拿到的上传服务",uploadService)
    
    // 使用loadMicroApp加载富文本编辑器微应用
    microAppInstance = await loadMicroApp({
      name: 'FastdotnetRichText',
      entry: richTextPluginUrl,
      container: container,
      props: {
        initialContent: richTextContent.value || '<p>请输入内容...</p>',
        showToolbar: true,
        uploadService: uploadService, // 传递上传服务
        onChange: (content: string) => {
          richTextContent.value = content;
          console.log('富文本内容已更新:', content);
        },
        // onReady 回调用于确认微应用已准备就绪
        onReady: () => {
          console.log('富文本编辑器已准备就绪');
        }
      }
    });
    
    ElMessage.success('富文本编辑器已加载');
  } catch (error) {
    console.error('加载富文本编辑器失败:', error);
    ElMessage.error('加载富文本编辑器失败');
  }
};

</script>

<style scoped>
.rich-text-demo {
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>