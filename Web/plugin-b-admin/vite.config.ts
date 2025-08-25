import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import qiankun from 'vite-plugin-qiankun'

export default defineConfig(({ mode }) => {
  return {
    plugins: [
      vue(),
      // 关键：为微应用启用 qiankun 插件
      // 'plugin-b' 是微应用的唯一名称，必须和主应用注册时一致
      qiankun('plugin-b', {
        useDevMode: true
      })
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    server: {
      port: 8089, // plugin-b 的端口
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
    },
    build: {
      lib: {
        name: 'PluginB',
        entry: path.resolve(__dirname, 'src/micro-main.ts'),
        formats: ['umd'],
        fileName: () => `plugin-b-admin.js`
      },
      rollupOptions: {
        external: ['vue', 'vue-router', 'pinia'],
        output: {
          globals: {
            vue: 'Vue',
            'vue-router': 'VueRouter',
            pinia: 'Pinia'
          },
          assetFileNames: `assets/[name].[ext]`,
        }
      }
    }
  }
})
