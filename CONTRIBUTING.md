# 贡献指南 (Contributing to Fastdotnet)

首先，感谢您对 Fastdotnet 的贡献！我们欢迎任何形式的参与，无论是报告 Bug、提出新功能建议，还是提交代码改进。

##  目录

- [行为准则](CODE_OF_CONDUCT.md)
- [如何报告 Bug](#如何报告-bug)
- [如何提出新功能](#如何提出新功能)
- [如何提交代码](#如何提交代码)
- [开发环境设置](#开发环境设置)

## 如何报告 Bug

如果您发现了 Bug，请通过 [GitHub Issues](https://github.com/CN-GodHei/fastdotnet/issues) 进行反馈。在创建 Issue 时，请使用 **Bug Report** 模板，并提供以下信息：
1. **复现步骤**：详细描述如何触发该 Bug。
2. **期望行为**：您认为程序应该如何运行。
3. **实际行为**：程序实际发生了什么。
4. **环境信息**：操作系统、.NET 版本、浏览器版本等。

## 如何提出新功能

如果您有新功能的想法，请先在 [GitHub Discussions](https://github.com/CN-GodHei/fastdotnet/discussions) 或 Issues 中与我们讨论。使用 **Feature Request** 模板可以让我们更快地理解您的需求。

## 如何提交代码

1. **Fork 仓库**：点击 GitHub 页面右上角的 "Fork" 按钮。
2. **克隆仓库**：`git clone https://github.com/your-username/fastdotnet.git`
3. **创建分支**：`git checkout -b feature/your-feature-name`
4. **进行修改**：请确保您的代码符合项目的编码规范。
5. **运行测试**：确保所有测试都能通过。
6. **提交更改**：`git commit -m "feat: add some feature"`
7. **推送分支**：`git push origin feature/your-feature-name`
8. **创建 Pull Request**：在 GitHub 上发起 PR，并描述您的更改内容。

## 开发环境设置

### 后端 (.NET)

```bash
cd backend
dotnet build
dotnet test
```

### 前端 (Vue 3)

```bash
cd Web/fastdotnet-admin
pnpm install
pnpm dev
```

## 编码规范

- **C#**：遵循 Microsoft C# 编码规范，使用 PascalCase 命名类和方法，camelCase 命名变量。
- **TypeScript/Vue**：遵循 ESLint 和 Prettier 配置。
- **注释**：请为公共 API 添加清晰的 XML 文档注释或 JSDoc 注释。

## 联系我们

如果您在贡献过程中遇到任何问题，欢迎加入我们的 QQ 交流群：**779454817**。

再次感谢您的支持！🚀
