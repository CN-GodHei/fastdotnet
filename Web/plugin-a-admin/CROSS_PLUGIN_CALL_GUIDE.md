# 演示插件调用富文本插件实现指南

## 概述

本文档介绍了如何在演示插件（Plugin-A）中调用富文本插件（Fastdotnet.RichText），实现跨插件通信与协作。

## 架构说明

### 微前端架构
- 主应用（fastdotnet-admin）作为基座，负责插件管理和通信协调
- 演示插件和富文本插件作为独立的微应用运行
- 使用 qiankun 框架实现微前端功能

### 通信机制
- 主应用提供统一的 PluginAPI 供插件使用
- 插件通过主应用进行间接通信
- 支持消息传递、服务调用等多种通信方式

## 实现细节

### 1. 演示插件中的实现

在 [RichTextDemo.vue](file://d:\WorkSpace\Code\gitee\开源项目开发\Fastdotnet\Web\plugin-a-admin\src\views\RichTextDemo.vue) 中实现了以下功能：

#### 访问主应用插件API
```typescript
// 获取主应用提供的插件API
const pluginAPI = (window as any).__PLUGIN_API__;

// 检查插件API是否可用
if (!pluginAPI) {
  console.error('主应用未提供插件API，请确保在微前端环境中运行');
}
```

#### 发送消息给富文本插件
```typescript
// 发送消息给富文本插件
pluginAPI.sendMessage({
  fromPlugin: 'PluginA',           // 发送方插件ID
  toPlugin: 'FastdotnetRichText',  // 接收方插件ID
  action: 'openEditor',            // 操作类型
  payload: {                       // 消息负载
    content: richTextContent.value || '<p>请输入内容...</p>',
    callbackId: 'editor_callback_' + Date.now()
  },
  timestamp: Date.now()
});
```

#### 请求主应用加载插件
```typescript
// 请求主应用加载富文本插件
pluginAPI.sendMessage({
  fromPlugin: 'PluginA',
  toPlugin: 'MainApp',             // 发送给主应用
  action: 'requestLoadPlugin',
  payload: { 
    pluginId: 'FastdotnetRichText', 
    targetContainer: '#richtext-container' 
  },
  timestamp: Date.now()
});
```

#### 订阅消息
```typescript
// 订阅来自其他插件的消息
const unsubscribe = pluginAPI.subscribeToMessages('PluginA', (message) => {
  console.log('收到消息:', message);
});
```

### 2. 通信协议

定义了标准化的插件间通信协议：

```typescript
interface PluginMessage {
  fromPlugin: string;      // 发送方插件ID
  toPlugin?: string;       // 接收方插件ID
  action: string;          // 操作类型
  payload: any;            // 消息负载
  timestamp: number;       // 时间戳
}
```

## 使用场景

### 场景1：表单中集成富文本编辑器
- 演示插件发送消息请求加载富文本编辑器
- 主应用响应请求并加载富文本插件
- 用户在富文本编辑器中编辑内容
- 内容变化时通过主应用同步回演示插件

### 场景2：内容管理系统
- 演示插件作为内容管理界面
- 富文本插件提供编辑功能
- 通过主应用协调两者的交互

## 部署说明

### 1. 演示插件
- 构建演示插件为独立的微前端应用
- 部署到主应用可访问的位置
- 在主应用的插件注册表中注册

### 2. 富文本插件
- 构建富文本插件为独立的微前端应用
- 部署到主应用可访问的位置
- 在主应用的插件注册表中注册

### 3. 主应用
- 确保主应用正确暴露 `__PLUGIN_API__`
- 配置插件注册表，包含演示插件和富文本插件的信息

## iframe嵌入方式（推荐）

考虑到用户体验，我们还实现了通过iframe直接嵌入富文本编辑器的方式：

```vue
<template>
  <div>
    <iframe 
      :src="richTextPluginUrl" 
      width="100%" 
      height="500px" 
      frameborder="0" 
      ref="richTextFrameRef"
      @load="onRichTextFrameLoad"
    ></iframe>
  </div>
</template>
```

其中，`richTextPluginUrl` 从主应用获取富文本插件的配置：

```typescript
// 从主应用获取富文本插件URL
if (pluginAPI) {
  const plugins = pluginAPI.getActivePlugins();
  // 使用实际的插件ID
  let richTextPlugin = plugins.find((p: any) => p.id === '11365281228129299');
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
```

## iframe通信机制

我们实现了双向通信机制：

1. **父页面向iframe发送消息**：
```typescript
// 同步内容到iframe
if (richTextFrameRef.value && richTextFrameRef.value.contentWindow) {
  richTextFrameRef.value.contentWindow.postMessage(
    { 
      type: 'setContent', 
      content: richTextContent.value || '<p>请输入内容...</p>' 
    }, 
    '*'  // 生产环境中应使用具体域名
  );
}
```

2. **iframe向父页面发送消息**：
```typescript
// 在富文本插件中监听内容变化
editorInstance.config.onchange = function (newHtml: string) {
  // 通知父窗口内容已更改
  if (window.parent && window.parent !== window) {
    window.parent.postMessage({
      type: 'contentChanged',
      content: newHtml
    }, '*');
  }
};
```

3. **父页面接收iframe消息**：
```typescript
// 监听来自富文本编辑器的消息
const handleIframeMessage = (event: MessageEvent) => {
  const { type, content } = event.data || {};
  
  if (type === 'contentChanged' && content !== undefined) {
    // 更新本地的富文本内容
    richTextContent.value = content;
    console.log('收到富文本编辑器内容更新:', content);
  }
};

// 添加和移除事件监听器
onMounted(() => {
  window.addEventListener('message', handleIframeMessage);
});
onUnmounted(() => {
  window.removeEventListener('message', handleIframeMessage);
});
```

这种方式可以让用户在当前页面直接使用富文本编辑器，无需跳转到主应用页面，同时确保了URL的准确性和一致性，并实现了内容的双向同步。

## 注意事项

### 1. 微前端限制
- 子应用（演示插件）无法直接加载其他子应用（富文本插件）
- 必须通过主应用作为中介进行加载和通信
- 所有插件通信需经由主应用协调

### 2. 安全考虑
- 验证消息来源，防止恶意插件注入
- 对共享数据进行验证和过滤
- 实施权限控制，限制敏感操作

### 3. 错误处理
- 检查插件API是否可用
- 处理插件不存在的情况
- 实现通信超时处理

## API参考

### PluginAPI 接口
- `sendMessage(message)` - 发送消息
- `subscribeToMessages(pluginId, handler)` - 订阅消息
- `callService(serviceName, ...args)` - 调用服务
- `registerService(service)` - 注册服务
- `getActivePlugins()` - 获取激活的插件列表

### 标准操作类型
- `openEditor` - 打开编辑器
- `setContent` - 设置内容
- `getContent` - 获取内容
- `updateContent` - 更新内容
- `requestLoadPlugin` - 请求加载插件

## 总结

通过主应用作为中介，演示插件可以与富文本插件进行有效的通信和协作。这种架构既保证了微前端的安全隔离，又实现了插件间的协作功能，为构建复杂的插件化应用提供了可行的方案。