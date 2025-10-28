# UnifiedResponseInterceptor 统一响应拦截器

## 概述

`UnifiedResponseInterceptor` 是一个自定义的 HTTP 消息拦截器，用于处理采用统一响应格式的 API。它能够自动解析统一响应格式，区分 HTTP 状态码和业务状态码，并提供全局的成功/错误事件通知机制。

## 统一响应格式

该拦截器专为处理以下格式的统一响应而设计：

```json
{
  "Code": 0,
  "Msg": "Success message",
  "Data": {
    // 实际的业务数据
  }
}
```

其中：
- `Code`: 业务状态码（0 表示成功，非 0 表示业务错误）
- `Msg`: 业务消息描述
- `Data`: 实际的业务数据

## 功能特性

### 1. HTTP 状态码处理
- 自动检测 HTTP 状态码（2xx, 4xx, 5xx）
- 区分网络错误和业务错误
- 提供详细的错误信息

### 2. 业务状态码处理
- 解析统一响应格式中的 `Code` 字段
- 根据业务状态码触发相应事件
- 自动提取 `Data` 字段内容

### 3. 事件通知系统
- `OnApiSuccess`: API 调用成功事件
- `OnApiError`: API 调用错误事件

### 4. 灵活的错误处理
- 支持自动重试机制
- 支持令牌刷新
- 支持全局错误通知

## 使用方法

### 1. 注册拦截器

在 `App.axaml.cs` 中注册拦截器：

```csharp
// 创建拦截器实例
var interceptor = new UnifiedResponseInterceptor();

// 订阅成功事件
interceptor.OnApiSuccess += (sender, e) =>
{
    Console.WriteLine($"API Success: {e.HttpMethod} {e.RequestUrl}");
    Console.WriteLine($"HTTP Status: {(int)e.HttpStatusCode}, Business Code: {e.ResponseCode}");
};

// 订阅错误事件
interceptor.OnApiError += (sender, e) =>
{
    Console.WriteLine($"API Error: {e.HttpMethod} {e.RequestUrl}");
    Console.WriteLine($"HTTP Status: {(int)e.HttpStatusCode}, Error Code: {e.ErrorCode}");
    Console.WriteLine($"Error Message: {e.ErrorMessage}");
};

// 配置 Refit 客户端
services.AddRefitClient<IApiInterface>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.example.com"))
    .ConfigurePrimaryHttpMessageHandler(provider => 
    {
        // 链式处理：拦截器 -> 其他处理器 -> HTTP 处理器
        return interceptor;
    });
```

### 2. 事件处理

#### 成功事件处理
```csharp
interceptor.OnApiSuccess += (sender, e) =>
{
    // 记录 API 调用成功日志
    _logger.LogInformation($"API Call Success: {e.HttpMethod} {e.RequestUrl}");
    
    // 更新 UI 状态
    UpdateUiForSuccess(e);
    
    // 性能监控
    RecordPerformanceMetrics(e);
};
```

#### 错误事件处理
```csharp
interceptor.OnApiError += (sender, e) =>
{
    // 记录错误日志
    _logger.LogError($"API Call Error: {e.HttpMethod} {e.RequestUrl} - {e.ErrorMessage}");
    
    // 根据错误类型采取不同措施
    switch (e.HttpStatusCode)
    {
        case HttpStatusCode.Unauthorized:
            // 未授权，跳转到登录页面
            NavigateToLogin();
            break;
        case HttpStatusCode.Forbidden:
            // 禁止访问，显示权限不足提示
            ShowAccessDeniedMessage();
            break;
        default:
            // 显示通用错误消息
            ShowErrorMessage(e.ErrorMessage);
            break;
    }
};
```

## 高级用法

### 1. 自动令牌刷新

```csharp
interceptor.OnApiError += async (sender, e) =>
{
    if (e.HttpStatusCode == HttpStatusCode.Unauthorized)
    {
        // 尝试刷新令牌
        var refreshTokenResult = await RefreshAccessTokenAsync();
        if (refreshTokenResult.Success)
        {
            // 重新发送原始请求
            // 这需要在拦截器内部实现重试逻辑
        }
        else
        {
            // 跳转到登录页面
            NavigateToLogin();
        }
    }
};
```

### 2. 重试机制

```csharp
interceptor.OnApiError += (sender, e) =>
{
    if ((int)e.HttpStatusCode >= 500)
    {
        // 对于服务器错误，可以实现重试逻辑
        // 或者通知用户稍后重试
        ShowRetryMessage();
    }
};
```

### 3. 速率限制处理

```csharp
interceptor.OnApiError += (sender, e) =>
{
    if (e.HttpStatusCode == (HttpStatusCode)429) // Too Many Requests
    {
        // 处理速率限制
        var retryAfter = ParseRetryAfterHeader(e.RawResponse);
        ScheduleRetry(retryAfter);
    }
};
```

## 事件参数说明

### ApiSuccessEventArgs

| 属性 | 类型 | 描述 |
|------|------|------|
| RequestUrl | string | 请求 URL |
| HttpMethod | string | HTTP 方法 |
| ResponseCode | int | 业务状态码 |
| ResponseMessage | string? | 业务消息 |
| RawResponse | string | 原始响应内容 |
| HttpStatusCode | HttpStatusCode | HTTP 状态码 |

### ApiErrorEventArgs

| 属性 | 类型 | 描述 |
|------|------|------|
| RequestUrl | string | 请求 URL |
| HttpMethod | string | HTTP 方法 |
| ErrorCode | int | 错误码 |
| ErrorMessage | string? | 错误消息 |
| RawResponse | string | 原始响应内容 |
| Exception | Exception? | 异常对象 |
| HttpStatusCode | HttpStatusCode | HTTP 状态码 |

## 最佳实践

### 1. 全局错误处理

```csharp
// 在应用程序启动时配置全局错误处理
interceptor.OnApiError += (sender, e) =>
{
    // 记录所有 API 错误
    _logger.LogError($"API Error: {e.RequestUrl} - {e.ErrorMessage}");
    
    // 根据错误类型显示不同消息
    if (IsNetworkError(e))
    {
        ShowNetworkErrorMessage();
    }
    else if (IsBusinessError(e))
    {
        ShowBusinessErrorMessage(e.ErrorMessage);
    }
    else
    {
        ShowGenericErrorMessage(e.ErrorMessage);
    }
};
```

### 2. 性能监控

```csharp
interceptor.OnApiSuccess += (sender, e) =>
{
    // 记录 API 调用性能
    _telemetry.TrackEvent("ApiCall", new Dictionary<string, string>
    {
        ["Method"] = e.HttpMethod,
        ["Url"] = e.RequestUrl,
        ["HttpStatus"] = ((int)e.HttpStatusCode).ToString(),
        ["BusinessCode"] = e.ResponseCode.ToString()
    });
};
```

### 3. 用户体验优化

```csharp
interceptor.OnApiError += (sender, e) =>
{
    // 对于用户可见的操作，显示友好的错误消息
    if (IsUserFacingOperation(e.RequestUrl))
    {
        ShowUserFriendlyErrorMessage(e.ErrorMessage);
    }
    
    // 对于后台操作，只记录日志
    else
    {
        _logger.LogError($"Background API Error: {e.ErrorMessage}");
    }
};
```

## 注意事项

1. **线程安全**: 事件处理程序应在适当的情况下考虑线程安全
2. **异常处理**: 在事件处理程序中应妥善处理异常，避免影响主流程
3. **性能考虑**: 避免在事件处理程序中执行耗时操作
4. **内存泄漏**: 如果动态订阅事件，记得在适当时候取消订阅

## 扩展性

拦截器设计为可扩展的，您可以：

1. 继承 `UnifiedResponseInterceptor` 类添加自定义功能
2. 实现额外的事件处理逻辑
3. 添加自定义的响应处理规则
4. 集成第三方监控或日志系统