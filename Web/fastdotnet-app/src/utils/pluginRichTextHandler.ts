/**
 * Plugin富文本编辑器处理器
 * 用于处理来自插件的富文本编辑器调用请求
 */

import { sendRichTextEvent, RichTextEventType, listenRichTextEvent } from '@/utils/richTextCommunication';
import RichTextEditorModal from '@/components/RichTextEditorModal.vue';
import { createApp, ref, h } from 'vue';

// 当前打开的富文本编辑器实例
let currentRichTextModal: any = null;
let currentResolve: ((value: string) => void) | null = null;

// 打开富文本编辑器模态框
export function openRichTextEditorFromPlugin(initialContent: string = ''): Promise<string> {
  return new Promise((resolve) => {
    // 记录resolve函数，用于后续返回结果
    currentResolve = resolve;

    // 创建并挂载富文本编辑器模态框
    const modalRef = ref(true);
    const app = createApp({
      render: () => h(RichTextEditorModal, {
        modelValue: modalRef.value,
        initialContent,
        title: '富文本编辑器',
        width: '80%',
        'onUpdate:modelValue': (val: boolean) => {
          modalRef.value = val;
          if (!val) {
            // 模态框关闭时，返回空字符串
            if (currentResolve) {
              currentResolve('');
              currentResolve = null;
            }
            app.unmount();
          }
        },
        onSave: (content: string) => {
          // 保存内容
          if (currentResolve) {
            currentResolve(content);
            currentResolve = null;
          }
          modalRef.value = false;
          app.unmount();
        },
        onCancel: () => {
          // 取消操作
          if (currentResolve) {
            currentResolve('');
            currentResolve = null;
          }
          modalRef.value = false;
          app.unmount();
        }
      })
    });

    // 挂载到body
    const div = document.createElement('div');
    document.body.appendChild(div);
    app.mount(div);

    currentRichTextModal = app;
  });
}

// 监听来自插件的富文本编辑器调用请求
export function listenPluginRichTextRequests() {
  // 监听插件发送的打开富文本编辑器请求
  window.addEventListener('openRichTextEditor', async (event: any) => {
    if (event.detail && event.detail.initialContent) {
      const content = await openRichTextEditorFromPlugin(event.detail.initialContent);
      
      // 如果提供了回调函数，执行回调
      if (event.detail.callback && typeof event.detail.callback === 'function') {
        event.detail.callback(content);
      }
    }
  });
  
  // 监听富文本编辑器相关的通信事件
  listenRichTextEvent(RichTextEventType.SAVE_CONTENT, (data) => {
    // 处理保存内容事件
    console.log('Received save content event:', data);
  });
}

// 初始化插件富文本处理器
export function initPluginRichTextHandler() {
  listenPluginRichTextRequests();
}