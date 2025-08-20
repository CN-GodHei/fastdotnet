1. 当前项目使用的.net版本为 8.0
2. 当前项目使用的数据库为 MySql,调试时是sqlite
3. 当前项目ORM为sqlsugar
4. 当前项目使用Autofac作为IOC容器
5. 当前项目插件化开发，分为主框架和应用插件
6. 当前项目插件和主应用分Admin和App端，Admin是后台管理端，App是客户使用端
7. 当前项目插件和主应用针对业务接口是分开的，有些系统级的是共用的
8. 插件和主应用封装了通用控制器和通用仓储，插件和主应用都可以直接使用，在没有特殊需求的情况下，不需要自己写控制器和仓储
9. 当前项目使用Jwt作为认证和授权,根据是否需要精细化权限控制可以使用        [Authorize(Policy = Permissions.Admin.Users.View)]，\fd\src\Fastdotnet.Core\Constants\Permissions.cs里定义
10. 当前项目使用对象转换工具为AutoMapper
11. 当前项目使用分为数据库实体和传输实体，数据库实体是和数据库表对应的，传输实体是和前端交互的，传输实体可以和数据库实体进行转换
12. fd\src目录是后端的代码目录，fd\Web目录是前端的代码目录，fastdotnet-docs目录是文档目录，如果功能变更，需要在文档目录下进行更新
13. fd\Auxiliary code目录是辅助代码目录，里面是一些辅助代码，比如用到了一些开源库，你使用方式不清楚的时候可以自己下载到这个目录，或者让我拉取
14. 后端nuget包是使用中央包管理器管理的，要增加nuget包时需要注意这个问题
15. 当前项目使用git作为版本管理，如果你需要修改复杂的逻辑并不确定是否会影响其他功能，建议先创建一个新的分支，在新的分支上进行修改，修改完成后，再合并到主分支
16. 前端技术栈
17. TypeScript (作为主要语言)
18. Vite (构建工具)
19. qiankun (微前端框架)
20. Vue Router 4
21. Pinia (状态管理)
22. Element Plus (UI 组件库)
23. Axios (HTTP 客户端)，自己封装了一个，封装了请求拦截器和响应拦截器，自己定义了一个请求参数的格式，自己定义了一个响应参数的格式，自己定义了一个错误处理的格式
24. 前端代码目录结构
管理端主应用 (admin-base)
admin-base/
├── src/
│   ├── api/              # 主应用API接口
│   ├── assets/           # 静态资源
│   ├── components/       # 公共组件
│   ├── composables/      # 可组合函数
│   ├── config/           # 配置文件
│   ├── layouts/          # 布局组件
│   ├── micro-apps/       # 微前端插件配置
│   ├── router/           # 路由配置
│   ├── stores/           # 状态管理
│   ├── styles/           # 样式文件
│   ├── utils/            # 工具函数
│   ├── views/            # 本地页面视图
│   ├── App.vue           # 根组件
│   ├── main.ts           # 入口文件
│   └── micro-main.ts     # 微前端入口文件
├── public/
│   └── index.html        # 主页面
└── vite.config.ts        # Vite 配置
客户端主应用 (app-base)
app-base/
├── src/
│   ├── api/              # 主应用API接口
│   ├── assets/           # 静态资源
│   ├── components/       # 公共组件
│   ├── composables/      # 可组合函数
│   ├── config/           # 配置文件
│   ├── layouts/          # 布局组件
│   ├── micro-apps/       # 微前端插件配置
│   ├── router/           # 路由配置
│   ├── stores/           # 状态管理
│   ├── styles/           # 样式文件
│   ├── utils/            # 工具函数
│   ├── views/            # 本地页面视图
│   ├── App.vue           # 根组件
│   ├── main.ts           # 入口文件
│   └── micro-main.ts     # 微前端入口文件
├── public/
│   └── index.html        # 主页面
└── vite.config.ts        # Vite 配置
管理端插件示例
plugin-a-admin/
├── src/
│   ├── api/              # 插件API接口
│   ├── components/       # 插件组件
│   ├── composables/      # 可组合函数
│   ├── router/           # 插件路由（可选）
│   ├── stores/           # 插件状态管理
│   ├── utils/            # 工具函数
│   ├── views/            # 插件页面视图
│   ├── App.vue           # 插件根组件
│   ├── main.ts           # 插件主入口
│   └── micro-main.ts     # 微前端入口
├── public/
│   └── index.html        # 插件页面
└── vite.config.ts        # Vite 配置
客户端插件示例
plugin-a-app/
├── src/
│   ├── api/              # 插件API接口
│   ├── components/       # 插件组件
│   ├── composables/      # 可组合函数
│   ├── router/           # 插件路由（可选）
│   ├── stores/           # 插件状态管理
│   ├── utils/            # 工具函数
│   ├── views/            # 插件页面视图
│   ├── App.vue           # 插件根组件
│   ├── main.ts           # 插件主入口
│   └── micro-main.ts     # 微前端入口
├── public/
│   └── index.html        # 插件页面
└── vite.config.ts        # Vite 配置

25. qiankun 集成方案，这是一个很重要技术，需要详细说明，包括主应用和插件应用的代码结构，以及插件应用的加载和卸载机制
26. 这是一个类似php的fastadmin的框架，不过fastdotnet是.net生态的框架，所以fastdotnet的框架和php的框架有区别，但是功能和基本要和php的框架一样