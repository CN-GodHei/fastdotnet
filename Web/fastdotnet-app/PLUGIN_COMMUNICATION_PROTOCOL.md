# Fastdotnet 插件通信协议

## 概述

本文档定义了 Fastdotnet 框架中插件间通信的标准协议。该协议基于主应用提供的 PluginAPI，确保各插件之间能够安全、高效地进行通信和数据交换。

## 通信架构

### 1. 主应用角色
- 提供统一的 PluginAPI 供所有插件使用
- 实现事件总线机制，协调插件间通信
- 管理插件生命周期和状态

### 2. 插件角色
- 通过标准化接口与其它插件通信
- 遵循消息协议格式
- 独立处理自身业务逻辑

## 通信协议定义

### 1. 消息格式

```typescript
interface PluginAPIMessage {
  fromPlugin: string;      // 发送方插件ID
  toPlugin?: string;       // 接收方插件ID，为空时表示广播
  action: string;          // 操作类型
  payload: any;            // 消息负载
  timestamp: number;       // 时间戳
  messageId?: string;      // 消息唯一标识
}
```

### 2. 标准操作类型

#### 通用操作
- `ping`: 心跳检测，确认插件可用性
- `getStatus`: 获取插件状态
- `registerService`: 注册服务
- `callService`: 调用服务

#### 富文本插件专用操作
- `openEditor`: 打开富文本编辑器
- `setContent`: 设置编辑器内容
- `getContent`: 获取编辑器内容
- `updateContent`: 更新编辑器内容
- `saveContent`: 保存编辑器内容
- `insertImage`: 插入图片
- `insertVideo`: 插入视频

#### 文件上传操作
- `uploadFile`: 上传文件
- `getUploadProgress`: 获取上传进度
- `cancelUpload`: 取消上传

## API 接口定义

### 1. 消息发送

```typescript
// 发送点对点消息
pluginAPI.sendMessage({
  fromPlugin: 'PluginA',
  toPlugin: 'FastdotnetRichText',
  action: 'openEditor',
  payload: { content: '<p>Hello World</p>' },
  timestamp: Date.now()
});

// 广播消息
pluginAPI.sendMessage({
  fromPlugin: 'PluginA',
  action: 'dataChanged',
  payload: { newData: '...' },
  timestamp: Date.now()
});
```

### 2. 消息订阅

```typescript
// 订阅特定插件消息
const unsubscribe = pluginAPI.subscribeToMessages('FastdotnetRichText', (message) => {
  console.log('收到消息:', message);
});

// 使用后记得取消订阅
// unsubscribe();
```

### 3. 服务注册与调用

```typescript
// 注册服务
pluginAPI.registerService({
  name: 'textProcessor',
  provider: 'FastdotnetRichText',
  execute: async (content) => {
    // 处理文本内容
    return processedContent;
  }
});

// 调用服务
const result = await pluginAPI.callService('textProcessor', content);
```

## 使用示例

### 1. 演示插件调用富文本插件

```typescript
// 在演示插件中
import { pluginAPI } from '@/plugins/PluginAPI';

// 检查富文本插件是否可用
if (pluginAPI.isPluginAvailable('FastdotnetRichText')) {
  // 发送消息打开编辑器
  pluginAPI.sendMessage({
    fromPlugin: 'PluginA',
    toPlugin: 'FastdotnetRichText',
    action: 'openEditor',
    payload: {
      content: initialContent,
      options: { readOnly: false }
    },
    timestamp: Date.now()
  });
  
  // 订阅响应
  const unsubscribe = pluginAPI.subscribeToMessages('FastdotnetRichText', (message) => {
    if (message.action === 'editorOpened') {
      console.log('编辑器已打开');
    } else if (message.action === 'contentUpdated') {
      console.log('内容已更新:', message.payload.content);
    }
  });
}
```

### 2. 富文本插件响应消息

```typescript
// 在富文本插件中
import { pluginAPI } from '@/plugins/PluginAPI';

// 订阅来自其他插件的消息
const unsubscribe = pluginAPI.subscribeToMessages('FastdotnetRichText', (message) => {
  switch (message.action) {
    case 'openEditor':
      openEditor(message.payload.content);
      // 回复确认消息
      pluginAPI.sendMessage({
        fromPlugin: 'FastdotnetRichText',
        toPlugin: message.fromPlugin,
        action: 'editorOpened',
        payload: { success: true },
        timestamp: Date.now()
      });
      break;
      
    case 'setContent':
      setEditorContent(message.payload.content);
      break;
      
    case 'getContent':
      const content = getEditorContent();
      pluginAPI.sendMessage({
        fromPlugin: 'FastdotnetRichText',
        toPlugin: message.fromPlugin,
        action: 'contentResponse',
        payload: { content, requestId: message.payload.requestId },
        timestamp: Date.now()
      });
      break;
      
    default:
      console.warn('未知的消息类型:', message.action);
  }
});
```

## 数据共享机制

### 1. 共享数据 API

```typescript
// 设置共享数据
pluginAPI.setSharedData('key', data, 'PluginId');

// 获取自己插件的共享数据
const data = pluginAPI.getSharedData('key', 'PluginId');

// 获取其他插件的共享数据
const otherData = pluginAPI.getOtherPluginSharedData('key', 'OtherPluginId');
```

### 2. 数据共享最佳实践

- 重要数据应加密后再共享
- 设置合适的数据过期时间
- 避免共享过多数据影响性能

## 错误处理

### 1. 通信错误类型

- `PluginNotAvailable`: 目标插件不可用
- `MessageTimeout`: 消息超时
- `InvalidPayload`: 无效的消息负载
- `ServiceNotFound`: 服务未找到

### 2. 错误处理策略

```typescript
try {
  const result = await pluginAPI.callService('someService', param);
} catch (error) {
  if (error.message.includes('PluginNotAvailable')) {
    console.error('插件不可用');
  } else if (error.message.includes('ServiceNotFound')) {
    console.error('服务未找到');
  } else {
    console.error('通信错误:', error);
  }
}
```

## 安全考虑

1. 验证消息来源，防止恶意插件注入
2. 对共享数据进行验证和过滤
3. 实施权限控制，限制敏感操作
4. 避免在消息中传输敏感信息

## 性能优化

1. 批量发送小消息以减少通信开销
2. 使用缓存机制减少重复通信
3. 合理设置消息优先级
4. 实现消息压缩机制

## 调试和监控

1. 提供详细的通信日志
2. 实现消息追踪机制
3. 监控通信性能指标
4. 提供可视化调试工具

## 总结

该通信协议为 Fastdotnet 插件系统提供了标准化的通信机制，确保了插件间的安全、高效协作。通过遵循此协议，开发者可以轻松实现插件间的复杂交互，构建功能丰富的插件化应用。

## 插件间协调模式

特别地，该协议支持主应用作为协调者，实现子应用间的间接通信：

1. **中介通信模式**：子应用无法直接与其他子应用通信，必须通过主应用中转
2. **插件加载协调**：主应用使用 PluginPortal 组件动态加载子应用
3. **消息路由机制**：主应用根据消息目标路由到对应插件
4. **状态同步机制**：主应用协调插件间的状态同步

这种设计既保证了微前端架构的安全隔离，又实现了插件间的协作功能。

## iframe通信模式

对于需要在同一页面中直接显示其他插件的场景，可以使用iframe嵌入方式：

1. **iframe嵌入**：通过iframe标签嵌入其他插件
2. **postMessage通信**：使用window.postMessage进行跨窗口通信
3. **URL配置**：通过主应用的插件注册表获取正确的插件URL
4. **双向数据同步**：实现父页面与iframe之间的数据同步

这种方式允许在一个页面中集成多个插件的功能，提升用户体验。

### iframe通信实现要点

1. **消息类型定义**：
   - `setContent`: 设置iframe内内容
   - `contentChanged`: iframe内容变更通知
   - `getContent`: iframe主动发送内容
   - `setVisibility`: 控制iframe可见性

2. **安全注意事项**：
   - 在生产环境中应验证消息来源（origin）
   - 避免使用通配符`'*'`作为目标origin
   - 对消息内容进行验证和过滤

3. **生命周期管理**：
   - 在组件挂载时添加message监听器
   - 在组件卸载时移除message监听器