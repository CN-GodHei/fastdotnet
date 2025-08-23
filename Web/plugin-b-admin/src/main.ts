import { createApp, type App as VueApp } from 'vue';
import App from './App.vue';
import { renderWithQiankun, qiankunWindow } from 'vite-plugin-qiankun/dist/helper';

let app: VueApp | null = null;

function render(props: any) {
  const { container } = props;
  const mountPoint = container ? container : '#app';
  app = createApp(App);
  app.mount(mountPoint);
}

renderWithQiankun({
  mount(props) {
    console.log('[PluginB] mount');
    render(props);
  },
  bootstrap() {
    console.log('[PluginB] bootstrap');
  },
  unmount() {
    console.log('[PluginB] unmount');
    if (app) {
      app.unmount();
      app = null;
    }
  },
  update(props) {
    console.log('[PluginB] update');
  },
});

if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  render({});
}
