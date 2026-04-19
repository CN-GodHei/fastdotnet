import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import qiankun from 'vite-plugin-qiankun'
import fs from 'fs'

export default defineConfig(({ mode }) => {
  return {
    plugins: [
      vue(),
      qiankun('11375910391972869', {
        useDevMode: true
      }),
      // 构建完成后复制 micro-index.html 到发布目录并重命名为 index.html
      {
        name: 'copy-micro-index',
        closeBundle() {
          const sourceFile = path.resolve(__dirname, 'micro-index.html')
          const outFile = path.resolve(__dirname, '../../publish/11375910391972869/wwwroot/admin/index.html')
          
          if (fs.existsSync(sourceFile)) {
            // 确保目标目录存在
            const outDir = path.dirname(outFile)
            if (!fs.existsSync(outDir)) {
              fs.mkdirSync(outDir, { recursive: true })
            }
            
            // 直接复制文件（占位符已在生成项目时被 CLI 替换）
            fs.copyFileSync(sourceFile, outFile)
            console.log('[copy-micro-index] ✓ Copied to:', outFile)
          }
        }
      }
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    server: {
      port: 8099, // 确保端口与您部署和访问的一致
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
    },
    define: {
      global: 'globalThis',
      process: {
        env: {}
      }
    },
    build: {
      outDir: '../../publish/11375910391972869/wwwroot/admin',  // 输出到发布目录
      emptyOutDir: true,  // 构建前清空输出目录
      lib: {
        name: '11375910391972869',
        entry: path.resolve(__dirname, 'src/micro-main.ts'),
        formats: ['umd'],
        fileName: () => `11375910391972869-admin.js`
      },
      rollupOptions: {
        external: ['vue', 'vue-router', 'pinia', 'element-plus'],
        output: {
          globals: {
            vue: 'Vue',
            'vue-router': 'VueRouter',
            pinia: 'Pinia',
            'element-plus': 'ElementPlus',
            '@element-plus/icons-vue': 'ElementPlusIconsVue'
          },
          assetFileNames: `assets/[name].[ext]`,
        }
      }
    }
  }
})
