import type { RouteRecordRaw } from 'vue-router'

// 插件内部路由：不要带 /micro/{PluginId} 前缀（history base 已由宿主传入）
const routes: RouteRecordRaw[] = [
  { path: '/', redirect: '/home' },
  { path: '/home', name: 'Home', component: () => import('../views/Home.vue'), meta: { title: '__DISPLAY_NAME__' } },
  { path: '/:pathMatch(.*)*', name: 'NotFound', component: { render: () => null } }
]

export default routes
