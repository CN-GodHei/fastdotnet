# 快速启动指南

## ⚠️ 网络问题解决方案

由于 Docker Hub 访问限制，如果遇到拉取镜像失败，可以选择以下方案：

---

## 🔧 方案一：配置 Docker 镜像加速器（推荐）

### Windows Docker Desktop 配置

1. 打开 Docker Desktop
2. 点击设置图标 ⚙️
3. 选择 **Docker Engine**
4. 在 `registry-mirrors` 中添加国内镜像源：

```json
{
  "builder": {
    "gc": {
      "defaultKeepStorage": "20GB",
      "enabled": true
    }
  },
  "experimental": false,
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live"
  ]
}
```

5. 点击 **Apply & Restart**

### 重启后执行

```bash
cd docker
docker-compose up -d
```

---

## 🌐 方案二：使用外部 MySQL 服务器（无需等待）

如果急需启动，可以先使用外部 MySQL：

### 步骤 1：准备 MySQL

确保你有可用的 MySQL 服务器（版本 >= 8.0）

### 步骤 2：修改配置

复制外部数据库配置文件：

```powershell
cd docker
Copy-Item docker-compose.external-db.yml docker-compose.override.yml
```

编辑 `docker-compose.override.yml`，修改为你的实际数据库信息：

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=你的 MySQL 服务器;Database=fastdotnet;Uid=用户名;Pwd=密码;SslMode=None;AllowLoadLocalInfile=true;AllowUserVariables=true;
```

### 步骤 3：启动 API 服务

```bash
docker-compose up -d fastdotnet-api
```

---

## 📦 方案三：手动下载镜像

### 1. 通过其他方式获取 MySQL 镜像

如果有同事已经有 MySQL 镜像，可以导出分享：

```bash
# 从有镜像的机器导出
docker save mysql:8.0 -o mysql-8.tar

# 传输到你的机器
# 然后加载
docker load -i mysql-8.tar
```

### 2. 标记镜像

```bash
docker tag mysql:8.0 mysql:8.0
```

### 3. 启动容器

```bash
cd docker
docker-compose up -d
```

---

## 💡 临时建议

### 如果只是测试应用功能

可以先用 SQLite 模式运行，不需要 MySQL：

1. 修改 `docker-compose.yml`，注释掉 MySQL 相关配置
2. 应用会自动使用 SQLite（appsettings.json 中的默认配置）

```yaml
# 注释掉 MySQL 服务
# fastdotnet-mysql:
#   image: mysql:8.0
#   ...

fastdotnet-api:
  # ... 其他配置保持不变
```

---

## 🚀 验证启动成功

```bash
# 查看容器状态
docker-compose ps

# 应该看到：
# NAME                    STATUS         PORTS
# fastdotnet-api          Up (healthy)   0.0.0.0:18889->18889/tcp
# fastdotnet-mysql        Up (healthy)   3306/tcp
```

---

## 📞 需要帮助？

如果以上方案都无法解决，请检查：

1. **网络连接** - 确保能访问外网
2. **防火墙设置** - Docker 可能需要特殊配置
3. **DNS 配置** - 尝试修改 DNS 为 8.8.8.8 或 1.1.1.1
4. **公司代理** - 某些公司网络可能限制 Docker Hub

---

**最后更新：** 2025-03-12
