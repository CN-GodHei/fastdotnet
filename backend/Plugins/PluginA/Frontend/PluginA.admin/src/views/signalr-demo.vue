<template>
  <div class="plugin-a-signalr-demo">
    <h2>Plugin A SignalR Demo</h2>
    
    <div class="section">
      <h3>发送通知</h3>
      <input v-model="notificationMessage" placeholder="输入通知消息" />
      <button @click="sendNotification" :disabled="!isConnected">发送通知</button>
    </div>

    <div class="section">
      <h3>房间功能</h3>
      <div class="room-controls">
        <input v-model="roomName" placeholder="房间名称" />
        <button @click="joinRoom" :disabled="!isConnected">加入房间</button>
        <button @click="leaveRoom" :disabled="!isConnected">离开房间</button>
      </div>
      <div class="room-message-controls">
        <input v-model="roomMessage" placeholder="房间消息" />
        <button @click="sendRoomMessage" :disabled="!isConnected || !roomName">发送房间消息</button>
      </div>
    </div>

    <div class="section">
      <h3>收到的通知</h3>
      <div class="notifications">
        <ul>
          <li v-for="(notification, index) in notifications" :key="index">
            <strong>{{ notification.pluginId }}:</strong> {{ notification.message }} 
            <span class="timestamp">({{ notification.timestamp }})</span>
          </li>
        </ul>
      </div>
    </div>

    <div class="section">
      <h3>房间消息</h3>
      <div class="room-messages">
        <ul>
          <li v-for="(message, index) in roomMessages" :key="index">
            <strong>{{ message.roomName }}:</strong> {{ message.message }} 
            <span class="sender">by {{ message.sender }}</span>
            <span class="timestamp">({{ message.timestamp }})</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { PluginASignalRManager } from '../utils/plugin-a-signalr-manager';

// 响应式数据
const notificationMessage = ref('');
const roomName = ref('');
const roomMessage = ref('');
const notifications = ref<any[]>([]);
const roomMessages = ref<any[]>([]);
const isConnected = ref(false);

// SignalR管理器
let signalRManager: PluginASignalRManager | null = null;

// 发送通知
const sendNotification = async () => {
  if (!notificationMessage.value) return;
  
  try {
    await signalRManager?.sendMessage({
      message: notificationMessage.value,
      timestamp: new Date().toISOString()
    });
    
    notificationMessage.value = '';
  } catch (error) {
    console.error('[PluginA SignalR Demo] 发送通知失败:', error);
  }
};

// 加入房间
const joinRoom = async () => {
  if (!roomName.value || !signalRManager) return;
  
  try {
    await signalRManager.baseSignalRManager.invokeHubMethod("UniversalHub", "JoinPluginRoom", "PluginA", roomName.value);
    //console.log(`[PluginA SignalR Demo] 已加入房间: ${roomName.value}`);
  } catch (error) {
    console.error('[PluginA SignalR Demo] 加入房间失败:', error);
  }
};

// 离开房间
const leaveRoom = async () => {
  if (!roomName.value || !signalRManager) return;
  
  try {
    await signalRManager.baseSignalRManager.invokeHubMethod("UniversalHub", "LeavePluginRoom", "PluginA", roomName.value);
    //console.log(`[PluginA SignalR Demo] 已离开房间: ${roomName.value}`);
  } catch (error) {
    console.error('[PluginA SignalR Demo] 离开房间失败:', error);
  }
};

// 发送房间消息
const sendRoomMessage = async () => {
  if (!roomMessage.value || !roomName.value || !signalRManager) return;
  
  try {
    await signalRManager.baseSignalRManager.invokeHubMethod("UniversalHub", "SendToPluginRoom", "PluginA", roomName.value, roomMessage.value);
    roomMessage.value = '';
    //console.log(`[PluginA SignalR Demo] 已发送房间消息到: ${roomName.value}`);
  } catch (error) {
    console.error('[PluginA SignalR Demo] 发送房间消息失败:', error);
  }
};

// 组件挂载时的操作
onMounted(async () => {
  try {
    // 插件完全自主初始化SignalR
    signalRManager = new PluginASignalRManager();
    await signalRManager.initialize();
    
    isConnected.value = true;
    //console.log('[PluginA SignalR Demo] 插件SignalR已初始化并连接');
  } catch (error) {
    console.error('[PluginA SignalR Demo] 初始化失败:', error);
  }
});

// 组件卸载时的操作
onUnmounted(() => {
  // 插件自主清理资源
  if (signalRManager) {
    signalRManager.cleanup();
    signalRManager = null;
  }
  
  isConnected.value = false;
  //console.log('[PluginA SignalR Demo] 插件SignalR已清理');
});
</script>

<style scoped>
.plugin-a-signalr-demo {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.section {
  margin-bottom: 30px;
  padding: 20px;
  border: 1px solid #e0e0e0;
  border-radius: 5px;
}

.section h3 {
  margin-top: 0;
  color: #333;
}

.room-controls, .room-message-controls {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
  align-items: center;
}

.room-controls input, .room-message-controls input {
  flex: 1;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 3px;
}

.room-controls button, .room-message-controls button {
  padding: 8px 15px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 3px;
  cursor: pointer;
}

.room-controls button:disabled, .room-message-controls button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.notifications ul, .room-messages ul {
  list-style-type: none;
  padding: 0;
}

.notifications li, .room-messages li {
  background: #f8f9fa;
  margin: 5px 0;
  padding: 10px;
  border-radius: 3px;
  border-left: 3px solid #007bff;
}

.timestamp {
  color: #6c757d;
  font-size: 0.9em;
}

.sender {
  color: #28a745;
  font-weight: bold;
}
</style>