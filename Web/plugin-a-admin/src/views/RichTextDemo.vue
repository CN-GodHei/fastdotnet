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
        通过Portal加载富文本插件
      </el-button>
    </div>

    <!-- 显示富文本内容 -->
    <div v-if="richTextContent" style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>富文本内容预览：</h3>
      <div v-html="richTextContent" style="border-top: 1px solid #eee; padding-top: 15px;"></div>
    </div>

    <!-- 通过PluginPortal嵌入富文本插件 -->
    <div v-if="showPluginPortal" style="margin-top: 30px; border: 1px solid #ddd; padding: 20px; border-radius: 4px;">
      <h3>通过PluginPortal加载的富文本编辑器：</h3>
      <div id="richtext-container" style="height: 400px;">
        <p>请在主应用中使用 PluginPortal 组件来加载富文本插件</p>
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
        <h4>来自富文本插件的消息：</h4>
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

// 富文本内容
const richTextContent = ref('');
const showPluginPortal = ref(false);
const messageToSend = ref('');
const receivedMessages = ref<string[]>([]);

// 插件属性
const pluginProps = ref({
  initialContent: richTextContent.value || '<p>初始内容</p>',
  onUpdateContent: (content: string) => {
    richTextContent.value = content;
  }
});

// 为了演示目的，创建一个模拟的插件通信对象
const pluginComm = {
  sendMessage: (toPlugin: string, action: string, payload: any) => {
    // 模拟发送消息
    console.log(`发送消息到 ${toPlugin}: ${action}`, payload);
    receivedMessages.value.push(`已发送消息给 ${toPlugin}: ${action}`);
    
    // 模拟一个响应
    setTimeout(() => {
      receivedMessages.value.push(`收到来自 ${toPlugin} 的响应: 消息已接收`);
    }, 1000);
  },
  getActivePlugins: () => [
    { id: 'FastdotnetRichText', name: '富文本编辑器', description: 'WangEditor富文本编辑器插件', version: '1.0.0', entry: '/plugins/FastdotnetRichText/index.html' },
    { id: 'PluginA', name: '演示插件A', description: '插件系统演示插件', version: '1.0.0', entry: '/micro/PluginA' }
  ]
};

// 用于插件间通信的事件监听器
const handleMessageEvent = (message: any) => {
  receivedMessages.value.push(`收到来自${message.fromPlugin}插件的消息: ${message.action} - ${JSON.stringify(message.payload)}`);
};

// 打开富文本编辑器 - 通过插件API调用富文本插件
const openRichTextEditor = async () => {
  try {
    // 检查富文本插件是否可用
    const plugins = pluginComm.getActivePlugins();
    const richTextPlugin = plugins.find((p: any) => p.id === 'FastdotnetRichText');
    
    if (!richTextPlugin) {
      ElMessage.error('富文本插件未找到或未启用');
      return;
    }
    
    // 发送消息给富文本插件，请求打开编辑器
    pluginComm.sendMessage('FastdotnetRichText', 'openEditor', {
      content: richTextContent.value || '<p>请输入内容...</p>',
      callbackId: 'editor_callback_' + Date.now()
    });
    
    ElMessage.success('已发送请求给富文本插件');
  } catch (error) {
    console.error('打开富文本编辑器失败:', error);
    ElMessage.error('打开富文本编辑器失败');
  }
};

// 通过PluginPortal加载富文本插件
const loadRichTextPluginViaPortal = () => {
  showPluginPortal.value = true;
};

// 发送消息给富文本插件
const sendMessageToRichTextPlugin = () => {
  if (!messageToSend.value.trim()) {
    ElMessage.warning('请输入要发送的消息');
    return;
  }

  // 通过插件API发送消息
  pluginComm.sendMessage('FastdotnetRichText', 'customMessage', {
    content: messageToSend.value,
    sender: 'PluginA',
    timestamp: Date.now()
  });
  
  // 记录发送的消息
  receivedMessages.value.push(`已发送消息给富文本插件: ${messageToSend.value}`);
  
  // 清空输入框
  messageToSend.value = '';
};

onMounted(() => {
  // 订阅来自其他插件的消息
  // 这里仅为演示，实际的订阅逻辑会更复杂
  console.log('插件通信已准备就绪');
});

onUnmounted(() => {
  // 取消订阅
  console.log('组件卸载，清理资源');
});
</script>

<style scoped>
.rich-text-demo {
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>