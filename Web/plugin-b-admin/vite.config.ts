import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import qiankun from 'vite-plugin-qiankun';

export default defineConfig({
  plugins: [
    vue(),
    qiankun('plugin-b', { // 必须与主应用注册的 name 一致
      useDevMode: true,
    }),
  ],
  server: {
    port: 8083,
    headers: {
      'Access-Control-Allow-Origin': '*',
    },
  },
  build: {
    lib: {
      name: 'plugin-b',
      entry: 'src/main.ts',
      formats: ['umd'],
      fileName: () => `plugin-b-admin.js`,
    },
    // 关键：添加 rollupOptions 来处理外部依赖
    rollupOptions: {
      // 告诉 Rollup, 'vue' 是外部依赖，不要打包它
      external: ['vue'],
      output: {
        // 在 UMD 构建模式下, 全局模式下访问 `vue` 模块的名称是 `Vue`
        // 这必须与主应用在 window 上暴露的 `window.Vue` 一致
        globals: {
          vue: 'Vue',
        },
      },
    },
  },
});