import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import qiankun from 'vite-plugin-qiankun'

export default defineConfig(({ mode }) => {
  return {
    plugins: [
      vue(),
      // 关键：为微应用启用 qiankun 插件
      qiankun('rich-text-editor', {
        useDevMode: true
      })
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    server: {
      port: 8090, // 富文本编辑器微应用端口
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
    },
    build: {
      lib: {
        name: 'RichTextEditor',
        entry: path.resolve(__dirname, 'src/micro-main.ts'),
        formats: ['umd'],
        fileName: () => `rich-text-plugin.js`
      },
      rollupOptions: {
        external: ['vue', 'vue-router', 'pinia', 'element-plus', '@wangeditor/editor', '@wangeditor/editor-for-vue'],
        output: {
          globals: {
            vue: 'Vue',
            'vue-router': 'VueRouter',
            pinia: 'Pinia',
            'element-plus': 'ElementPlus',
            '@wangeditor/editor': 'WangEditor',
            '@wangeditor/editor-for-vue': 'WangEditorForVue'
          },
          assetFileNames: `assets/[name].[ext]`,
        }
      }
    }
  }
})