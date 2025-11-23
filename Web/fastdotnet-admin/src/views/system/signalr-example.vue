<template>
  <div class="signalr-example">
    <h2>SignalR集成示例</h2>
    
    <div class="section">
      <h3>连接状态</h3>
      <p>状态: {{ connectionState }}</p>
      <button @click="connect" :disabled="isConnected">连接</button>
      <button @click="disconnect" :disabled="!isConnected">断开连接</button>
    </div>

    <div class="section">
      <h3>生成登录二维码</h3>
      <button @click="generateQRCode" :disabled="!isConnected">生成二维码</button>
      <p v-if="qrCode">二维码: {{ qrCode }}</p>
    </div>

    <div class="section">
      <h3>发送消息</h3>
      <input v-model="message" placeholder="输入消息" />
      <button @click="sendMessage" :disabled="!isConnected">发送</button>
      <div class="messages">
        <h4>收到的消息:</h4>
        <ul>
          <li v-for="(msg, index) in messages" :key="index">{{ msg }}</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { baseSignalRManager } from '@/utils/signalr';

// 响应式数据
const connectionState = ref('Disconnected');
const isConnected = ref(false);
const qrCode = ref('');
const message = ref('');
const messages = ref<string[]>([]);

// 更新连接状态
const updateConnectionState = () => {
  const state = baseSignalRManager.getConnectionState();
  connectionState.value = state;
  isConnected.value = state === 'Connected';
};

// 连接SignalR
const connect = async () => {
  try {
    await baseSignalRManager.initialize();
    updateConnectionState();
    
    // 注册事件监听器
    baseSignalRManager.on('loginConfirmed', () => {
      messages.value.push('登录已确认');
    });
    
    //console.log('[SignalR Example] SignalR连接成功');
  } catch (error) {
    console.error('[SignalR Example] SignalR连接失败:', error);
  }
};

// 断开连接
const disconnect = async () => {
  try {
    // 移除事件监听器
    baseSignalRManager.off('loginConfirmed', () => {});
    
    // 这里需要实现断开连接的逻辑
    // 由于SignalR库的限制，我们暂时不实现断开连接的功能
    //console.log('[SignalR Example] SignalR断开连接');
  } catch (error) {
    console.error('[SignalR Example] SignalR断开连接失败:', error);
  }
};

// 生成二维码
const generateQRCode = async () => {
  try {
    const code = await baseSignalRManager.invokeHubMethod('UniversalHub', 'GenerateLoginQRCode');
    qrCode.value = code;
    //console.log('[SignalR Example] 生成二维码:', code);
  } catch (error) {
    console.error('[SignalR Example] 生成二维码失败:', error);
  }
};

// 发送消息
const sendMessage = async () => {
  if (!message.value) return;
  
  try {
    // 这里需要根据实际的Hub方法来调用
    // await baseSignalRManager.invokeHubMethod('UniversalHub', 'SendMessage', message.value);
    messages.value.push(`发送消息: ${message.value}`);
    message.value = '';
    //console.log('[SignalR Example] 发送消息成功');
  } catch (error) {
    console.error('[SignalR Example] 发送消息失败:', error);
  }
};

// 组件挂载时的操作
onMounted(() => {
  // 监听连接状态变化
  const interval = setInterval(updateConnectionState, 1000);
  
  // 清理定时器
  onUnmounted(() => {
    clearInterval(interval);
  });
  
  //console.log('[SignalR Example] 组件已挂载');
});

// 组件卸载时的操作
onUnmounted(() => {
  // 清理资源
  //console.log('[SignalR Example] 组件已卸载');
});
</script>

<style scoped>
.signalr-example {
  padding: 20px;
}

.section {
  margin-bottom: 20px;
}

.section h3 {
  margin-top: 0;
}

.messages {
  margin-top: 10px;
}

.messages ul {
  list-style-type: none;
  padding: 0;
}

.messages li {
  background: #f0f0f0;
  margin: 5px 0;
  padding: 5px;
  border-radius: 3px;
}
</style>