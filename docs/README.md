# Fastdotnet 文档项目

这是 Fastdotnet 快速开发框架的官方文档，使用 VitePress 构建。

## 📖 在线文档

访问 [https://docs.fastdotnet.top](https://docs.fastdotnet.top) 查看最新文档。

## 🚀 本地开发

### 安装依赖

```bash
pnpm install
```

### 启动开发服务器

```bash
pnpm run docs:dev
```

访问 http://localhost:5173 预览文档（端口可能自动调整）。

### 构建生产版本

```bash
pnpm run docs:build
```

构建产物位于 `.vitepress/dist/` 目录。

### 预览生产构建

```bash
pnpm run docs:preview
```

## 📝 文档结构

```
docs/
├── .vitepress/          # VitePress 配置
│   ├── config.js        # 站点配置
│   └── dist/            # 构建产物（不提交到 Git）
├── public/              # 静态资源
├── index.md             # 首页
├── 01-快速开始/         # 新手入门
├── 02-核心概念/         # 核心概念讲解
├── 03-后端开发/         # 后端详细指南
├── 04-前端开发/         # 前端详细指南
├── 05-插件开发/         # 插件开发专题
├── 06-高级主题/         # 进阶内容
└── 07-API参考/          # API 文档
```

## ✍️ 编写文档

### 文档命名规范

- **使用中文文件名**：便于查找和维护
- **简洁明了**：文件名能清晰表达内容
- **统一格式**：使用 `-` 连接多个词

示例：
```
环境准备.md
安装部署.md
创建第一个插件.md  # Fastdotnet 采用插件化架构，所有功能都以插件形式开发
```

### Markdown 扩展

VitePress 支持标准 Markdown 语法，并提供了以下扩展：

#### 1. 容器语法

```markdown
::: tip 提示
这是一个提示信息
:::

::: warning 注意
这是一个警告信息
:::

::: danger 危险
这是一个危险信息
:::

::: details 详情
这是一个可折叠的详情块
:::
```

#### 2. 代码块高亮

````markdown
```csharp
public class UserService
{
    public async Task<User> GetUserAsync(int id)
    {
        return await _db.Queryable<User>()
            .Where(u => u.Id == id)
            .FirstAsync();
    }
}
```
````

#### 3. Frontmatter

在每个文档开头可以添加元数据：

```yaml
---
title: 页面标题
description: 页面描述
outline: deep  # 显示多级大纲
---
```

### 添加新页面

1. 在对应目录创建 `.md` 文件
2. 在 `.vitepress/config.js` 中添加侧边栏配置
3. 确保链接正确

示例：

```javascript
// .vitepress/config.js
sidebar: {
  '/01-快速开始/': [
    {
      text: '快速开始',
      items: [
        { text: '介绍', link: '/01-快速开始/index' },
        { text: '新页面', link: '/01-快速开始/新页面' }  // ← 添加这里
      ]
    }
  ]
}
```

## 🌐 部署

### GitHub Pages

推送到 `main` 分支时，GitHub Actions 会自动构建并部署文档。

### 自定义域名

在 DNS 中设置 `docs.fastdotnet.top` 指向部署服务器。

## 📋 最佳实践

### 1. 内容组织

- **由浅入深**：从入门到进阶，循序渐进
- **示例驱动**：提供完整可运行的代码示例
- **图文并茂**：复杂流程配合截图和图表
- **交叉引用**：相关主题互相链接

### 2. 写作风格

- **简洁明了**：避免冗长复杂的句子
- **术语统一**：保持专业术语的一致性
- **面向读者**：站在用户角度思考
- **及时更新**：代码变更时同步更新文档

### 3. 质量控制

- **链接检查**：定期运行构建检查死链接
- **代码验证**：确保示例代码可以正常运行
- **拼写检查**：避免错别字和语法错误
- **SEO 优化**：为每个页面设置合适的标题和描述

## 🔗 相关链接

- [VitePress 官方文档](https://vitepress.dev/)
- [Fastdotnet GitHub](https://github.com/CN-GodHei/fastdotnet.git)
- [Fastdotnet Gitee](https://gitee.com/CN-GodHei/fastdotnet.git)

## 📺 关注我们

- [Bilibili](https://space.bilibili.com/280446602)
- [抖音](https://v.douyin.com/2nIDPl5dvMo)

## 📄 许可证

MIT License © 2024-present Fastdotnet Team
