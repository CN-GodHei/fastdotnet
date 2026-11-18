import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import { qiankun } from '@qiankunjs/bundler-plugin/vite'

/**
 * qiankun 3.0 微应用（原生 ESM 路线）
 *
 * 与 2.x + vite-plugin-qiankun 的差异：
 *   - 不再需要 UMD：没有 build.lib / formats / rollupOptions.external / globals，
 *     vue / vue-router / pinia / element-plus 随本应用一起打包。
 *   - 不再需要 micro-main.ts 薄壳与手写 micro-index.html：入口就是普通的 index.html + src/main.ts，
 *     生命周期由入口模块直接 `export`，qiankun 用原生 ESM 沙箱加载。
 *   - 不再需要 copy-micro-index 自定义插件：Vite 正常产出 index.html。
 *   - qiankun() 官方插件只做两件事：dev/preview 开 CORS；构建时给入口模块脚本打 `entry` 标记。
 *
 * 下面 build 里的三项是「让 qiankun 找得到生命周期」的必需配置，缺一不可（均已逐项实测）：
 *   1. modulePreload.polyfill = false —— 否则 Vite 会给 HTML 入口另生成一个 facade chunk，
 *      qiankun 插件把 entry 标记打在它身上，而它的命名空间是空的；
 *      依据 qiankun 源码 packages/shared/src/esm-sandbox/engine.ts:912，
 *      一旦存在 entry 标记就不再扫描其他模块，生命周期永远找不到。
 *   2. rolldownOptions.input 显式包含 src/main.ts —— HTML 里 <script src> 引用的模块
 *      不算 bundler 的 entry，其未被引用的导出会被裁掉。
 *   3. preserveEntrySignatures: 'strict' + output.minifyInternalExports: false ——
 *      保住导出，并且不做导出名压缩（否则会变成 `export{xS as _}` 这种，名字对不上）。
 */
export default defineConfig(() => {
  return {
    // 微应用由后端静态托管在 /plugins/<PluginId>/app/ 前缀下，
    // 且入口 HTML 与宿主不同源（VITE_API_URL 指向后端 18889，宿主在 18887）。
    // 必须用相对 base：qiankun 以入口 URL 为基准解析这些相对资源路径；
    // 用绝对 /plugins/... 会解析到宿主页面所在源，而不是后端源。
    base: './',
    plugins: [
      vue(),
      qiankun(),
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
      outDir: '../../publish/11375910391972869/wwwroot/app',  // 输出到发布目录
      emptyOutDir: true,  // 构建前清空输出目录
      modulePreload: { polyfill: false },
      rolldownOptions: {
        input: {
          index: path.resolve(__dirname, 'index.html'),
          main: path.resolve(__dirname, 'src/main.ts'),
        },
        preserveEntrySignatures: 'strict',
        output: {
          minifyInternalExports: false,
        },
      },
    }
  }
})

