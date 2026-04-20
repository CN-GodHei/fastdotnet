# 前端开发指南

本章节详细介绍 Fastdotnet 前端开发的核心概念和最佳实践，包括项目架构、技术栈、插件系统、路由管理等。

---

## 📖 本章内容

- [项目概述](#📋-项目概述) - 技术栈和项目结构
- [开发环境](#💻-开发环境) - 环境配置和启动
- [核心技术](#🔧-核心技术) - Vue3、TypeScript、Element Plus
- [插件系统](#🔌-插件系统-⭐) - 微前端架构和插件管理 ⭐
- [路由管理](#🗺️-路由管理) - 动态路由和权限控制
- [状态管理](#📦-状态管理) - Pinia Store 使用
- [API 调用](#🌐-api-调用) - 请求封装和安全机制
- [UI 组件](#🎨-ui-组件) - 常用组件和自定义组件
- [主题配置](#🎭-主题配置) - 动态主题和后端配置
- [最佳实践](#💡-最佳实践) - 代码规范和性能优化

---

## 📋 项目概述

### 技术栈

Fastdotnet 前端基于 **Vue Next Admin** 框架二次开发，采用现代化的前端技术栈：

| 技术 | 版本 | 用途 |
|------|------|------|
| **Vue** | ^3.4.21 | 核心框架 |
| **TypeScript** | Latest | 类型安全 |
| **Vite** | ^7.2.4 | 构建工具 |
| **Element Plus** | ^2.6.1 | UI 组件库 |
| **Pinia** | ^2.1.7 | 状态管理 |
| **Vue Router** | ^4.3.0 | 路由管理 |
| **Axios** | ^1.6.8 | HTTP 客户端 |
| **Qiankun** | ^2.10.16 | 微前端框架 |
| **SignalR** | ^9.0.6 | 实时通信 |
| **ECharts** | ^5.5.0 | 数据可视化 |
| **WangEditor** | ^5.1.23 | 富文本编辑器 |
| **Monaco Editor** | ^0.54.0 | 代码编辑器 |

### 项目结构

```
fastdotnet-admin/
├── src/
│   ├── api/                  # API 接口定义
│   │   └── fd-system-api-admin/  # 系统管理 API
│   ├── assets/               # 静态资源
│   ├── components/           # 公共组件
│   ├── directive/            # 自定义指令
│   ├── i18n/                 # 国际化
│   ├── layout/               # 布局组件
│   ├── plugins/              # 插件系统核心 ⭐
│   │   ├── PluginAPI.ts      # 插件 API
│   │   ├── PluginLoader.ts   # 插件加载器
│   │   ├── PluginManager.ts  # 插件管理器
│   │   └── PluginRegistry.ts # 插件注册中心
│   ├── router/               # 路由配置
│   │   ├── backEnd.ts        # 后端控制路由
│   │   ├── frontEnd.ts       # 前端控制路由
│   │   ├── index.ts          # 路由主文件
│   │   └── route.ts          # 静态路由
│   ├── services/             # 业务服务
│   │   └── uploadService.ts  # 上传服务
│   ├── stores/               # Pinia 状态管理
│   │   ├── microApps.ts      # 微应用配置
│   │   ├── plugin.ts         # 插件状态
│   │   ├── themeConfig.ts    # 主题配置
│   │   └── userInfo.ts       # 用户信息
│   ├── theme/                # 主题样式
│   ├── types/                # TypeScript 类型定义
│   ├── utils/                # 工具函数
│   │   ├── request.ts        # Axios 封装
│   │   ├── signalr.ts        # SignalR 封装
│   │   ├── storage.ts        # 本地存储
│   │   └── queryBuilder.ts   # 查询构建器
│   ├── views/                # 页面视图
│   ├── App.vue               # 根组件
│   └── main.ts               # 入口文件
├── .env                      # 环境变量
├── .env.development          # 开发环境配置
├── .env.production           # 生产环境配置
├── vite.config.ts            # Vite 配置
├── package.json              # 依赖配置
└── tsconfig.json             # TypeScript 配置
```

---

## 💻 开发环境

### 环境要求

- **Node.js**: >= 18.0.0
- **pnpm**: >= 8.0.0（推荐使用 pnpm）
- **浏览器**: Chrome 90+、Edge 90+、Firefox 88+

### 安装依赖

```bash
# 进入项目目录
cd fastdotnet-admin

# 安装依赖（推荐使用 pnpm）
pnpm install

# 或使用 npm
npm install
```

### 启动开发服务器

```bash
# 启动开发服务器
pnpm dev

# 或
npm run dev
```

默认访问地址：`http://localhost:8080`

### 环境变量配置

**开发环境** (`.env.development`)：

```env
ENV = development
VITE_API_URL = http://localhost:18889
VITE_SYSTEM_CATEGORY = "Admin"
```

**生产环境** (`.env.production`)：

```env
ENV = production
VITE_API_URL = /api
VITE_SYSTEM_CATEGORY = "Admin"
VITE_PUBLIC_PATH = /
```

### 代理配置

在 `vite.config.ts` 中配置了开发环境的代理：

```typescript
server: {
  proxy: {
    // API 代理
    '/api': {
      target: env.VITE_API_URL,
      ws: true,
      changeOrigin: true,
    },
    // SignalR WebSocket 代理
    '/api/universalhub': {
      target: env.VITE_API_URL,
      ws: true,
      changeOrigin: true,
    },
    // 插件静态文件代理
    '/plugins/': {
      target: env.VITE_API_URL,
      changeOrigin: true,
    },
  }
}
```

---

## 🔧 核心技术

### Vue 3 Composition API

Fastdotnet 全面采用 Vue 3 Composition API：

```vue
<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/userInfo'

// 响应式数据
const loading = ref(false)
const form = reactive({
  username: '',
  password: ''
})

// 计算属性
const isValid = computed(() => {
  return form.username && form.password
})

// 生命周期
onMounted(() => {
  console.log('组件已挂载')
})

// 方法
const handleSubmit = async () => {
  loading.value = true
  try {
    // 提交逻辑
  } finally {
    loading.value = false
  }
}
</script>
```

### TypeScript 类型安全

所有代码都使用 TypeScript 编写，提供完整的类型支持：

```typescript
// 定义接口
interface UserInfo {
  id: string
  username: string
  roles: string[]
  permissions: string[]
}

// 使用类型
const user = ref<UserInfo | null>(null)

// API 返回类型
interface ApiResponse<T> {
  Code: number
  Msg: string
  Data: T
}
```

### Element Plus 组件库

项目深度集成 Element Plus，提供了丰富的 UI 组件：

```vue
<template>
  <el-table :data="tableData" v-loading="loading">
    <el-table-column prop="name" label="名称" />
    <el-table-column prop="status" label="状态">
      <template #default="{ row }">
        <el-tag :type="row.status === 1 ? 'success' : 'danger'">
          {{ row.status === 1 ? '启用' : '禁用' }}
        </el-tag>
      </template>
    </el-table-column>
    <el-table-column label="操作">
      <template #default="{ row }">
        <el-button type="primary" size="small" @click="handleEdit(row)">
          编辑
        </el-button>
        <el-button type="danger" size="small" @click="handleDelete(row)">
          删除
        </el-button>
      </template>
    </el-table-column>
  </el-table>
</template>
```

---

## 🔌 插件系统 ⭐

Fastdotnet 采用 **Qiankun 微前端架构**，支持插件化开发，这是项目的核心特色。

### 架构设计

```
┌─────────────────────────────────────┐
│         主应用 (fastdotnet-admin)    │
│  ┌──────────────────────────────┐   │
│  │   Plugin Registry            │   │
│  │   Plugin Manager             │   │
│  │   Plugin API                 │   │
│  └──────────────────────────────┘   │
│                                     │
│  ┌─────────┐ ┌─────────┐           │
│  │ PluginA │ │ PluginB │  ...      │
│  │ (微应用) │ │ (微应用) │           │
│  └─────────┘ └─────────┘           │
└─────────────────────────────────────┘
```

### 插件注册中心 (PluginRegistry)

负责插件的注册、发现和管理：

```typescript
import { pluginRegistry } from '@/plugins/PluginRegistry'

// 获取所有插件
const allPlugins = pluginRegistry.getAllPlugins()

// 搜索插件
const activePlugins = pluginRegistry.searchPlugins(
  plugin => plugin.enabled
)

// 检查插件是否存在
if (pluginRegistry.hasPlugin('PluginA')) {
  console.log('PluginA 已注册')
}
```

### 插件管理器 (PluginManager)

负责插件的加载、初始化和生命周期管理：

```typescript
import { pluginManager } from '@/plugins/PluginManager'

// 预加载所有插件（用于 UI 组件注册）
await pluginManager.preloadAllPluginsForUIRegistration()

// 手动加载插件
await pluginManager.loadPlugin('PluginA')

// 卸载插件
await pluginManager.unloadPlugin('PluginA')
```

### 插件 API (PluginAPI)

为主应用和子应用提供统一的通信接口：

```typescript
import { pluginAPI } from '@/plugins/PluginAPI'

// 注册插件组件
pluginAPI.registerComponent('PluginA', 'MyComponent', component)

// 获取插件组件
const MyComponent = pluginAPI.getComponent('PluginA', 'MyComponent')

// 跨插件调用
pluginAPI.callPluginMethod('PluginA', 'someMethod', args)
```

### 微应用配置

微应用通过菜单配置自动加载：

```typescript
// 从菜单中提取微应用配置
const extractMicroApps = (menus: any[]) => {
  for (const menu of menus) {
    if (menu.IsFdMicroApp && menu.PluginId) {
      const appName = `${menu.PluginId}`
      const activeRule = `/micro/${menu.PluginId}`
      
      microAppConfigs.set(menu.PluginId, {
        name: appName,
        entry: import.meta.env.VITE_API_URL + "/plugins/" + menu.PluginId + "/admin/index.html",
        container: '#subapp-viewport',
        activeRule: activeRule,
        props: {
          base: activeRule,
          FdRequest: request,              // 共享 Axios 实例
          FdQueryBuilder: { buildMixedQuery }, // 共享查询构建器
          signalRManager: baseSignalRManager,  // 共享 SignalR
          uploadService: uploadService,        // 共享上传服务
          menuInfo: {
            isKeepAlive: menu.IsKeepAlive
          }
        }
      })
    }
  }
}
```

### 开发调试模式

在开发环境中，可以为特定插件指定本地开发服务器：

```typescript
const getDebugEntry = (pluginId: string) => {
  const isDevelopment = import.meta.env.MODE === 'development'
  
  if (isDevelopment) {
    // 为特定插件指定本地开发服务器
    if (pluginId === 'RichTextEditor') {
      return '//localhost:8090'
    }
    // 添加更多插件的调试入口
  }
  return null
}
```

### 创建插件

详细教程请参考：[插件开发指南](../05-插件开发/)

---

## 🗺️ 路由管理

### 路由模式

Fastdotnet 支持两种路由模式：

1. **后端控制路由**（推荐）：路由由后端动态返回，支持权限控制
2. **前端控制路由**：路由在前端静态配置

通过 `themeConfig.isRequestRoutes` 配置切换。

### 后端控制路由

**工作流程：**

```
用户登录 → 获取菜单树 → 解析路由 → 动态添加路由 → 渲染菜单
```

**实现代码：**

```typescript
// src/router/backEnd.ts
export async function initBackEndControlRoutes() {
  // 1. 从后端获取菜单树
  const menuResponse = await getApiAdminFdMenuTree()
  const allMenus = Array.isArray(menuResponse) ? menuResponse : (menuResponse.data || [])
  
  // 2. 解析菜单为路由
  const routes = parseMenusToRoutes(allMenus)
  
  // 3. 动态添加路由
  routes.forEach(route => {
    router.addRoute(route)
  })
  
  // 4. 保存路由列表到 Store
  const storesRoutesList = useRoutesList()
  storesRoutesList.setRoutesList(routes)
}
```

**菜单数据结构：**

```typescript
interface MenuItem {
  Id: string
  Name: string
  Path: string
  Component: string
  Icon: string
  IsFdMicroApp: boolean    // 是否为微应用
  PluginId: string         // 插件ID
  IsKeepAlive: boolean     // 是否缓存
  Children: MenuItem[]
}
```

### 路由守卫

```typescript
router.beforeEach(async (to, from, next) => {
  NProgress.start()
  
  const token = Session.get('token')
  
  // 未登录跳转到登录页
  if (!token) {
    next(`/login?redirect=${to.path}`)
    NProgress.done()
    return
  }
  
  // 已登录访问登录页，跳转到首页
  if (token && to.path === '/login') {
    next('/home')
    NProgress.done()
    return
  }
  
  // 初始化路由
  const storesRoutesList = useRoutesList()
  if (storesRoutesList.routesList.length === 0) {
    if (isRequestRoutes) {
      // 后端控制路由
      await initBackEndControlRoutes()
      next({ path: to.path, query: to.query })
    } else {
      // 前端控制路由
      await initFrontEndControlRoutes()
      next({ path: to.path, query: to.query })
    }
  } else {
    next()
  }
})

router.afterEach(() => {
  NProgress.done()
})
```

### Keep-Alive 缓存

支持二级路由的 keep-alive 缓存：

```typescript
// 在路由 meta 中标记
{
  path: '/system/user',
  name: 'SystemUser',
  meta: {
    isKeepAlive: true  // 开启缓存
  }
}

// 路由处理时收集需要缓存的组件
if (newArr[0].meta.isKeepAlive && v.meta.isKeepAlive) {
  cacheList.push(v.name)
  const stores = useKeepALiveNames()
  stores.setCacheKeepAlive(cacheList)
}
```

---

## 📦 状态管理

使用 **Pinia** 进行状态管理，替代传统的 Vuex。

### Store 结构

```
src/stores/
├── index.ts              # Pinia 实例
├── userInfo.ts           # 用户信息
├── themeConfig.ts        # 主题配置
├── routesList.ts         # 路由列表
├── keepAliveNames.ts     # Keep-Alive 缓存
├── tagsViewRoutes.ts     # TagsView 标签页
├── microApps.ts          # 微应用配置
└── plugin.ts             # 插件状态
```

### 用户信息 Store

```typescript
// src/stores/userInfo.ts
import { defineStore } from 'pinia'

export const useUserInfo = defineStore('userInfo', {
  state: () => ({
    userInfos: {
      id: '',
      username: '',
      roles: [],
      permissions: []
    }
  }),
  actions: {
    setUserInfos(data: any) {
      this.userInfos = data
    },
    clearUserInfos() {
      this.userInfos = {
        id: '',
        username: '',
        roles: [],
        permissions: []
      }
    }
  },
  persist: {
    key: 'userInfo',
    storage: localStorage
  }
})
```

**使用 Store：**

```vue
<script setup lang="ts">
import { useUserInfo } from '@/stores/userInfo'
import { storeToRefs } from 'pinia'

const userStore = useUserInfo()
const { userInfos } = storeToRefs(userStore)

// 读取数据
console.log(userInfos.value.username)

// 修改数据
userStore.setUserInfos({
  id: '123',
  username: 'admin',
  roles: ['admin'],
  permissions: ['*']
})
</script>
```

### 主题配置 Store

主题配置支持从后端动态获取：

```typescript
// src/stores/themeConfig.ts
export const useThemeConfig = defineStore('themeConfig', {
  state: (): ThemeConfigState => ({
    themeConfig: {
      primary: '#0F59A4',
      isIsDark: false,
      layout: 'defaults',
      isRequestRoutes: true,
      globalTitle: 'Fastdotnet',
      additionalConfig: {}  // 后端动态配置
    }
  }),
  actions: {
    // 从后端获取配置
    async setThemeConfigFromBackend() {
      const response = await getApiAdminFdSystemInfoConfigPublicAll()
      const configData = response
      
      if (configData) {
        const updatedConfig = { ...this.themeConfig }
        
        // 遍历后端配置，更新对应字段
        for (const [key, value] of Object.entries(configData)) {
          if (this.themeConfig.hasOwnProperty(key)) {
            (updatedConfig as any)[key] = value
          } else {
            // 存储额外配置
            updatedConfig.additionalConfig[key] = value
          }
        }
        
        this.themeConfig = updatedConfig
      }
    },
    
    // 获取配置值
    getConfigValue(key: string) {
      if (this.themeConfig.hasOwnProperty(key)) {
        return (this.themeConfig as any)[key]
      }
      return this.themeConfig.additionalConfig?.[key]
    }
  },
  persist: {
    key: 'themeConfig',
    storage: localStorage,
    paths: ['themeConfig']
  }
})

// 初始化主题配置
export async function initializeThemeConfig() {
  const themeConfigStore = useThemeConfig()
  await themeConfigStore.setThemeConfigFromBackend()
}
```

### 持久化配置

使用 `pinia-plugin-persistedstate` 实现状态持久化：

```typescript
// src/stores/index.ts
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

export default pinia
```

---

## 🌐 API 调用

### Axios 封装

项目对 Axios 进行了完整封装，包含以下特性：

- ✅ **防重放攻击**：HMAC-SHA256 签名
- ✅ **时间同步**：自动校准服务器时间
- ✅ **自动重试**：防重放失败后自动重试
- ✅ **Token 管理**：自动携带 Token
- ✅ **错误处理**：统一错误提示
- ✅ **数据加密**：支持 RSA 加密传输

**文件位置：** `src/utils/request.ts`

### 基本用法

```typescript
import request from '@/utils/request'

// GET 请求
const getUsers = () => {
  return request({
    url: '/api/admin/FdUser/GetPage',
    method: 'get',
    params: {
      pageIndex: 1,
      pageSize: 10
    }
  })
}

// POST 请求
const createUser = (data: any) => {
  return request({
    url: '/api/admin/FdUser/Create',
    method: 'post',
    data
  })
}

// 使用
const users = await getUsers()
```

### 防重放机制

**签名生成流程：**

```typescript
// 1. 生成时间戳和随机数
const timestamp = timeSyncService.getServerTimestampInSeconds()
const nonce = `${generateUUID()}-${Date.now()}`

// 2. 构建签名字符串
const method = config.method?.toUpperCase() || 'GET'
const path = config.url || ''
const body = JSON.stringify(config.data) || ''

const signContent = `${timestamp}|${nonce}|${method}|${path}|${body}`

// 3. 生成 HMAC-SHA256 签名
const signature = CryptoJS.HmacSHA256(signContent, secretKey)
  .toString(CryptoJS.enc.Base64)

// 4. 添加到请求头
config.headers!['X-Timestamp'] = timestamp.toString()
config.headers!['X-Nonce'] = nonce
config.headers!['X-Signature'] = signature
```

**时间同步服务：**

```typescript
class TimeSyncService {
  private offset: number = 0
  
  // 精确同步（考虑 RTT）
  async syncTime(force: boolean = false): Promise<void> {
    const localSendTime = Date.now()
    const response = await fetch('/api/GetServiceDateTime')
    const serverTime = parseInt(response.headers.get('X-Server-Timestamp'))
    const localRecvTime = Date.now()
    const rtt = localRecvTime - localSendTime
    
    // 核心算法：offset = ServerTime - (SendTime + RTT/2)
    this.offset = serverTime - (localSendTime + rtt / 2)
  }
  
  // 获取服务器时间戳
  getServerTimestampInSeconds(): number {
    return Math.floor((Date.now() + this.offset) / 1000)
  }
}
```

### 自动重试机制

当遇到防重放错误（408/409）时，自动同步时间并重试：

```typescript
service.interceptors.response.use(
  (response) => {
    const status = response.status
    
    // 防重放错误处理
    if (status === 408 || status === 409) {
      const currentRetryCount = (response.config as any).__retryCount || 0
      
      if (currentRetryCount < MAX_RETRY_COUNT) {
        console.log(`[Anti-Replay] 尝试第 ${currentRetryCount + 1} 次自动重试...`)
        
        // 标记重试次数
        ;(response.config as any).__retryCount = currentRetryCount + 1
        
        // 强制同步时间并重试
        return timeSyncService.syncTime(true).then(() => {
          return service(response.config)
        })
      } else {
        ElMessage.error('自动重试失败，请刷新页面')
        return Promise.reject(response)
      }
    }
    
    return response.data.Data
  }
)
```

### 错误处理

```typescript
service.interceptors.response.use(
  (response) => {
    const res = response.data
    const status = response.status
    
    if (status === 200) {
      // 业务错误
      if (res.Code !== 0) {
        if (res.Code === 401 || res.Code === 4001) {
          handleLogout()  // 登录过期
          return Promise.reject(new Error('登录已过期'))
        }
        ElMessage.error(res.Msg || `Error Code: ${res.Code}`)
        return Promise.reject(new Error(res.Msg))
      }
      return res.Data
    }
    
    // 401 HTTP 状态码
    if (status === 401) {
      handleLogout()
      return Promise.reject(response)
    }
    
    // 422 验证错误
    if (status === 422) {
      ElMessage.error(res.msg || '验证错误')
      return Promise.reject({ message: res.msg, code: 422 })
    }
  },
  (error) => {
    // 网络错误
    if (error.request) {
      ElMessage.error('网络断开，请检查连接')
    } else {
      ElMessage.error('请求配置错误')
    }
    return Promise.reject(error)
  }
)
```

### 查询构建器

提供强大的动态查询构建功能：

```typescript
import { buildMixedQuery } from '@/utils/queryBuilder'

// 构建查询条件
const query = buildMixedQuery({
  conditions: [
    {
      propertyName: 'Username',
      operator: 'Contains',
      value: 'admin'
    },
    {
      propertyName: 'Status',
      operator: 'Equal',
      value: 1
    },
    {
      propertyName: 'CreatedAt',
      operator: 'Between',
      value: '2024-01-01',
      value2: '2024-12-31'
    }
  ],
  logic: 'And',
  orderBy: 'CreatedAt',
  orderType: 'Desc'
})

// 发送到后端
const result = await request({
  url: '/api/admin/FdUser/Search',
  method: 'post',
  data: query
})
```

**支持的操作符：**

| 操作符 | 说明 | 示例 |
|--------|------|------|
| `Equal` | 等于 | Status = 1 |
| `NotEqual` | 不等于 | Status != 0 |
| `GreaterThan` | 大于 | Age > 18 |
| `LessThan` | 小于 | Age < 60 |
| `Contains` | 包含 | Username LIKE '%admin%' |
| `StartsWith` | 开头 | Username LIKE 'admin%' |
| `In` | 在列表中 | Status IN (0, 1) |
| `Between` | 范围 | CreatedAt BETWEEN ... AND ... |

---

## 🎨 UI 组件

### 公共组件

项目提供了一系列公共组件，位于 `src/components/` 目录：

- **auth** - 权限控制组件
- **cropper** - 图片裁剪组件
- **editor** - 富文本编辑器
- **iconSelector** - 图标选择器
- **noticeBar** - 滚动通知栏
- **svgIcon** - SVG 图标组件
- **table** - 表格增强组件
- **upload** - 文件上传组件

### 权限控制组件

```vue
<template>
  <!-- 根据权限显示内容 -->
  <Auth value="system:user:add">
    <el-button type="primary" @click="handleAdd">新增</el-button>
  </Auth>
  
  <!-- 多个权限（满足一个即可） -->
  <Auth :value="['system:user:add', 'system:user:edit']">
    <el-button type="primary">操作</el-button>
  </Auth>
</template>
```

### 图标选择器

```vue
<template>
  <IconSelector v-model="icon" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import IconSelector from '@/components/iconSelector/index.vue'

const icon = ref('el-icon-user')
</script>
```

### 富文本编辑器

```vue
<template>
  <WangEditor v-model="content" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import WangEditor from '@/components/editor/wangEditor.vue'

const content = ref('<p>Hello World</p>')
</script>
```

---

## 🎭 主题配置

### 动态主题

支持从后端动态获取主题配置：

```typescript
// 在 main.ts 中初始化
import { initializeThemeConfig } from '@/stores/themeConfig'

initializeThemeConfig().then(() => {
  console.log('[MainApp] Theme configuration initialized from backend')
}).catch(error => {
  console.error('[MainApp] Failed to initialize theme configuration:', error)
})
```

### 主题配置项

```typescript
interface ThemeConfig {
  // 全局主题
  primary: string              // 主题色
  isIsDark: boolean            // 深色模式
  
  // 顶栏设置
  topBar: string               // 顶栏背景色
  topBarColor: string          // 顶栏字体颜色
  
  // 菜单设置
  menuBar: string              // 菜单背景色
  menuBarColor: string         // 菜单字体颜色
  
  // 界面设置
  isCollapse: boolean          // 菜单折叠
  isUniqueOpened: boolean      // 手风琴效果
  isFixedHeader: boolean       // 固定 Header
  
  // 界面显示
  isShowLogo: boolean          // 显示 Logo
  isBreadcrumb: boolean        // 显示面包屑
  isTagsview: boolean          // 显示标签页
  isFooter: boolean            // 显示页脚
  
  // 布局切换
  layout: 'defaults' | 'classic' | 'transverse' | 'columns'
  
  // 其他
  globalTitle: string          // 网站标题
  globalI18n: 'zh-cn' | 'en'   // 语言
}
```

### 切换主题

```vue
<script setup lang="ts">
import { useThemeConfig } from '@/stores/themeConfig'

const themeStore = useThemeConfig()

// 切换深色模式
const toggleDark = () => {
  themeStore.themeConfig.isIsDark = !themeStore.themeConfig.isIsDark
}

// 切换布局
const changeLayout = (layout: string) => {
  themeStore.themeConfig.layout = layout
}
</script>
```

---

## 💡 最佳实践

### 1. 代码规范

```typescript
✅ 推荐：
- 使用 TypeScript 编写所有代码
- 使用 Composition API（<script setup>）
- 组件命名使用 PascalCase
- 文件名使用 kebab-case
- 接口命名以 I 开头（可选）

❌ 避免：
- 使用 any 类型
- 直接修改 Props
- 在模板中使用复杂表达式
- 忘记清理定时器/事件监听器
```

### 2. 组件开发

```vue
<script setup lang="ts">
import { ref, computed, watch } from 'vue'

// 定义 Props
interface Props {
  title: string
  visible?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  visible: false
})

// 定义 Emits
const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'confirm'): void
}>()

// 响应式数据
const loading = ref(false)

// 计算属性
const isVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

// 方法
const handleConfirm = async () => {
  loading.value = true
  try {
    // 业务逻辑
    emit('confirm')
  } finally {
    loading.value = false
  }
}

// 监听器
watch(() => props.visible, (val) => {
  if (val) {
    // 打开时的逻辑
  }
})
</script>
```

### 3. API 调用

```typescript
✅ 推荐：
// 在 api 目录统一管理
// src/api/fd-system-api-admin/FdUser.ts
export const getApiAdminFdUserPage = (params: any) => {
  return request({
    url: '/api/admin/FdUser/GetPage',
    method: 'get',
    params
  })
}

// 在组件中使用
import { getApiAdminFdUserPage } from '@/api/fd-system-api-admin/FdUser'

const loadData = async () => {
  loading.value = true
  try {
    const data = await getApiAdminFdUserPage({
      pageIndex: 1,
      pageSize: 10
    })
    tableData.value = data.Items
    total.value = data.Total
  } catch (error) {
    console.error('加载失败', error)
  } finally {
    loading.value = false
  }
}

❌ 避免：
// 不要在组件中直接写 URL
const loadData = async () => {
  const data = await axios.get('/api/admin/FdUser/GetPage')
}
```

### 4. 性能优化

```typescript
✅ 推荐：
// 1. 路由懒加载
const UserList = () => import('@/views/system/user/index.vue')

// 2. 大组件异步加载
const HeavyComponent = defineAsyncComponent(
  () => import('@/components/HeavyComponent.vue')
)

// 3. 列表虚拟化（大数据量）
import { useVirtualList } from '@vueuse/core'

// 4. 防抖/节流
import { useDebounceFn } from '@vueuse/core'
const handleSearch = useDebounceFn((keyword: string) => {
  // 搜索逻辑
}, 300)

// 5. 合理使用 Keep-Alive
{
  path: '/system/user',
  name: 'SystemUser',
  meta: {
    isKeepAlive: true  // 仅对频繁访问的页面开启
  }
}

❌ 避免：
// 不要一次性加载所有组件
import UserList from '@/views/system/user/index.vue'
import RoleList from '@/views/system/role/index.vue'
// ... 更多导入
```

### 5. 错误处理

```typescript
✅ 推荐：
const handleSubmit = async () => {
  try {
    loading.value = true
    await submitForm()
    ElMessage.success('操作成功')
  } catch (error: any) {
    // 422 验证错误已在拦截器中处理
    if (error.code !== 422) {
      ElMessage.error(error.message || '操作失败')
    }
  } finally {
    loading.value = false
  }
}

❌ 避免：
const handleSubmit = async () => {
  await submitForm()  // 没有错误处理
  ElMessage.success('操作成功')
}
```

### 6. 插件开发

```typescript
✅ 推荐：
// 1. 使用共享的 Axios 实例（通过 props 传递）
const request = props.FdRequest

// 2. 使用共享的查询构建器
const { buildMixedQuery } = props.FdQueryBuilder

// 3. 使用共享的 SignalR
const signalR = props.signalRManager

// 4. 使用共享的上传服务
const upload = props.uploadService

// 5. 遵循微应用生命周期
export async function bootstrap() {
  console.log('PluginA bootstrapped')
}

export async function mount(props: any) {
  console.log('PluginA mounted', props)
  render(props)
}

export async function unmount() {
  console.log('PluginA unmounted')
  instance.unmount()
}

❌ 避免：
// 不要在插件中创建独立的 Axios 实例
const axios = axios.create({ baseURL: '/api' })

// 不要硬编码 API 地址
fetch('http://localhost:18889/api/...')
```

---

## ❓ 常见问题

### Q: 如何调试微应用？

**A**: 在 `main.ts` 的 `getDebugEntry` 函数中配置本地开发服务器：

```typescript
const getDebugEntry = (pluginId: string) => {
  if (import.meta.env.MODE === 'development') {
    if (pluginId === 'YourPluginId') {
      return '//localhost:8090'  // 你的插件开发服务器地址
    }
  }
  return null
}
```

### Q: 路由刷新后 404？

**A**: 确保 `vite.config.ts` 中设置了 `appType: 'spa'`：

```typescript
export default defineConfig({
  appType: 'spa',  // 关键配置
  // ...
})
```

### Q: 如何清除缓存？

**A**: 
```typescript
// 清除 localStorage
localStorage.clear()

// 清除特定 Store
const themeStore = useThemeConfig()
themeStore.$reset()

// 清除 Keep-Alive 缓存
const keepAliveStore = useKeepALiveNames()
keepAliveStore.setCacheKeepAlive([])
```

### Q: 如何实现权限控制？

**A**: 使用 `<Auth>` 组件或 `v-auth` 指令：

```vue
<!-- 组件方式 -->
<Auth value="system:user:add">
  <el-button>新增</el-button>
</Auth>

<!-- 指令方式 -->
<el-button v-auth="system:user:add">新增</el-button>
```

### Q: 如何自定义主题色？

**A**: 在后端系统配置中修改 `PrimaryColor`，前端会自动同步。

---

## 🔗 相关链接

- [Vue 3 官方文档](https://cn.vuejs.org/)
- [Element Plus 文档](https://element-plus.org/zh-CN/)
- [Pinia 文档](https://pinia.vuejs.org/zh/)
- [Qiankun 微前端](https://qiankun.umijs.org/zh)
- [Vite 官方文档](https://cn.vitejs.dev/)
- [后端开发指南](../03-后端开发/)
- [插件开发指南](../05-插件开发/)
