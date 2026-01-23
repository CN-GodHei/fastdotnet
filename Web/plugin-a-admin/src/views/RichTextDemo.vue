<template>
  <div class="rich-text-demo" style="padding: 20px;">
    <h1 style="font-size: 32px; color: #1890ff; margin-bottom: 20px;">富文本插件调用演示</h1>
    <p style="font-size: 16px; margin-bottom: 30px;">此页面演示如何在演示插件中调用富文本插件的功能</p>

    <!-- 调用富文本插件按钮 -->
    <div style="margin-bottom: 30px;">
      <el-button type="primary" size="large" @click="openRichTextEditor">
        打开富文本编辑器
      </el-button>
      <el-button @click="loadRichTextPluginViaPortal" style="margin-left: 10px;">
        通过iframe加载富文本插件
      </el-button>
    </div>

    <!-- 显示富文本内容 -->
    <div v-if="richTextContent" style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>富文本内容预览：</h3>
      <div v-html="richTextContent" style="border-top: 1px solid #eee; padding-top: 15px;"></div>
    </div>

    <!-- 通过iframe嵌入富文本插件 -->
    <div v-if="showPluginPortal" style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>通过iframe嵌入的富文本编辑器：</h3>
      <div id="richtext-container" style="height: 500px;">
        <iframe 
          :src="richTextPluginUrl" 
          width="100%" 
          height="100%" 
          frameborder="0" 
          ref="richTextFrameRef"
          @load="onRichTextFrameLoad"
        ></iframe>
      </div>
      <div style="margin-top: 10px;">
        <el-button @click="toggleFrameVisibility">{{ frameVisible ? '隐藏' : '显示' }}编辑器</el-button>
        <el-button @click="syncContentToFrame">同步内容到编辑器</el-button>
      </div>
    </div>

    <!-- 插件间通信演示 -->
    <div style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>插件间通信演示</h3>
      <el-input 
        v-model="messageToSend" 
        placeholder="输入要发送给富文本插件的消息" 
        style="margin-bottom: 10px;"
      />
      <el-button @click="sendMessageToRichTextPlugin">发送消息给富文本插件</el-button>
      
      <div v-if="receivedMessages.length > 0" style="margin-top: 20px;">
        <h4>来自其他插件的消息：</h4>
        <div v-for="(msg, index) in receivedMessages" :key="index" 
             style="padding: 10px; margin: 5px 0; background-color: #f0f0f0; border-radius: 4px;">
          {{ msg }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { ElButton, ElInput, ElMessage } from 'element-plus';

// 获取主应用提供的插件API
const pluginAPI = (window as any).__PLUGIN_API__;

// 检查插件API是否可用
if (!pluginAPI) {
  console.error('主应用未提供插件API，请确保在微前端环境中运行');
}

// 富文本内容
const richTextContent = ref('');
const showPluginPortal = ref(false);
const messageToSend = ref('');
const receivedMessages = ref<string[]>([]);

// iframe相关
const richTextPluginUrl = ref(''); // 富文本插件URL，从主应用获取
const frameVisible = ref(true);
const richTextFrameRef = ref<HTMLIFrameElement | null>(null);

// 从主应用获取富文本插件URL
if (pluginAPI) {
  const plugins = pluginAPI.getActivePlugins();
  const richTextPlugin = plugins.find((p: any) => p.id === '11365281228129299');
  console.log("日志",richTextPlugin)
  if (richTextPlugin) {
    richTextPluginUrl.value = richTextPlugin.entry || `/plugins/FastdotnetRichText/index.html`;
  } else {
    // 如果未找到插件配置，则使用默认URL
    richTextPluginUrl.value = '/plugins/FastdotnetRichText/index.html';
  }
} else {
  // 如果插件API不可用，则使用默认URL
  richTextPluginUrl.value = '/plugins/FastdotnetRichText/index.html';
}
console.log(richTextPluginUrl)
// 插件属性
const pluginProps = ref({
  initialContent: richTextContent.value || '<p>初始内容</p>',
  onUpdateContent: (content: string) => {
    richTextContent.value = content;
  }
});

// 用于插件间通信的事件监听器
const handleMessageEvent = (message: any) => {
  receivedMessages.value.push(`收到来自${message.fromPlugin}插件的消息: ${message.action} - ${JSON.stringify(message.payload)}`);
};

// 订阅来自其他插件的消息
let unsubscribe: (() => void) | null = null;

// 打开富文本编辑器 - 通过主应用API调用富文本插件
const openRichTextEditor = async () => {
  try {
    if (!pluginAPI) {
      ElMessage.error('插件API不可用');
      return;
    }
    
    // 检查富文本插件是否可用
    const plugins = pluginAPI.getActivePlugins();
    const richTextPlugin = plugins.find((p: any) => p.id === 'FastdotnetRichText');
    
    if (!richTextPlugin) {
      ElMessage.error('富文本插件未找到或未启用');
      return;
    }
    
    // 发送消息给富文本插件，请求打开编辑器
    pluginAPI.sendMessage({
      fromPlugin: 'PluginA',
      toPlugin: 'FastdotnetRichText',
      action: 'openEditor',
      payload: {
        content: richTextContent.value || '<p>请输入内容...</p>',
        callbackId: 'editor_callback_' + Date.now()
      },
      timestamp: Date.now()
    });
    
    ElMessage.success('已发送请求给富文本插件');
  } catch (error) {
    console.error('打开富文本编辑器失败:', error);
    ElMessage.error('打开富文本编辑器失败');
  }
};

// 通过iframe加载富文本插件
const loadRichTextPluginViaPortal = () => {
  showPluginPortal.value = true;
  ElMessage.success('富文本编辑器已嵌入到当前页面');
};

// 发送消息给富文本插件
const sendMessageToRichTextPlugin = () => {
  if (!messageToSend.value.trim()) {
    ElMessage.warning('请输入要发送的消息');
    return;
  }
  
  if (!pluginAPI) {
    ElMessage.error('插件API不可用');
    return;
  }

  // 通过插件API发送消息
  pluginAPI.sendMessage({
    fromPlugin: 'PluginA',
    toPlugin: 'FastdotnetRichText',
    action: 'customMessage',
    payload: {
      content: messageToSend.value,
      sender: 'PluginA',
      timestamp: Date.now()
    },
    timestamp: Date.now()
  });
  
  // 记录发送的消息
  receivedMessages.value.push(`已发送消息给富文本插件: ${messageToSend.value}`);
  
  // 清空输入框
  messageToSend.value = '';
};

// iframe相关方法
const onRichTextFrameLoad = () => {
  console.log('富文本编辑器iframe已加载');
  // 可以在这里进行一些初始化操作
};

const toggleFrameVisibility = () => {
  frameVisible.value = !frameVisible.value;
  
  if (richTextFrameRef.value && richTextFrameRef.value.contentWindow) {
    // 通过postMessage控制iframe内内容的可见性
    richTextFrameRef.value.contentWindow.postMessage(
      { type: 'setVisibility', visible: frameVisible.value }, 
      '*'  // 生产环境中应使用具体域名
    );
  }
};

const syncContentToFrame = () => {
  if (richTextFrameRef.value && richTextFrameRef.value.contentWindow) {
    // 通过postMessage同步内容到iframe
    richTextFrameRef.value.contentWindow.postMessage(
      { 
        type: 'setContent', 
        content: richTextContent.value || '<p>请输入内容...</p>' 
      }, 
      '*'  // 生产环境中应使用具体域名
    );
  }
};

onMounted(() => {
  if (pluginAPI) {
    // 订阅来自其他插件的消息
    unsubscribe = pluginAPI.subscribeToMessages('PluginA', handleMessageEvent);
  } else {
    console.warn('插件API不可用，无法订阅消息');
  }
});

onUnmounted(() => {
  // 取消订阅
  if (unsubscribe) {
    unsubscribe();
  }
});
</script>

<style scoped>
.rich-text-demo {
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>