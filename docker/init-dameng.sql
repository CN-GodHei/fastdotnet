-- 达梦数据库初始化脚本
-- 用于在 Docker 容器启动时自动创建用户

-- 注意：
-- 1. 达梦数据库不需要手动 CREATE DATABASE，它使用实例 (Instance) 的概念
-- 2. 创建用户会自动创建对应的 Schema/表空间
-- 3. 默认管理员账号：SYSDBA / SYSDBA001

-- 创建应用用户 fastdotnet
-- 格式：CREATE USER "用户名" IDENTIFIED BY "密码"
CREATE USER fastdotnet IDENTIFIED BY fastdotnet;

-- 给用户授权 RESOURCE 角色（允许创建表、视图、存储过程等）
GRANT RESOURCE TO fastdotnet;

-- 授予 DBA 权限（SqlSugar 需要访问系统表来生成代码）
-- 这是必须的，因为 SqlSugar 需要查询 SYSCOLUMNCOMMENTS 等系统表
GRANT DBA TO fastdotnet;
