# Frontend Dependencies

This document lists all major third-party dependencies used in the Fastdotnet framework frontend along with their license types. This serves both as acknowledgments to the open-source community and helps users understand the third-party libraries used in the project, enabling them to assess whether these dependencies meet their requirements, licensing needs, or compliance requirements.

> **Last Updated**: 2026-05-14  
> **Fastdotnet Version**: 1.0.x  
> **Frontend Projects**: fastdotnet-admin (Admin Panel), fastdotnet-app (Application)

## Table of Contents

- [Core Framework](#core-framework)
- [UI Component Library](#ui-component-library)
- [State Management & Routing](#state-management--routing)
- [HTTP Client](#http-client)
- [Data Visualization](#data-visualization)
- [Editors & Rich Text](#editors--rich-text)
- [Utility Libraries](#utility-libraries)
- [Security & Encryption](#security--encryption)
- [Micro-Frontends](#micro-frontends)
- [Development Tools](#development-tools)
- [Build Tools](#build-tools)

---

## Core Framework

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Vue** | 3.4.21 | MIT | Progressive JavaScript framework | [vuejs.org](https://vuejs.org/) |
| **Vue Router** | 4.3.0 | MIT | Official router for Vue.js | [router.vuejs.org](https://router.vuejs.org/) |
| **Pinia** | 2.1.7 | MIT | State management library for Vue | [pinia.vuejs.org](https://pinia.vuejs.org/) |
| **Vue I18n** | 9.10.2 | MIT | Internationalization plugin for Vue | [kazupon.github.io/vue-i18n](https://kazupon.github.io/vue-i18n/) |
| **Vue Demi** | 0.14.7 | MIT | Development utility supporting Vue 2/3 | [github.com/vueuse/vue-demi](https://github.com/vueuse/vue-demi) |

**Notes**:
- Vue 3 uses Composition API, providing more flexible component logic organization
- Pinia is a Vuex alternative, lighter with better TypeScript support
- Vue I18n supports multi-language switching for internationalization needs

---

## UI Component Library

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Element Plus** | 2.6.1 | MIT | Enterprise-grade UI component library based on Vue 3 | [element-plus.org](https://element-plus.org/) |
| **@element-plus/icons-vue** | 2.3.1 | MIT | Element Plus icon library | [element-plus.org](https://element-plus.org/) |
| **vue3-avatar** | 3.1.0 | MIT | Avatar component | [github.com/eliep/vue-avatar](https://github.com/eliep/vue-avatar) |

**Notes**:
- Element Plus provides rich enterprise-grade components including forms, tables, dialogs, etc.
- Icon library contains 2000+ SVG icons, supporting on-demand import

---

## State Management & Routing

Already listed in [Core Framework](#core-framework).

---

## HTTP Client

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Axios** | 1.6.8 | MIT | Promise-based HTTP client | [axios-http.com](https://axios-http.com/) |
| **Qs** | 6.12.0 | BSD-3-Clause | URL query string parsing and serialization | [github.com/ljharb/qs](https://github.com/ljharb/qs) |

**Notes**:
- Axios is used for frontend-backend data interaction, supporting request/response interceptors
- Qs handles complex query parameter serialization

---

## Data Visualization

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **ECharts** | 5.5.0 | Apache-2.0 | Powerful data visualization chart library | [echarts.apache.org](https://echarts.apache.org/) |
| **ECharts GL** | 2.0.9 | Apache-2.0 | ECharts 3D extension | [github.com/ecomfe/echarts-gl](https://github.com/ecomfe/echarts-gl) |
| **ECharts WordCloud** | 2.1.0 | Apache-2.0 | ECharts word cloud extension | [github.com/ecomfe/echarts-wordcloud](https://github.com/ecomfe/echarts-wordcloud) |
| **CountUp.js** | 2.8.0 | MIT | Animated number counter | [inorganik.github.io/countUp.js](https://inorganik.github.io/countUp.js/) |

**Notes**:
- ECharts supports various chart types including line, bar, pie, map, etc.
- ECharts GL provides advanced visualizations like 3D scatter plots and globes
- CountUp.js implements number scrolling animation effects

---

## Editors & Rich Text

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **@wangeditor/editor** | 5.1.23 | MIT | WangEditor v5 rich text editor | [wangeditor.com](https://www.wangeditor.com/) |
| **@wangeditor/editor-for-vue** | 5.1.12 | MIT | WangEditor Vue integration | [wangeditor.com](https://www.wangeditor.com/) |
| **Monaco Editor** | 0.54.0 | MIT | VS Code-powered code editor | [microsoft.github.io/monaco-editor](https://microsoft.github.io/monaco-editor/) |

**Notes**:
- WangEditor is a lightweight rich text editor supporting images, videos, tables, etc.
- Monaco Editor provides complete code editing features with syntax highlighting and IntelliSense

---

## Utility Libraries

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Day.js** | 1.11.19 | MIT | Lightweight date processing library | [day.js.org](https://day.js.org/) |
| **JS Cookie** | 3.0.5 | MIT | Cookie manipulation library | [github.com/js-cookie/js-cookie](https://github.com/js-cookie/js-cookie) |
| **NProgress** | 0.2.0 | MIT | Page loading progress bar | [ricostacruz.com/nprogress](https://ricostacruz.com/nprogress/) |
| **Mitt** | 3.0.1 | MIT | Tiny event emitter | [github.com/developit/mitt](https://github.com/developit/mitt) |
| **SortableJS** | 1.15.2 | MIT | Drag-and-drop sorting library | [sortablejs.github.io/Sortable](https://sortablejs.github.io/Sortable/) |
| **Splitpanes** | 3.1.5 | MIT | Resizable panes component | [github.com/antoniandre/splitpanes](https://github.com/antoniandre/splitpanes) |
| **Screenfull** | 6.0.2 | MIT | Fullscreen API wrapper | [github.com/sindresorhus/screenfull.js](https://github.com/sindresorhus/screenfull.js) |
| **Vue Clipboard3** | 2.0.0 | MIT | Clipboard operations | [github.com/JamieCurnow/vue-clipboard3](https://github.com/JamieCurnow/vue-clipboard3) |
| **Print.js** | 1.6.0 | MIT | Print functionality library | [printjs.crabbly.com](https://printjs.crabbly.com/) |
| **JS Table2Excel** | 1.1.2 | MIT | Table export to Excel | [github.com/wenjianzhang/js-table2excel](https://github.com/wenjianzhang/js-table2excel) |
| **CropperJS** | 1.6.1 | MIT | Image cropping library | [fengyuanchen.github.io/cropperjs](https://fengyuanchen.github.io/cropperjs/) |
| **QRCodeJS2 Fixes** | 0.0.2 | MIT | QR code generation | [github.com/davidshimjs/qrcodejs](https://github.com/davidshimjs/qrcodejs) |
| **JsPlumb** | 2.15.6 | MIT | Flowchart drawing library | [jsplumbtoolkit.com](https://jsplumbtoolkit.com/) |
| **Vue Grid Layout V3** | 3.1.2 | MIT | Grid layout system | [github.com/jbaysolutions/vue-grid-layout](https://github.com/jbaysolutions/vue-grid-layout) |

**Notes**:
- Day.js is lighter than Moment.js (2KB) with compatible API
- SortableJS supports touch devices for drag-and-drop sorting
- JsPlumb is used for flowchart scenarios like workflow designers

---

## Security & Encryption

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Crypto-JS** | 4.2.0 | MIT | JavaScript crypto library | [github.com/brix/crypto-js](https://github.com/brix/crypto-js) |
| **JSEncrypt** | 3.5.4 | MIT | RSA encryption library | [github.com/travist/jsencrypt](https://github.com/travist/jsencrypt) |
| **Node Forge** | 1.3.3 | BSD-3-Clause / GPL-2.0 | Native JavaScript cryptographic toolbox | [github.com/digitalbazaar/forge](https://github.com/digitalbazaar/forge) |
| **SM-Crypto** | 0.4.0 | MIT | Chinese national cryptographic algorithms (SM2/SM3/SM4) | [github.com/JuneAndGreen/sm-crypto](https://github.com/JuneAndGreen/sm-crypto) |
| **@types/crypto-js** | 4.2.2 | MIT | Crypto-JS TypeScript type definitions | - |
| **@types/node-forge** | 1.3.14 | MIT | Node Forge TypeScript type definitions | - |

**Notes**:
- Crypto-JS provides common encryption algorithms like AES, DES, SHA
- JSEncrypt is used for RSA public key encryption to protect sensitive data transmission
- SM-Crypto supports Chinese national cryptographic standards for compliance requirements
- Node Forge provides complete TLS/SSL, PKI, X.509 implementations

---

## Micro-Frontends

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Qiankun** | 2.10.16 | MIT | Micro-frontend framework | [qiankun.umijs.org](https://qiankun.umijs.org/) |
| **@microsoft/signalr** | 9.0.6 | MIT | SignalR WebSocket client | [docs.microsoft.com/en-us/aspnet/core/signalr](https://docs.microsoft.com/en-us/aspnet/core/signalr) |

**Notes**:
- Qiankun is based on single-spa, implementing micro-frontend architecture with dynamic plugin loading
- SignalR provides real-time communication capabilities for message push, online status, etc.

---

## Development Tools

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Husky** | 9.0.0 | MIT | Git hooks tool | [typicode.github.io/husky](https://typicode.github.io/husky/) |
| **Lint-Staged** | 15.0.0 | MIT | Lint tool for Git staged files | [github.com/okonet/lint-staged](https://github.com/okonet/lint-staged) |
| **@eslint/eslintrc** | 3.3.1 | MIT | ESLint configuration tool | [eslint.org](https://eslint.org/) |
| **@eslint/js** | 9.34.0 | MIT | ESLint JavaScript parser | [eslint.org](https://eslint.org/) |
| **fastdotnet-openapi2ts** | 1.1.1 | MIT | OpenAPI to TypeScript code generation | - |

**Notes**:
- Husky + Lint-Staged implement automatic code checking before commits
- ESLint ensures code quality and consistency
- openapi2ts automatically generates TypeScript type definitions from backend Swagger documentation

---

## Build Tools

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Vite** | 7.2.4 | MIT | Ultra-fast frontend build tool | [vitejs.dev](https://vitejs.dev/) |
| **@vitejs/plugin-vue** | 6.0.2 | MIT | Vite Vue plugin | [github.com/vitejs/vite-plugin-vue](https://github.com/vitejs/vite-plugin-vue) |
| **@vitejs/plugin-vue-jsx** | 5.1.2 | MIT | Vite Vue JSX plugin | [github.com/vitejs/vite-plugin-vue-jsx](https://github.com/vitejs/vite-plugin-vue-jsx) |
| **Sass** | 1.77.8 | MIT | CSS preprocessor | [sass-lang.com](https://sass-lang.com/) |
| **vite-plugin-compression2** | 2.3.1 | MIT | Vite compression plugin | [github.com/nonzzz/vite-plugin-compression](https://github.com/nonzzz/vite-plugin-compression) |
| **vite-plugin-cdn-import** | 1.0.1 | MIT | CDN import plugin | [github.com/MMF-FE/vite-plugin-cdn-import](https://github.com/MMF-FE/vite-plugin-cdn-import) |
| **vite-plugin-vue-devtools** | 8.0.5 | MIT | Vue DevTools integration | [github.com/webfansplz/vite-plugin-vue-devtools](https://github.com/webfansplz/vite-plugin-vue-devtools) |
| **vite-plugin-vue-setup-extend-plus** | 0.1.0 | MIT | Vue setup script name support | - |
| **vite-auto-i18n-plugin** | 1.1.13 | MIT | Automatic i18n plugin | - |
| **@plugin-web-update-notification/vite** | 2.0.1 | MIT | Web update notification plugin | - |

**Notes**:
- Vite uses native ES modules, providing ultra-fast dev server startup and hot updates
- Compression2 supports Gzip/Brotli compression to reduce bundle size
- CDN Import loads large libraries via CDN to optimize first-screen loading speed

---

## License Summary

### By License Type

#### MIT License (Most Permissive, Highest Proportion)
- Vue ecosystem (Vue, Vue Router, Pinia, Vue I18n, etc.)
- Element Plus
- Axios
- Most utility libraries (Day.js, JS Cookie, NProgress, etc.)
- All encryption libraries (Crypto-JS, JSEncrypt, SM-Crypto)
- All development and build tools
- Qiankun, SignalR

#### Apache-2.0 License (Permissive, requires attribution)
- ECharts core library
- ECharts GL
- ECharts WordCloud

#### BSD-3-Clause License
- Qs
- Node Forge

### Compatibility Notes

✅ **Commercial Friendly**: All dependencies use MIT, Apache-2.0, or BSD-3-Clause licenses, allowing commercial use  
✅ **No Open Source Required**: Using these libraries does not require your project to be open source  
⚠️ **Apache-2.0 Note**: If using Apache-2.0 licensed libraries, it's recommended to retain copyright notices in your project  
⚠️ **Node Forge Dual License**: Can choose BSD-3-Clause or GPL-2.0; commercial projects should use BSD-3-Clause

---

## Dependency Statistics

| Category | Count |
|----------|-------|
| Production Dependencies | 36 |
| Development Dependencies | 17 |
| **Total** | **53** |

---

## How to Verify Dependency Licenses

If you need to verify a package's license information, you can use the following methods:

1. **NPM Website**: Visit [npmjs.com](https://www.npmjs.com/) and search for the package name
2. **GitHub Repository**: Check the project's LICENSE file
3. **Command Line Tools**:
   ```bash
   npm list --license
   # or
   yarn licenses list
   ```
4. **License Checker Tool**:
   ```bash
   npx license-checker --summary
   ```

---

## Changelog

- **2026-05-14**: Initial version, documenting Fastdotnet 1.0.x frontend dependencies

---

## Feedback & Contributions

If you find incorrect dependency information or have new dependencies to add, please contact us through:

- 📧 Email: [yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)
- 💬 QQ Group: 779454817
- 🐛 GitHub Issues: [https://github.com/CN-GodHei/fastdotnet/issues](https://github.com/CN-GodHei/fastdotnet/issues)

---

**Thank you to all open-source project contributors! It is thanks to these excellent open-source libraries that Fastdotnet frontend can be so powerful and easy to use.**