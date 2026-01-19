// src/router/index.ts
import { RouteRecordRaw } from 'vue-router';

// 定义插件内部的路由
const routes: RouteRecordRaw[] = [
  {
    path: '/home', // 根路径
    name: 'Home',
    component: () => import('../views/Home.vue'),
    meta: {
      title: 'Plugin A Home'
    }
  },
  {
    path: '/about', // about 页面
    name: 'About',
    component: () => import('../views/About.vue'),
    meta: {
      title: 'Plugin A About'
    }
  },
  {
    path: '/signalr-demo', // SignalR Demo页面
    name: 'SignalRDemo',
    component: () => import('../views/signalr-demo.vue'),
    meta: {
      title: 'Plugin A SignalR Demo'
    }
  },
  {
    path: '/rich-text-demo', // 富文本编辑器演示页面
    name: 'RichTextDemo',
    component: () => import('../views/RichTextView.vue'),
    meta: {
      title: 'Plugin A Rich Text Editor Demo'
    }
  },
  {
    path: '/rich-text-plugin-demo', // 通过PluginPortal使用富文本编辑器演示
    name: 'RichTextPluginDemo',
    component: () => import('../views/RichTextDemo.vue'),
    meta: {
      title: 'Plugin A Rich Text Plugin Demo'
    }
  },
  {
    path: '/form-with-richtext', // 表单中集成富文本编辑器演示
    name: 'FormWithRichText',
    component: () => import('../views/FormWithRichText.vue'),
    meta: {
      title: 'Plugin A Form with Rich Text'
    }
  },
  {
    path: '/:pathMatch(.*)*', // 匹配所有未定义路由
    name: 'NotFound',
    component: { render: () => null }, // 渲染空内容，避免警告
  }
];

export default routes;