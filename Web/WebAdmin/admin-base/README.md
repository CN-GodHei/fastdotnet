# Admin Base 主框架

## 技术栈

本项目是基于 Vue 3 + Vite 构建的现代化前端主框架，采用了以下核心技术：

- Vue 3：采用 Composition API 和 `<script setup>` 语法
- Vite：下一代前端构建工具，提供极速的开发体验
- Element Plus：基于 Vue 3 的组件库
- Pinia：Vue 3 的状态管理方案
- Vue Router：Vue.js 官方路由
- Qiankun：阿里巴巴开源的微前端框架

## 项目结构

```
├── src/
│   ├── App.vue                # 根组件
│   ├── main.js                # 入口文件
│   ├── router/                # 路由配置
│   ├── store/                 # Pinia 状态管理
│   ├── components/            # 公共组件
│   ├── views/                 # 页面组件
│   ├── assets/               # 静态资源
│   ├── config/               # 配置文件
│   │   └── qiankun.js        # 微前端配置
│   └── mock/                 # 模拟数据
├── public/                   # 静态公共资源
└── vite.config.js           # Vite 配置文件
```

## 微前端架构

本项目采用 qiankun 微前端框架，实现了以下功能：

1. 主应用负责基础框架和公共功能
2. 子应用作为插件独立开发、独立部署
3. 统一的插件注册和管理机制
4. 应用间通信和状态共享

### 插件开发指南

1. 创建新的 Vue 3 + Vite 项目作为插件
2. 配置插件的入口文件，实现 qiankun 生命周期函数
3. 使用主应用提供的 `registerPlugin` 方法注册插件
4. 遵循主应用的开发规范和UI设计规范

## 开发指南

1. 安装依赖：
```bash
npm install
```

2. 启动开发服务器：
```bash
npm run dev
```

3. 构建生产版本：
```bash
npm run build
```

## IDE 支持

推荐使用 Visual Studio Code 编辑器，并安装以下插件：

- Volar：Vue 3 官方推荐的 IDE 支持插件
- ESLint：代码质量检查
- Prettier：代码格式化

更多 IDE 支持信息请参考 [Vue 文档](https://vuejs.org/guide/scaling-up/tooling.html#ide-support)。
