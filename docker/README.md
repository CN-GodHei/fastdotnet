# Fastdotnet Docker 部署指南

## 📋 目录结构

```
Fastdotnet/
├── backend/                 # 后端源代码
├── docker/                  # Docker 配置目录
│   ├── secrets/            # 密钥文件目录（不要提交到 Git）
│   │   └── marketplace_private_key.txt
│   ├── docker-compose.yml  # Docker Compose 配置
│   └── start.ps1          # PowerShell 启动脚本
├── Dockerfile              # Docker 镜像构建文件
└── .dockerignore          # Docker 忽略文件
```

## 🔧 端口说明

你的程序在 `Program.cs` 中指定了 **18889** 端口：

```csharp
app.Run("http://*:18889");
```

Docker 配置中的端口映射：
- **宿主机端口**: 18889
- **容器内部端口**: 18889

这意味着：
- ✅ 在宿主机访问 `http://localhost:18889` 
- ✅ Docker 会将请求转发到容器内的 18889 端口
- ✅ 不影响程序的正常运行

## 🚀 快速开始

### 前提条件

1. 安装 [Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. 确保 Docker Desktop 正在运行

### 步骤 1：准备密钥文件

打开 PowerShell，运行以下命令导出私钥：

```powershell
# 进入 docker 目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker

# 从 user-secrets 导出私钥
$privateKey = dotnet user-secrets get "Marketplace:PrivateKey" --project ../backend/Fastdotnet.WebApi

# 保存到密钥文件
$privateKey | Out-File -FilePath "secrets\marketplace_private_key.txt" -Encoding ASCII -NoNewline

Write-Host "✅ 私钥已导出到 secrets\marketplace_private_key.txt"
```

或者手动操作：
1. 运行：`dotnet user-secrets get "Marketplace:PrivateKey" --project ../backend/Fastdotnet.WebApi`
2. 复制输出的 Base64 字符串
3. 粘贴到 `docker\secrets\marketplace_private_key.txt` 文件中

### 步骤 2：首次启动（需要构建镜像）

```powershell
# 进入 docker 目录
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker

# 首次启动（会自动构建镜像）
.\start.ps1 -Build
```

首次构建可能需要几分钟，请耐心等待。

### 步骤 3：验证部署

```powershell
# 查看容器状态
docker-compose ps

# 查看日志
docker-compose logs -f

# 测试 API（新窗口）
curl http://localhost:18889/health
```

## 📝 常用命令

### 正常启动
```powershell
.\start.ps1
```

### 重新构建后启动
```powershell
.\start.ps1 -Build
```

### 启动并查看实时日志
```powershell
.\start.ps1 -Logs
```

### 停止服务
```powershell
.\start.ps1 -Stop
```

### 重启服务
```powershell
.\start.ps1 -Restart
```

### 完全清理（删除所有数据和卷）
```powershell
.\start.ps1 -Clean
```

## 🔍 调试和排错

### 查看容器状态
```powershell
docker-compose ps
```

### 查看实时日志
```powershell
docker-compose logs -f
```

### 查看最近 100 行日志
```powershell
docker-compose logs --tail=100
```

### 进入容器内部调试
```powershell
docker exec -it fastdotnet-api /bin/bash

# 在容器内可以执行：
printenv | grep Marketplace           # 查看环境变量
cat /app/secrets/marketplace_private_key.txt  # 查看密钥文件
ls -la /app/secrets/                  # 查看密钥目录
```

### 检查密钥文件是否正确挂载
```powershell
# 在宿主机查看
Get-Content secrets\marketplace_private_key.txt

# 在容器内查看
docker exec fastdotnet-api cat /app/secrets/marketplace_private_key.txt
```

## 🔒 安全建议

### 1. 保护密钥文件

```powershell
# 设置文件权限（仅当前用户可访问）
$acl = Get-Acl "secrets\marketplace_private_key.txt"
$acl.SetAccessRuleProtection($true, $false)
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:USERNAME, "Read", "Allow")
$acl.AddAccessRule($rule)
Set-Acl "secrets\marketplace_private_key.txt" $acl
```

### 2. 不要提交密钥到 Git

`.gitignore` 已经配置好，会自动忽略：
- `docker/secrets/*`
- `.env` 文件

### 3. 定期轮换密钥

建议每 3-6 个月更换一次私钥：

```powershell
# 生成新密钥并更新
$newKey = Generate-NewPrivateKey  # 你的密钥生成逻辑
$newKey | Out-File -FilePath "secrets\marketplace_private_key.txt" -Encoding ASCII -NoNewline

# 重启容器使新密钥生效
docker-compose restart
```

## ⚙️ 配置说明

### docker-compose.yml 关键配置

```yaml
ports:
  - "18889:18889"  # 宿主机端口：容器端口
  
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPNETCORE_URLS=http://+:18889
  - Marketplace__PrivateKeyPath=/app/secrets/marketplace_private_key.txt

volumes:
  - ./secrets/marketplace_private_key.txt:/app/secrets/marketplace_private_key.txt:ro
```

### 环境变量说明

| 变量名 | 说明 | 示例值 |
|--------|------|--------|
| `ASPNETCORE_ENVIRONMENT` | 运行环境 | `Production` |
| `ASPNETCORE_URLS` | 监听地址 | `http://+:18889` |
| `Marketplace__PrivateKeyPath` | 密钥文件路径 | `/app/secrets/marketplace_private_key.txt` |

## 🐛 常见问题

### Q1: 端口已被占用

**错误信息**: "Bind for 0.0.0.0:18889 failed: port is already allocated"

**解决方案**:
```powershell
# 方法 1: 找出占用端口的进程
netstat -ano | findstr :18889

# 方法 2: 修改 docker-compose.yml 的端口映射
ports:
  - "8080:18889"  # 使用其他宿主机端口
```

### Q2: 密钥文件未找到

**错误信息**: "InvalidOperationException: 私钥未配置"

**检查步骤**:
```powershell
# 1. 确认密钥文件存在
Test-Path secrets\marketplace_private_key.txt

# 2. 确认文件内容不为空
Get-Content secrets\marketplace_private_key.txt

# 3. 重启容器
docker-compose restart
```

### Q3: 容器无法启动

**排查步骤**:
```powershell
# 1. 查看完整日志
docker-compose logs

# 2. 删除容器重新创建
docker-compose down
docker-compose up -d

# 3. 完全清理后重试
.\start.ps1 -Clean
.\start.ps1 -Build
```

## 📊 性能优化

### 1. 使用多阶段构建（已配置）

Dockerfile 使用了多阶段构建，减小最终镜像大小：
- 构建阶段：使用 SDK 镜像
- 运行阶段：使用 ASP.NET 运行时镜像

### 2. 数据持久化

使用 Docker volumes 持久化数据：
- `fastdotnet-data`: 应用数据
- `fastdotnet-logs`: 日志文件

### 3. 健康检查（已配置）

容器会自动进行健康检查，确保服务可用。

## 🎯 生产环境部署

### Linux 服务器部署

```bash
# 1. 上传项目到服务器
scp -r Fastdotnet/ user@server:/opt/

# 2. 准备密钥
echo "你的私钥 Base64" > /opt/Fastdotnet/docker/secrets/marketplace_private_key.txt

# 3. 启动服务
cd /opt/Fastdotnet/docker
docker-compose up -d
```

### 使用 systemd 管理（推荐）

创建 `/etc/systemd/system/fastdotnet-docker.service`:

```ini
[Unit]
Description=Fastdotnet Docker Container
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/Fastdotnet/docker
ExecStart=/usr/bin/docker-compose up -d
ExecStop=/usr/bin/docker-compose down

[Install]
WantedBy=multi-user.target
```

启用服务：
```bash
sudo systemctl enable fastdotnet-docker
sudo systemctl start fastdotnet-docker
sudo systemctl status fastdotnet-docker
```

## 📞 获取帮助

如有问题，请检查：
1. Docker Desktop 是否正常运行
2. 密钥文件是否存在且内容正确
3. 端口 18889 是否被占用
4. 查看容器日志了解详细错误信息

---

**祝你部署成功！** 🎉
