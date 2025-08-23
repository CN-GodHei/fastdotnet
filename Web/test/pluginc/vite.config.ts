import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import qiankun from 'vite-plugin-qiankun';

export default defineConfig({
  plugins: [
    vue(),
    qiankun('pluginc', { // 插件名称，与主应用注册时一致
      useDevMode: true,
    }),
  ],
  server: {
    port: 9001, // 新端口
    headers: {
      'Access-Control-Allow-Origin': '*',
    },
  },
  build: {
    lib: {
      name: 'pluginc-lib',
      entry: 'src/main.ts',
      formats: ['umd'],
      fileName: () => `pluginc.js`,
    },
    rollupOptions: {
      external: ['vue'],
      output: {
        globals: {
          vue: 'Vue',
        },
      },
    },
  },
});
