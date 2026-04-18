import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import qiankun from 'vite-plugin-qiankun'

export default defineConfig(({ mode }) => {
  return {
    define: {
      global: 'globalThis',
      process: {
        env: {}
      }
    },
    plugins: [
      vue(),
      // 关键：为微应用启用 qiankun 插件
      // '11375910391972869' 是微应用的唯一名称，必须和主应用注册时一致
      // useDevMode: true 可确保在开发模式下也能正确处理模块导出
      qiankun('11375910391972869', {
        useDevMode: true
      })
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    server: {
      port: 8089, // 确保端口与您部署和访问的一致
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
    },
    build: {
      lib: {
        name: '11375910391972869',
        entry: path.resolve(__dirname, 'src/micro-main.ts'),
        formats: ['umd'],
        fileName: () => `11375910391972869-admin.js`
      },
      rollupOptions: {
        // 【修复】只外部化 vue 和 element-plus，将 vue-router 和 pinia 打包进去
        external: ['vue', 'element-plus'],
        output: {
          globals: {
            vue: 'Vue',
            'element-plus': 'ElementPlus'
          },
          assetFileNames: `assets/[name].[ext]`,
        }
      }
    }
  }
})
