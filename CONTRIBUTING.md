# 贡献指南 (Contributing to Fastdotnet)

感谢你对 Fastdotnet 的关注！无论是报告 Bug、提功能建议、改进文档，还是提交代码与插件，都是对这个项目的贡献。

在参与之前，请先阅读 [行为准则](CODE_OF_CONDUCT.md)；安全漏洞请**不要**公开提 Issue，按 [安全政策](SECURITY.md) 私下报告。

## 目录

- [我能贡献什么](#我能贡献什么)
- [开发环境准备](#开发环境准备)
- [本地运行项目](#本地运行项目)
- [分支与提交规范](#分支与提交规范)
- [Pull Request 流程](#pull-request-流程)
- [代码规范](#代码规范)
- [测试要求](#测试要求)
- [贡献插件](#贡献插件)
- [联系我们](#联系我们)

## 我能贡献什么

| 类型 | 入口 |
| --- | --- |
| 🐛 报告 Bug | [Bug Report 模板](https://github.com/CN-GodHei/fastdotnet/issues/new?template=bug_report.yml) |
| 💡 功能建议 | [Feature Request 模板](https://github.com/CN-GodHei/fastdotnet/issues/new?template=feature_request.yml) 或 [Discussions](https://github.com/CN-GodHei/fastdotnet/discussions) |
| 📖 文档改进 | 直接修改 `docs/`、`README.md` 或各模块 README，提 PR 即可 |
| 🔧 代码贡献 | 见下方流程；建议先在 Issue/Discussions 里对齐方案 |
| 🔌 插件贡献 | 见[贡献插件](#贡献插件) |
| 🔒 安全问题 | 邮件 `yunnanzuyuankeji@163.com`，勿公开 |

## 开发环境准备

| 依赖 | 版本 |
| --- | --- |
| .NET SDK | 10.0+（本项目在 `10.0.402` 验证） |
| Node.js | 20 LTS 及以上 |
| pnpm | 8+（前端包管理器） |
| Git | 任意较新版本 |

```bash
git clone https://github.com/CN-GodHei/fastdotnet.git
cd fastdotnet
```

## 本地运行项目

### 后端

```bash
cd backend
dotnet build                     # 构建整个解决方案
dotnet test                      # 运行单元测试（74 个用例）

cd Fastdotnet.WebApi
dotnet run                       # Development 环境默认使用 SQLite，无需准备数据库
# Swagger: http://localhost:18889/swagger
```

首次启动会自动建表并初始化超级管理员 `superadmin` / `123456`（请在登录后立即修改）。

### 前端

```bash
cd Web/fastdotnet-admin
pnpm install
pnpm dev                         # http://localhost:18888

cd Web/fastdotnet-app
pnpm install && pnpm dev         # http://localhost:18887
```

### 文档站

```bash
cd docs
pnpm install
pnpm dev                         # VitePress 本地预览
```

## 分支与提交规范

- 从 `develop` 切出特性分支，命名建议：`feat/xxx`、`fix/xxx`、`docs/xxx`
- 提交信息遵循 [Conventional Commits](https://www.conventionalcommits.org/zh-hans/)：

```
feat: 新增插件热更新事件通知
fix: 修复插件卸载后静态资源 404
docs: 补充事件总线使用说明
style: 统一代码格式
refactor: 重构缓存键生成逻辑
test: 补充 CacheKeyGenerator 单元测试
```

- 一个 PR 尽量只做一件事；避免混入无关的格式化改动

## Pull Request 流程

1. Fork 仓库并克隆到本地
2. 从 `develop` 创建分支：`git checkout -b feat/your-feature`
3. 完成修改，确保本地构建与测试通过
4. 推送分支并创建 PR，目标分支为 **`develop`**
5. 在 PR 描述中说明：**改了什么、为什么改、如何验证**
6. 等待 Review，按意见迭代

**PR 自检清单：**

- [ ] `cd backend && dotnet build` 无错误、无新增警告
- [ ] `dotnet test` 全部通过
- [ ] 前端改动已本地跑通，无控制台报错
- [ ] 新增/变更的公共 API 已补 XML 文档注释
- [ ] 涉及界面或行为变化的，已在 PR 中附截图/说明
- [ ] 未提交本地配置、密钥、`bin/`、`obj/`、`node_modules/` 等产物

## 代码规范

- **C#**：遵循 Microsoft C# 编码规范；类/方法用 `PascalCase`，局部变量用 `camelCase`；公共成员补 XML 注释
- **TypeScript / Vue**：遵循仓库内的 ESLint + Prettier 配置；组件使用 `<script setup lang="ts">`
- **分层约束**：Controller / Service 层通过 Service 访问数据，不要跨层直接调用 `IRepository`（见提交 `d70ea99` 的约定）
- **注释**：解释「为什么」，而不是复述「做了什么」

## 测试要求

- 新增公共工具类、算法、缓存键生成等纯逻辑，请补充单元测试（项目：`backend/Fastdotnet.Core.Tests`）
- 修复 Bug 时，建议先补一个能复现问题的用例
- 插件相关改动，请至少手动验证：加载 → 启用 → 停用 → 卸载 全流程

## 贡献插件

1. 在 `backend/Plugins` 下创建插件项目，实现 `IPlugin` 或继承 `PluginBase`
2. 在 `.csproj` 中配置唯一的 `<PluginId>`
3. 参照 [PluginA 示例](backend/Plugins/PluginA) 组织 Backend / Frontend 目录
4. 详细规范见 [插件开发文档](docs/05-插件开发/index.md)

如果你希望把插件上架到商城，请先通过 [商城入驻](https://fastdotnet.top/marketplace/seller) 了解审核与授权要求。

## 联系我们

- 💬 QQ 交流群：[779454817](https://qm.qq.com/q/779454817)
- 🗣️ [GitHub Discussions](https://github.com/CN-GodHei/fastdotnet/discussions)
- 📧 邮箱：yunnanzuyuankeji@163.com

再次感谢你的支持！🚀
