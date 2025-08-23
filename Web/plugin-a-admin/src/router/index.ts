// src/router/index.ts
import { RouteRecordRaw } from 'vue-router';

// 定义插件内部的路由
const routes: RouteRecordRaw[] = [
  {
    path: '/', // 根路径
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
  }
];

export default routes;