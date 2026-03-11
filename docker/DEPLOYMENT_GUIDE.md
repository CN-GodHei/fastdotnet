# 🚀 Fastdotnet 服务器部署完整指南

## 📋 目录

1. [方案对比](#方案对比)
2. [方案一：本地构建镜像推送到远程仓库（推荐）](#方案一本地构建镜像推送到远程仓库推荐)
3. [方案二：直接在服务器上构建](#方案二直接在服务器上构建)
4. [方案三：使用 CI/CD自动化部署](#方案三使用-cicd自动化部署)
5. [详细操作步骤](#详细操作步骤)
6. [常见问题解答](#常见问题解答)

---

## 🎯 方案对比

| 方案 | 优点 | 缺点 | 适用场景 | 推荐度 |
|------|------|------|----------|--------|
| **本地构建 + 推送镜像** | ✅ 开发环境可控<br>✅ 可复用镜像<br>✅ 回滚方便 | ❌ 需要镜像仓库<br>❌ 网络传输慢 | 生产环境<br>多服务器部署 | ⭐⭐⭐⭐⭐ |
| **服务器直接构建** | ✅ 简单直接<br>✅ 无需镜像仓库 | ❌ 占用服务器资源<br>❌ 不可复用 | 测试环境<br>快速验证 | ⭐⭐⭐ |
| **CI/CD自动部署** | ✅ 自动化<br>✅ 标准化<br>✅ 可追溯 | ❌ 配置复杂<br>❌ 学习成本 | 团队协作<br>持续集成 | ⭐⭐⭐⭐⭐ |

---

## 方案一：本地构建镜像推送到远程仓库（推荐）⭐

### 工作流程

```mermaid
graph LR
    A[本地开发] --> B[构建镜像]
    B --> C[推送到镜像仓库]
    C --> D[服务器拉取]
    D --> E[启动容器]
```

### 架构图

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  本地开发机器   │     │   Docker Hub     │     │  生产服务器     │
│                 │     │   /私有仓库      │     │                 │
│  git clone      │────▶│                  │────▶│  docker pull    │
│  docker build   │     │  fastdotnet-api  │     │  docker run     │
│  docker push    │     │                  │     │                 │
└─────────────────┘     └──────────────────┘     └─────────────────┘
```

### 优点

✅ **开发环境一致** - 在开发机器上构建，确保镜像质量  
✅ **可复用** - 一次构建，多次部署  
✅ **快速回滚** - 保留历史镜像版本，随时回滚  
✅ **资源节约** - 不占用服务器 CPU 和内存  

### 缺点

❌ **需要镜像仓库** - Docker Hub、阿里云容器镜像服务等  
❌ **网络依赖** - 上传下载需要良好网络  
❌ **初次配置复杂** - 需要设置认证、权限等  

---

## 方案二：直接在服务器上构建

### 工作流程

```mermaid
graph LR
    A[本地开发] --> B[git push]
    B --> C[SSH 到服务器]
    C --> D[拉取代码]
    D --> E[构建并启动]
```

### 架构图

```
┌─────────────────┐                          ┌─────────────────┐
│  本地开发机器   │                          │  生产服务器     │
│                 │                          │                 │
│  git push       │─────────────────────────▶│  git pull       │
│                 │                          │  docker build   │
│                 │                          │  docker-compose │
│                 │◀─────────────────────────│  up -d          │
│                 │     SSH 连接              │                 │
└─────────────────┘                          └─────────────────┘
```

### 优点

✅ **简单直接** - 不需要镜像仓库  
✅ **快速部署** - 一条命令完成  
✅ **成本低** - 免费，无额外服务  

### 缺点

❌ **占用资源** - 构建过程消耗服务器 CPU/内存  
❌ **不可复用** - 每次都要重新构建  
❌ **环境依赖** - 服务器需要安装 .NET SDK  
❌ **回滚困难** - 没有镜像历史记录  

---

## 方案三：使用 CI/CD 自动化部署

### 工作流程

```mermaid
graph LR
    A[代码提交] --> B[GitHub Actions]
    B --> C[自动构建]
    C --> D[推送镜像]
    D --> E[触发部署]
    E --> F[服务器拉取]
```

### 架构图

```
┌──────────┐     ┌──────────────┐     ┌─────────────┐     ┌──────────┐
│ GitHub   │────▶│ CI/CD Pipeline│────▶│Docker Registry│────▶│ 服务器   │
│          │     │ (Actions/Jenkins)│     │             │     │          │
│ git push │     │              │     │             │     │ docker   │
│          │     │ auto build   │     │ auto push   │     │ deploy   │
└──────────┘     └──────────────┘     └─────────────┘     └──────────┘
```

### 优点

✅ **全自动化** - 提交代码即部署  
✅ **标准化** - 统一的构建流程  
✅ **可追溯** - 每次部署都有记录  
✅ **团队协作** - 多人开发也能轻松管理  

### 缺点

❌ **配置复杂** - 需要学习 CI/CD工具  
❌ **时间成本** - 设置工作流需要时间  
❌ **可能收费** - 某些 CI/CD服务按使用量计费  

---

## 📖 详细操作步骤

### 方案一详细步骤：本地构建 + 推送镜像

#### 第 1 步：准备镜像仓库

**选择 1：Docker Hub（免费公开）**
```bash
# 注册账号：https://hub.docker.com/
# 创建仓库：fastdotnet-api
```

**选择 2：阿里云容器镜像服务（推荐国内用户）**
```bash
# 访问：https://cr.console.aliyun.com/
# 创建个人实例
# 创建命名空间：your-namespace
# 创建仓库：fastdotnet-api
```

**选择 3：腾讯云容器镜像服务**
```bash
# 访问：https://console.cloud.tencent.com/tcr
```

#### 第 2 步：登录镜像仓库

**Docker Hub:**
```bash
docker login
# 输入用户名和密码
```

**阿里云:**
```bash
# 获取登录密码（在阿里云控制台）
docker login --username=your-alias registry.cn-hangzhou.aliyuncs.com
# 输入登录密码（不是阿里云账号密码）
```

#### 第 3 步：本地构建镜像

```bash
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker

# 构建镜像
docker-compose build

# 或者手动构建
docker build -t fastdotnet-api:latest ..
```

#### 第 4 步：标记镜像

**Docker Hub:**
```bash
docker tag fastdotnet-api:latest your-dockerhub-username/fastdotnet-api:latest
# 例如：
docker tag fastdotnet-api:registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
```

**阿里云:**
```bash
docker tag fastdotnet-api:latest registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
```

#### 第 5 步：推送镜像

```bash
docker push your-dockerhub-username/fastdotnet-api:latest
# 或
docker push registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
```

#### 第 6 步：在服务器上拉取并运行

**SSH 登录服务器：**
```bash
ssh user@your-server-ip
# 例如：ssh root@192.168.1.100
```

**拉取镜像：**
```bash
docker pull your-dockerhub-username/fastdotnet-api:latest
# 或
docker pull registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
```

**创建密钥文件：**
```bash
mkdir -p /opt/fastdotnet/secrets
nano /opt/fastdotnet/secrets/marketplace_private_key.txt
# 粘贴私钥内容，保存退出
```

**启动容器：**
```bash
docker run -d \
  --name fastdotnet-api \
  -p 18889:18889 \
  -v /opt/fastdotnet/secrets/marketplace_private_key.txt:/app/secrets/marketplace_private_key.txt:ro \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:18889 \
  -e Marketplace__PrivateKeyPath=/app/secrets/marketplace_private_key.txt \
  --restart unless-stopped \
  your-dockerhub-username/fastdotnet-api:latest
```

**或使用 docker-compose（推荐）：**

在服务器上创建 `docker-compose.yml`：
```yaml
version: '3.8'

services:
  fastdotnet-api:
    image: registry.cn-hangzhou.aliyuncs.com/your-namespace/fastdotnet-api:latest
    container_name: fastdotnet-api
    ports:
      - "18889:18889"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:18889
      - Marketplace__PrivateKeyPath=/app/secrets/marketplace_private_key.txt
    volumes:
      - ./secrets/marketplace_private_key.txt:/app/secrets/marketplace_private_key.txt:ro
      - fastdotnet-data:/app/data
      - fastdotnet-logs:/app/logs
    restart: unless-stopped

volumes:
  fastdotnet-data:
  fastdotnet-logs:
```

启动：
```bash
docker-compose up -d
```

---

### 方案二详细步骤：服务器直接构建

#### 第 1 步：准备服务器环境

**安装 Docker:**
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# CentOS/RHEL
sudo yum install -y yum-utils
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
sudo yum install docker-ce docker-ce-cli containerd.io

# 启动 Docker
sudo systemctl start docker
sudo systemctl enable docker
```

**安装 Docker Compose:**
```bash
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
docker-compose --version
```

#### 第 2 步：上传代码到服务器

**方法 A：使用 Git**
```bash
# SSH 到服务器
ssh user@your-server-ip

# 克隆代码
cd /opt
git clone https://github.com/your-username/Fastdotnet.git
cd Fastdotnet/docker

# 如果有私钥文件，需要先导出
# 可以在本地执行后上传
```

**方法 B：使用 SCP 上传**
```bash
# 在本地 PowerShell 执行
scp -r D:\GodHeiWorkSpace\开源项目开发\Fastdotnet user@server:/opt/
```

**方法 C：使用 rsync**
```bash
rsync -avz --exclude '.git' --exclude 'bin' --exclude 'obj' \
  D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\ \
  user@server:/opt/Fastdotnet/
```

#### 第 3 步：在服务器上构建

```bash
# SSH 到服务器
ssh user@your-server-ip

# 进入项目目录
cd /opt/Fastdotnet/docker

# 准备密钥文件
mkdir -p secrets
nano secrets/marketplace_private_key.txt
# 粘贴私钥内容

# 构建并启动
docker-compose build
docker-compose up -d
```

#### 第 4 步：查看状态

```bash
# 查看容器状态
docker-compose ps

# 查看日志
docker-compose logs -f

# 测试 API
curl http://localhost:18889/health
```

---

### 方案三详细步骤：使用 GitHub Actions 自动部署

#### 第 1 步：创建 GitHub 仓库

```bash
# 初始化 git（如果还没有）
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet
git init
git add .
git commit -m "Initial commit"

# 在 GitHub 创建仓库，然后关联
git remote add origin https://github.com/your-username/fastdotnet.git
git push -u origin main
```

#### 第 2 步：创建 GitHub Actions 工作流

在项目根目录创建 `.github/workflows/deploy.yml`：

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
    - name: Checkout repository
      uses: actions/checkout@v3

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v2

    - name: Log in to Container Registry
      uses: docker/login-action@v2
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}

    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v4
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=branch
          type=sha,prefix=
          type=semver,pattern={{version}}

    - name: Build and push Docker image
      uses: docker/build-push-action@v4
      with:
        context: .
        file: ./Dockerfile
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    steps:
    - name: Deploy to server via SSH
      uses: appleboy/ssh-action@master
      with:
        host: ${{ secrets.SERVER_HOST }}
        username: ${{ secrets.SERVER_USER }}
        key: ${{ secrets.SSH_PRIVATE_KEY }}
        script: |
          cd /opt/fastdotnet
          docker-compose pull
          docker-compose up -d
```

#### 第 3 步：配置 GitHub Secrets

在 GitHub 仓库 Settings → Secrets and variables → Actions 中添加：

```
SERVER_HOST=你的服务器 IP
SERVER_USER=root
SSH_PRIVATE_KEY=你的 SSH 私钥
```

#### 第 4 步：在服务器上配置

```bash
# SSH 到服务器
ssh user@your-server-ip

# 创建部署目录
mkdir -p /opt/fastdotnet
cd /opt/fastdotnet

# 创建 docker-compose.yml
cat > docker-compose.yml << 'EOF'
version: '3.8'

services:
  fastdotnet-api:
    image: ghcr.io/your-username/fastdotnet:main
    container_name: fastdotnet-api
    ports:
      - "18889:18889"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:18889
      - Marketplace__PrivateKeyPath=/app/secrets/marketplace_private_key.txt
    volumes:
      - ./secrets/marketplace_private_key.txt:/app/secrets/marketplace_private_key.txt:ro
      - fastdotnet-data:/app/data
      - fastdotnet-logs:/app/logs
    restart: unless-stopped

volumes:
  fastdotnet-data:
  fastdotnet-logs:
EOF

# 准备密钥文件
mkdir -p secrets
nano secrets/marketplace_private_key.txt

# 登录 GitHub Container Registry
echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u your-username --password-stdin

# 拉取并启动
docker-compose pull
docker-compose up -d
```

#### 第 5 步：自动部署

现在每次 push 到 main/master 分支，都会自动：
1. 在 GitHub Actions 构建镜像
2. 推送到 ghcr.io
3. SSH 到服务器拉取最新镜像
4. 重启容器

---

## 🔧 实用部署脚本

### 本地部署脚本（deploy.ps1）

```powershell
#!/usr/bin/env pwsh

param(
    [string]$ServerHost = "your-server-ip",
    [string]$ServerUser = "root",
    [string]$ImageTag = "latest"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Fastdotnet 一键部署脚本" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 1. 构建镜像
Write-Host "🔨 构建 Docker 镜像..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot"
docker-compose build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 构建失败！" -ForegroundColor Red
    exit 1
}

Write-Host "✅ 构建成功" -ForegroundColor Green

# 2. 标记镜像
$RegistryHost = "registry.cn-hangzhou.aliyuncs.com"
$Namespace = "your-namespace"
$FullImageName = "$RegistryHost/$Namespace/fastdotnet-api:$ImageTag"

Write-Host "🏷️  标记镜像为：$FullImageName" -ForegroundColor Yellow
docker tag fastdotnet-api:$ImageTag $FullImageName

# 3. 推送镜像
Write-Host "📤 推送镜像到远程仓库..." -ForegroundColor Yellow
docker push $FullImageName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 推送失败！" -ForegroundColor Red
    exit 1
}

Write-Host "✅ 推送成功" -ForegroundColor Green

# 4. SSH 到服务器部署
Write-Host "🔌 连接到服务器并部署..." -ForegroundColor Yellow
$DeployScript = @"
cd /opt/fastdotnet
docker pull $FullImageName
docker-compose down
docker-compose up -d
docker-compose ps
"@

# 使用 plink 或 ssh 执行远程命令
ssh $ServerUser@$ServerHost $DeployScript

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  部署完成！" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "访问地址：http://$ServerHost`:18889" -ForegroundColor Cyan
Write-Host ""
```

### 服务器接收脚本（receive-deploy.sh）

```bash
#!/bin/bash

# 服务器端接收部署脚本

set -e

REGISTRY_HOST="registry.cn-hangzhou.aliyuncs.com"
NAMESPACE="your-namespace"
IMAGE_NAME="$REGISTRY_HOST/$NAMESPACE/fastdotnet-api:latest"

echo "======================================"
echo "  Fastdotnet 自动部署脚本"
echo "======================================"

# 1. 拉取最新镜像
echo "📥 拉取最新镜像..."
docker pull $IMAGE_NAME

# 2. 停止旧容器
echo "🛑 停止旧容器..."
cd /opt/fastdotnet
docker-compose down

# 3. 启动新容器
echo "🚀 启动新容器..."
docker-compose up -d

# 4. 查看状态
echo "📊 容器状态:"
docker-compose ps

# 5. 查看健康状态
echo ""
echo "🏥 健康检查:"
curl -s http://localhost:18889/health || echo "⚠️  健康检查失败"

echo ""
echo "✅ 部署完成！"
echo "访问地址：http://$(hostname -I | awk '{print $1}'):18889"
```

---

## 📊 各方案时间对比

| 阶段 | 方案一（本地构建） | 方案二（服务器构建） | 方案三（CI/CD） |
|------|------------------|-------------------|---------------|
| **首次设置** | 30 分钟 | 20 分钟 | 2 小时 |
| **每次部署时间** | 5-10 分钟 | 10-15 分钟 | 自动（5-10 分钟） |
| **网络传输** | 中等（只传镜像） | 较慢（传源码） | 快（内网传输） |
| **服务器资源** | 低 | 高 | 低 |
| **适合频率** | 每天数次 | 偶尔部署 | 持续部署 |

---

## 🎯 推荐方案总结

### 🏆 最佳实践：方案一（本地构建 + 推送镜像）

**适合：**
- ✅ 生产环境
- ✅ 多服务器部署
- ✅ 需要快速回滚
- ✅ 团队协作

**理由：**
- 开发和生产环境一致
- 镜像可复用，节省服务器资源
- 版本管理清晰，易于回滚

### 🥈 快速验证：方案二（服务器直接构建）

**适合：**
- ✅ 个人项目
- ✅ 测试环境
- ✅ 快速原型验证
- ✅ 网络条件差

**理由：**
- 简单直接，无需额外服务
- 适合学习和测试

### 🥇 专业团队：方案三（CI/CD 自动化）

**适合：**
- ✅ 企业级应用
- ✅ 敏捷开发团队
- ✅ 需要审计追踪
- ✅ 频繁发布

**理由：**
- 全自动化，减少人为错误
- 标准化流程，提高质量

---

## ❓ 常见问题解答

### Q1: 我应该选择哪个方案？

**A:** 
- **个人项目/学习** → 方案二
- **生产环境/商业项目** → 方案一
- **团队协作/企业项目** → 方案三

### Q2: 镜像仓库选哪个？

**A:**
- **国内用户** → 阿里云容器镜像服务（免费、速度快）
- **国际用户** → Docker Hub（最流行）
- **企业用户** → 私有 Harbor 仓库

### Q3: 如何保证密钥安全？

**A:**
1. 不要将密钥提交到 Git
2. 使用环境变量或文件挂载
3. 限制文件权限：`chmod 600 secrets/marketplace_private_key.txt`
4. 定期轮换密钥

### Q4: 服务器需要多少资源？

**A:**
- **最低配置**: 1 核 CPU, 512MB 内存
- **推荐配置**: 2 核 CPU, 2GB 内存
- **磁盘空间**: 至少 10GB 可用空间

### Q5: 如何备份数据？

**A:**
```bash
# 备份 Docker Volume
docker run --rm \
  -v fastdotnet_fastdotnet-data:/data/source \
  -v $(pwd)/backup:/data/backup \
  alpine tar czf /data/backup/fastdotnet-data.tar.gz -C /data/source .

# 恢复
docker run --rm \
  -v fastdotnet_fastdotnet-data:/data/target \
  -v $(pwd)/backup:/data/backup \
  alpine tar xzf /data/backup/fastdotnet-data.tar.gz -C /data/target
```

### Q6: 如何监控容器状态？

**A:**
```bash
# 使用 docker stats
docker stats fastdotnet-api

# 使用 Portainer（图形化管理）
docker run -d -p 9000:9000 \
  --name portainer \
  -v /var/run/docker.sock:/var/run/docker.sock \
  portainer/portainer-ce

# 访问 http://server-ip:9000
```

---

## 📞 需要帮助？

如有问题，请检查：
1. Docker 是否正常运行：`docker info`
2. 端口是否开放：`firewall-cmd --list-ports`
3. 查看容器日志：`docker-compose logs -f`
4. 测试网络连接：`curl http://localhost:18889/health`

---

**祝你部署成功！** 🎉
