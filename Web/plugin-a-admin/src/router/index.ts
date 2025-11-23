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
    path: '/:pathMatch(.*)*', // 匹配所有未定义路由
    name: 'NotFound',
    component: { render: () => null }, // 渲染空内容，避免警告
  }
];

export default routes;