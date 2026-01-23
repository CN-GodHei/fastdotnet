/**
 * 富文本编辑器通信工具
 * 用于主应用和富文本编辑器微应用之间的通信
 */

interface RichTextEvent {
  type: string;
  data: any;
}

// 事件类型枚举
export enum RichTextEventType {
  GET_CONTENT = 'richText:getContent',
  SET_CONTENT = 'richText:setContent',
  SAVE_CONTENT = 'richText:saveContent',
  INSERT_IMAGE = 'richText:insertImage',
  INSERT_LINK = 'richText:insertLink',
}

// 发送事件到富文本编辑器
export function sendRichTextEvent(type: RichTextEventType, data?: any) {
  const event = new CustomEvent('richTextEvent', {
    detail: { type, data }
  });
  window.dispatchEvent(event);
}

// 监听来自富文本编辑器的事件
export function listenRichTextEvent(
  type: RichTextEventType,
  callback: (data: any) => void
) {
  const handler = (event: any) => {
    if (event.detail && event.detail.type === type) {
      callback(event.detail.data);
    }
  };
  
  window.addEventListener('richTextEvent', handler as EventListener);
  
  // 返回取消监听的函数
  return () => {
    window.removeEventListener('richTextEvent', handler as EventListener);
  };
}

// 请求获取富文本内容
export function requestRichTextContent(): Promise<string> {
  return new Promise((resolve) => {
    const eventId = `getContent_${Date.now()}_${Math.random()}`;
    
    // 监听回复事件
    const unsubscribe = listenRichTextEvent(RichTextEventType.GET_CONTENT, (data) => {
      if (data.eventId === eventId) {
        resolve(data.content);
        unsubscribe();
      }
    });
    
    // 发送请求事件
    sendRichTextEvent(RichTextEventType.GET_CONTENT, { eventId });
  });
}

// 设置富文本内容
export function setRichTextContent(content: string) {
  sendRichTextEvent(RichTextEventType.SET_CONTENT, { content });
}

// 保存富文本内容
export function saveRichTextContent(content: string) {
  sendRichTextEvent(RichTextEventType.SAVE_CONTENT, { content });
}

// 插入图片
export function insertImageToRichText(imageUrl: string) {
  sendRichTextEvent(RichTextEventType.INSERT_IMAGE, { imageUrl });
}

// 插入链接
export function insertLinkToRichText(url: string, text: string) {
  sendRichTextEvent(RichTextEventType.INSERT_LINK, { url, text });
}