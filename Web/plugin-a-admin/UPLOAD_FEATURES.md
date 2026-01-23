# 演示插件调用富文本插件实现方案

## 概述

本文档说明了如何在演示插件（Plugin-A）中调用富文本插件（Fastdotnet.RichText），实现跨插件通信和协作。

## 技术架构

### 1. 微前端架构
- 主应用使用 qiankun 框架管理微前端插件
- 每个插件独立运行在沙箱环境中
- 通过主应用提供的 PluginAPI 实现插件间通信

### 2. 插件间通信机制
- 消息传递：通过 PluginAPI.sendMessage() 发送消息
- 服务调用：通过 PluginAPI.callService() 调用服务
- 共享数据：通过 PluginAPI.setSharedData()/getSharedData() 共享数据

## 实现步骤

### 1. 富文本插件改造

需要在富文本插件中添加消息监听和响应逻辑：

```typescript
// 在富文本插件的 main.ts 或 micro-main.ts 中添加
import { pluginAPI } from '@/plugins/PluginAPI';

// 订阅来自其他插件的消息
const unsubscribe = pluginAPI.subscribeToMessages('FastdotnetRichText', (message) => {
  switch(message.action) {
    case 'openEditor':
      // 打开富文本编辑器
      openEditor(message.payload.content);
      break;
    case 'setContent':
      // 设置编辑器内容
      setEditorContent(message.payload.content);
      break;
    case 'getContent':
      // 获取编辑器内容并回复
      const content = getEditorContent();
      pluginAPI.sendMessage({
        fromPlugin: 'FastdotnetRichText',
        toPlugin: message.fromPlugin,
        action: 'contentResponse',
        payload: { content, callbackId: message.payload.callbackId }
      });
      break;
    default:
      console.log('未知的消息类型:', message.action);
  }
});

// 组件卸载时取消订阅
// unsubscribe(); // 在适当的时候调用
```

### 2. 演示插件实现

在演示插件中，我们已经创建了 RichTextDemo.vue 页面，用于演示跨插件调用：

```vue
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
```

### 3. 主应用配置

主应用需要正确配置插件注册和通信机制：

```typescript
// 在主应用的 PluginRegistry 中注册插件
pluginRegistry.registerPlugin({
  id: 'FastdotnetRichText',
  name: '富文本编辑器',
  description: '基于WangEditor的富文本编辑器微应用插件',
  version: '1.0.0',
  author: 'Fastdotnet Team',
  enabled: true,
  entryPoint: 'index.html',
  microAppConfig: {
    name: 'FastdotnetRichText',
    entry: '/plugins/FastdotnetRichText/index.html',
    container: '#subapp-viewport',
    activeRule: '/micro/FastdotnetRichText'
  }
});
```

## 使用场景

### 场景1：在表单中嵌入富文本编辑器

1. 演示插件发送消息请求打开富文本编辑器
2. 富文本插件响应并显示编辑器界面
3. 用户在富文本编辑器中输入内容
4. 内容变化时实时同步回演示插件

### 场景2：内容预览和编辑

1. 演示插件将现有内容发送给富文本插件
2. 富文本插件加载并显示内容
3. 用户编辑后，富文本插件将更新内容发回演示插件

### 场景3：批量内容处理

1. 演示插件调用富文本插件的服务进行内容处理
2. 富文本插件执行处理并返回结果

## 实现细节

### 1. 消息协议

定义了一套标准的消息协议用于插件间通信：

```typescript
interface PluginMessage {
  fromPlugin: string;      // 发送方插件ID
  toPlugin?: string;       // 接收方插件ID（可选，若为空则广播）
  action: string;          // 操作类型
  payload: any;            // 消息负载
  timestamp: number;       // 时间戳
  messageId?: string;      // 消息ID
}
```

### 2. 标准操作类型

- `openEditor`: 打开富文本编辑器
- `setContent`: 设置编辑器内容
- `getContent`: 获取编辑器内容
- `updateContent`: 更新编辑器内容
- `saveContent`: 保存编辑器内容

### 3. 错误处理

- 插件不存在时的错误处理
- 通信超时处理
- 数据格式验证

## 部署说明

1. 确保主应用的 PluginAPI 已正确初始化
2. 确保富文本插件已正确注册到插件系统
3. 确保演示插件可以访问主应用的共享API
4. 验证跨域通信配置（如有必要）

## 注意事项

1. 插件间通信需要通过主应用中转，确保主应用的 PluginAPI 可用
2. 每个插件需要独立处理自己的状态和生命周期
3. 通信数据需要序列化，注意复杂对象的处理
4. 插件卸载时需要清理订阅和资源

## 测试验证

1. 验证消息发送和接收功能
2. 验证跨插件数据传输
3. 验证错误处理机制
4. 验证插件生命周期管理