# 后端开发

本章节详细介绍 Fastdotnet 后端开发的各个方面，从基础的实体设计到高级的性能优化。

---

## 📖 本章内容

- [核心概念](./核心概念) - 实体、仓储、服务、控制器 ⭐
- 项目结构（待补充）
- API 设计规范（待补充）
- 数据库操作（待补充）
- 依赖注入（待补充）
- 中间件开发（待补充）
- 事件总线（待补充）
- 缓存策略（待补充）
- 性能优化（待补充）

---

## 🎯 学习路径

### 新手路线

1. **[核心概念](./核心概念)** - 掌握 BaseEntity、IRepository、IBaseService、GenericDtoControllerBase
2. **创建第一个插件** - 实践 CRUD 开发
3. **PluginA 演示插件** - 学习完整示例

### 进阶路线

1. **自定义业务逻辑** - 扩展 BaseService
2. **复杂查询优化** - 投影查询、动态搜索
3. **事务管理** - IUnitOfWork 使用
4. **性能调优** - 索引、缓存、分页

---

## 💡 快速开始

### 1. 创建实体

```csharp
[SugarTable("my_tasks")]
public class TaskItem : AuditableEntity
{
    [SugarColumn(Length = 200)]
    public string Title { get; set; }
    
    public int Status { get; set; }
}
```

### 2. 创建服务

```csharp
public interface ITaskService : IBaseService<TaskItem> { }

public class TaskService : BaseService<TaskItem>, ITaskService
{
    public TaskService(IRepository<TaskItem> repository) 
        : base(repository) { }
}
```

### 3. 创建控制器

```csharp
[Route("api/my-plugin/[controller]")]
public class TaskController : GenericDtoControllerBase<TaskItem>
{
    public TaskController(IBaseService<TaskItem> service) 
        : base(service) { }
        
    // 自动获得完整的 CRUD API
}
```

就这么简单！✨

---

## 🔗 相关链接

- [快速开始](../01-快速开始/)
- [创建第一个插件](../01-快速开始/创建第一个插件)
- [PluginA演示插件](../05-插件开发/PluginA演示插件)
- [前端开发](../04-前端开发/)
