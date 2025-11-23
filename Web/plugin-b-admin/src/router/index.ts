import { RouteRecordRaw } from 'vue-router';
import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'PluginBHome',
    component: () => import('../App.vue'), // 暂时让根路径指向 App.vue
  },
  {
    path: '/:pathMatch(.*)*', // 匹配所有未定义路由
    name: 'NotFound',
    component: { render: () => null }, // 渲染空内容，避免警告
  }
];

export default routes;