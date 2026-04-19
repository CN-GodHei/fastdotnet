# PluginA - 用户扩展功能演示

这是一个演示插件，展示了如何使用 Fastdotnet 的插件扩展功能来为用户添加自定义扩展数据。

## 功能说明

PluginA 实现了一个用户扩展功能，允许为用户添加自定义数据字段，包括：

- 用户偏好设置
- 用户积分
- 创建和更新时间

## API 接口

### 获取用户扩展数据
- **GET** `/api/plugin-a/user/extension/{userId}`
- 获取指定用户的扩展数据

### 更新用户扩展数据
- **PUT** `/api/plugin-a/user/extension/{userId}`
- 更新指定用户的扩展数据

### 创建带扩展数据的用户
- **POST** `/api/plugin-a/user/extension`
- 创建新用户并同时设置扩展数据

## 核心组件

### 1. PluginAUserExtension 实体
定义了插件需要存储的用户扩展数据结构。

### 2. PluginAUserExtensionHandler
实现了 `IFdAppUserExtensionHandler<PluginAUserExtension>` 接口，处理扩展数据的保存和加载。

### 3. PluginAUserController
提供 REST API 接口，供外部调用。

### 4. PluginAUserExtensionInitializer
初始化数据库表结构。

## 使用方法

1. 启动主应用程序
2. 确保插件已加载
3. 通过 API 接口访问功能

## 数据库表

该插件会自动创建 `Fd_PluginAUserExtension` 表来存储用户扩展数据。

## 扩展机制

这个插件演示了 Fastdotnet 插件系统的扩展机制：

- 插件可以通过实现 `IFdAppUserExtensionHandler<T>` 接口来扩展用户数据
- 使用 `IStorageContext` 接口安全地访问数据库
- 与主应用程序的事务保持一致
- 自动处理数据的保存和加载