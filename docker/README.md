# Fastdotnet Docker 部署方案总结

## 📋 当前状态

✅ **SQLite 模式已成功启动**

- 容器名：fastdotnetwebapi
- 访问地址：http://localhost:18889
- 数据库：SQLite（内置）
- 数据持久化：fastdotnet-data volume

---

## 🎯 三种部署方案对比

| 方案 | 优点 | 缺点 | 适用场景 | 状态 |
|------|------|------|----------|------|
| **SQLite** | ✅ 无需 MySQL<br>✅ 快速启动<br>✅ 最简单 | ❌ 不适合生产<br>❌ 性能有限 | 开发测试<br>功能验证 | ✅ 已启动 |
| **MySQL 容器** | ✅ 完整功能<br>✅ 数据持久化<br>✅ 易管理 | ⚠️ 需拉取镜像<br>⚠️ 网络要求 | 开发环境<br>测试环境 | ⏸️ 待配置 |
| **外部 MySQL** | ✅ 高性能<br>✅ 生产可用<br>✅ 易维护 | ❌ 需单独部署 | 生产环境 | 📝 待配置 |

---

## 🚀 方案一：SQLite 模式（当前运行）

### 启动命令

```bash
cd docker
docker-compose -f docker-compose.sqlite.yml up -d
```

### 停止服务

```bash
docker-compose -f docker-compose.sqlite.yml down
```

### 查看日志

```bash
docker-compose -f docker-compose.sqlite.yml logs -f
```

### 访问 API

```bash
curl http://localhost:18889/health
```

---

## 🐳 方案二：MySQL 容器模式（需配置镜像加速）

### 问题说明

由于 Docker Hub 访问限制，需要配置镜像加速器才能拉取 MySQL 镜像。

### 解决步骤

#### 1. 配置 Docker Desktop

打开 Docker Desktop → Settings → Docker Engine，添加：

```json
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live"
  ]
}
```

点击 **Apply & Restart**

#### 2. 启动服务

```bash
cd docker
docker-compose up -d
```

#### 3. 验证

```bash
docker-compose ps
docker-compose logs fastdotnet-mysql
```

---

## 🌐 方案三：外部 MySQL 服务器（生产推荐）

### 准备工作

确保有可用的 MySQL 8.0+ 服务器

### 配置步骤

#### 1. 创建数据库

```sql
CREATE DATABASE fastdotnet CHARACTER SET utf8mb4;
CREATE DATABASE fastdotnet_log CHARACTER SET utf8mb4;
CREATE USER 'fastdotnet'@'%' IDENTIFIED BY '你的密码';
GRANT ALL PRIVILEGES ON fastdotnet.* TO 'fastdotnet'@'%';
GRANT ALL PRIVILEGES ON fastdotnet_log.* TO 'fastdotnet'@'%';
FLUSH PRIVILEGES;
```

#### 2. 修改配置

复制并编辑配置文件：

```powershell
cd docker
Copy-Item docker-compose.external-db.yml docker-compose.override.yml
```

编辑 `docker-compose.override.yml`，替换为你的实际信息：

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=你的服务器;Database=fastdotnet;Uid=fastdotnet;Pwd=你的密码;SslMode=None;AllowLoadLocalInfile=true;AllowUserVariables=true;
```

#### 3. 启动服务

```bash
docker-compose up -d
```

---

## 📊 详细配置说明

### SQLite 模式

**配置文件：** `docker-compose.sqlite.yml`

**特点：**
- 使用 appsettings.json 中的默认 SQLite 配置
- 数据库文件存储在 `/app/data/fd.db`
- 适合快速测试和开发

### MySQL 容器模式

**配置文件：** `docker-compose.yml`

**连接信息：**
- 主机：fastdotnet-mysql（容器名）
- 端口：3306
- 数据库：fastdotnet, fastdotnet_log
- 用户：fastdotnet / fastdotnet
- Root 密码：fastdotnet_root

### 外部 MySQL 模式

**配置文件：** `docker-compose.override.yml` + `docker-compose.yml`

**优势：**
- 完全控制 MySQL 配置
- 可以使用现有数据库基础设施
- 生产环境最佳选择

---

## 🔧 常用命令

### 容器管理

```bash
# 启动
docker-compose [-f 文件名] up -d

# 停止
docker-compose [-f 文件名] down

# 重启
docker-compose [-f 文件名] restart

# 查看状态
docker-compose [-f 文件名] ps

# 查看日志
docker-compose [-f 文件名] logs -f
```

### 进入容器

```bash
# 进入 API 容器
docker exec -it fastdotnetwebapi /bin/bash

# 进入 MySQL 容器（如果使用容器模式）
docker exec -it fastdotnet-mysql /bin/bash
```

### 数据管理

```bash
# 查看数据卷
docker volume ls

# 检查数据卷内容
docker volume inspect fastdotnet-data

# 删除数据卷（谨慎！会丢失数据）
docker volume rm fastdotnet-data
```

---

## ⚠️ 注意事项

### 1. 数据持久化

- SQLite 数据存储在 Docker volume 中
- 即使删除容器，数据也不会丢失
- 完全清理需要删除 volume：`docker volume rm fastdotnet-data`

### 2. 切换方案

在不同方案间切换时：

```bash
# 1. 停止当前服务
docker-compose down

# 2. 清理旧数据（可选）
docker volume rm fastdotnet-data

# 3. 修改配置

# 4. 重新启动
docker-compose up -d
```

### 3. 密钥文件

确保密钥文件存在：

```bash
# 检查密钥文件
Test-Path ./secrets/marketplace_private_key.txt

# 如果不存在，需要创建或导出
```

---

## 📖 相关文档

- [QUICKSTART.md](QUICKSTART.md) - 快速启动指南（包含网络问题解决方案）
- [DEPLOYMENT.md](DEPLOYMENT.md) - 完整部署指南
- [MYSQL_CONFIG.md](MYSQL_CONFIG.md) - MySQL 配置详细说明
- [docker-compose.sqlite.yml](docker-compose.sqlite.yml) - SQLite 模式配置
- [docker-compose.yml](docker-compose.yml) - MySQL 容器模式配置
- [docker-compose.external-db.yml](docker-compose.external-db.yml) - 外部数据库配置示例

---

## 🆘 故障排查

### API 无法启动

```bash
# 查看详细日志
docker-compose logs fastdotnetwebapi

# 常见原因：
# 1. 密钥文件不存在
# 2. 端口被占用
# 3. 数据库连接失败
```

### 健康检查失败

```bash
# 等待几分钟，应用需要时间初始化
docker-compose ps

# 手动测试
curl http://localhost:18889/health
```

### 数据库问题

```bash
# SQLite 模式下，检查数据目录
docker exec fastdotnetwebapi ls -la /app/data

# MySQL 模式下，检查连接
docker exec fastdotnetwebapi curl -f http://localhost:18889/health
```

---

## 💡 推荐路径

### 开发环境

1. **先用 SQLite** - 快速验证功能
2. **再配 MySQL 容器** - 完整测试
3. **最后外部 MySQL** - 接近生产

### 生产环境

直接使用 **外部 MySQL 服务器** 方案

---

**最后更新：** 2025-03-12  
**当前状态：** SQLite 模式运行正常
