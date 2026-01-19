<template>
  <div class="form-with-richtext-container">
    <h2>Plugin A - 表单中集成富文本编辑器</h2>
    <p>这个页面演示了如何在表单中集成富文本编辑器插件</p>
    
    <el-card class="form-card">
      <template #header>
        <div class="card-header">
          <span>内容发布表单</span>
        </div>
      </template>
      
      <el-form 
        :model="formModel" 
        :rules="formRules" 
        ref="formRef"
        label-width="100px"
      >
        <el-form-item label="标题" prop="title">
          <el-input 
            v-model="formModel.title" 
            placeholder="请输入标题"
          />
        </el-form-item>
        
        <el-form-item label="摘要" prop="summary">
          <el-input 
            v-model="formModel.summary" 
            type="textarea"
            :rows="3"
            placeholder="请输入摘要"
          />
        </el-form-item>
        
        <el-form-item label="内容" prop="content">
          <!-- 通过iframe嵌入富文本编辑器 -->
          <div class="editor-wrapper">
            <iframe 
              :src="richTextEditorUrl" 
              width="100%" 
              height="400"
              frameborder="0"
              ref="editorFrame"
              @load="onEditorLoad"
            ></iframe>
          </div>
        </el-form-item>
        
        <el-form-item label="标签">
          <el-select 
            v-model="formModel.tags" 
            multiple 
            placeholder="请选择标签"
            style="width: 100%"
          >
            <el-option
              v-for="tag in availableTags"
              :key="tag.value"
              :label="tag.label"
              :value="tag.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="submitForm">提交</el-button>
          <el-button @click="resetForm">重置</el-button>
          <el-button @click="previewContent" type="info">预览内容</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 内容预览弹窗 -->
    <el-dialog 
      v-model="previewVisible" 
      title="内容预览" 
      width="80%"
      top="5vh"
    >
      <div class="preview-content" v-html="formModel.content"></div>
      <template #footer>
        <el-button @click="previewVisible = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';

// 表单数据模型
const formModel = reactive({
  title: '',
  summary: '',
  content: '<p>请输入内容...</p>',
  tags: [] as string[]
});

// 可用标签选项
const availableTags = [
  { value: 'tech', label: '技术' },
  { value: 'news', label: '新闻' },
  { value: 'tutorial', label: '教程' },
  { value: 'review', label: '评测' }
];

// 表单验证规则
const formRules = {
  title: [
    { required: true, message: '请输入标题', trigger: 'blur' },
    { min: 3, max: 50, message: '长度在 3 到 50 个字符', trigger: 'blur' }
  ],
  content: [
    { required: true, message: '请输入内容', trigger: 'change' }
  ]
};

// 其他响应式数据
const formRef = ref();
const previewVisible = ref(false);
const richTextEditorUrl = ref(''); // 富文本编辑器插件URL
const editorFrame = ref<HTMLIFrameElement | null>(null);

// 从插件系统获取富文本编辑器插件的URL
const getRichTextPluginUrl = () => {
  // 从全局插件API获取富文本编辑器插件的入口地址
  if ((window as any).__PLUGIN_API__) {
    const pluginInfo = (window as any).__PLUGIN_API__.getPluginInfo('11365281228129299');
    if (pluginInfo && pluginInfo.microAppConfig && pluginInfo.microAppConfig.entry) {
      richTextEditorUrl.value = pluginInfo.microAppConfig.entry;
      return;
    }
  }
  
  // 如果全局插件API不可用，尝试其他方式获取插件URL
  richTextEditorUrl.value = `${window.location.protocol}//${window.location.host}/plugins/11365281228129299/index.html`;
};

// 初始化时获取插件URL
getRichTextPluginUrl();

// 当编辑器加载完成时
const onEditorLoad = () => {
  console.log('富文本编辑器已加载');
  // 发送初始内容到编辑器
  if (editorFrame.value?.contentWindow) {
    editorFrame.value.contentWindow.postMessage({
      type: 'setContent',
      content: formModel.content
    }, '*');
  }
};

// 监听来自富文本编辑器的消息
window.addEventListener('message', (event) => {
  // 从URL中提取origin部分进行验证
  try {
    if (richTextEditorUrl.value) {
      const pluginOrigin = new URL(richTextEditorUrl.value).origin;
      if (event.origin === pluginOrigin) {
        if (event.data.type === 'contentChanged') {
          formModel.content = event.data.content;
          console.log('内容已更新:', event.data.content);
        }
      }
    }
  } catch (e) {
    // 如果URL格式无效，跳过origin验证
    if (event.data.type === 'contentChanged') {
      formModel.content = event.data.content;
      console.log('内容已更新:', event.data.content);
    }
  }
});

// 提交表单
const submitForm = async () => {
  if (!formRef.value) return;
  
  try {
    await formRef.value.validate();
    console.log('提交表单:', formModel);
    ElMessage.success('内容提交成功！');
  } catch (error) {
    console.error('表单验证失败:', error);
    ElMessage.error('请检查表单内容');
  }
};

// 重置表单
const resetForm = () => {
  if (!formRef.value) return;
  formRef.value.resetFields();
  formModel.content = '<p>请输入内容...</p>';
};

// 预览内容
const previewContent = () => {
  if (!formModel.content || formModel.content.trim() === '') {
    ElMessage.warning('请先输入内容');
    return;
  }
  previewVisible.value = true;
};
</script>

<style scoped>
.form-with-richtext-container {
  padding: 20px;
}

.form-card {
  max-width: 1000px;
  margin: 20px auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.editor-wrapper {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
  min-height: 400px;
}

.preview-content {
  min-height: 300px;
  border: 1px solid #eee;
  padding: 15px;
  border-radius: 4px;
  background-color: #fafafa;
}
</style>