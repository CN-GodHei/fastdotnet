import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'PluginBHome',
    component: () => import('../App.vue'), // 暂时让根路径指向 App.vue
  }
];

export default routes;