# 🔧 Docker Build 故障排查指南

## ❌ 错误信息

```
ERROR: request returned 500 Internal Server Error for API route and version
http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine/_ping
```

## 📋 问题诊断

### 1. 检查 Docker 状态

```powershell
# 检查 Docker 是否运行
docker info

# 检查 Docker 上下文
docker context ls

# 查看当前上下文
docker context show
```

### 2. 当前问题分析

根据你的输出：
- ✅ Docker CLI 已安装（版本 28.1.1）
- ❌ Docker 上下文：`desktop-linux`（这是 Linux 环境）
- ❌ 你在 Windows 上运行，但 Docker 尝试连接 Linux 引擎

## 🛠️ 解决方案

### 方案 1：重启 Docker Desktop（推荐）

1. **完全退出 Docker Desktop**
   - 右键点击系统托盘的 Docker 图标
   - 选择 "Quit Docker Desktop"

2. **重新启动 Docker Desktop**
   - 从开始菜单启动 Docker Desktop
   - 等待鲸鱼图标变成绿色（约 30-60 秒）

3. **验证**
   ```powershell
   docker context show
   # 应该显示：desktop-windows 或 default
   
   docker info
   # 应该显示服务器信息而不是错误
   ```

### 方案 2：切换 Docker 上下文

```powershell
# 列出所有上下文
docker context ls

# 切换到 Windows 上下文
docker context use desktop-windows
# 或
docker context use default

# 验证
docker info
```

### 方案 3：重置 Docker 到出厂设置

如果以上都不行：

1. 打开 Docker Desktop 设置
2. 选择 "Troubleshoot"
3. 点击 "Reset to factory defaults"

⚠️ **警告**: 这会删除所有容器和镜像！

### 方案 4：检查 WSL 2

Docker Desktop on Windows 需要 WSL 2：

```powershell
# 检查 WSL 状态
wsl --list --verbose

# 如果没有输出或报错，启用 WSL
wsl --install

# 重启电脑后再次尝试
```

## ✅ 验证步骤

完成上述任一方案后，运行：

```powershell
# 1. 检查 Docker 状态
docker info

# 应该看到类似输出：
# Server: Docker Desktop ...
# Containers: 0
# Running: 0
# ...

# 2. 测试构建
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet
.\build.ps1
```

## 🎯 预期结果

成功的构建应该显示：

```
=====================================
  Fastdotnet Docker Build
=====================================

Building Docker image...
Sending build context to Docker daemon  45.06MB
Step 1/15 : FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
 ---> Using cache
...
Successfully built a1b2c3d4e5f6
Successfully tagged fastdotnetwebapi:latest

Build successful! Duration: 85.32s

REPOSITORY       TAG       IMAGE ID       CREATED         SIZE
fastdotnetwebapi   latest    a1b2c3d4e5f6   10 seconds ago   98.5MB

Done!
```

## 📝 常见错误对照

| 错误信息 | 原因 | 解决方案 |
|---------|------|----------|
| `Cannot connect to the Docker daemon` | Docker 未运行 | 启动 Docker Desktop |
| `desktop-linux` 上下文错误 | 上下文配置错误 | `docker context use desktop-windows` |
| `500 Internal Server Error` | Docker 引擎故障 | 重启 Docker Desktop |
| `WSL 2 not installed` | 缺少 WSL 2 | `wsl --install` |

## 💡 预防措施

### 1. 设置 Docker Desktop 开机自启

- Docker Desktop 设置 → General
- 勾选 "Start Docker Desktop when you log in"

### 2. 分配足够资源

- Docker Desktop 设置 → Resources
- CPUs: 至少 2 核
- Memory: 至少 4GB
- Disk: 至少 50GB

### 3. 使用正确的上下文

创建别名快速切换：

```powershell
# 在 PowerShell profile 中添加
function Use-DockerWindows { docker context use desktop-windows }
function Use-DockerLinux { docker context use desktop-linux }
```

## 🔍 如果问题依然存在

### 收集诊断信息

```powershell
# Docker 诊断
docker diagnose

# 查看 Docker 日志
Get-EventLog -LogName Application -Source Docker -Newest 20

# Docker Desktop 日志
# 位置：%LOCALAPPDATA%\Docker\log.txt
```

### 寻求帮助的渠道

1. Docker Desktop 官方文档：https://docs.docker.com/desktop/
2. Docker 社区论坛：https://forums.docker.com/
3. GitHub Issues: https://github.com/docker/for-win/issues

## 📞 快速修复脚本

创建 `fix-docker.ps1`:

```powershell
Write-Host "Fixing Docker..." -ForegroundColor Cyan

# 1. 停止 Docker
Write-Host "Stopping Docker..." -ForegroundColor Yellow
Stop-Process -Name "Docker Desktop" -Force -ErrorAction SilentlyContinue

# 2. 等待
Start-Sleep -Seconds 5

# 3. 启动 Docker
Write-Host "Starting Docker..." -ForegroundColor Yellow
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"

# 4. 等待启动
Write-Host "Waiting for Docker to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# 5. 验证
Write-Host "Verifying..." -ForegroundColor Green
docker info --format "{{.ServerVersion}}"

Write-Host "Done!" -ForegroundColor Green
```

运行：
```powershell
.\fix-docker.ps1
```

---

## ✅ 成功标志

当你看到以下输出时，说明 Docker 已经正常工作：

```bash
$ docker info
Client:
 Version:    28.1.1
 Context:    desktop-windows  ← 应该是这个
 ...

Server: Docker Desktop ...
 ...
```

然后就可以正常构建镜像了！

---

**祝你顺利解决问题！** 🎉
