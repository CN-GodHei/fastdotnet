import { createApp, type App as VueApp } from 'vue';
import App from './App.vue';
import { renderWithQiankun, qiankunWindow } from 'vite-plugin-qiankun/dist/helper';

let app: VueApp | null = null;

function render(props: any) {
  const { container } = props;
  const mountPoint = container ? container.querySelector('#app') : '#app';
  app = createApp(App);
  app.mount(mountPoint);
}

renderWithQiankun({
  mount(props) {
    console.log('[PluginC] mount');
    render(props);
  },
  bootstrap() {
    console.log('[PluginC] bootstrap');
  },
  unmount() {
    console.log('[PluginC] unmount');
    if (app) {
      app.unmount();
      app = null;
    }
  },
});

if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  render({});
}
