# ⚡ 本地构建快速指南

## 🎯 三种构建方法（从简单到复杂）

### 方法 1️⃣：使用自动化脚本（最简单）⭐⭐⭐⭐⭐

```powershell
# 进入项目目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet

# 一键构建
.\build.ps1
```

**优点：**
- ✅ 自动检查环境
- ✅ 彩色输出，易于阅读
- ✅ 显示构建时间和统计信息
- ✅ 支持多种选项（清理、推送等）

---

### 方法 2️⃣：使用 docker-compose（推荐）⭐⭐⭐⭐

```powershell
# 进入 docker 目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker

# 构建镜像
docker-compose build

# 或使用无缓存模式
docker-compose build --no-cache
```

**优点：**
- ✅ 与 docker-compose 配置一致
- ✅ 可以使用 compose 的其他功能
- ✅ 适合后续直接启动容器

---

### 方法 3️⃣：直接使用 docker build（灵活）⭐⭐⭐

```powershell
# 进入项目根目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet

# 构建镜像
docker build -t fastdotnet-api:latest -f Dockerfile .
```

**优点：**
- ✅ 完全控制构建参数
- ✅ 适合熟悉 Docker 的用户
- ✅ 可以添加各种高级选项

---

## 📝 完整构建流程示例

### 首次构建（预计 5-10 分钟）

```powershell
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  开始首次构建" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# 1. 进入项目目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet

# 2. 检查环境
Write-Host "`n📋 检查环境..." -ForegroundColor Yellow
docker --version
dotnet --version

# 3. 开始构建
Write-Host "`n🔨 开始构建..." -ForegroundColor Yellow
.\build.ps1

# 4. 验证结果
Write-Host "`n✅ 验证构建结果..." -ForegroundColor Yellow
docker images fastdotnet-api
```

### 日常构建（预计 30 秒 -2 分钟）

```powershell
# 使用缓存快速构建
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet
.\build.ps1
```

### 强制重新构建

```powershell
# 不使用缓存，完全重新构建
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet
.\build.ps1 -NoCache
```

---

## 🧪 测试构建的镜像

### 1. 启动容器

```powershell
# 使用 docker-compose 启动
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker
docker-compose up -d
```

### 2. 查看状态

```powershell
# 查看容器状态
docker-compose ps

# 查看实时日志
docker-compose logs -f
```

### 3. 测试健康检查

```powershell
# 在新窗口执行
curl http://localhost:18889/health
```

**预期响应：**
```json
{
  "status": "Healthy",
  "timestamp": "2026-03-11T12:00:00Z",
  "version": "1.0.0.0",
  "environment": "Production"
}
```

### 4. 测试 Swagger UI

```powershell
# 浏览器访问
Start-Process "http://localhost:18889/swagger"
```

### 5. 停止容器

```powershell
docker-compose down
```

---

## 🔧 常用构建命令速查

| 命令 | 说明 | 时间 |
|------|------|------|
| `.\build.ps1` | 快速构建（使用缓存） | 30 秒 -2 分钟 |
| `.\build.ps1 -NoCache` | 完全重新构建 | 5-10 分钟 |
| `.\build.ps1 -Clean` | 清理后构建 | 5-10 分钟 |
| `docker-compose build` | 使用 compose 构建 | 同 `.\build.ps1` |
| `docker build -t name:tag .` | 手动构建 | 同 `.\build.ps1` |

---

## 🐛 故障排查

### 问题 1：构建失败

**检查步骤：**
```powershell
# 1. 确认在项目根目录
pwd

# 2. 确认 Dockerfile 存在
Test-Path Dockerfile

# 3. 确认 backend 目录存在
Test-Path backend

# 4. 确认 Docker 正在运行
docker info
```

### 问题 2：找不到 .NET SDK

**解决方法：**
```powershell
# 检查 .NET 版本
dotnet --version

# 如果未安装，下载：
# https://dotnet.microsoft.com/download/dotnet/10.0
```

### 问题 3：磁盘空间不足

**清理命令：**
```powershell
# 清理悬空镜像
docker image prune

# 清理所有未使用数据
docker system prune -a
```

---

## 📊 构建性能对比

| 场景 | 方法 | 时间 | 适用情况 |
|------|------|------|----------|
| **首次构建** | 任意 | 5-10 分钟 | 第一次构建 |
| **代码修改后** | `.\build.ps1` | 30 秒 -2 分钟 | 日常开发 |
| **依赖更新后** | `.\build.ps1 -NoCache` | 5-8 分钟 | NuGet 包更新 |
| **环境变更后** | `.\build.ps1 -Clean` | 5-10 分钟 | Docker 升级 |

---

## 💡 最佳实践建议

### ✅ 推荐做法

1. **日常开发使用缓存**
   ```powershell
   .\build.ps1
   ```

2. **提交前强制重新构建**
   ```powershell
   .\build.ps1 -NoCache
   ```

3. **定期清理旧镜像**
   ```powershell
   docker image prune -a
   ```

### ❌ 避免的做法

1. **不要每次都使用 `-NoCache`** - 浪费时间
2. **不要在错误的目录执行** - 确保在 `Fastdotnet/` 根目录
3. **不要忽略构建警告** - 及时修复问题

---

## 🎯 完整的开发 - 构建 - 部署流程

```powershell
# 1. 本地开发
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\backend\Fastdotnet.WebApi
dotnet run

# 2. 测试通过后构建镜像
cd ..\..\..
.\build.ps1

# 3. 本地测试容器
cd docker
docker-compose up -d
curl http://localhost:18889/health

# 4. 推送到镜像仓库
cd ..
.\build.ps1 -Push -RegistryHost "registry.cn-hangzhou.aliyuncs.com" -Namespace "your-namespace"

# 5. 部署到服务器
# SSH 到服务器执行：
# docker pull registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
# docker-compose up -d
```

---

## 📚 详细文档

- [LOCAL_BUILD_GUIDE.md](LOCAL_BUILD_GUIDE.md) - 完整本地构建指南
- [DEPLOYMENT_GUIDE.md](docker/DEPLOYMENT_GUIDE.md) - 部署指南
- [QUICKSTART.md](docker/QUICKSTART.md) - 快速开始

---

## ⚡ 一行命令搞定

```powershell
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet && .\build.ps1
```

---

**就是这么简单！** 🎉
