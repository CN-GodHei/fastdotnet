# 前端依赖清单

本文档列出了 Fastdotnet 框架前端使用的所有主要第三方依赖库及其许可证类型。这既是对开源社区的鸣谢，也帮助使用者了解项目中使用的第三方库，以便评估是否符合您的需求、许可要求或合规性要求。

> **最后更新时间**：2026-05-14  
> **Fastdotnet 版本**：1.0.x  
> **前端项目**：fastdotnet-admin（管理后台）、fastdotnet-app（应用端）

## 目录

- [核心框架](#核心框架)
- [UI 组件库](#ui-组件库)
- [状态管理与路由](#状态管理与路由)
- [HTTP 客户端](#http-客户端)
- [数据可视化](#数据可视化)
- [编辑器与富文本](#编辑器与富文本)
- [工具库](#工具库)
- [安全与加密](#安全与加密)
- [微前端](#微前端)
- [开发工具](#开发工具)
- [构建工具](#构建工具)

---

## 核心框架

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Vue** | 3.4.21 | MIT | 渐进式 JavaScript 框架 | [vuejs.org](https://vuejs.org/) |
| **Vue Router** | 4.3.0 | MIT | Vue.js 官方路由管理器 | [router.vuejs.org](https://router.vuejs.org/) |
| **Pinia** | 2.1.7 | MIT | Vue 的状态管理库 | [pinia.vuejs.org](https://pinia.vuejs.org/) |
| **Vue I18n** | 9.10.2 | MIT | Vue 国际化插件 | [kazupon.github.io/vue-i18n](https://kazupon.github.io/vue-i18n/) |
| **Vue Demi** | 0.14.7 | MIT | 支持 Vue 2/3 的开发工具 | [github.com/vueuse/vue-demi](https://github.com/vueuse/vue-demi) |

**说明**：
- Vue 3 采用 Composition API，提供更灵活的组件逻辑组织方式
- Pinia 是 Vuex 的替代方案，更轻量且对 TypeScript 支持更好
- Vue I18n 支持多语言切换，满足国际化需求

---

## UI 组件库

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Element Plus** | 2.6.1 | MIT | 基于 Vue 3 的企业级 UI 组件库 | [element-plus.org](https://element-plus.org/) |
| **@element-plus/icons-vue** | 2.3.1 | MIT | Element Plus 图标库 | [element-plus.org](https://element-plus.org/) |
| **vue3-avatar** | 3.1.0 | MIT | 头像组件 | [github.com/eliep/vue-avatar](https://github.com/eliep/vue-avatar) |

**说明**：
- Element Plus 提供丰富的企业级组件，包括表单、表格、对话框等
- 图标库包含 2000+ SVG 图标，支持按需引入

---

## 状态管理与路由

已在[核心框架](#核心框架)中列出。

---

## HTTP 客户端

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Axios** | 1.6.8 | MIT | 基于 Promise 的 HTTP 客户端 | [axios-http.com](https://axios-http.com/) |
| **Qs** | 6.12.0 | BSD-3-Clause | URL 查询字符串解析和序列化 | [github.com/ljharb/qs](https://github.com/ljharb/qs) |

**说明**：
- Axios 用于前后端数据交互，支持请求/响应拦截器
- Qs 用于处理复杂的查询参数序列化

---

## 数据可视化

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **ECharts** | 5.5.0 | Apache-2.0 | 强大的数据可视化图表库 | [echarts.apache.org](https://echarts.apache.org/) |
| **ECharts GL** | 2.0.9 | Apache-2.0 | ECharts 3D 扩展 | [github.com/ecomfe/echarts-gl](https://github.com/ecomfe/echarts-gl) |
| **ECharts WordCloud** | 2.1.0 | Apache-2.0 | ECharts 词云图扩展 | [github.com/ecomfe/echarts-wordcloud](https://github.com/ecomfe/echarts-wordcloud) |
| **CountUp.js** | 2.8.0 | MIT | 数字动画计数器 | [inorganik.github.io/countUp.js](https://inorganik.github.io/countUp.js/) |

**说明**：
- ECharts 支持折线图、柱状图、饼图、地图等多种图表类型
- ECharts GL 提供 3D 散点图、地球仪等高级可视化
- CountUp.js 用于实现数字滚动动画效果

---

## 编辑器与富文本

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **@wangeditor/editor** | 5.1.23 | MIT | WangEditor v5 富文本编辑器 | [wangeditor.com](https://www.wangeditor.com/) |
| **@wangeditor/editor-for-vue** | 5.1.12 | MIT | WangEditor Vue 集成 | [wangeditor.com](https://www.wangeditor.com/) |
| **Monaco Editor** | 0.54.0 | MIT | VS Code 同款代码编辑器 | [microsoft.github.io/monaco-editor](https://microsoft.github.io/monaco-editor/) |

**说明**：
- WangEditor 轻量级富文本编辑器，支持图片、视频、表格等
- Monaco Editor 提供完整的代码编辑功能，支持语法高亮、智能提示

---

## 工具库

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Day.js** | 1.11.19 | MIT | 轻量级日期处理库 | [day.js.org](https://day.js.org/) |
| **JS Cookie** | 3.0.5 | MIT | Cookie 操作库 | [github.com/js-cookie/js-cookie](https://github.com/js-cookie/js-cookie) |
| **NProgress** | 0.2.0 | MIT | 页面加载进度条 | [ricostacruz.com/nprogress](https://ricostacruz.com/nprogress/) |
| **Mitt** | 3.0.1 | MIT | 小型事件发射器 | [github.com/developit/mitt](https://github.com/developit/mitt) |
| **SortableJS** | 1.15.2 | MIT | 拖拽排序库 | [sortablejs.github.io/Sortable](https://sortablejs.github.io/Sortable/) |
| **Splitpanes** | 3.1.5 | MIT | 可调整大小的面板组件 | [github.com/antoniandre/splitpanes](https://github.com/antoniandre/splitpanes) |
| **Screenfull** | 6.0.2 | MIT | 全屏 API 封装 | [github.com/sindresorhus/screenfull.js](https://github.com/sindresorhus/screenfull.js) |
| **Vue Clipboard3** | 2.0.0 | MIT | 剪贴板操作 | [github.com/JamieCurnow/vue-clipboard3](https://github.com/JamieCurnow/vue-clipboard3) |
| **Print.js** | 1.6.0 | MIT | 打印功能库 | [printjs.crabbly.com](https://printjs.crabbly.com/) |
| **JS Table2Excel** | 1.1.2 | MIT | 表格导出 Excel | [github.com/wenjianzhang/js-table2excel](https://github.com/wenjianzhang/js-table2excel) |
| **CropperJS** | 1.6.1 | MIT | 图片裁剪库 | [fengyuanchen.github.io/cropperjs](https://fengyuanchen.github.io/cropperjs/) |
| **QRCodeJS2 Fixes** | 0.0.2 | MIT | 二维码生成 | [github.com/davidshimjs/qrcodejs](https://github.com/davidshimjs/qrcodejs) |
| **JsPlumb** | 2.15.6 | MIT | 流程图绘制库 | [jsplumbtoolkit.com](https://jsplumbtoolkit.com/) |
| **Vue Grid Layout V3** | 3.1.2 | MIT | 网格布局系统 | [github.com/jbaysolutions/vue-grid-layout](https://github.com/jbaysolutions/vue-grid-layout) |

**说明**：
- Day.js 比 Moment.js 更轻量（2KB），API 兼容
- SortableJS 支持触摸设备，用于拖拽排序功能
- JsPlumb 用于工作流设计器等流程图场景

---

## 安全与加密

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Crypto-JS** | 4.2.0 | MIT | JavaScript 加密库 | [github.com/brix/crypto-js](https://github.com/brix/crypto-js) |
| **JSEncrypt** | 3.5.4 | MIT | RSA 加密库 | [github.com/travist/jsencrypt](https://github.com/travist/jsencrypt) |
| **Node Forge** | 1.3.3 | BSD-3-Clause / GPL-2.0 | 原生 JavaScript 加密工具包 | [github.com/digitalbazaar/forge](https://github.com/digitalbazaar/forge) |
| **SM-Crypto** | 0.4.0 | MIT | 国密算法（SM2/SM3/SM4） | [github.com/JuneAndGreen/sm-crypto](https://github.com/JuneAndGreen/sm-crypto) |
| **@types/crypto-js** | 4.2.2 | MIT | Crypto-JS TypeScript 类型定义 | - |
| **@types/node-forge** | 1.3.14 | MIT | Node Forge TypeScript 类型定义 | - |

**说明**：
- Crypto-JS 提供 AES、DES、SHA 等常用加密算法
- JSEncrypt 用于 RSA 公钥加密，保护敏感数据传输
- SM-Crypto 支持中国国密标准，满足合规要求
- Node Forge 提供完整的 TLS/SSL、PKI、X.509 等实现

---

## 微前端

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Qiankun** | 2.10.16 | MIT | 微前端框架 | [qiankun.umijs.org](https://qiankun.umijs.org/) |
| **@microsoft/signalr** | 9.0.6 | MIT | SignalR WebSocket 客户端 | [docs.microsoft.com/en-us/aspnet/core/signalr](https://docs.microsoft.com/en-us/aspnet/core/signalr) |

**说明**：
- Qiankun 基于 single-spa，实现微前端架构，支持插件动态加载
- SignalR 提供实时通信能力，支持消息推送、在线状态等

---

## 开发工具

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Husky** | 9.0.0 | MIT | Git hooks 工具 | [typicode.github.io/husky](https://typicode.github.io/husky/) |
| **Lint-Staged** | 15.0.0 | MIT | Git staged 文件 lint 工具 | [github.com/okonet/lint-staged](https://github.com/okonet/lint-staged) |
| **@eslint/eslintrc** | 3.3.1 | MIT | ESLint 配置工具 | [eslint.org](https://eslint.org/) |
| **@eslint/js** | 9.34.0 | MIT | ESLint JavaScript 解析器 | [eslint.org](https://eslint.org/) |
| **fastdotnet-openapi2ts** | 1.1.1 | MIT | OpenAPI 到 TypeScript 代码生成 | - |

**说明**：
- Husky + Lint-Staged 实现提交前自动代码检查
- ESLint 保证代码质量和一致性
- openapi2ts 根据后端 Swagger 文档自动生成 TypeScript 类型定义

---

## 构建工具

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Vite** | 7.2.4 | MIT | 极速前端构建工具 | [vitejs.dev](https://vitejs.dev/) |
| **@vitejs/plugin-vue** | 6.0.2 | MIT | Vite Vue 插件 | [github.com/vitejs/vite-plugin-vue](https://github.com/vitejs/vite-plugin-vue) |
| **@vitejs/plugin-vue-jsx** | 5.1.2 | MIT | Vite Vue JSX 插件 | [github.com/vitejs/vite-plugin-vue-jsx](https://github.com/vitejs/vite-plugin-vue-jsx) |
| **Sass** | 1.77.8 | MIT | CSS 预处理器 | [sass-lang.com](https://sass-lang.com/) |
| **vite-plugin-compression2** | 2.3.1 | MIT | Vite 压缩插件 | [github.com/nonzzz/vite-plugin-compression](https://github.com/nonzzz/vite-plugin-compression) |
| **vite-plugin-cdn-import** | 1.0.1 | MIT | CDN 导入插件 | [github.com/MMF-FE/vite-plugin-cdn-import](https://github.com/MMF-FE/vite-plugin-cdn-import) |
| **vite-plugin-vue-devtools** | 8.0.5 | MIT | Vue DevTools 集成 | [github.com/webfansplz/vite-plugin-vue-devtools](https://github.com/webfansplz/vite-plugin-vue-devtools) |
| **vite-plugin-vue-setup-extend-plus** | 0.1.0 | MIT | Vue setup script name 支持 | - |
| **vite-auto-i18n-plugin** | 1.1.13 | MIT | 自动国际化插件 | - |
| **@plugin-web-update-notification/vite** | 2.0.1 | MIT | Web 更新通知插件 | - |

**说明**：
- Vite 采用原生 ES 模块，提供极速的开发服务器启动和热更新
- Compression2 支持 Gzip/Brotli 压缩，减小打包体积
- CDN Import 将大型库通过 CDN 加载，优化首屏加载速度

---

## 许可证总结

### 按许可证类型分类

#### MIT 许可证（最宽松，占比最高）
- Vue 生态（Vue、Vue Router、Pinia、Vue I18n 等）
- Element Plus
- Axios
- ECharts（部分组件）
- 大部分工具库（Day.js、JS Cookie、NProgress 等）
- 所有加密库（Crypto-JS、JSEncrypt、SM-Crypto）
- 所有开发工具和构建工具
- Qiankun、SignalR

#### Apache-2.0 许可证（宽松，需声明变更）
- ECharts 核心库
- ECharts GL
- ECharts WordCloud

#### BSD-3-Clause 许可证
- Qs
- Node Forge

### 兼容性说明

✅ **商业友好**：所有依赖均使用 MIT、Apache-2.0 或 BSD-3-Clause 许可证，允许商业用途  
✅ **无需开源**：使用这些库不需要将您的项目开源  
⚠️ **Apache-2.0 注意**：如果使用 Apache-2.0 许可证的库，建议在项目中保留版权声明  
⚠️ **Node Forge 双重许可**：可选择 BSD-3-Clause 或 GPL-2.0，商业项目建议使用 BSD-3-Clause

---

## 依赖统计

| 类别 | 数量 |
|------|------|
| 生产依赖（dependencies） | 36 |
| 开发依赖（devDependencies） | 17 |
| **总计** | **53** |

---

## 如何验证依赖许可证

如果您需要验证某个包的许可证信息，可以使用以下方法：

1. **NPM 官网**：访问 [npmjs.com](https://www.npmjs.com/) 搜索包名
2. **GitHub 仓库**：查看项目的 LICENSE 文件
3. **命令行工具**：
   ```bash
   npm list --license
   # 或
   yarn licenses list
   ```
4. **许可证检查工具**：
   ```bash
   npx license-checker --summary
   ```

---

## 更新日志

- **2026-05-14**：初始版本，记录 Fastdotnet 1.0.x 的前端依赖

---

## 反馈与贡献

如果您发现依赖信息有误或有新的依赖需要添加，请通过以下方式联系我们：

- 📧 邮箱：[yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)
- 💬 QQ 群：779454817
- 🐛 GitHub Issues：[https://github.com/CN-GodHei/fastdotnet/issues](https://github.com/CN-GodHei/fastdotnet/issues)

---

**感谢所有开源项目的贡献者！正是有了这些优秀的开源库，Fastdotnet 前端才能如此强大和易用。**