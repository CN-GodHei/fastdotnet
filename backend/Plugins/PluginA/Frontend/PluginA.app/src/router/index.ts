import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/Home.vue')
  },
  {
    path: '/:pathMatch(.*)*', // 匹配所有未定义路由
    name: 'NotFound',
    component: { render: () => null }, // 渲染空内容，避免警告
  }
]


export default routes
