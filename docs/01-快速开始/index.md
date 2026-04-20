# 快速开始

欢迎使用 Fastdotnet 快速开发框架！本指南将帮助你快速上手并开始开发。

---

## 🌐 官方网站

- **官网**：[https://fastdotnet.top](https://fastdotnet.top)
  - 框架介绍、特性展示、成功案例
  
- **插件商城**：[https://fastdotnet.top/marketplace](https://fastdotnet.top/marketplace)
  - 海量插件供选购，快速扩展系统功能
  - 支付、工作流、报表、OSS等常用插件应有尽有

---

## 🎮 在线体验

无需安装，立即体验 Fastdotnet：

### 管理端演示

- **地址**：http://admin.demo.fastdotnet.top/
- **账号**：`superadmin`
- **密码**：`123456`
- **功能**：系统管理、用户管理、权限配置等

### 应用端演示

- **地址**：http://app.demo.fastdotnet.top/
- **账号**：`admintest`
- **密码**：`123456`
- **功能**：业务操作、数据展示等

> 💡 **提示**：
> - 演示环境数据会定期重置
> - 请勿在演示环境中存储重要数据
> - 如需本地部署，请继续阅读下面的快速启动指南

---

## 📋 前置要求

在开始之前，请确保你的开发环境已安装以下软件：

### 必需软件

- **.NET 10 SDK** 或更高版本
  - 下载地址：[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
  - 验证安装：`dotnet --version`

- **Node.js 18+** 
  - 下载地址：[https://nodejs.org/](https://nodejs.org/)
  - 验证安装：`node --version`

- **pnpm 包管理器**
  - 安装命令：`npm install -g pnpm`
  - 验证安装：`pnpm --version`

- **数据库**（任选其一）
  - SQL Server 2019+
  - PostgreSQL 14+
  - MySQL 8.0+
  - SQLite（开发测试推荐）

### 推荐工具

- **IDE**: Visual Studio 2022 / JetBrains Rider / VS Code
- **数据库工具**: SQL Server Management Studio / pgAdmin / DBeaver
- **API 测试**: Postman / Swagger UI（内置）

---

## 🚀 5 分钟快速启动

### 步骤 1：克隆项目

```bash
git clone https://gitee.com/CN-GodHei/fastdotnet.git
cd fastdotnet
```

### 步骤 2：启动后端

```bash
# 进入后端 API 目录
cd backend/Fastdotnet.WebApi

# 恢复 NuGet 包
dotnet restore

# 运行项目（首次运行会自动创建数据库）
dotnet run
```

后端将在 `http://localhost:18889` 启动。

**验证后端是否启动成功：**
- 访问 Swagger UI：http://localhost:18889/swagger
- 查看健康检查：http://localhost:18889/health

### 步骤 3：启动前端

打开新的终端窗口：

```bash
# 进入前端管理后台目录
cd Web/fastdotnet-admin

# 安装依赖
pnpm install

# 启动开发服务器
pnpm dev
```

前端管理端将在 `http://localhost:18888` 启动。

如需启动 Web 应用端：

```bash
# 进入前端应用端目录
cd Web/fastdotnet-app

# 安装依赖
pnpm install

# 启动开发服务器
pnpm dev
```

前端应用端将在 `http://localhost:18887` 启动。

### 步骤 4：登录系统

1. 浏览器访问：http://localhost:18888
2. 使用默认账号登录：
   - 用户名：`amdintest`
   - 密码：`123456`

🎉 恭喜！你已经成功启动了 Fastdotnet 系统！

---

## 🐳 Docker 快速启动（推荐新手）

如果你不想配置开发环境，可以使用 Docker 一键启动：

```bash
# 进入 docker 目录
cd docker

# 启动所有服务（后端 + 前端 + 数据库）
docker-compose up -d
```

访问地址：
- 前端管理端：http://localhost:18888
- 前端应用端：http://localhost:18887
- 后端 API：http://localhost:18889
- Swagger：http://localhost:18889/swagger

停止服务：
```bash
docker-compose down
```

---

## ⚙️ 配置说明

### 后端配置

主要配置文件：`backend/Fastdotnet.WebApi/appsettings.json`

#### 数据库配置

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Fastdotnet;Trusted_Connection=True;"
  }
}
```

支持的数据库类型：
- **SQL Server**: `Server=localhost;Database=Fastdotnet;...`
- **PostgreSQL**: `Host=localhost;Database=Fastdotnet;...`
- **MySQL**: `Server=localhost;Database=Fastdotnet;...`
- **SQLite**: `Data Source=fastdotnet.db`

#### JWT 配置

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "Fastdotnet",
    "Audience": "FastdotnetUsers",
    "ExpireMinutes": 120
  }
}
```

### 前端配置

主要配置文件：`Web/fastdotnet-admin/.env.development`

```env
# API 基础地址
VITE_API_BASE_URL=http://localhost:18889/api

# 是否启用 Mock 数据
VITE_USE_MOCK=false
```

---

## 📁 项目结构概览

```
Fastdotnet/
├── backend/                    # 后端代码
│   ├── Fastdotnet.Core/       # 核心层（实体、接口、工具类）
│   ├── Fastdotnet.Orm/        # ORM 数据访问层
│   ├── Fastdotnet.Service/    # 业务服务层
│   ├── Fastdotnet.WebApi/     # Web API 入口（端口: 18889）
│   └── Plugins/               # 插件源码
├── Web/                       # 前端代码
│   ├── fastdotnet-admin/      # 管理后台 - Web端（端口: 18888）
│   ├── fastdotnet-app/        # 应用前台 - Web端
│   └── plugin-*-admin/        # 插件管理端
├── docker/                    # Docker 配置
└── docs/                      # 文档（当前位置）
```

### 多端架构说明

Fastdotnet 采用**一套后端，多端复用**的架构设计：

#### 前端项目分类

**1. 管理端 (Admin)**
- 项目：`Web/fastdotnet-admin`
- 面向：系统管理员和运营人员
- 功能：系统配置、用户管理、权限控制等
- 当前状态：✅ 已完成

**2. 应用端 (App)**
- Web 应用端：`Web/fastdotnet-app`（端口: 18887）✅
- 桌面端：Avalonia（已确定技术栈）📋
- 移动端：技术栈待考量 📋
- 面向：最终用户
- 功能：业务操作、数据展示等

#### API 作用域

通过 `ApiUsageScopeEnum` 区分不同端的 API：

```csharp
public enum ApiUsageScopeEnum
{
    AdminOnly = 1,  // 仅管理端可访问
    AppOnly = 2,    // 仅应用端可访问
    Both = 3        // 两端都可访问
}
```

使用特性标记 API 的作用域：

```csharp
[ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
public class SystemConfigController : ControllerBase
{
    // 仅管理端可访问的系统配置 API
}
```

详见 [架构设计](../02-核心概念/架构设计) 了解更多信息。

---

## 🔧 常见问题

### 1. 后端启动失败

**问题**：提示数据库连接失败

**解决**：
- 检查 `appsettings.json` 中的数据库连接字符串
- 确保数据库服务已启动
- 首次运行会自动创建数据库和表结构

### 2. 前端无法访问后端 API

**问题**：浏览器控制台显示 CORS 错误

**解决**：
- 检查 `.env.development` 中的 `VITE_API_BASE_URL` 是否正确
- 确保后端已启动并监听正确端口

### 3. 端口被占用

**问题**：提示端口 18889 或 18888 已被占用

**解决**：
```bash
# Windows 查找占用端口的进程
netstat -ano | findstr :18889
taskkill /PID <进程ID> /F

# Linux/Mac
lsof -i :18889
kill -9 <进程ID>
```

### 4. pnpm 安装依赖失败

**问题**：网络超时或下载失败

**解决**：
```bash
# 设置国内镜像源
pnpm config set registry https://registry.npmmirror.com

# 清除缓存后重试
pnpm store prune
pnpm install
```

---

## 📚 下一步

- [环境准备详细说明](./环境准备) - 详细的开发环境配置
- [安装部署指南](./安装部署) - 生产环境部署
- [创建第一个插件](./创建第一个插件) - 开发你的第一个插件 ⭐
- [项目结构详解](../03-后端开发/项目结构) - 深入了解代码组织

---

## 💡 小贴士

1. **开发建议**：使用 SQLite 进行本地开发，无需安装数据库服务器
2. **调试技巧**：后端支持热重载，修改代码后自动重启
3. **API 文档**：Swagger UI 提供了完整的 API 文档和在线测试
4. **日志查看**：后端日志输出到控制台和生产环境的日志文件

如有问题，欢迎查阅完整文档或提交 Issue！
