# 🚀 Fastdotnet Docker 快速开始指南

## ⚡ 3 分钟快速部署

### 前提条件

✅ 已安装 [Docker Desktop](https://www.docker.com/products/docker-desktop/)  
✅ Docker Desktop 正在运行  
✅ 你的项目在开发环境正常运行

---

### 步骤 1️⃣：导出私钥（1 分钟）

打开 PowerShell，运行：

```powershell
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker
.\export-secret.ps1
```

这个脚本会自动从 user-secrets 导出私钥到 Docker 密钥文件。

**输出示例：**
```
=====================================
  导出私钥到 Docker
=====================================

📁 项目路径：D:\...\Fastdotnet\backend\Fastdotnet.WebApi
📄 目标文件：D:\...\Fastdotnet\docker\secrets\marketplace_private_key.txt

🔍 检查 user-secrets 配置...
✅ 找到私钥配置
   长度：1824 字符

💾 保存私钥到文件...
✅ 私钥已成功导出！

📄 文件位置：...\marketplace_private_key.txt
📏 文件大小：1824 字节
```

---

### 步骤 2️⃣：启动 Docker 容器（2-5 分钟）

```powershell
# 首次启动需要构建镜像
.\start.ps1 -Build
```

**输出示例：**
```
=====================================
  Fastdotnet Docker 管理脚本
=====================================

🔨 构建 Docker 镜像（无缓存）...
[+] Building 120.5s (15/15) FINISHED
 => [internal] load build definition from Dockerfile
 => ...

✅ 构建成功

🚀 启动服务...
Creating network "docker_fastdotnet-network" with driver "bridge"
Creating volume "fastdotnet-data" with default driver
Creating volume "fastdotnet-logs" with default driver
Creating fastdotnet-api ... done

📊 查看服务状态:
NAME              STATUS                   PORTS
fastdotnet-api    Up (healthy)             0.0.0.0:18889->18889/tcp
```

---

### 步骤 3️⃣：验证部署（30 秒）

```powershell
# 在新窗口测试 API
curl http://localhost:18889/health
```

如果返回健康信息，说明部署成功！✅

---

## 🎉 完成！

现在你的应用已经在 Docker 容器中运行了！

### 访问地址

- **API**: http://localhost:18889
- **健康检查**: http://localhost:18889/health
- **Swagger UI**: http://localhost:18889/swagger

### 健康检查接口

**请求示例：**
```bash
curl http://localhost:18889/health
```

**响应示例：**
```json
{
  "status": "Healthy",
  "timestamp": "2026-03-11T12:00:00Z",
  "version": "1.0.0.0",
  "environment": "Production"
}
```

**用途：**
- ✅ Docker 容器健康检查
- ✅ 负载均衡器后端探测
- ✅ 监控系统状态检测
- ✅ 应用存活验证

---

## 📝 日常使用命令

### 启动应用
```powershell
cd D:\GodHeiWorkSpace\开源项目开发\Fastdotnet\docker
.\start.ps1
```

### 查看日志
```powershell
.\start.ps1 -Logs
```

### 停止应用
```powershell
.\start.ps1 -Stop
```

### 重启应用
```powershell
.\start.ps1 -Restart
```

### 完全清理（删除所有数据）
```powershell
.\start.ps1 -Clean
```

---

## 🔧 关于端口 18889

你的程序在 `Program.cs` 中指定了 18889 端口：

```csharp
app.Run("http://*:18889");
```

Docker 配置做了端口映射：
- **宿主机**: 18889
- **容器内**: 18889

所以你可以直接在浏览器访问：http://localhost:18889

---

## 🐛 遇到问题？

### 问题 1：提示找不到 user-secrets

**解决：**
```powershell
# 初始化 user-secrets
dotnet user-secrets init --project ../backend/Fastdotnet.WebApi

# 设置私钥
dotnet user-secrets set "Marketplace:PrivateKey" "你的私钥 Base64" --project ../backend/Fastdotnet.WebApi

# 重新导出
.\export-secret.ps1
```

### 问题 2：端口 18889 被占用

**解决 1：找出占用进程**
```powershell
netstat -ano | findstr :18889
```

**解决 2：修改 docker-compose.yml**
```yaml
ports:
  - "8080:18889"  # 改为其他端口
```

### 问题 3：容器启动失败

**查看详细日志：**
```powershell
docker-compose logs
```

**进入容器调试：**
```powershell
docker exec -it fastdotnet-api /bin/bash
```

---

## 📚 更多文档

详细文档请查看：[README.md](README.md)

---

**祝你使用愉快！** 🎈
