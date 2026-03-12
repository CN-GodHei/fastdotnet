-- 创建主业务数据库 (如果 POSTGRES_DB 环境变量已经创建了它，这行可以省略，但写上也没坏处)
-- 注意：如果环境变量设的是 fastdotnet_log，这里必须显式创建 fastdotnet
CREATE DATABASE fastdotnet;

-- 创建日志数据库 (如果环境变量设的是 fastdotnet，这里必须显式创建 fastdotnet_log)
CREATE DATABASE fastdotnet_log;

-- 授权用户访问这两个数据库 (防止权限问题)
GRANT ALL PRIVILEGES ON DATABASE fastdotnet TO fastdotnet;
GRANT ALL PRIVILEGES ON DATABASE fastdotnet_log TO fastdotnet;