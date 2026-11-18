import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import qiankun from 'vite-plugin-qiankun'
import path from 'path'
import fs from 'fs'

export default defineConfig({
  // 说明：以下路径/端口在生成时会被替换。
  plugins: [
    vue(),
    qiankun('__PLUGIN_ID__', { useDevMode: true }),
    {
      name: 'copy-micro-index',
      closeBundle() {
        const src = path.resolve(__dirname, 'micro-index.html')
        const out = path.resolve(__dirname, '../../publish/__PLUGIN_ID__/wwwroot/__FRONTEND_SEGMENT__/index.html')
        if (fs.existsSync(src)) {
          fs.mkdirSync(path.dirname(out), { recursive: true })
          fs.copyFileSync(src, out)
        }
      }
    }
  ],
  resolve: {
    alias: { '@': path.resolve(__dirname, 'src') }
  },
  server: {
    port: __FRONTEND_PORT__,
    headers: { 'Access-Control-Allow-Origin': '*' }
  },
  define: {
    global: 'globalThis',
    process: { env: {} }
  },
  build: {
    outDir: '../../publish/__PLUGIN_ID__/wwwroot/__FRONTEND_SEGMENT__',
    emptyOutDir: true,
    lib: {
      name: '__PLUGIN_ID__',
      entry: path.resolve(__dirname, 'src/micro-main.ts'),
      formats: ['umd'],
      fileName: () => `__PLUGIN_ID__-__FRONTEND_SEGMENT__.js`,
      cssFileName: '__PLUGIN_ID__-__FRONTEND_SEGMENT__'
    },
    rollupOptions: {
      // 默认不 external：宿主未注入 window.Vue 等全局变量时，external 会直接白屏。
      // 确认宿主提供全局变量后再加 external + globals。
      output: {
        globals: {},
        assetFileNames: 'assets/[name].[ext]'
      }
    }
  }
})
