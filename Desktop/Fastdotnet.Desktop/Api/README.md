# API 处理架构文档

## 概述

本项目采用 Refit + 自定义拦截器的方式来处理 API 调用，具有以下特点：

1. **类型安全**: 使用 Refit 自动生成类型安全的 REST API 客户端
2. **统一响应处理**: 通过自定义拦截器处理统一响应格式
3. **全局错误处理**: 提供全局的成功/错误事件通知机制
4. **灵活扩展**: 支持自定义序列化器和消息处理器

## 架构组件

### 1. Refit 客户端
- 自动生成的 API 接口
- 基于接口定义的 REST 调用
- 支持同步和异步调用

### 2. UnifiedResponseContentSerializer
自定义内容序列化器，负责：
- JSON 序列化/反序列化
- 处理标准的 JSON 格式响应

### 3. UnifiedResponseInterceptor
核心拦截器，负责：
- 解析统一响应格式
- 区分 HTTP 状态码和业务状态码
- 提供全局事件通知
- 处理常见错误场景

## 统一响应格式

所有 API 响应采用以下统一格式：

```json
{
  "Code": 0,
  "Msg": "Success message",
  "Data": {
    // 实际的业务数据
  }
}
```

## 使用示例

### 基本 API 调用

```csharp
// 登录示例
var loginDto = new LoginDto 
{ 
    Username = "user", 
    Password = "password" 
};

var response = await _apiClient.Login(loginDto);

if (response.IsSuccessStatusCode)
{
    var token = response.Content?.Token;
    // 处理成功登录
}
```

### 事件订阅

```csharp
// 在 App.xaml.cs 中订阅全局事件
interceptor.OnApiSuccess += (sender, e) =>
{
    Console.WriteLine($"API 调用成功: {e.HttpMethod} {e.RequestUrl}");
};

interceptor.OnApiError += (sender, e) =>
{
    Console.WriteLine($"API 调用失败: {e.ErrorMessage}");
};
```

## 错误处理策略

### HTTP 错误 (4xx, 5xx)
- 自动记录到日志
- 根据状态码采取相应措施
- 提供用户友好的错误提示

### 业务错误 (Code != 0)
- 解析业务错误消息
- 触发全局错误事件
- 支持业务特定的错误处理逻辑

## 扩展指南

### 添加新的 API 接口
1. 更新 OpenAPI 规范
2. 重新生成 Refit 客户端
3. 在依赖注入中注册新接口

### 自定义拦截器行为
1. 继承 `UnifiedResponseInterceptor`
2. 重写 `SendAsync` 方法
3. 添加自定义处理逻辑

### 集成监控系统
1. 在事件处理程序中添加监控代码
2. 记录 API 调用性能指标
3. 上报错误统计信息

## 最佳实践

1. **始终检查响应状态**: 使用 `IsSuccessStatusCode` 检查 HTTP 状态
2. **合理处理业务错误**: 区分 HTTP 错误和业务错误
3. **提供用户反馈**: 对用户操作提供及时的反馈
4. **记录重要事件**: 记录关键的 API 调用和错误信息
5. **优雅降级**: 在网络错误时提供合理的用户体验