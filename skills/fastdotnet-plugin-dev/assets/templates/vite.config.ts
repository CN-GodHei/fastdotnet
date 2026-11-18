import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import { qiankun } from '@qiankunjs/bundler-plugin/vite'

/**
 * qiankun 3.0 微应用（原生 ESM 路线）
 *
 * 与 2.x + vite-plugin-qiankun 的差别：
 *   - 没有 build.lib / formats / rollupOptions.external / globals —— vue、element-plus 等
 *     随本应用一起打包，不再要求宿主提供全局变量；
 *   - 没有 micro-main.ts 薄壳、没有手写 micro-index.html，入口就是普通的 index.html + src/main.ts；
 *   - 生命周期由入口模块直接 export，由 qiankun 的原生 ESM 沙箱加载。
 *
 * build 里这三项是「让 qiankun 找得到生命周期」的必需配置，缺一不可（已实测）：
 *   1. modulePreload.polyfill=false —— 否则 Vite 会给 HTML 入口另生成一个 facade chunk，
 *      qiankun 插件把 entry 标记打在它身上，而它命名空间是空的；依据 qiankun 源码
 *      packages/shared/src/esm-sandbox/engine.ts:912，存在 entry 标记时就不再扫描其它模块。
 *   2. rolldownOptions.input 显式包含 src/main.ts —— HTML 里 <script src> 引用的模块
 *      不算 bundler 的 entry，未被引用的导出会被裁掉。
 *   3. preserveEntrySignatures:'strict' + output.minifyInternalExports:false —— 保住导出且不改名。
 */
export default defineConfig(() => {
  return {
    // 插件静态资源由后端托管在 /plugins/__PLUGIN_ID__/__FRONTEND_SEGMENT__/ 下，
    // 入口 HTML 与宿主不同源，必须用相对 base；qiankun 以入口 URL 为基准解析相对路径。
    base: './',
    plugins: [vue(), qiankun()],
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
      modulePreload: { polyfill: false },
      rolldownOptions: {
        input: {
          index: path.resolve(__dirname, 'index.html'),
          main: path.resolve(__dirname, 'src/main.ts'),
        },
        preserveEntrySignatures: 'strict',
        output: { minifyInternalExports: false },
      },
    }
  }
})
