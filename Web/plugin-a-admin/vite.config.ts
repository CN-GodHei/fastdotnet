import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import qiankun from 'vite-plugin-qiankun'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // 判断是否为 qiankun 子应用模式 (开发或构建)
  const isMicroApp = mode === 'micro' || mode === 'development-micro'
  
  return {
    plugins: [
      vue(),
      // 使用 vite-plugin-qiankun
      // 只在需要时启用，例如开发模式下被 qiankun 加载，或构建微应用时
      isMicroApp ? qiankun({
        // 微应用的名称，需要与主应用注册时的 name 一致
        name: 'plugin-a',
        // 是否使用沙箱隔离，默认为 true
        sandbox: true,
        // 是否在 Dev 模式下使用插件提供的代理功能
        dev: mode === 'development-micro' || mode === 'development',
        // 明确指定入口文件 (可选，但有时有助于插件正确识别)
        // entry: path.resolve(__dirname, 'src/micro-main.ts') 
      }) : null
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src')
      }
    },
    server: {
      port: 8081,
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
      // 强制预构建 @vite/client 依赖，有时能解决模块执行问题 (不太可能解决根本问题，但可以尝试)
      // preTransformRequests: false 
    },
    build: {
      // 如果是构建 qiankun 子应用
      ...(mode === 'micro' ? {
        // 输出为库格式
        lib: {
          name: 'PluginA',
          entry: path.resolve(__dirname, 'src/micro-main.ts'), // 入口文件
          formats: ['umd'], // qiankun 通常需要 umd 格式
        },
        rollupOptions: {
          // 确保外部依赖不会被打包进去
          external: ['vue', 'vue-router', 'pinia'],
          output: {
            // 关键：为 umd 构建指定全局变量名
            globals: {
              vue: 'Vue',
              'vue-router': 'VueRouter',
              pinia: 'Pinia'
            },
            // 将所有 CSS 提取到一个文件中，避免样式隔离问题
            assetFileNames: `assets/[name].[ext]`,
            // 为库的入口文件指定文件名
            entryFileNames: `plugin-a-admin.js`,
          }
        }
      } : {
        // 独立运行时的普通构建配置（如果需要）
        rollupOptions: {
          output: {
            assetFileNames: `assets/[name].[ext]`
          }
        }
      })
    }
  }
})