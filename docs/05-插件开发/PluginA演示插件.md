# PluginA - 官方演示插件

PluginA 是 Fastdotnet 框架的**官方演示插件**，展示了从插件开发到部署使用的完整生命周期。所有 Fastdotnet 插件开发的最佳实践和技术特性都在这个插件中进行了实际演示。

---

## 🎯 定位与目标

### 为什么需要 PluginA？

Fastdotnet 采用**插件化架构**，遵循以下原则：

> **非特殊情况不改框架基座，所有功能都以插件形式开发**

PluginA 的作用：

1. **技术演示** - 展示框架支持的所有技术特性
2. **最佳实践** - 提供标准的插件开发示例
3. **学习参考** - 开发者可以快速了解如何开发插件
4. **测试验证** - 验证框架功能的正确性

### 设计理念

```
┌─────────────────────────────────────┐
│      Fastdotnet 框架基座             │
│  (稳定、核心、不轻易修改)              │
└──────────┬──────────────────────────┘
           │ 插件接口
    ┌──────┴──────┐
    ↓             ↓
┌────────┐  ┌────────┐
│PluginA │  │其他插件│  ← 所有业务功能都在这里
│(演示)  │  │        │
└────────┘  └────────┘
```

---

## 📦 项目结构

```
PluginA/
├── Backend/                    # 后端项目 (.NET 10)
│   ├── PluginA.cs             # 插件入口类
│   ├── PluginaPlugin.cs       # 插件主类（继承 PluginBase）
│   ├── plugin.json            # 插件元数据
│   │
│   ├── Controllers/           # API 控制器
│   │   ├── PluginATestController.cs      # 通用 CRUD 示例
│   │   ├── PluginAUserController.cs      # 用户扩展示例
│   │   ├── MallOrderController.cs        # 业务逻辑示例
│   │   ├── MessageController.cs          # 消息推送示例
│   │   └── TestController.cs             # 基础功能测试
│   │
│   ├── Services/              # 业务服务
│   ├── Dto/                   # 数据传输对象
│   ├── Entities/              # 数据实体
│   ├── Initializers/          # 初始化器（菜单、配置等）
│   ├── EventHandlers/         # 事件处理器
│   ├── Events/                # 事件定义
│   ├── Middleware/            # 中间件
│   ├── Hubs/                  # SignalR Hub
│   ├── Metrics/               # 指标监控
│   └── Mappings/              # AutoMapper 配置
│
└── Frontend/                   # 前端项目 (Vue 3)
    ├── PluginA.admin/         # 管理端
    │   ├── src/
    │   │   ├── views/         # 页面组件
    │   │   ├── api/           # API 调用
    │   │   └── router/        # 路由配置
    │   ├── vite.config.ts
    │   └── micro-index.html   # 微前端入口
    │
    └── PluginA.app/           # 应用端
        ├── src/
        ├── vite.config.ts
        └── micro-index.html
```

---

## 🔧 核心技术演示

### 1. 插件生命周期

**文件**: `PluginaPlugin.cs`

```csharp
public class PluginaPlugin : PluginBase
{
    public override string Name => "PluginA";
    public override string Version => "1.0.0";
    public override string PluginId => "11375910391972869";

    // 初始化阶段
    protected override async Task OnInitializeAsync(IServiceProvider serviceProvider)
    {
        // 注册中间件、初始化资源等
    }

    // 启动阶段
    protected override Task OnStartAsync()
    {
        // 插件启动逻辑
        return Task.CompletedTask;
    }

    // 停止阶段
    protected override Task OnStopAsync()
    {
        // 清理资源
        return Task.CompletedTask;
    }

    // 卸载阶段
    protected override Task OnUnloadAsync(IServiceProvider serviceProvider)
    {
        // 卸载清理
        return Task.CompletedTask;
    }

    // 依赖注入配置
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 注册服务
    }
}
```

**学习要点：**
- ✅ 完整的生命周期方法
- ✅ 依赖注入配置
- ✅ 中间件注册

---

### 2. 通用 CRUD 控制器

**文件**: `Controllers/PluginATestController.cs`

```csharp
[Route("api/[controller]")]
public class PluginATestController : GenericControllerBase<PluginATest>
{
    public PluginATestController(IRepository<PluginATest> repository) 
        : base(repository)
    {
    }
    
    // 自动继承 14+ 个标准 API：
    // GET    /api/plugintest              - 列表
    // GET    /api/plugintest/{id}         - 详情
    // POST   /api/plugintest              - 创建
    // PUT    /api/plugintest/{id}         - 更新
    // DELETE /api/plugintest/{id}         - 删除
    // GET    /api/plugintest/recyclebin   - 回收站
    // ... 等等
}
```

**学习要点：**
- ✅ 继承 `GenericControllerBase` 快速实现 CRUD
- ✅ 自动包含回收站功能
- ✅ 分页、搜索、批量操作

---

### 3. 用户扩展机制

**文件**: `Controllers/PluginAUserController.cs`

演示如何为用户添加自定义扩展数据：

```csharp
// 获取用户扩展数据
GET /api/plugin-a/user/extension/{userId}

// 更新用户扩展数据
PUT /api/plugin-a/user/extension/{userId}

// 创建带扩展数据的用户
POST /api/plugin-a/user/extension
```

**学习要点：**
- ✅ 实现 `IFdAppUserExtensionHandler<T>`
- ✅ 扩展用户数据结构
- ✅ 与主程序事务一致

---

### 4. 事件总线

**文件**: 
- `Events/` - 事件定义
- `EventHandlers/` - 事件处理器

演示插件内部和跨插件的事件通信：

```csharp
// 发布事件
await _eventBus.PublishAsync(new OrderCreatedEvent(orderId));

// 订阅事件
[EventHandler]
public async Task HandleOrderCreated(OrderCreatedEvent evt)
{
    // 处理订单创建事件
}
```

**学习要点：**
- ✅ 事件定义和发布
- ✅ 事件订阅和处理
- ✅ 异步事件处理

---

### 5. SignalR 实时通信

**文件**: `Hubs/`

演示 WebSocket 实时推送：

```csharp
public class PluginAMessageHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
```

**学习要点：**
- ✅ Hub 定义
- ✅ 客户端连接管理
- ✅ 消息广播

---

### 6. 中间件

**文件**: `Middleware/`

演示 HTTP 请求管道扩展：

```csharp
public class PluginACustomMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 前置处理
        await next(context);
        // 后置处理
    }
}
```

**学习要点：**
- ✅ 中间件注册
- ✅ 请求拦截
- ✅ 动态中间件

---

### 7. 指标监控

**文件**: `Metrics/`

演示性能指标收集：

```csharp
public class PluginAMetricProvider : IMetricProvider
{
    public IEnumerable<Metric> GetMetrics()
    {
        // 返回自定义指标
    }
}
```

**学习要点：**
- ✅ 自定义指标
- ✅ 性能监控
- ✅ 健康检查

---

### 8. 菜单和权限初始化

**文件**: `Initializers/`

演示插件菜单和权限的自动注册：

```csharp
public class PluginAMenuInitializer : IApplicationInitializer
{
    public async Task InitializeAsync()
    {
        // 注册菜单
        var menus = new List<FdMenu> { ... };
        
        // 注册权限
        var permissions = new List<FdPermission> { ... };
    }
}
```

**学习要点：**
- ✅ 菜单配置
- ✅ 权限定义
- ✅ 自动初始化

---

### 9. 前端双端开发

**管理端 (PluginA.admin)**:
- Vue 3 + TypeScript
- Element Plus UI
- qiankun 微前端集成
- 管理功能界面

**应用端 (PluginA.app)**:
- 面向最终用户
- 业务操作界面
- 独立的构建配置

**学习要点：**
- ✅ 微前端生命周期
- ✅ 路由配置
- ✅ API 调用
- ✅ 状态管理

---

## 📚 详细文档

PluginA 包含了多个详细的说明文档，位于源码目录中：

| 文档 | 说明 | 位置 |
|------|------|------|
| README.md | 用户扩展功能说明 | `Backend/README.md` |
| 事件总线示例说明.md | 事件总线使用指南 | `Backend/事件总线示例说明.md` |
| README_IPluginPipeline示例说明.md | 插件管道集成 | `Backend/README_IPluginPipeline示例说明.md` |
| README_HTTP与业务管道集成.md | HTTP 中间件集成 | `Backend/README_HTTP与业务管道集成.md` |
| CROSS_PLUGIN_CALL_GUIDE.md | 跨插件调用指南 | `Frontend/PluginA.admin/CROSS_PLUGIN_CALL_GUIDE.md` |
| UPLOAD_FEATURES.md | 文件上传功能 | `Frontend/PluginA.admin/UPLOAD_FEATURES.md` |

**查看方式：**
```bash
# 进入 PluginA 目录
cd Fastdotnet/backend/Plugins/PluginA

# 查看后端文档
code Backend/README.md
code Backend/事件总线示例说明.md

# 查看前端文档
code Frontend/PluginA.admin/CROSS_PLUGIN_CALL_GUIDE.md
```

---

## 🚀 如何使用 PluginA 学习

### 1. 阅读代码

按照以下顺序阅读 PluginA 的代码：

```
1️⃣ PluginaPlugin.cs        - 理解插件入口
2️⃣ plugin.json             - 了解元数据配置
3️⃣ Controllers/            - 学习 API 开发
4️⃣ Services/               - 学习业务逻辑
5️⃣ Initializers/           - 学习初始化流程
6️⃣ Frontend/               - 学习前端开发
```

### 2. 运行调试

```bash
# 1. 启动 Fastdotnet 主程序
cd Fastdotnet/backend/Fastdotnet.WebApi
dotnet run

# 2. PluginA 会自动加载（在 Plugins 目录）

# 3. 访问 Swagger
http://localhost:18889/swagger

# 4. 查看 PluginA 的 API
# 搜索 "PluginA" 相关接口
```

### 3. 修改实验

```bash
# 1. 修改 PluginA 代码
# 2. 重新编译
cd PluginA/Backend
dotnet build

# 3. 复制到主程序（如果需要）
copy bin\Debug\net10.0\*.dll ..\..\..\Fastdotnet.WebApi\bin\Debug\net10.0\Plugins\11375910391972869\dependencies\

# 4. 重启主程序或热重载
```

### 4. 参考开发自己的插件

```bash
# 使用 CLI 工具创建新插件
fd-plugin create --name MyPlugin --plugin-id YOUR_ID

# 参考 PluginA 的结构组织代码
# 复制需要的模式和最佳实践
```

---

## 💡 关键设计模式

### 1. 插件隔离

每个插件在独立的 `AssemblyLoadContext` 中运行：
- ✅ 版本隔离
- ✅ 故障隔离
- ✅ 热插拔支持

### 2. 依赖注入

插件可以：
- 注册自己的服务
- 访问主程序的服务
- 与其他插件共享服务

### 3. 事件驱动

通过事件总线实现：
- 松耦合通信
- 异步处理
- 跨插件交互

### 4. 微前端

前端采用 qiankun：
- 独立开发
- 独立部署
- 运行时集成

---

## 🎯 学习路径建议

### 初级开发者

1. **理解插件结构** - 阅读 `PluginaPlugin.cs` 和 `plugin.json`
2. **学习 CRUD** - 研究 `PluginATestController.cs`
3. **尝试修改** - 添加一个简单的 API
4. **前端入门** - 查看管理端页面

### 中级开发者

1. **用户扩展** - 学习 `PluginAUserController.cs`
2. **事件总线** - 阅读事件相关文档
3. **中间件** - 研究 Middleware 目录
4. **SignalR** - 查看 Hubs 实现

### 高级开发者

1. **跨插件调用** - 阅读 `CROSS_PLUGIN_CALL_GUIDE.md`
2. **性能优化** - 研究 Metrics 实现
3. **管道集成** - 阅读 `IPluginPipeline` 文档
4. **最佳实践** - 总结 PluginA 的设计模式

---

## 🔗 相关链接

- [插件概述](./插件概述)
- [创建插件](./创建插件)
- [后端开发](./后端开发)
- [前端开发](./前端开发)
- [PluginA 源码](../../backend/Plugins/PluginA) - 查看完整源代码
- [插件商城](https://fastdotnet.top/marketplace)

---

## ❓ 常见问题

### Q: PluginA 和生产插件有什么区别？

**A**: PluginA 是演示插件，包含了所有技术特性的示例。生产插件应该：
- 只包含必要的功能
- 遵循相同的架构模式
- 注重性能和安全性

### Q: 我可以直接复制 PluginA 的代码吗？

**A**: 可以借鉴结构和模式，但不要直接复制。应该：
- 理解代码背后的原理
- 根据实际需求调整
- 保持代码简洁

### Q: PluginA 会随框架更新吗？

**A**: 是的，PluginA 会跟随框架更新，展示最新的技术特性。建议定期查看更新。

### Q: 如何贡献 PluginA 的改进？

**A**: 欢迎提交 Pull Request 或 Issue，帮助完善演示内容。
