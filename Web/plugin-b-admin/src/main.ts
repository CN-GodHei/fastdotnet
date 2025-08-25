import { createApp, type App as VueApp } from 'vue';
import { createRouter, createWebHistory, type Router } from 'vue-router';
import { createPinia, type Pinia } from 'pinia';
import App from './App.vue';
import routes from './router';

let app: VueApp | null = null;

const init = (props: any = {}) => {
  const { container } = props;
  
  app = createApp(App);
  
  const historyBase = (window as any).__POWERED_BY_QIANKUN__ ? props.base : '/';
  const router = createRouter({
    history: createWebHistory(historyBase),
    routes,
  });

  const pinia = createPinia();

  app.use(router);
  app.use(pinia);
  
  const mountPoint = container ? container : '#app';
  app.mount(mountPoint);
  
  return {
    app,
    router,
    pinia,
    unmount() {
      if (app) {
        app.unmount();
        app = null;
      }
    },
  };
};

if (!(window as any).__POWERED_BY_QIANKUN__) {
  init();
}

export default init;